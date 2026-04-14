using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Financeiro.Holerite
{
    public class ReprocessarItensHoleriteInput
    {
        [JsonPropertyName("itensLoteId")]
        public List<string> ItensLoteId { get; set; }
    }
}
