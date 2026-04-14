using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.GestaoPessoa.Pdi;
using System.Threading.Tasks;

namespace GestaoPessoa.Domain.Interfaces.Services.Pdi
{
    /// <summary>
    /// Listagem de PDIs (RH / Minha Equipe) na org do usuário logado; escopo de diretoria quando houver restrição.
    /// </summary>
    public interface IPdisRhService
    {
        /// <param name="orgIdUsuarioLogado">Org do token (não vem de query).</param>
        Task<ApiGenericResult<PdiListagemRhResult>> ListarPdisRhAsync(
            string codigoInternoUsuario,
            int orgIdUsuarioLogado,
            int pagina,
            int tamanhoPagina,
            string filtroColaboradorId,
            string filtroGestorCodigoInterno);

        /// <summary>Contagens por status no mesmo escopo da listagem.</summary>
        /// <param name="orgIdUsuarioLogado">Org do token (não vem de query).</param>
        Task<ApiGenericResult<PdiRhBigNumbersResultDTO>> ObterBigNumbersAsync(
            string codigoInternoUsuario,
            int orgIdUsuarioLogado,
            string filtroColaboradorId,
            string filtroGestorCodigoInterno);
    }
}
