using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Comentario
{
    public class ListaComentarioResult : StatusResult
    {
        public ListaComentarioResult()
        {
            Comentario = new List<ComentarioDTO>();
        }

        [JsonPropertyName("comentarios")]
        public List<ComentarioDTO> Comentario { get; set; }

        [JsonPropertyName("totalResultCount")]
        public int TotalResultCount { get; set; }

        [JsonPropertyName("filteredResultCount")]
        public int FilteredResultCount { get; set; }
    }
}