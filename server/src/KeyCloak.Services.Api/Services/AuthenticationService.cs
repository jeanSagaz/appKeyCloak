using Core.Notifications;
using Core.Notifications.Interfaces;
using KeyCloak.Services.Api.Models;
using KeyCloak.Services.Api.Models.Request;
using KeyCloak.Services.Api.Models.Response;
using Microsoft.Extensions.Options;

namespace KeyCloak.Services.Api.Services
{
    public interface IAuthenticationService
    {
        Task<TokenResponse?> GetToken(LoginRequest loginUser, string realm);
        Task<TokenResponse?> GetRefreshToken(string refreshToken, string realm);
    }

    public class AuthenticationService : Service, IAuthenticationService
    {
        private readonly HttpClient _httpClient;
        private readonly IDomainNotifier _domainNotifier;
        private readonly KeyCloakAppsettings _keyCloakAppsettings;

        public AuthenticationService(IHttpClientFactory httpClientFactory,
            IOptions<KeyCloakAppsettings> keyCloakAppsettings,
            IDomainNotifier domainNotifier)
        {
            var httpClient = httpClientFactory.CreateClient(nameof(AuthenticationService));
            //httpClient.BaseAddress = new Uri(settings.Value.AuthServerUrl);

            _httpClient = httpClient;
            _keyCloakAppsettings = keyCloakAppsettings.Value;
            _domainNotifier = domainNotifier;
        }

        public async Task<TokenResponse?> GetToken(LoginRequest login, string realm)
        {
            var dictionary = new Dictionary<string, string>();
            dictionary.Add("grant_type", "password");
            dictionary.Add("scope", "openid");
            //dictionary.Add("client_id", _keyCloak.Resource);
            dictionary.Add("client_id", "client-api");
            dictionary.Add("username", login.UserName);
            dictionary.Add("password", login.Password);
            //dictionary.Add("client_secret", _keyCloak.Secret);
            dictionary.Add("client_secret", "fT60s3qHhzg3pM5O7tr99sXX4H1iEMAq");

            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{_keyCloakAppsettings.AuthServerUrl}realms/{realm}/protocol/openid-connect/token"),
                Content = new FormUrlEncodedContent(dictionary)
            };

            var response = await _httpClient.SendAsync(httpRequestMessage);

            if (!HandlingErrorsResponse(response))
            {
                var result = await DeserializeResponseObject<ErrorToken>(response);
                _domainNotifier.Add(new DomainNotification(result.Error, result.Description ?? result.Error));

                return null;
            }
            
            return await DeserializeResponseObject<TokenResponse>(response);
        }

        public async Task<TokenResponse?> GetRefreshToken(string refreshToken, string realm)
        {
            var uri = new Uri($"{_keyCloakAppsettings.AuthServerUrl}realms/{realm}/protocol/openid-connect/token");

            var dictionary = new Dictionary<string, string>
            {
                {"grant_type", "refresh_token"},
                {"client_id", _keyCloakAppsettings.Resource},
                {"client_secret", _keyCloakAppsettings.Secret},
                {"refresh_token", refreshToken }
            };

            var response = await _httpClient.PostAsync(uri, new FormUrlEncodedContent(dictionary));

            if (!HandlingErrorsResponse(response))
            {
                var result = await DeserializeResponseObject<ErrorToken>(response);
                _domainNotifier.Add(new DomainNotification(result.Error, result.Description ?? result.Error));

                return null;
            }

            return await DeserializeResponseObject<TokenResponse>(response);
        }
    }
}
