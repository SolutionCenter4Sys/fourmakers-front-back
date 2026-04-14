using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.SRS.Candidate
{
    public class AtualizaCadastroCandidatoInputResponse
    {
        public string? Mensagem { get; set; }

        [JsonPropertyName("candidateId"), JsonProperty()]
        public int? CandidateId { get; set; }
    }
}