using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Nivel;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Metodologia
{
    public class ListaMetodologiaResult : StatusResult
    {
        [JsonPropertyName("metodologia")]
        public List<ItemPerfilDTO> Metodologia { get; set; }

        public List<NivelDTO> Nivel { get; set; }
    }
}