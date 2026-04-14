using DataTransferObject.Domain.Base;

namespace DataTransferObject.Domain.Colaborador
{
    public class ComentarioResult : StatusResult
    {
        public ComentarioDTO Comentario { get; set; }
    }
}