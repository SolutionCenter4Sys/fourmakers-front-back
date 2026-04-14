using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.SRS.Candidate
{
    public class CadastroCandidatoInputResponse
    {
        public string? Mensagem { get; set; }
        public string UrlLinkedin { get; set; }
        public string Email { get; set; }
        public string UserId { get; set; }

        [JsonPropertyName("candidateId"), JsonProperty()]
        public int? CandidateId { get; set; }
    }
}