using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Softskill
{
    public class SoftskillDTO
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        [JsonPropertyName("descricao")]
        public string Descricao { get; set; }
        [JsonPropertyName("pendente")]
        public bool Pendente { get; set; }
        [JsonPropertyName("usuarioCriacaoId")]
        public long UsuarioCriacaoId { get; set; }
    }
}