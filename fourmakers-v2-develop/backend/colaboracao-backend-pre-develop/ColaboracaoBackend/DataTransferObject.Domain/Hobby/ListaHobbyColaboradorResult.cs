using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Hobby
{
    public class ListaHobbyColaboradorResult : StatusResult
    {
        [JsonPropertyName("hobbies")]
        public List<HobbyColaboradorDTO> Hobbies { get; set; }
    }
}