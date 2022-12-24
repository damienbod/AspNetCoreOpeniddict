using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Blazor.BFF.Yarp.CertificateAuth.Server;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddAntiforgery(options =>
        {
            options.HeaderName = "X-XSRF-TOKEN";
            options.Cookie.Name = "__Host-X-XSRF-TOKEN";
            options.Cookie.SameSite = SameSiteMode.Strict;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        });

        services.AddHttpClient();
        services.AddOptions();

        var openIDConnectSettings = Configuration.GetSection("OpenIDConnectSettings");

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
        })
        .AddCookie()
        .AddOpenIdConnect(options =>
        {
            options.SignInScheme = "Cookies";
            options.Authority = openIDConnectSettings["Authority"];
            options.ClientId = openIDConnectSettings["ClientId"];
            options.ClientSecret = openIDConnectSettings["ClientSecret"];
            options.RequireHttpsMetadata = true;
            options.ResponseType = OpenIdConnectResponseType.Code;
            options.UsePkce = true;
            options.Scope.Add("profile");
            options.SaveTokens = true;
            options.GetClaimsFromUserInfoEndpoint = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                NameClaimType = "name",
                RoleClaimType = "role"
            };
        });

        // Create an authorization policy used by YARP when forwarding requests
        // from the WASM application to the Dantooine.Api1 resource server.
        services.AddAuthorization(options => options.AddPolicy("CookieAuthenticationPolicy", builder =>
        {
            builder.AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme);
            builder.RequireAuthenticatedUser();
        }));

        services.AddControllersWithViews(options =>
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));

        services.AddRazorPages();

        var cert = new X509Certificate2("client.pfx", "1234");

        services.AddReverseProxy()
                .ConfigureHttpClient((context, handler) =>
                {
                    handler.SslOptions = new SslClientAuthenticationOptions
                    {
                    ClientCertificates = new X509CertificateCollection
                    {
                        cert
                    }
                    };
                })
            .LoadFromConfig(Configuration.GetSection("ReverseProxy"));
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseWebAssemblyDebugging();
        }
        else
        {
            app.UseExceptionHandler("/Error");
        }

        app.UseSecurityHeaders(
            SecurityHeadersDefinitions.GetHeaderPolicyCollection(env.IsDevelopment(),
                Configuration["OpenIDConnectSettings:Authority"]));

        app.UseHttpsRedirection();
        app.UseBlazorFrameworkFiles();
        app.UseStaticFiles();

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapRazorPages();
            endpoints.MapControllers();
            endpoints.MapReverseProxy();
            endpoints.MapFallbackToPage("/_Host");
        });
    }
}