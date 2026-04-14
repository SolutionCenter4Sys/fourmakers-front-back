using System.Text.Json.Serialization;

namespace NewApiAppColaboracao.Models.BI
{
    public class InfoCompetenciaGrafico
    {
        public InfoCompetenciaGrafico()
        {
            EndossoGraf = new InfoEndossoGrafico();
        }

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("descricao")]
        public string Descricao { get; set; }

        [JsonPropertyName("quantidade")]
        public long Quantidade { get; set; }

        [JsonPropertyName("total")]
        public long? Total { get; set; }

        [JsonPropertyName("endossoGraf")]
        public InfoEndossoGrafico EndossoGraf { get; set; }
    }
}