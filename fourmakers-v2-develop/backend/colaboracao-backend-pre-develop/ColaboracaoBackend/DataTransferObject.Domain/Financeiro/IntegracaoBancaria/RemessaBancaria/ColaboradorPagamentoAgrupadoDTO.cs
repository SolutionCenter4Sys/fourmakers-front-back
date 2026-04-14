using System.Collections.Generic;

namespace DataTransferObject.Domain.Financeiro.IntegracaoBancaria.RemessaBancaria
{
    public class ColaboradorPagamentoAgrupadoDTO
    {
        public string CodigoInternoColaborador { get; set; }
        public List<string> SolicitacaoPagamentoIds { get; set; }
        public Dictionary<string, decimal> SolicitacaoValores { get; set; } // Key: SolicitacaoId, Value: Valor
        public decimal ValorTotal { get; set; }
        public DadosBancariosColaboradorDTO? DadosBancarios { get; set; }
    }
}
