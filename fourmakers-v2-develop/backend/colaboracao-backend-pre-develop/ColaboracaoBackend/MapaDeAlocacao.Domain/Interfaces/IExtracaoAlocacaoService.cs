using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.MapaDeAlocacao;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MapaDeAlocacao.Domain.Interfaces
{
    public interface IExtracaoAlocacaoService
    {
        Task<ApiGenericResult<FileContentResult>> RelatorioAlocacoes(string cpf, int orgId);

        Task<ApiGenericResult<FileContentResult>> RelatorioAlocacoesPorPesquisa(ListarAlocacoesColabETbdInput dto, string cpfSolicitante, int orgId);
    }
}