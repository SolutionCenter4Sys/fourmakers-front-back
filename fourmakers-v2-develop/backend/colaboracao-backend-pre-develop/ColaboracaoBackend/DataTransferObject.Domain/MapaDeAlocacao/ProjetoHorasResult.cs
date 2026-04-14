using DataTransferObject.Domain.Base;
using System.Collections.Generic;

namespace DataTransferObject.Domain.VagasSRS
{
    public class ProjetoHorasResult : StatusResult
    {
        public List<ProjetoHorasDTO> ProjetoHorasCCH { get; set; }
    }
}