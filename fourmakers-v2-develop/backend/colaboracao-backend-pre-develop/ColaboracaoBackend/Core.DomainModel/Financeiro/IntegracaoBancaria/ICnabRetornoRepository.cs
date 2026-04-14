using DataTransferObject.Domain.Financeiro.IntegracaoBancaria.Externo;
using DataTransferObject.Domain.Financeiro.IntegracaoBancaria.RetornoBancaria;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.Financeiro.IntegracaoBancaria
{
    public interface ICnabRetornoRepository
    {
        Task<Guid> CriarRetorno(
            Guid remessaId,
            string hashRemessa,
            string nomeArquivo,
            string usuarioProcessamento);

        Task<Guid> CriarRetornoItem(
            Guid retornoId,
            string conteudoLinha,
            int ordem,
            string codigoOcorrencia,
            string tipoRegistro);

        Task<Guid> BuscarRemessaIdPorHash(string hashRemessa, int orgId);

        Task<List<string>> BuscarPagamentosCnabPorRemessa(Guid remessaId);

        Task<List<ReembolsoPagoExternoDTO>> BuscarReembolsosPagosViaCNAB(int orgId);

        Task AtualizarNomeArquivoRetorno(Guid retornoId, string urlArquivo);

        Task<List<int>> BuscarSolicitacoesPagamentoPorRemessa(Guid remessaId, string statusCnab);

        Task AtualizarStatusRemessa(Guid remessaId, string novoStatus);

        Task<List<PagamentoCnabOrdenadoDTO>> BuscarPagamentosCnabOrdenadosPorRemessa(Guid remessaId);

        Task<List<int>> BuscarReembolsosPorRemessa(Guid remessaId);

        Task AtualizarStatusReembolso(int reembolsoId, int statusId);
        Task<List<Guid>> BuscarSolicitacoesPagamentoNFPorRemessa(Guid remessaId, string statusCnab);
    }
}
