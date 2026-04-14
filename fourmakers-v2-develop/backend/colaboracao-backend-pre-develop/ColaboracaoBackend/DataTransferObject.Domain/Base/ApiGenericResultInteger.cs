using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Base
{
    public class ApiGenericResultInteger : ApiGenericResult
    {
        [JsonPropertyName("retorno")]
        public int Retorno { get; set; }
    }
}