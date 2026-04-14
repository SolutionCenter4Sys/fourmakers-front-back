using Colaboracao.Core.Interfaces;
using Colaboracao.Helper.Enum;
using Core.Domain.Financeiro.IntegracaoBancaria;
using Core.Domain.Usuario.Permissao;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Financeiro.IntegracaoBancaria.CnabOrg;
using DataTransferObject.Domain.Usuario;
using Financeiro.Domain.Interfaces.IntegracaoBancaria.CnabOrg;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Financeiro.Domain.Impl.IntegracaoBancaria.CnabOrg
{
    public class CnabOrgService : ICnabOrgService
    {
        public readonly static string DESCRICAO_ENTIDADE = "Configuração Cnab";

        private readonly ICnabOrgRepository _cnabOrgRepository;
        private readonly ICnabOrgValidatorService _cnabOrgValidatorService;
        private readonly IFuncionalidadeSistemaRepository _funcionalidadeSistemaRepository;
        private readonly IDBConnectionUnitOfWork _dbConnectionUnitOfWork;

        public CnabOrgService(ICnabOrgRepository cnabOrgRepository,
                                    ICnabOrgValidatorService cnabOrgValidatorService,
                                    IFuncionalidadeSistemaRepository funcionalidadeSistemaRepository,
                                    IDBConnectionUnitOfWork dbConnectionUnitOfWork)
        {
            _cnabOrgRepository = cnabOrgRepository;
            _cnabOrgValidatorService = cnabOrgValidatorService;
            _funcionalidadeSistemaRepository = funcionalidadeSistemaRepository;
            _dbConnectionUnitOfWork = dbConnectionUnitOfWork;
        }

        public async Task<ApiGenericResult<IEnumerable<CnabOrgResult>>> ListarCnabOrgs(string cpfRequest, int orgId)
        {
            var apiGenericResult = new ApiGenericResult<IEnumerable<CnabOrgResult>>();
            try
            {
                ValidaAcessoCnabOrg(cpfRequest, orgId);

                var result = await _cnabOrgRepository.ListarCnabOrgsAsync(orgId);

                apiGenericResult.Retorno = result;
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, DESCRICAO_ENTIDADE);
            }

            return apiGenericResult;
        }

        public async Task<ApiGenericResult<CnabOrgResult>> ObterCnabOrgPorId(Guid id, string cpfRequest, int orgId)
        {
            var apiGenericResult = new ApiGenericResult<CnabOrgResult>();

            try
            {
                ValidaAcessoCnabOrg(cpfRequest, orgId);

                var result = await _cnabOrgRepository.ObterCnabOrgPorIdAsync(id);

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

        public async Task<ApiGenericResult<CnabOrgResult>> InserirCnabOrg(CnabOrgInput cnabOrgInput, string cpfRequest, int orgId)
        {
            var apiGenericResult = new ApiGenericResult<CnabOrgResult>();

            try
            {
                ValidaAcessoCnabOrg(cpfRequest, orgId);

                cnabOrgInput.ConfigurarParaPersistencia(orgId, Guid.NewGuid());

                await _cnabOrgValidatorService.ValidaCnabOrg(cnabOrgInput, CRUDEnum.Create);

                _dbConnectionUnitOfWork.BeginTransaction();

                var result = await _cnabOrgRepository.InserirCnabOrgAsync(cnabOrgInput);

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

        public async Task<ApiGenericResult<CnabOrgResult>> AtualizarCnabOrg(CnabOrgInput cnabOrgInput, Guid id, string cpfRequest, int orgId)
        {
            var apiGenericResult = new ApiGenericResult<CnabOrgResult>();

            try
            {
                ValidaAcessoCnabOrg(cpfRequest, orgId);

                cnabOrgInput.ConfigurarParaPersistencia(orgId, id);

                await _cnabOrgValidatorService.ValidaCnabOrg(cnabOrgInput, CRUDEnum.Update);

                _dbConnectionUnitOfWork.BeginTransaction();

                var result = await _cnabOrgRepository.AtualizarCnabOrgAsync(cnabOrgInput);

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

        public async Task<ApiGenericResult> DeletarCnabOrg(Guid id, string cpfRequest, int orgId)
        {
            var apiGenericResult = new ApiGenericResult();

            try
            {
                ValidaAcessoCnabOrg(cpfRequest, orgId);

                _dbConnectionUnitOfWork.BeginTransaction();

                var cnabOrgResult = (await ObterCnabOrgPorId(id, cpfRequest, orgId)).Retorno;

                var cnabOrgInput = new CnabOrgInput();
                cnabOrgInput.AtualizarPropriedadesDaClasseBase(cnabOrgResult);
                cnabOrgInput.ConfigurarParaPersistencia(orgId, id);

                await _cnabOrgValidatorService.ValidaCnabOrg(cnabOrgInput, CRUDEnum.Delete);

                var sucesso = await _cnabOrgRepository.DeletarCnabOrgAsync(id);

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

        private void ValidaAcessoCnabOrg(string cpfRequest, int orgId)
        {
            //var isValid = _funcionalidadeSistemaRepository.ValidaAcessoFuncionalidade(
            //    cpfRequest, orgId, FuncionalidadeSistemaEnum.CADASTRO_XPTO);

            //if (!isValid.Result)
            //{
            //    throw new UnauthorizedAccessException($"Acesso negado para {DESCRICAO_ENTIDADE}");
            //}
        }

        public async Task<ApiGenericResult<IEnumerable<DiretoriaResultDTO>>> ListarDiretoriasDisponiveis(string cpfRequest, int orgId)
        {
            var apiGenericResult = new ApiGenericResult<IEnumerable<DiretoriaResultDTO>>();
            try
            {
                ValidaAcessoCnabOrg(cpfRequest, orgId);

                var diretorias = await _cnabOrgRepository.ListarDiretoriasAsync(orgId);

                foreach (var diretoria in diretorias)
                {
                    diretoria.Diretoria = diretoria.ConfiguracaoParaTodaOrg
                        ? "TODOS"
                        : diretoria.CodDiretoria;
                }

                apiGenericResult.Retorno = diretorias;
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, DESCRICAO_ENTIDADE);
            }

            return apiGenericResult;
        }
    }
}