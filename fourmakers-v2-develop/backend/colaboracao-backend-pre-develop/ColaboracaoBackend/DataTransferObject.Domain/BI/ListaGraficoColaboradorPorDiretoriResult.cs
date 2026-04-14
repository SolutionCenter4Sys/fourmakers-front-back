using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.BI
{
    public class ListaGraficoColaboradorPorDiretoriResult : StatusResult
    {
        public ListaGraficoColaboradorPorDiretoriResult()
        {
            DadosDiretoria = new List<GraficoColaboradorPorDiretoriaDTO>();
        }
        [JsonPropertyName("dadosDiretoria")]
        public List<GraficoColaboradorPorDiretoriaDTO> DadosDiretoria { get; set; }
    }
}