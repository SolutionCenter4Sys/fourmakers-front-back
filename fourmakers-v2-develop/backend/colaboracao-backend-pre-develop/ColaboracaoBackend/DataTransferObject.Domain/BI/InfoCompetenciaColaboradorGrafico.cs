using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.BI
{
    public class InfoCompetenciaColaboradorGrafico
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        [JsonPropertyName("descricao")]
        public string Descricao { get; set; }
        [JsonPropertyName("quantidade")]
        public long Quantidade { get; set; }
    }
}