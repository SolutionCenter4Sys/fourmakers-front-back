using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Foursys
{
    public class UnidadesDTO
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("descricao")]
        public string Descricao { get; set; }
    }
}