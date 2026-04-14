using Colaboracao.Helper.Enum;
using Colaboracao.Helper.Util;
using Core.Domain.Marketing.Comunicacao.Analytics;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Marketing.Comunicacao.Analytics;
using Marketing.Domain.Interfaces.Comunicacao.Analytics;
using System;
using System.Threading.Tasks;

namespace Marketing.Domain.Impl.Comunicacao.Analytics
{
    public class ComunicacaoAnalyticsService : IComunicacaoAnalyticsService
    {
        private const string DESCRICAO_ENTIDADE = "Marketing - Comunicacao - Analytics";
        private readonly IComunicacaoAnalyticsRepository _repository;

        public ComunicacaoAnalyticsService(IComunicacaoAnalyticsRepository repository)
        {
            _repository = repository;
        }

        public async Task<ApiGenericResult<AnalyticsResumoResponseDTO>> ObterResumoAsync(int orgId, AnalyticsResumoRequestDTO request)
        {
            var result = new ApiGenericResult<AnalyticsResumoResponseDTO>();
            try
            {
                request ??= new AnalyticsResumoRequestDTO();
                if (request.Pagina <= 0) request.Pagina = 1;
                if (request.TamanhoPagina <= 0) request.TamanhoPagina = 20;
                if (request.TamanhoPagina > 200) request.TamanhoPagina = 200;

                result.Retorno = await _repository.ObterResumoAsync(orgId, request);
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, DESCRICAO_ENTIDADE);
            }
            return result;
        }

        public async Task<ApiGenericResult<AnalyticsResumoResponseDTO>> ObterResumoComunicadosOficiaisAsync(int orgId, AnalyticsResumoRequestDTO request)
        {
            var result = new ApiGenericResult<AnalyticsResumoResponseDTO>();
            try
            {
                request ??= new AnalyticsResumoRequestDTO();
                if (request.Pagina <= 0) request.Pagina = 1;
                if (request.TamanhoPagina <= 0) request.TamanhoPagina = 20;
                if (request.TamanhoPagina > 200) request.TamanhoPagina = 200;

                result.Retorno = await _repository.ObterResumoComunicadosOficiaisAsync(orgId, request);
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, DESCRICAO_ENTIDADE);
            }
            return result;
        }

        public async Task<ApiGenericResult<PostsPorComunidadeResponseDTO>> ObterPostsPorComunidadeAsync(int orgId, PostsPorComunidadeRequestDTO request)
        {
            var result = new ApiGenericResult<PostsPorComunidadeResponseDTO>();
            try
            {
                result.Retorno = await _repository.ObterPostsPorComunidadeAsync(orgId, request ?? new PostsPorComunidadeRequestDTO());
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, DESCRICAO_ENTIDADE);
            }
            return result;
        }
    }
}
