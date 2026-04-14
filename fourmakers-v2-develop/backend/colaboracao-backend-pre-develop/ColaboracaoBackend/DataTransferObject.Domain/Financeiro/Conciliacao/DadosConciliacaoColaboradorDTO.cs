using System.Collections.Generic;
using DataTransferObject.Domain.Financeiro.Holerite;
using DataTransferObject.Domain.Apontamento.FolhaPonto;
using DataTransferObject.Domain.Financeiro.Reembolso.ControleDeSaldo;
using DataTransferObject.Domain.Financeiro.Rubrica.RubricaColaborador;

namespace DataTransferObject.Domain.Financeiro.Conciliacao
{
    /// <summary>
    /// DTO que representa os dados necessários para conciliação de um colaborador específico
    /// </summary>
    public class DadosConciliacaoColaboradorDTO
    {
        /// <summary>
        /// Dados do holerite do colaborador
        /// </summary>
        public HoleriteDTO Holerite { get; set; }

        /// <summary>
        /// Dados da folha de ponto do colaborador
        /// </summary>
        public RelatorioPontoRootDTO FolhaPonto { get; set; }

        /// <summary>
        /// Lista de solicitações de pagamento do colaborador
        /// </summary>
        public List<SolicitacaoPagamentoRelatorioDTO> Pagamentos { get; set; } = new List<SolicitacaoPagamentoRelatorioDTO>();

        /// <summary>
        /// Lista de rubricas do colaborador
        /// </summary>
        public List<RubricaColaboradorDetalhadoDTO> Rubricas { get; set; } = new List<RubricaColaboradorDetalhadoDTO>();

        /// <summary>
        /// ID do item de lote relacionado
        /// </summary>
        public string ItemLoteId { get; set; }

        /// <summary>
        /// Código interno do colaborador
        /// </summary>
        public string CodigoInternoColaborador { get; set; }
    }
}