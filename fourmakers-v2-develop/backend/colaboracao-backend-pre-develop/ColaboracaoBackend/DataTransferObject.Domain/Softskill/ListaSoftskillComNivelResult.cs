using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Nivel;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Softskill
{
    public class ListaSoftskillComNivelResult : StatusResult
    {
        [JsonPropertyName("retorno")]
        public List<SoftskillDTO> Retorno { get; set; }

        public List<NivelDTO> Nivel { get; set; }
    }
}