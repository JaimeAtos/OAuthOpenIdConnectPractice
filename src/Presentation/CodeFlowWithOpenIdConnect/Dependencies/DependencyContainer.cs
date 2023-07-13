using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace CodeFlowWithOpenIdConnect.Dependencies;

public static class DependencyContainer
{
    public static void AddAuthenticationConfigurations(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAuthentication(opt =>
        {
            opt.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;

        })
            .AddCookie()
            .AddOpenIdConnect(opt => configuration.Bind("OAuth", opt));
    }
}