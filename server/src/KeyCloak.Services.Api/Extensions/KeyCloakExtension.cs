using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using Keycloak.AuthServices.Sdk.Admin;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.Security.Claims;

namespace KeyCloak.Services.Api.Extensions
{
    public static class KeyCloakExtension
    {
        public static void AddKeyCloakConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var authenticationOptions = configuration
                            .GetSection(KeycloakAuthenticationOptions.Section)
                            .Get<KeycloakAuthenticationOptions>();

            services.AddKeycloakAuthentication(authenticationOptions);

            var authorizationOptions = configuration
                                        .GetSection(KeycloakProtectionClientOptions.Section)
                                        .Get<KeycloakProtectionClientOptions>();

            services.AddKeycloakAuthorization(authorizationOptions);

            var adminClientOptions = configuration
                                        .GetSection(KeycloakAdminClientOptions.Section)
                                        .Get<KeycloakAdminClientOptions>();

            services.AddKeycloakAdminHttpClient(adminClientOptions);
        }

        public static void AddJwtBearerConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            if (Debugger.IsAttached)
                IdentityModelEventSource.ShowPII = true;

            var resource = configuration.GetSection("KeyCloak:resource").Value;
            var realm = configuration.GetSection("KeyCloak:realm").Value;

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.Authority = $"http://localhost:8080/realms/{realm}";
                options.IncludeErrorDetails = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateAudience = true,
                    //ValidAudience = resource,
                    ValidAudience = "myclient",
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidIssuers = new[] { $"http://localhost:8080/realms/{realm}" },
                    ValidateLifetime = true
                };

                options.Events = new JwtBearerEvents()
                {
                    OnTokenValidated = c =>
                    {
                        Console.WriteLine("User successfully authenticated");
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = e =>
                    {
                        e.NoResult();
                        e.Response.StatusCode = StatusCodes.Status401Unauthorized;

                        IServiceProvider serviceProvider = services.BuildServiceProvider();
                        IWebHostEnvironment env = serviceProvider.GetService<IWebHostEnvironment>();

                        if (env.IsDevelopment())
                        //if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT").Equals("Development"))
                        {
                            e.Response.ContentType = "text/plain";
                            return e.Response.WriteAsync(e.Exception.ToString());
                        }

                        //return e.Response.WriteAsync("An error occured processing your authentication.");
                        return Task.CompletedTask;
                    }
                };
            });

            services.AddAuthorization(options =>
            {
                //options.AddPolicy("Administrator", policy => policy.RequireClaim("user_roles", "[Administrator]"));
            });
        }

        public static void AddOpenIdConnectConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            if (Debugger.IsAttached)
                IdentityModelEventSource.ShowPII = true;

            var resource = configuration.GetSection("KeyCloak:resource").Value;
            var realm = configuration.GetSection("KeyCloak:realm").Value;

            services.AddAuthentication(options =>
            {
                //options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                //options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                //options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

                //Sets cookie authentication scheme
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(cookie =>
            {
                //Sets the cookie name and maxage, so the cookie is invalidated.
                cookie.Cookie.Name = "keycloak.cookie";
                cookie.Cookie.MaxAge = TimeSpan.FromMinutes(60);
                cookie.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                cookie.SlidingExpiration = true;
            })
            .AddOpenIdConnect(options =>
            {
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                //Keycloak server
                options.Authority = "http://localhost:8080/realms/myrealm"; // configuration.GetSection("KeyCloak")["realm"];
                //Keycloak client ID
                options.ClientId = configuration.GetSection("KeyCloak")["resource"];
                //Keycloak client secret in user secrets for dev
                options.ClientSecret = configuration.GetSection("KeyCloak:credentials")["secret"];
                //Keycloak .wellknown config origin to fetch config
                options.MetadataAddress = "http://localhost:8080/realms/myrealm/.well-known/openid-configuration"; // configuration.GetSection("Keycloak")["Metadata"];
                //Require keycloak to use SSL

                options.GetClaimsFromUserInfoEndpoint = true;
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("email");
                options.SaveTokens = true;
                options.ResponseType = OpenIdConnectResponseType.Code;
                options.RequireHttpsMetadata = false; //dev
                options.NonceCookie.SameSite = SameSiteMode.None;
                options.CorrelationCookie.SameSite = SameSiteMode.None;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name",
                    RoleClaimType = ClaimTypes.Role,
                    ValidateIssuer = true
                };

                options.Events.OnRedirectToIdentityProvider = async context =>
                {
                    //context.ProtocolMessage.RedirectUri = "http://localhost:13636/home";
                    await Task.FromResult(0);
                };
            });

            services.AddAuthorization(options =>
            {
                //options.AddPolicy("Administrator", policy => policy.RequireClaim("user_roles", "[Administrator]"));
            });
        }
    }
}
