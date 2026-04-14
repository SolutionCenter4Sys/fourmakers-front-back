using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Foursys
{
    public class IdiomaDTO
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("descricao")]
        public string Descricao { get; set; }
    }
}