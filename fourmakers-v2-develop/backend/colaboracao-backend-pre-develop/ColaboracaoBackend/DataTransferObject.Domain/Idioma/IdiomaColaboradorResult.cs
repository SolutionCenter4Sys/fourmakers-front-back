using DataTransferObject.Domain.Base;
using System.Collections.Generic;

namespace DataTransferObject.Domain.Idioma
{
    public class IdiomaColaboradorResult : StatusResult
    {
        public List<ItemPerfilResult> Respostas { get; set; }
    }
}