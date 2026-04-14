using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NewApiAppColaboracao.Models.BI
{
    public class ListaGraficoCompetenciaEndossoResult : StatusResult
    {
        public ListaGraficoCompetenciaEndossoResult()
        {
            DadosCompetencia = new List<GraficoCompetenciaEndossoDTO>();
        }
        [JsonPropertyName("dadosCompetencia")]
        public List<GraficoCompetenciaEndossoDTO> DadosCompetencia { get; set; }
    }
}