using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace KeyCloak.Services.Api.Models.Request
{
    public class RefreshTokenRequest
    {
        [JsonPropertyName("refresh_token")]
        [Required(ErrorMessage = "RefreshToken deve ser informado.")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
