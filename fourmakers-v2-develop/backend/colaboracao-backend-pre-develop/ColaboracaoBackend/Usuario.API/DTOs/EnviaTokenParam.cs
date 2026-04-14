using System.Text.Json.Serialization;

namespace Usuario.API.DTOs
{
    public class EnviaTokenParam
    {
        [JsonPropertyName("email")]
        public string email { get; set; }

        public int orgId { get; set; }

        public bool forceCodigoEmail { get; set; } = false;
    }
}