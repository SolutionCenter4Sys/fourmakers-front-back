using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Filtro
{
    public class FiltroUnidadeDTO
    {
        [JsonIgnore]
        public long filtroId { get; set; }

        [JsonPropertyName("id")]
        public long UnidadeId { get; set; }

        [JsonPropertyName("descricao")]
        public string UnidadeDescricao { get; set; }
    }
}