using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.SSO
{
    public class TokenSSO
    {
        [JsonPropertyName("token")]
        public string Token { get; set; }
        [JsonIgnore]
        public string Cpf { get; set; }
        [JsonIgnore]
        public string RefreshToken { get; set; }
    }
}