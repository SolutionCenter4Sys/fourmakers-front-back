using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.BI
{
    public class ListaGraficoTrendingResult : StatusResult
    {
        [JsonPropertyName("dadosTrending")]
        public List<GraficoTrendingDTO> DadosTrending { get; set; }
    }
}