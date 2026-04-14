using Colaboracao.Helper;
using Colaboracao.Helper.Enum;
using Core.Domain.ParametroOrg;
using Core.Domain.Usuario;
using Core.Domain.Usuario.Permissao;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Fourmakers;
using DataTransferObject.Domain.Fourmakers.ParametroConfiguracao;
using DataTransferObject.Domain.Usuario;
using Foursys.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Logs.Infra.Attributes;

namespace Foursys.Domain.Impl.Services
{
    [LogDomainClass]
    public class ParametroConfiguracaoService : IParametroConfiguracaoService
    {
        private readonly IParametroConfiguracaoRepository _parametroConfiguracaoRepository;
        private readonly IParametroConfiguracaoValidatorService _parametroConfiguracaoValidatorService;
        private readonly IFuncionalidadeSistemaRepository _funcionalidadeSistemaRepository;
        private readonly IUsuarioColaboradorRepository _usuarioColaboradorRepository;

        public ParametroConfiguracaoService(IParametroConfiguracaoRepository parametroConfiguracaoRepository,
                                            IParametroConfiguracaoValidatorService parametroConfiguracaoValidatorService,
                                            IFuncionalidadeSistemaRepository funcionalidadeSistemaRepository,
                                            IUsuarioColaboradorRepository usuarioColaboradorRepository)
        {
            _parametroConfiguracaoRepository = parametroConfiguracaoRepository;
            _parametroConfiguracaoValidatorService = parametroConfiguracaoValidatorService;
            _funcionalidadeSistemaRepository = funcionalidadeSistemaRepository;
            _usuarioColaboradorRepository = usuarioColaboradorRepository;
        }

        public async Task<ApiGenericResult<IEnumerable<ParametroConfiguracaoResult>>> ListarParametroConfiguracaoDoUsuarioLogado(string cpfRequest, int orgId)
        {
            //não tem validação aqui

            var apiGenericResult = new ApiGenericResult<IEnumerable<ParametroConfiguracaoResult>>();

            var result = await _parametroConfiguracaoRepository.ListarParametroConfiguracaoDoUsuarioLogado(cpfRequest, orgId, TipoParametroEnum.FRONTEND);

            if (result == null)
            {
                apiGenericResult.Sucesso = false;
            }

            apiGenericResult.Retorno = result;

            return apiGenericResult;
        }

        public async Task<ApiGenericResult<IEnumerable<ParametroConfiguracaoResult>>> ListarParametroConfiguracao(string cpfRequest, int orgIdLogada, int orgIdInformada)
        {
            int orgIdValida = ValidaAcessoParametroConfiguracao(cpfRequest, orgIdLogada, orgIdInformada);

            var apiGenericResult = new ApiGenericResult<IEnumerable<ParametroConfiguracaoResult>>();

            var result = await _parametroConfiguracaoRepository.ListarParametroConfiguracaoPorOrg(orgIdValida, TipoParametroEnum.FRONTEND);

            if (result == null)
            {
                apiGenericResult.Sucesso = false;
            }

            apiGenericResult.Retorno = result;

            return apiGenericResult;
        }

        public async Task<ApiGenericResult<ParametroConfiguracaoResult>> ObterParametroConfiguracaoPorId(string id, string cpfRequest, int orgIdLogada, int orgIdInformada)
        {
            int orgIdValida = ValidaAcessoParametroConfiguracao(cpfRequest, orgIdLogada, orgIdInformada);

            var apiGenericResult = new ApiGenericResult<ParametroConfiguracaoResult>();

            var result = await _parametroConfiguracaoRepository.ObterParametroConfiguracaoPorId(id, TipoParametroEnum.FRONTEND, orgIdValida);

            if (result == null)
            {
                apiGenericResult.Sucesso = false;
                apiGenericResult.Mensagem = "Parametro não encontrado.";
            }

            apiGenericResult.Retorno = result;

            return apiGenericResult;
        }

