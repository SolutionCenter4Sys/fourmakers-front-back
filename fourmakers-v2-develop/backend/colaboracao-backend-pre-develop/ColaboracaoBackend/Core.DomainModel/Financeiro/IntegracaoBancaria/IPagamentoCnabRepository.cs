using DataTransferObject.Domain.Financeiro.IntegracaoBancaria.RemessaBancaria;
using DataTransferObject.Domain.Financeiro.Reembolso.ControleDeSaldo;
using System;
using System.Threading.Tasks;

namespace Core.Domain.Financeiro.IntegracaoBancaria
{
    public interface IPagamentoCnabRepository
    {
        Task<DadosBancariosColaboradorDTO> BuscarDadosBancariosColaborador(string codigoInternoColaborador, int orgId);
        Task<string> CriarColaboradorPagamentoCnab(int orgId, string codigoInternoColaborador, string cnabRemessaItemId, decimal valor, string formaPagamento, string codigoBanco, string agencia, string agenciaDv, string conta, string contaDv, string chavePix, string tipoChavePix, string statusCnab);
        Task CriarRelacaoComSolicitacaoPagamento(string pagamentoCnabId, string solicitacaoPagamentoId, decimal valorPagamento, string tipoRemessa);
        Task AtualizarStatusPagamentoCnab(Guid pagamentoCnabId, string novoStatus, string codigoOcorrencia, string descricaoErro = null);
        Task AtualizarStatusSolicitacaoPagamento(int solicitacaoPagamentoId, StatusSolicitacaoPagamentoEnum status);
    }
}
