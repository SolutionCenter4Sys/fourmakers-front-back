using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Hobby
{
    public class ListaHobbyResult : StatusResult
    {
        [JsonPropertyName("hobbie")]
        public List<ItemPerfilDTO> Hobbie { get; set; }
    }
}