        public async Task<ApiGenericResult<ParametroConfiguracaoResult>> InserirParametroConfiguracao(ParametroConfiguracaoInput parametroConfiguracaoInput, string cpfRequest, int orgIdLogada, int orgIdInformada)
        {
            int orgIdValida = ValidaAcessoParametroConfiguracao(cpfRequest, orgIdLogada, orgIdInformada);

            var apiGenericResult = new ApiGenericResult<ParametroConfiguracaoResult>();

            var usuario = _usuarioColaboradorRepository.GetUserByCPFEOrgId(cpfRequest, orgIdLogada); //salvando usuarioid, pois cpf não será mais chave

            var parametroConfiguracaoRepositoryInput = new ParametroConfiguracaoRepositoryInput(parametroConfiguracaoInput, Guid.NewGuid(), usuario.UsuarioId, orgIdValida);

            await _parametroConfiguracaoValidatorService.ValidaParametroConfiguracao(parametroConfiguracaoRepositoryInput, CRUDEnum.Create);

            var result = await _parametroConfiguracaoRepository.InserirParametroConfiguracao(parametroConfiguracaoRepositoryInput, TipoParametroEnum.FRONTEND);

            if (result == null)
            {
                apiGenericResult.Sucesso = false;
            }

            apiGenericResult.Mensagem = apiGenericResult.Sucesso ? "Parâmetro inserido com sucesso." : "Parâmetro não foi inserido ou recuperado corretamente.";
            apiGenericResult.Retorno = result;
            return apiGenericResult;
        }

        public async Task<ApiGenericResult<ParametroConfiguracaoResult>> AtualizarParametroConfiguracao(ParametroConfiguracaoInput parametroConfiguracaoInput, Guid id, string cpfRequest, int orgIdLogada, int orgIdInformada)
        {
            int orgIdValida = ValidaAcessoParametroConfiguracao(cpfRequest, orgIdLogada, orgIdInformada);

            var apiGenericResult = new ApiGenericResult<ParametroConfiguracaoResult>();

            var usuario = _usuarioColaboradorRepository.GetUserByCPFEOrgId(cpfRequest, orgIdLogada); //salvando usuarioid, pois cpf não será mais chave

            var parametroConfiguracaoRepositoryInput = new ParametroConfiguracaoRepositoryInput(parametroConfiguracaoInput, id, usuario.UsuarioId, orgIdValida);

            await _parametroConfiguracaoValidatorService.ValidaParametroConfiguracao(parametroConfiguracaoRepositoryInput, CRUDEnum.Update);

            var result = await _parametroConfiguracaoRepository.AtualizarParametroConfiguracao(parametroConfiguracaoRepositoryInput, TipoParametroEnum.FRONTEND);

            if (result == null)
            {
                apiGenericResult.Sucesso = false;
            }

            apiGenericResult.Mensagem = apiGenericResult.Sucesso ? "Parametro atualizado com sucesso." : "Parametro não foi atualizado ou recuperado corretamente.";
            apiGenericResult.Retorno = result;
            return apiGenericResult;
        }

        public async Task<ApiGenericResult> DeletarParametroConfiguracao(string id, string cpfRequest, int orgIdLogada, int orgIdInformada)
        {
            int orgIdValida = ValidaAcessoParametroConfiguracao(cpfRequest, orgIdLogada, orgIdInformada);

            var apiGenericResult = new ApiGenericResult();

            var sucesso = await _parametroConfiguracaoRepository.DeletarParametroConfiguracao(id, orgIdValida);

            apiGenericResult.Sucesso = sucesso;
            apiGenericResult.Mensagem = sucesso ? "Parametro excluído com sucesso." : "Parametro não encontrado.";

            return apiGenericResult;
        }

        private int ValidaAcessoParametroConfiguracao(string cpf, int orgIdLogada, int? orgIdInformada = null)
        {
            bool isValid;

            // garante que a validação seja feita corretamente, caso não seja informada uma orgId diferente da Logada
            if (orgIdInformada.ToIntOuZero() == 0)
            {
                orgIdInformada = orgIdLogada;
            }

            if (orgIdInformada != orgIdLogada)
            {
                isValid = _funcionalidadeSistemaRepository.ValidaAcessoFuncionalidade(
                    cpf, orgIdLogada, FuncionalidadeSistemaEnum.CADASTRO_PARAMETRO_CONFIGURACAO_OUTRAS_ORG).Result;

                if (!isValid)
                {
                    throw new UnauthorizedAccessException("Acesso negado para Parâmetro Configuração para Outras Orgs."); //não pode listar, adicionar, editar e remover para outras orgs
                }
            }

            isValid = _funcionalidadeSistemaRepository.ValidaAcessoFuncionalidade(
                cpf, orgIdLogada, FuncionalidadeSistemaEnum.CADASTRO_PARAMETRO_CONFIGURACAO).Result;

            if (!isValid)
            {
                throw new UnauthorizedAccessException("Acesso negado para Parâmetro Configuração."); //não pode adicionar, editar e remover
            }

            return orgIdInformada.Value;
        }
    }
}