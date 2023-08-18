using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using WeatherAPI.Policies;

namespace WeatherAPI.Dependencies;

public static class DependencyContainer
{
    public static void AddAuthenticationConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(opt =>
            {
                opt.Authority = configuration["AuthorizationServer:Authority"];
                opt.Audience = configuration["AuthorizationServer:Audience"];
                // opt.RequireHttpsMetadata = false;
            });
        
        services.AddSingleton<IAuthorizationHandler, ScopeHandler>();
        
        services.AddAuthorization(options =>
        {
            options.AddPolicy("weatherapi.read", policy =>
                policy.Requirements.Add(new ScopeRequirement(
                    "weatherapi.read", 
                    configuration["AuthorizationServer:Authority"])));
            options.AddPolicy("weatherapi.write", policy =>
                policy.Requirements.Add(new ScopeRequirement("weatherapi.write", 
                    configuration["AuthorizationServer:Authority"])));
        });

    }
}