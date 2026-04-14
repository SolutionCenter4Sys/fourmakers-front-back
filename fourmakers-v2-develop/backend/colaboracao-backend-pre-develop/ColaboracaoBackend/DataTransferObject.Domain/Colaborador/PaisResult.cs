using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Colaborador
{
    public class PaisResult : StatusResult
    {
        [JsonPropertyName("paises")]
        public List<PaisDTO> Pais { get; set; }
    }
}