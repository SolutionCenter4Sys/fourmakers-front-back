using DataTransferObject.Domain.Base;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.LG
{
    public class StatusPaginacaoLGResult : StatusResult
    {
        [JsonPropertyName("numeroDaPagina")]
        public int NumeroDaPagina { get; set; }

        [JsonPropertyName("quantidadePorPagina")]
        public int QuantidadePorPagina { get; set; }

        [JsonPropertyName("totalDePaginas")]
        public int TotalDePaginas { get; set; }

        [JsonPropertyName("totalGeral")]
        public int TotalGeral { get; set; }
    }
}