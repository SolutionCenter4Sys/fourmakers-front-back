using Colaboracao.Helper.Enum;
using Colaborador.Domain.Interfaces.Services;
using Core.DomainModel;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Colaborador.Cidadania;
using Logs.Infra.Attributes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Colaborador.Domain.Impl.Services
{
    [LogDomainClass]
    public class CidadaniaService : ICidadaniaService
    {
        private readonly ICidadaniaRepository _cidadaniaRepository;
        private readonly ICidadaniaValidadorService _cidadaniaValidadorService;

        public CidadaniaService(ICidadaniaRepository cidadaniaRepository, ICidadaniaValidadorService cidadaniaValidadorService)
        {
            _cidadaniaRepository = cidadaniaRepository;
            _cidadaniaValidadorService = cidadaniaValidadorService;
        }

        public async Task<ApiGenericResult<List<CidadaniaDTO>>> ListarCidadanias()
        {
            var apiGenericResult = new ApiGenericResult<List<CidadaniaDTO>>();

            try
            {
                apiGenericResult.Retorno = await _cidadaniaRepository.ListarCidadanias();
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, nameof(ListarCidadanias));
            }

            return apiGenericResult;
        }

        public async Task<ApiGenericResult<List<CidadaniaStatusDTO>>> ListarStatusCidadania()
        {
            var apiGenericResult = new ApiGenericResult<List<CidadaniaStatusDTO>>();

            try
            {
                apiGenericResult.Retorno = await _cidadaniaRepository.ListarStatusCidadania();
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, nameof(ListarStatusCidadania));
            }

            return apiGenericResult;
        }

        public async Task<ApiGenericResult<CidadaniaColaboradorDTO>> BuscarCidadaniaColaboradorPorId(int id)
        {
            var apiGenericResult = new ApiGenericResult<CidadaniaColaboradorDTO>();
            try
            {
                apiGenericResult.Retorno = await _cidadaniaRepository.BuscarCidadaniaColaboradorPorId(id);
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, nameof(BuscarCidadaniaColaboradorPorId));
            }

            return apiGenericResult;
        }

        public async Task<ApiGenericResult<CidadaniaColaboradorDTO>> InserirCidadaniaColaborador(int cidadaniaId, int cidadaniaStatusId, string codigoInternoColaborador)
        {
            var apiGenericResult = new ApiGenericResult<CidadaniaColaboradorDTO>();

            try
            {
                await _cidadaniaValidadorService.ValidaSeExisteCidadaniaJaCadastradaParaColaborador(cidadaniaId, codigoInternoColaborador);

                var result = await _cidadaniaRepository.InserirCidadaniaColaborador(cidadaniaId, cidadaniaStatusId, codigoInternoColaborador);
                apiGenericResult.Retorno = result;
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Create, nameof(InserirCidadaniaColaborador));
            }

            return apiGenericResult;
        }

        public async Task<ApiGenericResult<CidadaniaColaboradorDTO>> AtualizarCidadaniaColaborador(int statusId, int id, string codigoInternoColaborador)
        {
            var apiGenericResult = new ApiGenericResult<CidadaniaColaboradorDTO>();

            try
            {
                await _cidadaniaValidadorService.ValidaSeExisteCidadaniaParaOColaborador(id, codigoInternoColaborador);

                var result = await _cidadaniaRepository.AtualizarCidadaniaColaborador(statusId, id);

                apiGenericResult.Retorno = result;
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Update, nameof(AtualizarCidadaniaColaborador));
            }

            return apiGenericResult;
        }

        public async Task<ApiGenericResult<bool>> RemoverCidadaniaColaborador(int id, string codigoInternoColaborador)
        {
            var apiGenericResult = new ApiGenericResult<bool>();

            try
            {
                await _cidadaniaValidadorService.ValidaSeExisteCidadaniaParaOColaborador(id, codigoInternoColaborador);

                var result = await _cidadaniaRepository.RemoverCidadaniaColaborador(id);
                apiGenericResult.Retorno = result;
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Delete, nameof(RemoverCidadaniaColaborador));
            }

            return apiGenericResult;
        }
    }
}