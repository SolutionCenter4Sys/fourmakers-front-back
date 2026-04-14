using DataTransferObject.Domain.Base;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Colaborador
{
    public class SimpleColaboradorResult : StatusResult
    {
        [JsonPropertyName("colaborador")]
        public ColaboradorDTO Colaborador { get; set; }
    }
}