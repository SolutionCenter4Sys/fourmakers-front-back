using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Colaborador
{
    public class VistoColaboradorResult : StatusResult
    {
        [JsonPropertyName("vistoColaborador")]
        public IEnumerable<VistoColaboradorDTO> VistosColaborador { get; set; }
    }
}