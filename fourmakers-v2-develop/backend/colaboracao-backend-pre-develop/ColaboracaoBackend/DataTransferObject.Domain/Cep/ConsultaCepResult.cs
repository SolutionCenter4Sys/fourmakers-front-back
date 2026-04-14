using DataTransferObject.Domain.Base;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Cep
{
    public class ConsultaCepResult : StatusResult
    {
        [JsonPropertyName("cep")]
        public ConsultaCepDTO Cep { get; set; }
    }
}