using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Noticia
{
    public class ListaNoticiaResult : StatusResult
    {
        public ListaNoticiaResult()
        {
            Noticia = new List<NoticiaDTO>();
        }

        [JsonPropertyName("noticia")]
        public List<NoticiaDTO> Noticia { get; set; }

        [JsonPropertyName("filteredResultCount")]
        public int FilteredResultCount { get; set; }

        [JsonPropertyName("totalResultCount")]
        public int TotalResultCount { get; set; }
    }
}