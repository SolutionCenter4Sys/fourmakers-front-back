using DataTransferObject.Domain.Base;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Noticia
{
    public class NoticiaResult : StatusResult
    {
        [JsonPropertyName("noticia")]
        public NoticiaDTO Noticia { get; set; }
    }
}