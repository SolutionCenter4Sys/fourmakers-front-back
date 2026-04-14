using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Competencia
{
    public class ListaCompetenciaColaboradorResult : StatusResult
    {
        public ListaCompetenciaColaboradorResult()
        {
            Competencia = new List<CompetenciaColaboradorDTO>();
        }

        [JsonPropertyName("competencias")]
        public List<CompetenciaColaboradorDTO> Competencia { get; set; }
    }
}