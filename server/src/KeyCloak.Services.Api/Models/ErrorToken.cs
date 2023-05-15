using System.Text.Json.Serialization;

namespace KeyCloak.Services.Api.Models
{
    public class ErrorToken
    {
        [JsonPropertyName("error")]
        public string Error { get; set; }

        [JsonPropertyName("error_description")]
        public string Description { get; set; }
    }
}
