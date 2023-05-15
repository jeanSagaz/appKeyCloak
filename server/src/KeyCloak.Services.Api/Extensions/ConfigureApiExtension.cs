using Core.Notifications;
using Core.Notifications.Interfaces;
using KeyCloak.Services.Api.Models;
using KeyCloak.Services.Api.Services;
using Polly;

namespace KeyCloak.Services.Api.Extensions
{
    public static class ConfigureApiExtension
    {
        public static void AddApiConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddScoped<IDomainNotifier, DomainNotifier>();
            services.AddTransient<IAuthenticationService, AuthenticationService>();
            //services.AddHttpClient<IAuthenticationService, AuthenticationService>()
            services.AddHttpClient<AuthenticationService>(nameof(AuthenticationService), options =>
            {
                //options.BaseAddress = new Uri("https://localhost:5501/");
            })            
            .AddPolicyHandler(PollyExtension.WaitTry())
            .AddTransientHttpErrorPolicy(p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));

            services.Configure<KeyCloakAppsettings>(x =>
            {
                x.Realm = configuration.GetSection("KeyCloak:realm").Value ?? string.Empty;
                x.AuthServerUrl = configuration.GetSection("KeyCloak:auth-server-url").Value ?? string.Empty;
                x.SslRequired = configuration.GetSection("KeyCloak:ssl-required").Value ?? string.Empty;
                x.Resource = configuration.GetSection("KeyCloak:resource").Value ?? string.Empty;
                x.VerifyTokenAudience = Convert.ToBoolean(configuration.GetSection("KeyCloak:verify-token-audience").Value);
                x.Secret = configuration.GetSection("KeyCloak:credentials:secret").Value ?? string.Empty;
            });
        }
    }
}
