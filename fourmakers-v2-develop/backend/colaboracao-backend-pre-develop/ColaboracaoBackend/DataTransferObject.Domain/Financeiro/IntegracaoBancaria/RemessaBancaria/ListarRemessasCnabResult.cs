using System;
using System.Collections.Generic;

namespace DataTransferObject.Domain.Financeiro.IntegracaoBancaria.RemessaBancaria
{
    public class ListarRemessasCnabResult
    {
        public List<RemessaCnabDTO> Remessas { get; set; }
        public int TotalRemessas { get; set; }
    }

    public class RemessaCnabDTO
    {
        public Guid Id { get; set; }
        public string HashRemessa { get; set; }
        public string Tipo { get; set; }
        public string Status { get; set; }
        public string NomeArquivoRemessa { get; set; }
        public string NomeArquivoRetorno { get; set; }
        public decimal ValorTotal { get; set; }
        public DateTime DataCriacao { get; set; }
        public List<LancamentoCnabDTO> Lancamentos { get; set; }
    }

    public class LancamentoCnabDTO
    {
        public Guid Id { get; set; }
        public string CodigoInternoColaborador { get; set; }
        public string NomeColaborador { get; set; }
        public string CpfColaborador { get; set; }
        public decimal ValorPagamentoTotal { get; set; }
        public string FormaPagamento { get; set; }
        public string StatusCnab { get; set; }
        public string DescricaoErro { get; set; }
        public List<SolicitacaoPagamentoCnabDTO> Solicitacoes { get; set; }
    }

    public class SolicitacaoPagamentoCnabDTO
    {
        public Guid Id { get; set; }
        public string TipoSolicitacao { get; set; }
        public decimal ValorPagamento { get; set; }
        public string Descricao { get; set; }
        public DateTime DataSolicitacao { get; set; }
    }
}
