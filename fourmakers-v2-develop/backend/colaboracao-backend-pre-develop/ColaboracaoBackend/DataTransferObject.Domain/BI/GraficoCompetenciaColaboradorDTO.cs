using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.BI
{
    public class GraficoCompetenciaColaboradorDTO
    {
        public GraficoCompetenciaColaboradorDTO()
        {
            CompetenciaColaboradorGraf = new List<InfoCompetenciaColaboradorGrafico>();
        }

        [JsonPropertyName("competenciaColaboradorGraf")]
        public List<InfoCompetenciaColaboradorGrafico> CompetenciaColaboradorGraf { get; set; }
    }
}