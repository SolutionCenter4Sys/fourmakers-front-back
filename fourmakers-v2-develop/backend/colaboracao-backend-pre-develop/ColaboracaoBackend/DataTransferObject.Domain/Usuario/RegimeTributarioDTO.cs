using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Usuario
{
    public class RegimeTributarioDTO
    {
        [JsonPropertyName("descricao")]
        public int Descricao { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }
    }
}