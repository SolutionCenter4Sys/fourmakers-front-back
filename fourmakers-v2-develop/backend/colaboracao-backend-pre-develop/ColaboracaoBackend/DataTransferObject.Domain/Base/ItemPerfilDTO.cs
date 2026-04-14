using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Base
{
    public class ItemPerfilDTO
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        [JsonPropertyName("nome")]
        public string Nome { get; set; }
        [JsonPropertyName("descricao")]
        public string Descricao { get; set; }
        [JsonPropertyName("pendente")]
        public bool Pendente { get; set; }
    }
}