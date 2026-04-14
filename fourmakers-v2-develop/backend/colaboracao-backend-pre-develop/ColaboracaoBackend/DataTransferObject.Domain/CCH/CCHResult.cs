using DataTransferObject.Domain.Base;
using System.Collections.Generic;

namespace DataTransferObject.Domain.CCH
{
    public class CCHResult : StatusResult
    {
        public List<ColaboradorCCH> Colaboradores { get; set; }
        public RecursoCCH Recurso { get; set; }
        public List<ProjetoRecursoCCH> ProjetosRecurso { get; set; }
    }
}