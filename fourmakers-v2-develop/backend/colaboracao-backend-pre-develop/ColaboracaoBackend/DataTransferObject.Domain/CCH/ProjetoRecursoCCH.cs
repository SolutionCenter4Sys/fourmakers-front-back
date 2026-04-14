using System;

namespace DataTransferObject.Domain.CCH
{
    public class ProjetoRecursoCCH
    {
        public int cdProjeto { get; set; }
        public string nmProjeto { get; set; }
        public string nmCliente { get; set; }
        public int cdDivisao { get; set; }
        public string nmDivisao { get; set; }
        public int? cdGerenteSenior { get; set; }
        public string nmGerenteSenior { get; set; }
        public int cdGerenteExecutivo { get; set; }
        public string nmGerenteExecutivo { get; set; }
        public int cdGerenteProjeto { get; set; }
        public string nmGerenteProjeto { get; set; }
        public int cdTipoProjeto { get; set; }
        public string nmTipoProjeto { get; set; }
        public int cdTipoServico { get; set; }
        public string nmTipoServico { get; set; }
        public int? cdAreaConhecimento { get; set; }
        public string nmAreaConhecimento { get; set; }
        public DateTime dtInicioProjeto { get; set; }
        public DateTime? dtFimProjeto { get; set; }
        public object dtReplanejamento { get; set; }
        public DateTime? dtInicioDesenvolvimento { get; set; }
        public DateTime? dtFimDesenvolvimento { get; set; }
    }
}