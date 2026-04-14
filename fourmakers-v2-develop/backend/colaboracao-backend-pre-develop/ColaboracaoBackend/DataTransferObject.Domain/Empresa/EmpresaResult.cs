using DataTransferObject.Domain.Base;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Empresa
{
    public class EmpresaResult : StatusResult
    {
        [JsonPropertyName("empresa")]
        public EmpresaDTO Empresa { get; set; }
    }
}