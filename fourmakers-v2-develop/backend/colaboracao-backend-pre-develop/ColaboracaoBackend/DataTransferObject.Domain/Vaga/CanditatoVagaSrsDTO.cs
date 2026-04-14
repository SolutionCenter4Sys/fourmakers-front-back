using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Vaga
{
    public class CanditatoVagaSrsDTO
    {
        [JsonIgnore]
        [JsonPropertyName("idVaga")]
        public long? IdVaga { get; set; }
        [JsonPropertyName("candidatoNome")]
        public string CandidatoNome { get; set; }
        [JsonPropertyName("candidatoEmail")]
        public string CandidatoEmail { get; set; }

        [JsonPropertyName("hardSkills")]
        public string HardSkills { get; set; }

        [JsonPropertyName("softSkills")]
        public string SoftSkills { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("aderencia")]
        public double Aderencia { get; set; }
    }
}