using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Hobby
{
    public class HobbyDTO
    {
        [JsonPropertyName("idHobby")]
        public long IdHobby { get; set; }

        [JsonPropertyName("descricao")]
        public string Descricao { get; set; }

        [JsonPropertyName("usuarioCriacaoId")]
        public long UsuarioCriacaoId { get; set; }
    }
}