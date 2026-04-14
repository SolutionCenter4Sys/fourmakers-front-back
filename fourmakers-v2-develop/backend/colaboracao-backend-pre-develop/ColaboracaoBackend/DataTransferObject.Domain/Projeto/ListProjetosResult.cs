using DataTransferObject.Domain.Base;
using System.Collections.Generic;

namespace DataTransferObject.Domain.Projeto
{
    public class ListProjetosResult : StatusResult
    {
        public List<ProjetoDTO> Projetos { get; set; }
    }
}