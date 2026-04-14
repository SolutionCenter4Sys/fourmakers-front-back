using System.Text.Json.Serialization;
using Logs.Infra.Attributes;

namespace Usuario.API.DTOs
{
    public class ValidaTokenParam
    {
        [JsonPropertyName("email")]
        public string email { get; set; }
        
        [JsonPropertyName("token")]
        [LogMasked]
        public string token { get; set; }
        
        public int orgId { get; set; }
    }
}