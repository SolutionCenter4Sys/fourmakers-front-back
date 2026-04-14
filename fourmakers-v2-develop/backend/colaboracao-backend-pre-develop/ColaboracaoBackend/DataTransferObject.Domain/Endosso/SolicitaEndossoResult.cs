using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Endosso
{
    public class SolicitaEndossoResult : StatusResult
    {
        [JsonPropertyName("respostas")]
        public List<StatusEndossoResult> Respostas { get; set; }
    }
}