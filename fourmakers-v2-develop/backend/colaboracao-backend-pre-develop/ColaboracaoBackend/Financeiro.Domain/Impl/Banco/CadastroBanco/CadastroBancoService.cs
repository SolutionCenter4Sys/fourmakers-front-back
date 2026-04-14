using Colaboracao.Core.Interfaces;
using Colaboracao.Helper.Enum;
using Core.Domain.Financeiro.Banco;
using Core.Domain.Usuario.Permissao;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Financeiro.Banco.CadastroBanco;
using Financeiro.Domain.Interfaces.Banco.CadastroBanco;

namespace Financeiro.Domain.Impl.Banco.CadastroBanco
{
    public class CadastroBancoService : ICadastroBancoService
    {
        public readonly static string DESCRICAO_ENTIDADE = "Banco";

        private readonly ICadastroBancoRepository _cadastroBancoRepository;
        private readonly ICadastroBancoValidatorService _cadastroBancoValidatorService;
        private readonly IFuncionalidadeSistemaRepository _funcionalidadeSistemaRepository;
        private readonly IDBConnectionUnitOfWork _dbConnectionUnitOfWork;

        public CadastroBancoService(ICadastroBancoRepository cadastroBancoRepository,
                                    ICadastroBancoValidatorService cadastroBancoValidatorService,
                                    IFuncionalidadeSistemaRepository funcionalidadeSistemaRepository,
                                    IDBConnectionUnitOfWork dbConnectionUnitOfWork)
        {
            _cadastroBancoRepository = cadastroBancoRepository;
            _cadastroBancoValidatorService = cadastroBancoValidatorService;
            _funcionalidadeSistemaRepository = funcionalidadeSistemaRepository;
            _dbConnectionUnitOfWork = dbConnectionUnitOfWork;
        }

        public async Task<ApiGenericResult<IEnumerable<CadastroBancoResult>>> ListarCadastroBancos(string cpfRequest, int orgId)
        {
            var apiGenericResult = new ApiGenericResult<IEnumerable<CadastroBancoResult>>();
            try
            {
                ValidaAcessoCadastroBanco(cpfRequest, orgId);

                var result = await _cadastroBancoRepository.ListarCadastroBancosAsync();

                apiGenericResult.Retorno = result;
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, DESCRICAO_ENTIDADE);
            }

            return apiGenericResult;
        }

        public async Task<ApiGenericResult<CadastroBancoResult>> ObterCadastroBancoPorCodigo(string codigoBanco, string cpfRequest, int orgId)
        {
            var apiGenericResult = new ApiGenericResult<CadastroBancoResult>();

            try
            {
                ValidaAcessoCadastroBanco(cpfRequest, orgId);

                var result = await _cadastroBancoRepository.ObterCadastroBancoPorCodigoAsync(codigoBanco);

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

        public async Task<ApiGenericResult<CadastroBancoResult>> InserirCadastroBanco(CadastroBancoInput cadastroBancoInput, string cpfRequest, int orgId)
        {
            var apiGenericResult = new ApiGenericResult<CadastroBancoResult>();

            try
            {
                ValidaAcessoCadastroBanco(cpfRequest, orgId);

                await _cadastroBancoValidatorService.ValidaCadastroBanco(cadastroBancoInput, CRUDEnum.Create);

                _dbConnectionUnitOfWork.BeginTransaction();

                var result = await _cadastroBancoRepository.InserirCadastroBancoAsync(cadastroBancoInput);

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

        public async Task<ApiGenericResult<CadastroBancoResult>> AtualizarCadastroBanco(CadastroBancoInput cadastroBancoInput, string codigoBanco, string cpfRequest, int orgId)
        {
            var apiGenericResult = new ApiGenericResult<CadastroBancoResult>();

            try
            {
                ValidaAcessoCadastroBanco(cpfRequest, orgId);

                await _cadastroBancoValidatorService.ValidaCadastroBanco(cadastroBancoInput, CRUDEnum.Update);

                _dbConnectionUnitOfWork.BeginTransaction();

                var result = await _cadastroBancoRepository.AtualizarCadastroBancoAsync(cadastroBancoInput);

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

        public async Task<ApiGenericResult> DeletarCadastroBanco(string codigoBanco, string cpfRequest, int orgId)
        {
            var apiGenericResult = new ApiGenericResult();

            try
            {
                ValidaAcessoCadastroBanco(cpfRequest, orgId);

                _dbConnectionUnitOfWork.BeginTransaction();

                var cadastroBancoResult = (await ObterCadastroBancoPorCodigo(codigoBanco, cpfRequest, orgId)).Retorno;

                var cadastroBancoInput = new CadastroBancoInput();
                cadastroBancoInput.AtualizarPropriedadesDaClasseBase(cadastroBancoResult);

                await _cadastroBancoValidatorService.ValidaCadastroBanco(cadastroBancoInput, CRUDEnum.Delete);

                var sucesso = await _cadastroBancoRepository.DeletarCadastroBancoAsync(cadastroBancoInput.CodigoBanco);

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

        private void ValidaAcessoCadastroBanco(string cpfRequest, int orgId)
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