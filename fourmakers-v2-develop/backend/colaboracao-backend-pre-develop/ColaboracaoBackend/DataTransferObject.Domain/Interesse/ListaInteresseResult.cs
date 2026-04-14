using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Interesse
{
    public class ListaInteresseResult : StatusResult
    {
        [JsonPropertyName("interesse")]
        public List<ItemPerfilDTO> Interesse { get; set; }
    }
}