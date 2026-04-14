using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Nivel;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Dominio
{
    public class ListaDominioResult : StatusResult
    {
        public ListaDominioResult()
        {
            Dominio = new List<ItemPerfilDTO>();
        }

        [JsonPropertyName("dominio")]
        public List<ItemPerfilDTO> Dominio { get; set; }

        public List<NivelDTO> Nivel { get; set; }
    }
}