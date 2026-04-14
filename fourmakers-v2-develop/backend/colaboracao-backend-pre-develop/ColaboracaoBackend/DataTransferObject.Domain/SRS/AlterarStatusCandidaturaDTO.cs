using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.SRS
{
    public class AlterarStatusCandidaturaDTO
    {
        [JsonPropertyName("candidate_id")]
        public int Candidate_id { get; set; }
        [JsonPropertyName("joborder_id")]
        public int Joborder_id { get; set; }
        [JsonPropertyName("status_id")]
        public int Status_id { get; set; }
        [JsonPropertyName("nome_analista")]
        public string nomeAnalista { get; set; }
    }
}