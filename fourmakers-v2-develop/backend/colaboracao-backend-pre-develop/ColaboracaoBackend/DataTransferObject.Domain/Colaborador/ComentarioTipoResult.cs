using DataTransferObject.Domain.Base;
using System.Collections.Generic;

namespace DataTransferObject.Domain.Colaborador
{
    public class ComentarioTipoResult : StatusResult
    {
        public List<ComentarioTipoDTO> ListComentarioTipo { get; set; }
    }
}