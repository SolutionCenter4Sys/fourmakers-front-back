using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.BI
{
    public class ColaboradorCompletoResult : StatusResult
    {
        [JsonPropertyName("colaboradores")]
        public List<ColaboradorCompletoDTO> Colaboradores { get; set; }
    }
}