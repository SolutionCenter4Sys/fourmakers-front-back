using System.Text.Json.Serialization;

namespace NewApiAppColaboracao.Models.BI
{
    public class GraficoCompetenciaEndossoDTO
    {
        public GraficoCompetenciaEndossoDTO()
        {
            CompetenciaGraf = new InfoCompetenciaGrafico();
        }

        [JsonPropertyName("competenciaGraf")]
        public InfoCompetenciaGrafico CompetenciaGraf { get; set; }
    }
}