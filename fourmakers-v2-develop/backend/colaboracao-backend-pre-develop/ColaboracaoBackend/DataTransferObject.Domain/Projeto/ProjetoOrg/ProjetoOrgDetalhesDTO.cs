using System;
using System.Collections.Generic;

namespace DataTransferObject.Domain.Projeto.ProjetoOrg
{
    public class ProjetoOrgDetalhesDTO
    {
        public string CodProjeto { get; set; }
        public string NomeProjeto { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public decimal QuantidadeHorasPlanejadas { get; set; }
        public bool Prioritario { get; set; }
        public string CodOportunidade { get; set; }
        public int CodStatus { get; set; }
        public int ApontSemAlocacao { get; set; }
        public int ApontSemAlocacaoOutroColab { get; set; }
        public List<ProjetoOrgColaboradorGerenteDetalhesDTO> ColaboradoresGerentes { get; set; }
        public List<ProjetoOrgColaboradoresDetalhesDTO> Colaboradores { get; set; }
        public List<string> AtividadeProjeto { get; set; }
        public ClienteOrgDTO ClienteProjeto { get; set; }
        public string CodClienteRegistroCarga { get; set; }
        public string NomeClienteRegistroCarga { get; set; }
        public string TipoCadastro { get; set; }
    }
}