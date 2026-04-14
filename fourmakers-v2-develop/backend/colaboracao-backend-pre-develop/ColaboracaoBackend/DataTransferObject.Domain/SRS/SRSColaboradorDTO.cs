using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.SRS
{
    public class SRSColaboradorDTO
    {
        [JsonPropertyName("candidate_id")]
        public int CandidateId { get; set; }
        [JsonPropertyName("first_name")]
        public string FirstName { get; set; }
        [JsonPropertyName("cand_cpf")]
        public string CandidateCpf { get; set; }
        [JsonPropertyName("emailFoursys")]
        public string Email { get; set; }
    }
}