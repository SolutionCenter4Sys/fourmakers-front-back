using DataTransferObject.Domain.Base;
using System.Collections.Generic;

namespace DataTransferObject.Domain.Colaborador
{
    public class VincularItemPerfilResult : StatusResult
    {
        public List<ItemPerfilResult> Respostas { get; set; }
    }
}