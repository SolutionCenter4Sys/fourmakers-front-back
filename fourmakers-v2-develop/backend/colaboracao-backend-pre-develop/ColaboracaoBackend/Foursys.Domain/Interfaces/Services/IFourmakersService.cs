using System.Threading.Tasks;
using DataTransferObject.Domain.Base;

namespace Foursys.Domain.Interfaces.Services
{
    public interface IFourmakersService
    {
        Task<ApiGenericResult> InserirAvaliacaoAsync(int orgId, string codigoInternoColaborador, int servicoRate, int recomendacaoRate, string? experienciaDescricao, string aspectoDescricao);
    }
}