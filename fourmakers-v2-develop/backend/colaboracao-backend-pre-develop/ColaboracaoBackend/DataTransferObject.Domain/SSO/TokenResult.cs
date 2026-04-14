using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.SSO
{
    public class TokenResult
    {
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }
        [JsonPropertyName("scope")]
        public string Scope { get; set; }
        [JsonPropertyName("expires_in")]
        public long ExpiresIn { get; set; }
        [JsonPropertyName("ext_expires_in")]
        public long ExtExpiresIn { get; set; }
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }
        [JsonPropertyName("id_token")]
        public string IdToken { get; set; }
    }
}