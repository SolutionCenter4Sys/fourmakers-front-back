using DataTransferObject.Domain.Base;
using System.Collections.Generic;

namespace DataTransferObject.Domain.Dominio
{
    public class ListaDominioInfo : StatusResult
    {
        public List<DominioDTO> Dominio { get; set; }
    }
}