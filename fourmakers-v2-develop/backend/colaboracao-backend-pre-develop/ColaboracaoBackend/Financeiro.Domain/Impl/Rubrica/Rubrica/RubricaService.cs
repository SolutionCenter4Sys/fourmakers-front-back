using Colaboracao.Core.Interfaces;
using Colaboracao.Helper.Enum;
using Core.Domain.Financeiro.Rubrica;
using Core.Domain.Usuario.Permissao;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Financeiro.Rubrica.Rubrica;
using DataTransferObject.Domain.Usuario;
using Financeiro.Domain.Interfaces.Rubrica.Rubrica;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Logs.Infra.Attributes;

namespace Financeiro.Domain.Impl.Rubrica.Rubrica
{
    [LogDomainClass]
    public class RubricaService : IRubricaService
    {
        public readonly static string DESCRICAO_ENTIDADE = "Rubrica";

        private readonly IRubricaRepository _rubricaRepository;
        private readonly IRubricaValidatorService _rubricaValidatorService;
        private readonly IFuncionalidadeSistemaRepository _funcionalidadeSistemaRepository;
        private readonly IDBConnectionUnitOfWork _dbConnectionUnitOfWork;

        public RubricaService(IRubricaRepository rubricaRepository,
                                    IRubricaValidatorService rubricaValidatorService,
                                    IFuncionalidadeSistemaRepository funcionalidadeSistemaRepository,
                                    IDBConnectionUnitOfWork dbConnectionUnitOfWork)
        {
            _rubricaRepository = rubricaRepository;
            _rubricaValidatorService = rubricaValidatorService;
            _funcionalidadeSistemaRepository = funcionalidadeSistemaRepository;
            _dbConnectionUnitOfWork = dbConnectionUnitOfWork;
        }

        public async Task<ApiGenericResult<IEnumerable<RubricaResult>>> ListarRubricas(string cpfRequest, int orgId, bool somenteComTemplates = false)
        {
            var apiGenericResult = new ApiGenericResult<IEnumerable<RubricaResult>>();
            try
            {
                ValidaAcessoRubrica(cpfRequest, orgId);

                var result = await _rubricaRepository.ListarRubricasAsync(orgId, somenteComTemplates);

                apiGenericResult.Retorno = result;
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, DESCRICAO_ENTIDADE);
            }

            return apiGenericResult;
        }

        public async Task<ApiGenericResult<RubricaResult>> ObterRubricaPorId(Guid id, string cpfRequest, int orgId, bool validaAcesso = true)
        {
            var apiGenericResult = new ApiGenericResult<RubricaResult>();

            try
            {
                if (validaAcesso)
                {
                    ValidaAcessoRubrica(cpfRequest, orgId);
                }

                var result = await _rubricaRepository.ObterRubricaPorIdAsync(id);

                if (result == null)
                {
                    ExceptionUtil.NaoEncontrado(DESCRICAO_ENTIDADE);
                }

                apiGenericResult.Retorno = result;

            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, DESCRICAO_ENTIDADE);
            }

            return apiGenericResult;
        }

        public async Task<ApiGenericResult<RubricaResult>> InserirRubrica(RubricaInput rubricaInput, string cpfRequest, int orgId)
        {
            var apiGenericResult = new ApiGenericResult<RubricaResult>();

            try
            {
                ValidaAcessoRubrica(cpfRequest, orgId);

                rubricaInput.ConfigurarParaPersistencia(orgId, cpfRequest, Guid.NewGuid());

                await _rubricaValidatorService.ValidaRubrica(rubricaInput, CRUDEnum.Create);

                _dbConnectionUnitOfWork.BeginTransaction();

                var result = await _rubricaRepository.InserirRubricaAsync(rubricaInput);

                if (result == null)
                {
                    ExceptionUtil.NaoInserido(DESCRICAO_ENTIDADE);
                }

                _dbConnectionUnitOfWork.Commit();

                apiGenericResult.Retorno = result;
            }
            catch (Exception ex)
            {
                _dbConnectionUnitOfWork.SafeRollback();
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Create, DESCRICAO_ENTIDADE);
            }

            return apiGenericResult;
        }

        public async Task<ApiGenericResult<RubricaResult>> AtualizarRubrica(RubricaInput rubricaInput, Guid id, string cpfRequest, int orgId)
        {
            var apiGenericResult = new ApiGenericResult<RubricaResult>();

            try
            {
                ValidaAcessoRubrica(cpfRequest, orgId);

                rubricaInput.ConfigurarParaPersistencia(orgId, cpfRequest, id);

                await _rubricaValidatorService.ValidaRubrica(rubricaInput, CRUDEnum.Update);

                _dbConnectionUnitOfWork.BeginTransaction();

                var result = await _rubricaRepository.AtualizarRubricaAsync(rubricaInput);

                if (result == null)
                {
                    ExceptionUtil.NaoAtualizado(DESCRICAO_ENTIDADE);
                }

                _dbConnectionUnitOfWork.Commit();

                apiGenericResult.Retorno = result;
            }
            catch (Exception ex)
            {
                _dbConnectionUnitOfWork.SafeRollback();
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Update, DESCRICAO_ENTIDADE);
            }

            return apiGenericResult;
        }

        public async Task<ApiGenericResult> DeletarRubrica(Guid id, string cpfRequest, int orgId)
        {
            var apiGenericResult = new ApiGenericResult();

            try
            {
                ValidaAcessoRubrica(cpfRequest, orgId);

                _dbConnectionUnitOfWork.BeginTransaction();

                var rubricaResult = (await ObterRubricaPorId(id, cpfRequest, orgId)).Retorno;

                var rubricaInput = new RubricaInput();
                rubricaInput.AtualizarPropriedadesDaClasseBase(rubricaResult);
                rubricaInput.ConfigurarParaPersistencia(orgId, cpfRequest, id);

                await _rubricaValidatorService.ValidaRubrica(rubricaInput, CRUDEnum.Delete);

                var sucesso = await _rubricaRepository.DeletarRubricaAsync(id);

                if (!sucesso)
                {
                    ExceptionUtil.NaoExcluido(DESCRICAO_ENTIDADE);
                }

                _dbConnectionUnitOfWork.Commit();

                apiGenericResult.Sucesso = sucesso;
                apiGenericResult.Mensagem = $"{DESCRICAO_ENTIDADE} excluído com sucesso.";

            }
            catch (Exception ex)
            {
                _dbConnectionUnitOfWork.SafeRollback();
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Delete, DESCRICAO_ENTIDADE);
            }
            return apiGenericResult;
        }

        private void ValidaAcessoRubrica(string cpfRequest, int orgId)
        {
            var isValid = _funcionalidadeSistemaRepository.ValidaAcessoFuncionalidade(
                cpfRequest, orgId, FuncionalidadeSistemaEnum.RUBRICAS_CONFIGURACOES);

            if (!isValid.Result)
            {
                throw new UnauthorizedAccessException($"Acesso negado para {DESCRICAO_ENTIDADE}"); 
            }
        }
    }
}