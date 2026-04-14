using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Nivel
{
    public class ListaNivelResult : StatusResult
    {
        public ListaNivelResult()
        {
            Niveis = new List<NivelDTO>();
        }

        [JsonPropertyName("niveis")]
        public List<NivelDTO> Niveis { get; set; }
    }
}