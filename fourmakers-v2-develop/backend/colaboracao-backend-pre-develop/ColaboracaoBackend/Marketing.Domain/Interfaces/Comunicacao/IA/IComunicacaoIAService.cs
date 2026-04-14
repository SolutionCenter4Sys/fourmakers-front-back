using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Marketing.Comunicacao.IA;
using System.Threading.Tasks;

namespace Marketing.Domain.Interfaces.Comunicacao.IA
{
    public interface IComunicacaoIAService
    {
        Task<ApiGenericResult<string>> AssistenteAsync(AssistenteRequestDTO request);
    }
}
