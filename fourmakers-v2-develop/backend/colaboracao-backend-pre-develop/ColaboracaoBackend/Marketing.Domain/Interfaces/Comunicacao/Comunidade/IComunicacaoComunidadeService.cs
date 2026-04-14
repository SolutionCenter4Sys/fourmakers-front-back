using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Marketing.Comunicacao.Comunidade;
using System.Threading.Tasks;

namespace Marketing.Domain.Interfaces.Comunicacao.Comunidade
{
    public interface IComunicacaoComunidadeService
    {
        Task<ApiGenericResult<ComunidadesResumoResponseDTO>> ObterListarComunidadesResumoAsync(int orgId, string codigoInternoColaborador);
        Task<ApiGenericResult<ComunidadeDetalheDTO>> ObterComunidadePorIdAsync(Guid id, int orgId, string codigoInternoColaborador);
        Task<ApiGenericResult<ComunidadeResumoDTO>> InserirComunidadeAsync(string codigoInternoColaborador, int orgId, InserirComunidadeRequestDTO request, byte[] capaImagem);
        Task<ApiGenericResult> AtualizarComunidadeAsync(Guid id, string codigoInternoColaborador, int orgId, AtualizarComunidadeRequestDTO request, byte[] capaImagem);
        Task<ApiGenericResult> ParticiparComunidadeAsync(Guid comunidadeId, string codigoInternoColaborador, int orgId);
        Task<ApiGenericResult> SairComunidadeAsync(Guid comunidadeId, string codigoInternoColaborador, int orgId);
    }
}
