using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Colaborador
{
    public class NacionalidadeResult : StatusResult
    {
        [JsonPropertyName("nacionalidades")]
        public List<NacionalidadeDTO> Nacionalidade { get; set; }
    }
}