using Colaboracao.Helper.Enum;
using Core.Domain.ParametroOrg;
using Core.Domain.Usuario;
using Core.Domain.Usuario.Permissao;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Fourmakers;
using DataTransferObject.Domain.Fourmakers.Parametro;
using DataTransferObject.Domain.Usuario;
using Foursys.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Logs.Infra.Attributes;

namespace Foursys.Domain.Impl.Services
{
    [LogDomainClass]
    public class ParametroService : IParametroService
    {
        private readonly IParametroRepository _parametroRepository;
        private readonly IParametroValidatorService _parametroValidatorService;
        private readonly IFuncionalidadeSistemaRepository _funcionalidadeSistemaRepository;
        private readonly IUsuarioColaboradorRepository _usuarioColaboradorRepository;

        public ParametroService(IParametroRepository parametroRepository,
                                IParametroValidatorService parametroValidatorService,
                                IFuncionalidadeSistemaRepository funcionalidadeSistemaRepository,
                                IUsuarioColaboradorRepository usuarioColaboradorRepository)
        {
            _parametroRepository = parametroRepository;
            _parametroValidatorService = parametroValidatorService;
            _funcionalidadeSistemaRepository = funcionalidadeSistemaRepository;
            _usuarioColaboradorRepository = usuarioColaboradorRepository;
        }

        public async Task<ApiGenericResult<IEnumerable<ParametroResult>>> ListarParametros(string cpfRequest, int orgId)
        {
            ValidaAcessoParametro(cpfRequest, orgId);

            var apiGenericResult = new ApiGenericResult<IEnumerable<ParametroResult>>();

            var result = await _parametroRepository.ListarParametros(TipoParametroEnum.FRONTEND);

            if (result == null)
            {
                apiGenericResult.Sucesso = false;
            }

            apiGenericResult.Retorno = result;

            return apiGenericResult;
        }

        public async Task<ApiGenericResult<ParametroResult>> ObterParametroPorId(string id, string cpfRequest, int orgId)
        {
            ValidaAcessoParametro(cpfRequest, orgId);

            var apiGenericResult = new ApiGenericResult<ParametroResult>();

            var result = await _parametroRepository.ObterParametroPorId(id, TipoParametroEnum.FRONTEND);

            if (result == null)
            {
                apiGenericResult.Sucesso = false;
                apiGenericResult.Mensagem = "Parametro não encontrado.";
            }

            apiGenericResult.Retorno = result;

            return apiGenericResult;
        }

        public async Task<ApiGenericResult<ParametroResult>> InserirParametro(ParametroInput parametroInput, string cpfRequest, int orgId)
        {
            ValidaAcessoParametro(cpfRequest, orgId);

            var apiGenericResult = new ApiGenericResult<ParametroResult>();

            var usuario = _usuarioColaboradorRepository.GetUserByCPFEOrgId(cpfRequest, orgId); //salvando usuarioid, pois cpf não será mais chave

            var parametroRepositoryInput = new ParametroRepositoryInput(parametroInput, Guid.NewGuid(), usuario.UsuarioId);

            await _parametroValidatorService.ValidaParametro(parametroRepositoryInput, CRUDEnum.Create);

            var result = await _parametroRepository.InserirParametro(parametroRepositoryInput, TipoParametroEnum.FRONTEND);

            if (result == null)
            {
                apiGenericResult.Sucesso = false;
                apiGenericResult.Mensagem = "Parametro não foi inserido ou recuperado corretamente.";
            }

            apiGenericResult.Retorno = result;
            return apiGenericResult;
        }

        public async Task<ApiGenericResult<ParametroResult>> AtualizarParametro(ParametroInput parametroConfiguracaoInput, Guid id, string cpfRequest, int orgId)
        {
            ValidaAcessoParametro(cpfRequest, orgId);

            var apiGenericResult = new ApiGenericResult<ParametroResult>();

            var usuario = _usuarioColaboradorRepository.GetUserByCPFEOrgId(cpfRequest, orgId); //salvando usuarioid, pois cpf não será mais chave

            var parametroRepositoryInput = new ParametroRepositoryInput(parametroConfiguracaoInput, id, usuario.UsuarioId);

            await _parametroValidatorService.ValidaParametro(parametroRepositoryInput, CRUDEnum.Update);

            var result = await _parametroRepository.AtualizarParametro(parametroRepositoryInput, TipoParametroEnum.FRONTEND);

            if (result == null)
            {
                apiGenericResult.Sucesso = false;
                apiGenericResult.Mensagem = "Parametro não foi atualizado ou recuperado corretamente.";
            }

            apiGenericResult.Retorno = result;
            return apiGenericResult;
        }

        public async Task<ApiGenericResult> DeletarParametro(string id, string cpfRequest, int orgId)
        {
            ValidaAcessoParametro(cpfRequest, orgId);

            var apiGenericResult = new ApiGenericResult();

            var parametroResult = (await ObterParametroPorId(id, cpfRequest, orgId)).Retorno;

            var parametroRepositoryInput = new ParametroRepositoryInput(parametroResult, parametroResult.Id, 0);

            await _parametroValidatorService.ValidaParametro(parametroRepositoryInput, CRUDEnum.Delete);

            var sucesso = await _parametroRepository.DeletarParametro(id);

            apiGenericResult.Sucesso = sucesso;
            apiGenericResult.Mensagem = sucesso ? "Parametro excluído com sucesso." : "Parametro não encontrado.";
            return apiGenericResult;
        }

        private void ValidaAcessoParametro(string cpfRequest, int orgId)
        {
            var isValid = _funcionalidadeSistemaRepository.ValidaAcessoFuncionalidade(
                cpfRequest, orgId, FuncionalidadeSistemaEnum.CADASTRO_PARAMETRO);

            if (!isValid.Result)
            {
                throw new UnauthorizedAccessException("Acesso negado para Parâmetro."); //não pode adicionar, editar e remover
            }
        }
    }
}