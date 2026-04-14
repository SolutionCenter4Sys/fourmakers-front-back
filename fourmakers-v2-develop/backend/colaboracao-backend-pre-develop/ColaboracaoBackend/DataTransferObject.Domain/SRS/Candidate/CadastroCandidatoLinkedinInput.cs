using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.SRS.Candidate
{
    public class CadastroCandidatoLinkedinInput
    {
        [JsonPropertyName("urlLinkedin")]
        public string UrlLinkedin { get; set; }
        [JsonPropertyName("email")]
        public string Email { get; set; }
        [JsonPropertyName("telefone")]
        public string Telefone { get; set; }
    }
}