using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Marketing.Comunicacao.Publicacao;
using System.Threading.Tasks;

namespace Marketing.Domain.Interfaces.Comunicacao.PublicacaoGerencial
{
    public interface IComunicacaoPublicacaoGerencialService
    {
        //Task<ApiGenericResult<PublicacaoGerencialResponseDTO>> ObterListaPublicacaoAgendadoEAprovacaoAsync(int orgId, string codigoInternoColaborador);
        Task<ApiGenericResult> PublicarAgoraAsync(string publicacaoId, string codigoInternoColaborador, int orgId);
        Task<ApiGenericResult> AprovarComunicacaoAsync(string publicacaoId, string codigoInternoColaborador, int orgId);
        Task<ApiGenericResult> RejeitarComunicacaoAsync(string publicacaoId, string codigoInternoColaborador, int orgId, RejeitarComunicacaoRequestDTO request);
    }
}
