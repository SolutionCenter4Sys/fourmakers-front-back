using Colaboracao.Core.Interfaces;
using Colaboracao.Helper.Enum;
using Colaborador.Domain.Interfaces.Colaborador.DepartamentoOrg;
using Core.Domain.Colaborador.Colaborador;
using Core.Domain.Usuario.Permissao;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Colaborador.DepartamentoOrg;
using Logs.Infra.Attributes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Colaborador.Domain.Impl.Services.Colaborador.DepartamentoOrg
{
    [LogDomainClass]
    public class DepartamentoOrgService : IDepartamentoOrgService
    {
        public static readonly string DESCRICAO_ENTIDADE = "Departamento";

        private readonly IDepartamentoOrgRepository _departamentoOrgRepository;
        private readonly IDepartamentoOrgValidatorService _departamentoOrgValidatorService;
        private readonly IFuncionalidadeSistemaRepository _funcionalidadeSistemaRepository;
        private readonly IDBConnectionUnitOfWork _dbConnectionUnitOfWork;

        public DepartamentoOrgService(IDepartamentoOrgRepository departamentoOrgRepository,
                                    IDepartamentoOrgValidatorService departamentoOrgValidatorService,
                                    IFuncionalidadeSistemaRepository funcionalidadeSistemaRepository,
                                    IDBConnectionUnitOfWork dbConnectionUnitOfWork)
        {
            _departamentoOrgRepository = departamentoOrgRepository;
            _departamentoOrgValidatorService = departamentoOrgValidatorService;
            _funcionalidadeSistemaRepository = funcionalidadeSistemaRepository;
            _dbConnectionUnitOfWork = dbConnectionUnitOfWork;
        }

        public async Task<ApiGenericResult<IEnumerable<DepartamentoOrgResult>>> ListarDepartamentoOrgs(string cpfRequest, int orgId)
        {
            var apiGenericResult = new ApiGenericResult<IEnumerable<DepartamentoOrgResult>>();
            try
            {
                ValidaAcessoDepartamentoOrg(cpfRequest, orgId);

                var result = await _departamentoOrgRepository.ListarDepartamentoOrgsAsync(orgId);

                apiGenericResult.Retorno = result;
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, DESCRICAO_ENTIDADE);
            }

            return apiGenericResult;
        }

        public async Task<ApiGenericResult<DepartamentoOrgResult>> ObterDepartamentoOrgPorId(Guid id, string cpfRequest, int orgId)
        {
            var apiGenericResult = new ApiGenericResult<DepartamentoOrgResult>();

            try
            {
                ValidaAcessoDepartamentoOrg(cpfRequest, orgId);

                var result = await _departamentoOrgRepository.ObterDepartamentoOrgPorIdAsync(id);

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

        public async Task<ApiGenericResult<DepartamentoOrgResult>> InserirDepartamentoOrg(DepartamentoOrgInput departamentoOrgInput, string cpfRequest, int orgId)
        {
            var apiGenericResult = new ApiGenericResult<DepartamentoOrgResult>();

            try
            {
                ValidaAcessoDepartamentoOrg(cpfRequest, orgId);

                departamentoOrgInput.ConfigurarParaPersistencia(orgId, cpfRequest, Guid.NewGuid());

                await _departamentoOrgValidatorService.ValidaDepartamentoOrg(departamentoOrgInput, CRUDEnum.Create);

                _dbConnectionUnitOfWork.BeginTransaction();

                var result = await _departamentoOrgRepository.InserirDepartamentoOrgAsync(departamentoOrgInput);

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

        public async Task<ApiGenericResult<DepartamentoOrgResult>> AtualizarDepartamentoOrg(DepartamentoOrgInput departamentoOrgInput, Guid id, string cpfRequest, int orgId)
        {
            var apiGenericResult = new ApiGenericResult<DepartamentoOrgResult>();

            try
            {
                ValidaAcessoDepartamentoOrg(cpfRequest, orgId);

                departamentoOrgInput.ConfigurarParaPersistencia(orgId, cpfRequest, id);

                await _departamentoOrgValidatorService.ValidaDepartamentoOrg(departamentoOrgInput, CRUDEnum.Update);

                _dbConnectionUnitOfWork.BeginTransaction();

                var result = await _departamentoOrgRepository.AtualizarDepartamentoOrgAsync(departamentoOrgInput);

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

        public async Task<ApiGenericResult> DeletarDepartamentoOrg(Guid id, string cpfRequest, int orgId)
        {
            var apiGenericResult = new ApiGenericResult();

            try
            {
                ValidaAcessoDepartamentoOrg(cpfRequest, orgId);

                _dbConnectionUnitOfWork.BeginTransaction();

                var departamentoOrgResult = (await ObterDepartamentoOrgPorId(id, cpfRequest, orgId)).Retorno;

                var departamentoOrgInput = new DepartamentoOrgInput();
                departamentoOrgInput.AtualizarPropriedadesDaClasseBase(departamentoOrgResult);
                departamentoOrgInput.ConfigurarParaPersistencia(orgId, cpfRequest, id);

                await _departamentoOrgValidatorService.ValidaDepartamentoOrg(departamentoOrgInput, CRUDEnum.Delete);

                var sucesso = await _departamentoOrgRepository.DeletarDepartamentoOrgAsync(id);

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

        private void ValidaAcessoDepartamentoOrg(string cpfRequest, int orgId)
        {
            //var isValid = _funcionalidadeSistemaRepository.ValidaAcessoFuncionalidade(
            //    cpfRequest, orgId, FuncionalidadeSistemaEnum.CADASTRO_XPTO);

            //if (!isValid.Result)
            //{
            //    throw new UnauthorizedAccessException($"Acesso negado para {DESCRICAO_ENTIDADE}");
            //}
        }
    }
}