using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.BI
{
    public class GraficoCadastroCandidatosDTO
    {
        public GraficoCadastroCandidatosDTO()
        {
            CandidatoGraf = new List<InfoCandidatoGrafico>();
        }

        [JsonPropertyName("candidatoGraf")]
        public List<InfoCandidatoGrafico> CandidatoGraf { get; set; }
    }
}