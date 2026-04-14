using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Marketing.Comunicacao.Profissionais;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Marketing.Domain.Interfaces.Comunicacao.Profissionais
{
    public interface IComunicacaoProfissionaisService
    {
        Task<ApiGenericResult<List<ProfissionalDTO>>> ObterListaProfissionaisAsync(int orgId, string codigoColaboradorUsuarioLogado);
        Task<ApiGenericResult> FavoritarProfissionalAsync(string codigoInternoColaboradorProfissional, string codigoColaboradorUsuarioLogado, int orgId);
        Task<ApiGenericResult> DesfavoritarProfissionalAsync(string codigoInternoColaboradorProfissional, string codigoColaboradorUsuarioLogado, int orgId);
        Task<ApiGenericResult<List<string>>> ListarProfissionaisFavoritadosAsync(string codigoColaboradorUsuarioLogado, int orgId);
    }
}
