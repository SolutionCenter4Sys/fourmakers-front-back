using System.Collections.Generic;

namespace DataTransferObject.Domain.Projeto.ProjetoOrg
{
    public class ProjetoComPropostaDTO
    {
        public string CodigoProjeto { get; set; }
        public string NomeProjeto { get; set; }
        public string CodCliente { get; set; }
        public List<string> CodigosProposta { get; set; }
    }
}