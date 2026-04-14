using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Dependentes
{
    public class RemoverDependentesDTO
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("colaboradorCpf")]
        public string ColaboradorCpf { get; set; }
    }
}