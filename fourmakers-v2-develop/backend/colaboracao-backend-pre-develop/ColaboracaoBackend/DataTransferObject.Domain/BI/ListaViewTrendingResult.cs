using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.BI
{
    public class ListaViewTrendingResult : StatusResult
    {
        public ListaViewTrendingResult()
        {
            DadosViewTrending = new List<TrendingInfoViewDTO>();
        }
        [JsonPropertyName("dadosViewTrending")]
        public List<TrendingInfoViewDTO> DadosViewTrending { get; set; }
    }
}