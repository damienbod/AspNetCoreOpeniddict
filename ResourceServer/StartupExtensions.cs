using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using NetEscapades.AspNetCore.SecurityHeaders.Infrastructure;
using ResourceServer.Model;
using ResourceServer.Repositories;
using WebApiDuende;

namespace ResourceServer;

internal static class StartupExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;

        // Open up security restrictions to allow this to work
        // Not recommended in production
        var deploySwaggerUI = builder.Configuration.GetValue<bool>("DeploySwaggerUI");
        var isDev = builder.Environment.IsDevelopment();

        builder.Services.AddSecurityHeaderPolicies()
            .SetPolicySelector((PolicySelectorContext ctx) =>
            {
                // sum is weak security headers due to Swagger UI deployment
                // should only use in development
                if (deploySwaggerUI)
                {
                    // Weakened security headers for Swagger UI
                    if (ctx.HttpContext.Request.Path.StartsWithSegments("/swagger"))
                    {
                        return SecurityHeadersDefinitionsSwagger.GetHeaderPolicyCollection(isDev);
                    }

                    // Strict security headers
                    return SecurityHeadersDefinitionsAPI.GetHeaderPolicyCollection(isDev);
                }
                // Strict security headers for production
                else
                {
                    return SecurityHeadersDefinitionsAPI.GetHeaderPolicyCollection(isDev);
                }
            });

        var connection = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<DataEventRecordContext>(options =>
            options.UseSqlite(connection)
        );

        services.AddCors(options =>
        {
            options.AddPolicy("AllowAllOrigins",
                builder =>
                {
                    builder
                        .AllowCredentials()
                        .WithOrigins("https://localhost:4200")
                        .SetIsOriginAllowedToAllowWildcardSubdomains()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
        });

        var guestPolicy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .RequireClaim("scope", "dataEventRecords")
            .Build();

        builder.Services.AddAuthentication()
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                var oauthConfig = builder.Configuration.GetSection("ProfileApiConfigurations");
                options.Authority = oauthConfig["Authority"];
                options.Audience = oauthConfig["Audience"];
                options.MapInboundClaims = false;
                options.TokenValidationParameters.ValidTypes = ["at+jwt"];
            });

        builder.Services.AddOpenApi(options =>
        {
            //options.UseTransformer((document, context, cancellationToken) =>
            //{
            //    document.Info = new()
            //    {
            //        Title = "My API",
            //        Version = "v1",
            //        Description = "API for Damien"
            //    };
            //    return Task.CompletedTask;
            //});
            options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
        });

        services.AddScoped<IAuthorizationHandler, RequireScopeHandler>();

        services.AddAuthorization(options =>
        {
            options.AddPolicy("dataEventRecordsAdmin", policyAdmin =>
            {
                policyAdmin.RequireClaim("role", "dataEventRecords.admin");
            });
            options.AddPolicy("dataEventRecordsUser", policyUser =>
            {
                policyUser.RequireClaim("role", "dataEventRecords.user");
            });
            options.AddPolicy("dataEventRecordsPolicy", policyUser =>
            {
                policyUser.Requirements.Add(new RequireScope());
            });
        });

        services.AddControllers();

        services.AddScoped<IDataEventRecordRepository, DataEventRecordRepository>();

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        var deploySwaggerUI = app.Configuration.GetValue<bool>("DeploySwaggerUI");
        app.UseCors("AllowAllOrigins");

        app.UseSecurityHeaders();

        app.UseHttpsRedirection();
        app.UseAuthorization();

        app.MapControllers();

        //app.MapOpenApi(); // /openapi/v1.json
        app.MapOpenApi("/openapi/v1/openapi.json");
        //app.MapOpenApi("/openapi/{documentName}/openapi.json");

        if (deploySwaggerUI)
        {
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/openapi/v1/openapi.json", "v1");
            });
        }
        return app;
    }
}
