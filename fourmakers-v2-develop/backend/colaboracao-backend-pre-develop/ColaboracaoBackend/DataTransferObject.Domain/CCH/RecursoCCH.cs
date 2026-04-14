using System;

namespace DataTransferObject.Domain.CCH
{
    public class RecursoCCH
    {
        public int cdProfissional { get; set; }
        public string nmProfissional { get; set; }
        public int cdCargo { get; set; }
        public string nmCargo { get; set; }
        public string nmLogin { get; set; }
        public string nmLoginRede { get; set; }
        public string nmEnderecoEletronico { get; set; }
        public string cdRg { get; set; }
        public string cdCpf { get; set; }
        public int cdProfissionalSuperior { get; set; }
        public string nmProfissionalSuperior { get; set; }
        public int cdDivisao { get; set; }
        public string nmDivisao { get; set; }
        public int cdTipoContratacao { get; set; }
        public string nmTipoContratacao { get; set; }
        public string statusFuncionario { get; set; }
        public bool flAlocado { get; set; }
        public bool flFuncionarioAtivo { get; set; }
        public DateTime? dtContratacao { get; set; }
        public DateTime? dtDesligamento { get; set; }
    }
}