using DataTransferObject.Domain.Marketing.Comunicacao.Profissionais;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.Marketing.Comunicacao.Profissionais
{
    public interface IComunicacaoProfissionaisRepository
    {
        Task<List<ProfissionalDTO>> ObterListaProfissionaisAsync(int orgId, string codigoColaboradorUsuarioLogado);
        /// <summary>
        /// Retorna os profissionais (mesma estrutura de ObterListaProfissionaisAsync) para os códigos informados.
        /// </summary>
        Task<List<ProfissionalDTO>> ObterProfissionaisPorCodigosAsync(int orgId, IReadOnlyList<string> codigos, string codigoColaboradorUsuarioLogado);
        Task FavoritarProfissionalAsync(string codigoColaboradorQuemFavoritou, string codigoColaboradorFavoritado, int orgId);
        Task DesfavoritarProfissionalAsync(string codigoColaboradorQuemFavoritou, string codigoColaboradorFavoritado, int orgId);
        Task<List<string>> ListarCodigosProfissionaisFavoritadosAsync(string codigoColaboradorQuemFavoritou, int orgId);
    }
}
