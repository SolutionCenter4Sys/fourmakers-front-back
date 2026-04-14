using Colaboracao.Core.Interfaces;
using Colaboracao.Helper.Enum;
using Core.Domain.Financeiro.Rubrica;
using Core.Domain.Usuario.Permissao;
using DataTransferObject.Domain.Apontamento;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Financeiro.Rubrica.RubricaColaborador;
using Financeiro.Domain.Interfaces.NotaFiscal;
using Financeiro.Domain.Interfaces.Rubrica.Rubrica;
using Financeiro.Domain.Interfaces.Rubrica.RubricaColaborador;
using SRS.Infra.Constantes;

using Logs.Infra.Attributes;

namespace Financeiro.Domain.Impl.Rubrica.RubricaColaborador
{
    [LogDomainClass]
    public class RubricaColaboradorService : IRubricaColaboradorService
    {
        public readonly static string DESCRICAO_ENTIDADE = "Rubrica do Colaborador";

        private readonly IRubricaColaboradorRepository _rubricaColaboradorRepository;
        private readonly IRubricaService _rubricaService;
        private readonly IRubricaColaboradorValidatorService _rubricaColaboradorValidatorService;
        private readonly IFuncionalidadeSistemaRepository _funcionalidadeSistemaRepository;
        private readonly IDBConnectionUnitOfWork _dbConnectionUnitOfWork;
        private readonly INotaFiscalService _notaFiscalService;
        private readonly IRestricaoDeAcessoService restricaoDeAcessoService;

        public RubricaColaboradorService(IRubricaColaboradorRepository rubricaColaboradorRepository,
                                    IRubricaColaboradorValidatorService rubricaColaboradorValidatorService,
                                    IFuncionalidadeSistemaRepository funcionalidadeSistemaRepository,
                                    IDBConnectionUnitOfWork dbConnectionUnitOfWork, 
                                    INotaFiscalService notaFiscalService, 
                                    IRubricaService rubricaService, IRestricaoDeAcessoService restricaoDeAcessoService)
        {
            _rubricaColaboradorRepository = rubricaColaboradorRepository;
            _rubricaColaboradorValidatorService = rubricaColaboradorValidatorService;
            _funcionalidadeSistemaRepository = funcionalidadeSistemaRepository;
            _dbConnectionUnitOfWork = dbConnectionUnitOfWork;
            _notaFiscalService = notaFiscalService;
            _rubricaService = rubricaService;
            this.restricaoDeAcessoService = restricaoDeAcessoService;
        }

        public async Task<ApiGenericResult<IEnumerable<RubricaColaboradorResult>>> ListarRubricasColaborador(string cpfRequest, int orgId, string? unidadeId, string? codigoInternoColaborador, int? mes, int? ano, string? rubricaId, int cursor, int limite)
        {
            var apiGenericResult = new ApiGenericResult<IEnumerable<RubricaColaboradorResult>>();
            try
            {
                ValidaAcessoRubricaColaborador(cpfRequest, orgId);
                var restricaoDiretoria = await restricaoDeAcessoService.ListarMinhasRestricoesDeAcessoPorCodigoInternoETipo(cpfRequest, orgId, RestricaoDeAcessoTipoConstants.DIRETORIA, unidadeId);
                var result = await _rubricaColaboradorRepository.ListarRubricasColaboradorAsync(orgId, restricaoDiretoria, codigoInternoColaborador, mes, ano, rubricaId, cursor, limite);

                apiGenericResult.Retorno = result;
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, DESCRICAO_ENTIDADE);
            }

            return apiGenericResult;
        }

