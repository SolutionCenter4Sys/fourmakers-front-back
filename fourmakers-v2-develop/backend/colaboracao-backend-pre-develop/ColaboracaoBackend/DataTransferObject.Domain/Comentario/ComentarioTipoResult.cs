using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Comentario
{
    public class ComentarioTipoResult : StatusResult
    {
        public ComentarioTipoResult()
        {
            ListComentarioTipo = new List<ComentarioTipoDTO>();
        }
        [JsonPropertyName("listComentarioTipo")]
        public List<ComentarioTipoDTO> ListComentarioTipo { get; set; }
    }
}