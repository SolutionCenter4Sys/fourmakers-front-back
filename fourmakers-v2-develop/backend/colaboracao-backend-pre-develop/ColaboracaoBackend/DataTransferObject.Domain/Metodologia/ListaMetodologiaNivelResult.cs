using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Nivel;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Metodologia
{
    public class ListaMetodologiaNivelResult : StatusResult
    {
        [JsonPropertyName("niveis")]
        public List<NivelDTO> Niveis { get; set; }
    }
}