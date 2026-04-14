using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.CRM
{
    public class CRMContatoResponsavelOutputDTO
    {
        [JsonPropertyName("nome")]
        public string Nome { get; set; }

        [JsonPropertyName("sobrenome")]
        public string SobreNome { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("celular")]
        public string Celular { get; set; }
    }
}