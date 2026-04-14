using DataTransferObject.Domain.Base;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Endosso
{
    public class StatusEndossoResult : StatusResult
    {
        [JsonPropertyName("endosso")]
        public StatusEndossoDTO Endosso { get; set; }
    }
}