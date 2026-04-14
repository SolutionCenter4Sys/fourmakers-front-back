using System.Text.Json.Serialization;

namespace Usuario.API.DTOs
{
    public class EnviarEmailParam
    {
        [JsonPropertyName("email")]
        public string Email { get; set; }
    }
}