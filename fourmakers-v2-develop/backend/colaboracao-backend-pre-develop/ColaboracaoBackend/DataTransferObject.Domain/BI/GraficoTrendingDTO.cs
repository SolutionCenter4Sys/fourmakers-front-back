using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.BI
{
    public class GraficoTrendingDTO
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
    }
}