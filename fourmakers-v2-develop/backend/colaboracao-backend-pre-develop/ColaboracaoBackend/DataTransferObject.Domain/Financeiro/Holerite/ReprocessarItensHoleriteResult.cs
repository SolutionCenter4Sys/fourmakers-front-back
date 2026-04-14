using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Financeiro.Holerite
{
    public class ReprocessarItensHoleriteResult
    {
        [JsonPropertyName("totalSolicitados")]
        public int TotalSolicitados { get; set; }

        [JsonPropertyName("totalEnfileirados")]
        public int TotalEnfileirados { get; set; }

        [JsonPropertyName("itensNaoEncontrados")]
        public List<string> ItensNaoEncontrados { get; set; }
    }
}
