using DataTransferObject.Domain.Financeiro.Reembolso.ControleDeSaldo;
using System.Collections.Generic;

namespace DataTransferObject.Domain.Financeiro.IntegracaoBancaria.RemessaBancaria
{
    public class ListarSolicitacoesPagamentoCNABResult
    {
        /// <summary>
        /// Lista de pagamentos agrupados por colaborador com solicitações discriminadas
        /// </summary>
        public List<ColaboradorComSolicitacoesDTO> Colaboradores { get; set; }

        /// <summary>
        /// Total geral de solicitações
        /// </summary>
        public int TotalSolicitacoes { get; set; }

        /// <summary>
        /// Total geral de colaboradores
        /// </summary>
        public int TotalColaboradores { get; set; }

        /// <summary>
        /// Valor total geral
        /// </summary>
        public decimal ValorTotal { get; set; }
    }

    public class ColaboradorComSolicitacoesDTO
    {
        public string CodigoColaborador { get; set; }
        public string NomeColaborador { get; set; }
        public int QuantidadeSolicitacoes { get; set; }
        public decimal ValorTotal { get; set; }

        /// <summary>
        /// Forma de pagamento do colaborador (PIX ou TED)
        /// </summary>
        public string? FormaPagamento { get; set; }

        /// <summary>
        /// Solicitações discriminadas do colaborador
        /// </summary>
        public List<SolicitacaoRemessaDTO> Solicitacoes { get; set; }
    }
}
