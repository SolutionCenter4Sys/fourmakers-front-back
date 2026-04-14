using System;
using System.Collections.Generic;

namespace DataTransferObject.Domain.Projeto.ProjetoOrg
{
    public class ProjetoOrgDTO
    {
        public string CodProjeto { get; set; }
        public string NomeProjeto { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public bool Prioritario { get; set; }
        public string CodOportunidade { get; set; }
        public int CodStatus { get; set; }
        public List<string> CodigoColaboradorGerente { get; set; }
        public List<string> CodigoColaborador { get; set; }
        public List<string> AtividadeProjeto { get; set; }
        public ClienteOrgDTO ClienteProjeto { get; set; }
        public int ApontSemAlocacao { get; set; }
        public int ApontSemAlocacaoOutroColab { get; set; }
    }
}