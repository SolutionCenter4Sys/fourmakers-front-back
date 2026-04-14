using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.BI
{
    public class GraficoCompetenciaCandidatoDTO
    {
        public GraficoCompetenciaCandidatoDTO()
        {
            CompetenciaCandidatoGraf = new List<InfoCompetenciaCandidatoGrafico>();
        }

        [JsonPropertyName("competenciaCandidatoGraf")]
        public List<InfoCompetenciaCandidatoGrafico> CompetenciaCandidatoGraf { get; set; }
    }
}