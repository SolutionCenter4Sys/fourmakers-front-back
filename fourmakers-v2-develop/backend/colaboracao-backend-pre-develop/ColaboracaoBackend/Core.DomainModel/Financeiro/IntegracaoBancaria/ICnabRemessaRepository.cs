using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain.Financeiro.IntegracaoBancaria.RemessaBancaria;

namespace Core.Domain.Financeiro.IntegracaoBancaria
{
    public interface ICnabRemessaRepository
    {
        Task<string> CriarRemessa(
            string hashRemessa,
            string nomeArquivo,
            string modeloCnab,
            string tipo,
            string codDiretoria,
            string status,
            string codigoInternoColaboradorCriacao,
            int orgId,
            string codigoBanco,
            string agencia,
            string agenciaDv,
            string conta,
            string contaDv,
            string codigoConvenio,
            string descricao,
            decimal valorTotal);

        Task<string> CriarRemessaItem(string remessaId, string conteudoLinha, int ordem, string tipoRegistro);
        Task AtualizarStatusRemessa(string remessaId, string status, string codigoInternoColaboradorAlteracao);
        Task AtualizarNomeArquivo(string remessaId, string nomeArquivoRemessa, string codigoInternoColaboradorAlteracao);
        Task<List<string>> BuscarItensRemessa(string remessaId);
        Task<ListarRemessasCnabResult> ListarRemessas(int orgId, string tipoRemessa, string mesAnoProcessamento = null, string status = null, List<string> ids = null);
        Task<int> ObterProximoNumeroSequencial(int orgId);
        Task<List<SolicitacaoRemessaDTO>> BuscarSolicitacoesPagamentos(int orgId, string tipoRemessa);
        Task<List<string>> BuscarRemessaItemSegmentoAPorRemessaId(Guid remessaId);
    }
}
