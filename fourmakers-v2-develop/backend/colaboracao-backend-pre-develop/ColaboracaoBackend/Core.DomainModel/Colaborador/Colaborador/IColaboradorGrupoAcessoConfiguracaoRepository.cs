using DataTransferObject.Domain.Colaborador;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Colaboracao.Core.Interfaces
{
    public interface IColaboradorGrupoAcessoConfiguracaoRepository
    {
        Task<List<ColaboradorGrupoAcessoConfiguracaoDTO>> ObterConfiguracaoPorOrgAsync(int orgId);
        Task<string> ObterValorColunaEAcaoAsync(string tabela, string coluna, string condicao, int tbOrgId, string codigoInternoColaborador);
    }
}