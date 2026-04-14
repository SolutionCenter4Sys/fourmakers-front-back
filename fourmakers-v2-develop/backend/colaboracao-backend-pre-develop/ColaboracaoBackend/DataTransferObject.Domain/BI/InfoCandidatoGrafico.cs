using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.BI
{
    public class InfoCandidatoGrafico
    {
        [JsonPropertyName("quantidade")]
        public long Quantidade { get; set; }
        [JsonPropertyName("mes")]
        public int? Mes { get; set; }
        [JsonPropertyName("ano")]
        public int? Ano { get; set; }
    }
}