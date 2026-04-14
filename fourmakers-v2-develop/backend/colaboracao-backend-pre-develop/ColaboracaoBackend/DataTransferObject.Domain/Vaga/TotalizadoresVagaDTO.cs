using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Vaga
{
    public class TotalizadoresVagaDTO
    {
        [JsonPropertyName("candidaturas")]
        public string Candidaturas { get; set; }

        [JsonPropertyName("aprovados")]
        public string Aprovados { get; set; }
        [JsonPropertyName("reprovados")]
        public string Reprovados { get; set; }
    }
}