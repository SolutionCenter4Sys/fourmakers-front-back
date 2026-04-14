using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Comentario
{
    public class ListaComentarioTipoResult : StatusResult
    {
        public ListaComentarioTipoResult()
        {
            ComentarioTipoDTO = new List<ComentarioTipoDTO>();
        }
        [JsonPropertyName("comentarioTipo")]
        public List<ComentarioTipoDTO> ComentarioTipoDTO { get; set; }
    }
}