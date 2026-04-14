using DataTransferObject.Domain.MapaDeAlocacao;
using System;
using System.Threading.Tasks;

namespace MapaDeAlocacao.Domain.Interfaces
{
    public interface IMapaDeAlocacaoValidadorService
    {
        bool ValidaSeExisteCodigoProjetoParaOrgId(string codigoProjeto, int orgId);
        void ValidaAcesso(string cpfSolicitant, int orgId);
        Task ValidaMapaAlocacaoCadastro(CadastroMapaAlocacaoValidacaoDTO cadastroMapaAlocacaoValidacao);
        Task ValidaMapaAlocacaoEditar(long periodoAlocacaoId, DateTime? dataInicio, DateTime? dataFim, double? quantidadeDeHoras, string cpfRequest, int orgId);
        Task<bool> ValidarConflitoAlocacaoCadastrar(DateTime dataInicio, DateTime dataFim, string codigoProjeto, string codigoColaborador, bool ehTbd, int orgId);
        Task<bool> ValidarConflitoColaboradorAlocacaoEditar(DateTime dataInicio, DateTime dataFim, long periodoAlocacaoId, string codigoColaborador, string codigoProjeto, bool ehTbd, int orgId);
    }
}