        public async Task<ApiGenericResult<RubricaColaboradorResult>> ObterRubricaColaboradorPorId(Guid id, string cpfRequest, int orgId)
        {
            var apiGenericResult = new ApiGenericResult<RubricaColaboradorResult>();

            try
            {
                ValidaAcessoRubricaColaborador(cpfRequest, orgId);

                var result = await _rubricaColaboradorRepository.ObterRubricaColaboradorPorIdAsync(id);

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

        public async Task<ApiGenericResult<RubricaColaboradorResult>> InserirRubricaColaborador(RubricaColaboradorInput rubricaColaboradorInput, string cpfRequest, int orgId, bool useTransaction = true, bool validaAcesso = true)
        {
            var apiGenericResult = new ApiGenericResult<RubricaColaboradorResult>();

            try
            {
                ValidaAcessoRubricaColaborador(cpfRequest, orgId);

                rubricaColaboradorInput.ConfigurarParaPersistencia(orgId, cpfRequest, Guid.NewGuid());

                await _rubricaColaboradorValidatorService.ValidaRubricaColaborador(rubricaColaboradorInput, CRUDEnum.Create);

               if(useTransaction)
                   _dbConnectionUnitOfWork.BeginTransaction();

                var result = await _rubricaColaboradorRepository.InserirRubricaColaboradorAsync(rubricaColaboradorInput);
                
                if (result == null)
                {
                    ExceptionUtil.NaoInserido(DESCRICAO_ENTIDADE);
                }

                if(useTransaction)
                    _dbConnectionUnitOfWork.Commit();

                apiGenericResult.Retorno = result;
            }
            catch (Exception ex)
            {
                if(useTransaction)
                   _dbConnectionUnitOfWork.SafeRollback();
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Create, DESCRICAO_ENTIDADE);
            }

            return apiGenericResult;
        }

        public async Task<ApiGenericResult<RubricaColaboradorResult>> AtualizarRubricaColaborador(RubricaColaboradorInput rubricaColaboradorInput, Guid id, string cpfRequest, int orgId,  bool validaAcesso = true)
        {
            var apiGenericResult = new ApiGenericResult<RubricaColaboradorResult>();

            try
            {
                ValidaAcessoRubricaColaborador(cpfRequest, orgId);

                rubricaColaboradorInput.ConfigurarParaPersistencia(orgId, cpfRequest, id);

                await _rubricaColaboradorValidatorService.ValidaRubricaColaborador(rubricaColaboradorInput, CRUDEnum.Update);

                _dbConnectionUnitOfWork.BeginTransaction();

                var result = await _rubricaColaboradorRepository.AtualizarRubricaColaboradorAsync(rubricaColaboradorInput);

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

        public async Task<ApiGenericResult> DeletarRubricaColaborador(Guid id, string cpfRequest, int orgId, bool validaAcesso = true)
        {
            var apiGenericResult = new ApiGenericResult();

            try
            {
                ValidaAcessoRubricaColaborador(cpfRequest, orgId);

                _dbConnectionUnitOfWork.BeginTransaction();

                var rubricaColaboradorResult = (await ObterRubricaColaboradorPorId(id, cpfRequest, orgId)).Retorno;

                var rubricaColaboradorInput = new RubricaColaboradorInput();
                rubricaColaboradorInput.AtualizarPropriedadesDaClasseBase(rubricaColaboradorResult);
                rubricaColaboradorInput.ConfigurarParaPersistencia(orgId, cpfRequest, id);

                await _rubricaColaboradorValidatorService.ValidaRubricaColaborador(rubricaColaboradorInput, CRUDEnum.Delete);

                var sucesso = await _rubricaColaboradorRepository.DeletarRubricaColaboradorAsync(id);
                

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

        public async Task<ApiGenericResult<List<VigenciaDTO>>> ListarMesEAnosLancadosPorOrgId(int orgId)
        {
            var apiGenericResult = new ApiGenericResult<List<VigenciaDTO>>();
            try
            {
                apiGenericResult.Retorno = await _rubricaColaboradorRepository.ListarMesEAnosLancadosPorOrgId(orgId);
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, "Meses e Anos Lançados");
            }
            return apiGenericResult;
        }

        private void ValidaAcessoRubricaColaborador(string cpfRequest, int orgId)
        {
            //var isValid = _funcionalidadeSistemaRepository.ValidaAcessoFuncionalidade(
            //    cpfRequest, orgId, FuncionalidadeSistemaEnum.CADASTRO_XPTO);

            //if (!isValid.Result)
            //{
            //    throw new UnauthorizedAccessException($"Acesso negado para {DESCRICAO_ENTIDADE}"); 
            //}
        }

        public async Task<ApiGenericResult<IEnumerable<RubricaColaboradorDetalhadoDTO>>> ListarRubricasColaboradorDetalhado(string cpfRequest, int mes, int ano, int orgId)
        {
            var apiGenericResult = new ApiGenericResult<IEnumerable<RubricaColaboradorDetalhadoDTO>>();
            try
            {
                if(cpfRequest != "#NAO_VALIDAR") ValidaAcessoRubricaColaborador(cpfRequest, orgId);

                var result = await _rubricaColaboradorRepository.ListarRubricasColaboradorDetalhadoAsync(mes, ano, orgId);

                apiGenericResult.Retorno = result;
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, DESCRICAO_ENTIDADE);
            }

            return apiGenericResult;
        }
      
        public async Task<bool> ValidaSeEPrestadorERubricaContabil(Guid rubricaId, string codigoInternoColaborador, int orgId, string cpfRequest, bool validaAcesso = true)
        {
            var rubrica = (await _rubricaService.ObterRubricaPorId(rubricaId, cpfRequest, orgId, validaAcesso)).Retorno;

            var permiteEmissaoColaborador = await _rubricaColaboradorRepository.ValidaSePodeRefletirEmissaoDeRubricaNFColaborador(codigoInternoColaborador, orgId);
            if (rubrica.RefletirContabil && permiteEmissaoColaborador)
            {
                
                return true;
            }

            return false;
        }
    }
}