using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Interesse
{
    public class ListaInteresseColaboradorResult : StatusResult
    {
        [JsonPropertyName("interesses")]
        public List<InteresseColaboradorDTO> Interesses { get; set; }
    }
}