using DataTransferObject.Domain.Base;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Comentario
{
    public class ComentarioResult : StatusResult
    {
        public ComentarioResult()
        {
            Comentario = new ComentarioDTO();
        }

        [JsonPropertyName("comentario")]
        public ComentarioDTO Comentario { get; set; }
    }
}