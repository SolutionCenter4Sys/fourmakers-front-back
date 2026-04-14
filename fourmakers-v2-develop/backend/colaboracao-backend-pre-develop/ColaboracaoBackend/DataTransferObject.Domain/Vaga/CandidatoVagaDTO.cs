using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Vaga
{
    public class CandidatoVagaDTO
    {
        [JsonIgnore]
        [JsonPropertyName("vagaId")]
        public int VagaId { get; set; }
        [JsonIgnore]
        [JsonPropertyName("candidatoId")]
        public int CandidatoId { get; set; }
        [JsonPropertyName("candidatoNome")]
        public string CandidatoNome { get; set; }
        [JsonPropertyName("candidatoEmail")]
        public string CandidatoEmail { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; }
    }
}