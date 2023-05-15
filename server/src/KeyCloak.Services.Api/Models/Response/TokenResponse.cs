using System.Text.Json.Serialization;

namespace KeyCloak.Services.Api.Models.Response
{
    public class TokenResponse
    {
        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }

        [JsonPropertyName("refresh_expires_in")]
        public double? RefreshTokenExpiresIn { get; set; }

        [JsonPropertyName("refresh_token")]
        public string? RefreshToken { get; set; }

        [JsonPropertyName("expires_in")]
        public double? ExpiresIn { get; set; }

        [JsonPropertyName("token_type")]
        public string? TokenType { get; set; }

        [JsonPropertyName("id_token")]
        public string? IdToken { get; set; }

        [JsonPropertyName("not-before-policy")]
        public double? NotBeforePolicy { get; set; }

        [JsonPropertyName("session_state")]
        public string? SessionState { get; set; }

        [JsonPropertyName("scope")]
        public string? Scope { get; set; }
    }
}
