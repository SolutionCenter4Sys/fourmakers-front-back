using System.Text.Json.Serialization;

namespace Usuario.API.DTOs
{
    public class SubmeterConviteEmpresaParam
    {
        [JsonPropertyName("cnpj")]
        public string Cnpj { get; set; }
        [JsonPropertyName("confirmado")]
        public bool Confirmado { get; set; }
    }
}