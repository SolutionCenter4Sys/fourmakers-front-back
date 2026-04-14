using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Metodologia
{
    public class MetodologiaColaboradorResult : StatusResult
    {
        [JsonPropertyName("respostas")]
        public List<ItemPerfilResult> Respostas { get; set; }
    }
}