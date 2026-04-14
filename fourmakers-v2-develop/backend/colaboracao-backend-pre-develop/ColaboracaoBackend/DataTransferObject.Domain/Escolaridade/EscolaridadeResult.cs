using DataTransferObject.Domain.Base;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Escolaridade
{
    public class EscolaridadeResult : StatusResult
    {
        [JsonPropertyName("Escolaridade")]
        public EscolaridadeDTO escolaridade { get; set; }
    }
}