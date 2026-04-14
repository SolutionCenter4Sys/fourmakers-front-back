using System.Threading.Tasks;
using Core.Domain.Fourmakers;
using DataTransferObject.Domain.Base;
using Foursys.Domain.Interfaces.Services;
using Logs.Infra.Attributes;

namespace Foursys.Domain.Impl.Services
{
    [LogDomainClass]
    public class FourmakersService(IFourmakersRepository fourmakersRepository) : IFourmakersService
    {
        public async Task<ApiGenericResult> InserirAvaliacaoAsync(int orgId, string codigoInternoColaborador, int servicoRate, int recomendacaoRate, string? experienciaDescricao, string aspectoDescricao)
        {
            var result = new ApiGenericResult();
            await fourmakersRepository.InserirAvaliacaoAsync(orgId, codigoInternoColaborador, servicoRate, recomendacaoRate, experienciaDescricao, aspectoDescricao);
            return result;
        }
    }
}