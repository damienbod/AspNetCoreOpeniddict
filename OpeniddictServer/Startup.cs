using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Primitives;
using OpeniddictServer.Models;
using OpeniddictServer.Services;
using OpenIddict.Core;
using OpenIddict.Models;
using System;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace OpeniddictServer
{
    public class Startup
    {
        private readonly IHostingEnvironment _environment;

        public Startup(IHostingEnvironment env)
        {
            _cert = new X509Certificate2(Path.Combine(env.ContentRootPath, "damienbodserver.pfx"), "");

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            _environment = env;

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        private X509Certificate2 _cert;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                // Configure the context to use Microsoft SQL Server.
                options.UseSqlite(Configuration.GetConnectionString("DefaultConnection"));

                // Register the entity sets needed by OpenIddict.
                // Note: use the generic overload if you need
                // to replace the default OpenIddict entities.
                options.UseOpenIddict();
            });

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            // Configure Identity to use the same JWT claims as OpenIddict instead
            // of the legacy WS-Federation claims it uses by default (ClaimTypes),
            // which saves you from doing the mapping in your authorization controller.
            services.Configure<IdentityOptions>(options =>
            {
                options.ClaimsIdentity.UserNameClaimType = OpenIdConnectConstants.Claims.Name;
                options.ClaimsIdentity.UserIdClaimType = OpenIdConnectConstants.Claims.Subject;
                options.ClaimsIdentity.RoleClaimType = OpenIdConnectConstants.Claims.Role;
            });

            // Register the OpenIddict services.
            services.AddOpenIddict(options =>
            {
                // Register the Entity Framework stores.
                options.AddEntityFrameworkCoreStores<ApplicationDbContext>();

                // Register the ASP.NET Core MVC binder used by OpenIddict.
                // Note: if you don't call this method, you won't be able to
                // bind OpenIdConnectRequest or OpenIdConnectResponse parameters.
                options.AddMvcBinders();

                // Enable the authorization, logout, userinfo, and introspection endpoints.
                options.EnableAuthorizationEndpoint("/connect/authorize")
                       .EnableLogoutEndpoint("/connect/logout")
                       .EnableIntrospectionEndpoint("/connect/introspect")
                       .EnableUserinfoEndpoint("/api/userinfo");

                // Note: the sample only uses the implicit code flow but you can enable
                // the other flows if you need to support implicit, password or client credentials.
                options.AllowImplicitFlow();

                // During development, you can disable the HTTPS requirement.
                options.DisableHttpsRequirement();

                // Register a new ephemeral key, that is discarded when the application
                // shuts down. Tokens signed using this key are automatically invalidated.
                // This method should only be used during development.
                options.AddEphemeralSigningKey();

                // On production, using a X.509 certificate stored in the machine store is recommended.
                // You can generate a self-signed certificate using Pluralsight's self-cert utility:
                // https://s3.amazonaws.com/pluralsight-free/keith-brown/samples/SelfCert.zip
                //
                // options.AddSigningCertificate("7D2A741FE34CC2C7369237A5F2078988E17A6A75");
                //
                // Alternatively, you can also store the certificate as an embedded .pfx resource
                // directly in this assembly or in a file published alongside this project:
                //
                // options.AddSigningCertificate(
                //     assembly: typeof(Startup).GetTypeInfo().Assembly,
                //     resource: "AuthorizationServer.Certificate.pfx",
                //     password: "OpenIddict");

                // Note: to use JWT access tokens instead of the default
                // encrypted format, the following line is required:
                //
                // options.UseJsonWebTokens();
            });

            services.AddAuthentication()
                .AddOAuthValidation();

            var policy = new Microsoft.AspNetCore.Cors.Infrastructure.CorsPolicy();

            policy.Headers.Add("*");
            policy.Methods.Add("*");
            policy.Origins.Add("*");
            policy.SupportsCredentials = true;

            services.AddCors(x => x.AddPolicy("corsGlobalPolicy", policy));

            services.AddMvc();

            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
        }

        //public void ConfigureServices(IServiceCollection services)
        //{

        //    services.AddDbContext<ApplicationDbContext>(options =>
        //        options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));

        //    services.AddAuthentication();


        //    JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        //    JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();

        //    //var jwtOptions = new JwtBearerOptions()
        //    //{
        //    //    AutomaticAuthenticate = true,
        //    //    AutomaticChallenge = true,
        //    //    RequireHttpsMetadata = true,
        //    //    Audience = "dataEventRecords",
        //    //    ClaimsIssuer = "https://localhost:44319/",
        //    //    TokenValidationParameters = new TokenValidationParameters
        //    //    {
        //    //        NameClaimType = OpenIdConnectConstants.Claims.Name,
        //    //        RoleClaimType = OpenIdConnectConstants.Claims.Role
        //    //    }
        //    //};

        //    //jwtOptions.TokenValidationParameters.ValidAudience = "dataEventRecords";
        //    //jwtOptions.TokenValidationParameters.ValidIssuer = "https://localhost:44319/";
        //    //jwtOptions.TokenValidationParameters.IssuerSigningKey = new RsaSecurityKey(_cert.GetRSAPrivateKey().ExportParameters(false));
        //    //app.UseJwtBearerAuthentication(jwtOptions);


        //    services.AddIdentity<ApplicationUser, IdentityRole>()
        //        .AddEntityFrameworkStores<ApplicationDbContext>();

        //    services.Configure<IdentityOptions>(options =>
        //    {
        //        options.ClaimsIdentity.UserNameClaimType = OpenIdConnectConstants.Claims.Name;
        //        options.ClaimsIdentity.UserIdClaimType = OpenIdConnectConstants.Claims.Subject;
        //        options.ClaimsIdentity.RoleClaimType = OpenIdConnectConstants.Claims.Role;
        //    });
        //    services.AddOpenIddict<ApplicationDbContext>();

        //    services.AddOpenIddict(options =>
        //    {
        //        options.AddEntityFrameworkCoreStores<ApplicationDbContext>();

        //        // Register the ASP.NET Core MVC binder used by OpenIddict.
        //        // Note: if you don't call this method, you won't be able to
        //        // bind OpenIdConnectRequest or OpenIdConnectResponse parameters.
        //        options.AddMvcBinders();

        //        options.EnableAuthorizationEndpoint("/connect/authorize")
        //               .EnableLogoutEndpoint("/connect/logout")
        //               .EnableIntrospectionEndpoint("/connect/introspect")
        //               .EnableUserinfoEndpoint("/api/userinfo");

        //        options.AllowImplicitFlow();

        //        options.AddSigningCertificate(_cert);

        //        options.UseJsonWebTokens();
        //    });

        //    services.AddAuthentication()
        //        .AddOAuthValidation();

        //    var policy = new Microsoft.AspNetCore.Cors.Infrastructure.CorsPolicy();

        //    policy.Headers.Add("*");
        //    policy.Methods.Add("*");
        //    policy.Origins.Add("*");
        //    policy.SupportsCredentials = true;

        //    services.AddCors(x => x.AddPolicy("corsGlobalPolicy", policy));

        //    services.AddMvc();

        //    services.AddTransient<IEmailSender, AuthMessageSender>();
        //    services.AddTransient<ISmsSender, AuthMessageSender>();
        //}

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseCors("corsGlobalPolicy");

            app.UseAuthentication();

            app.UseMvcWithDefaultRoute();
        }

        private async Task InitializeAsync(IServiceProvider services, CancellationToken cancellationToken)
        {
            // Create a new service scope to ensure the database context is correctly disposed when this methods returns.
            using (var scope = services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
               
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                await context.Database.EnsureCreatedAsync();

                var manager = scope.ServiceProvider.GetRequiredService<OpenIddictApplicationManager<OpenIddictApplication>>();

                if (await manager.FindByClientIdAsync("angular4client", cancellationToken) == null)
                {
                    var application = new OpenIddictApplication
                    {
                        ClientId = "angular4client",
                        DisplayName = "Angular 4 client SPA",
                        PostLogoutRedirectUris = "https://localhost:44308/Unauthorized",
                        RedirectUris = "https://localhost:44308"
                    };

                    await manager.CreateAsync(application, cancellationToken);
                }

                if (await manager.FindByClientIdAsync("dataEventRecords", cancellationToken) == null)
                {
                    var application = new OpenIddictApplication
                    {
                        ClientId = "dataEventRecords"
                    };

                    await manager.CreateAsync(application, "77be52c7-06a2-4830-90bc-715b03b97119", cancellationToken);
                }
            }
        }
    }
}