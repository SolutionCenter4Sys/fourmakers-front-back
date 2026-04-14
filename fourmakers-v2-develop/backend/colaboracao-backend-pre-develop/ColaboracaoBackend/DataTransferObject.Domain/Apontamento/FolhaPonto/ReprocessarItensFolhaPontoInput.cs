using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Apontamento.FolhaPonto
{
    public class ReprocessarItensFolhaPontoInput
    {
        [JsonPropertyName("itensLoteId")]
        public List<string> ItensLoteId { get; set; }
    }
}
