using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.SRS.Candidate
{
    public class AtualizaCadastroCandidatoInput
    {
        [JsonPropertyName("candidateId")]
        public int CandidateId { get; set; }
    }
}