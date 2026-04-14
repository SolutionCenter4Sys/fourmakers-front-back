using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Marketing.Comunicacao.Configuracao;
using System.Threading.Tasks;

namespace Marketing.Domain.Interfaces.Comunicacao.Configuracao
{
    public interface IComunicacaoConfiguracaoService
    {
        Task<ApiGenericResult<ConfiguracaoNotificacaoDTO>> InserirOuAtualizarConfiguracaoAsync(string codigoInternoColaborador, int orgId, ConfiguracaoNotificacaoDTO request);
        Task<ApiGenericResult<ConfiguracaoNotificacaoDTO>> ObterConfiguracaoAsync(string codigoInternoColaborador, int orgId);
    }
}
