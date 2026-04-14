using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Interesse
{
    public class InteresseDTO
    {
        [JsonPropertyName("idInteresse")]
        public long IdInteresse { get; set; }

        [JsonPropertyName("descricao")]
        public string Descricao { get; set; }

        [JsonPropertyName("usuarioCriacaoId")]
        public long UsuarioCriacaoId { get; set; }
    }
}