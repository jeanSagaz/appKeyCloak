namespace KeyCloak.Services.Api.Models
{
    public class KeyCloakAppsettings
    {
        public string Realm { get; set; } = string.Empty;
        public string AuthServerUrl { get; set; } = string.Empty;
        public string SslRequired { get; set; } = string.Empty;
        public string Resource { get; set; } = string.Empty;
        public bool VerifyTokenAudience { get; set; }
        public string Secret { get; set; } = string.Empty;
    }
}
