using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Dependentes
{
    public class DependentesResult : StatusResult
    {
        public DependentesResult()
        {
            DependentesDTO = new List<DependentesDTO>();
        }

        [JsonPropertyName("dependentes")]
        public List<DependentesDTO> DependentesDTO { get; set; }
    }
}