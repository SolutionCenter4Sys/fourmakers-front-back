using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.BI
{
    public class TrendingInfoViewDTO
    {
        [JsonPropertyName("resultadoBusca")]
        public string ResultadoBusca { get; set; }
        [JsonPropertyName("dataBusca")]
        public string DataConsulta { get; set; }
        [JsonPropertyName("totalConsultas")]
        public long TotalConsultas { get; set; }
    }
}