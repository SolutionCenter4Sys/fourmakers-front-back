using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.BI
{
    public class ListaGraficoCompetenciaColaboradorResult : StatusResult
    {
        public ListaGraficoCompetenciaColaboradorResult()
        {
            DadosCompetenciaColaborador = new List<GraficoCompetenciaColaboradorDTO>();
        }
        [JsonPropertyName("dadosCompetenciaColaborador")]
        public List<GraficoCompetenciaColaboradorDTO> DadosCompetenciaColaborador { get; set; }
    }
}