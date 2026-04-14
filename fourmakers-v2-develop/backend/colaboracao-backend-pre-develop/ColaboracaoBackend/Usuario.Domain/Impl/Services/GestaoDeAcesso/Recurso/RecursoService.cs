using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Colaboracao.Helper.Enum;
using Colaboracao.Helper.Util;
using Core.Domain.Usuario.GestaoDeAcesso;
using Core.Domain.Usuario.Permissao;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Fourmakers;
using DataTransferObject.Domain.Log;
using DataTransferObject.Domain.Usuario;
using DataTransferObject.Domain.Usuario.GestaoDeAcesso.Recurso;
using DataTransferObject.Domain.Usuario.GestaoDeAcesso.Recurso.Constantes;
using Foursys.Domain.Interfaces.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Usuario.Domain.Interfaces.Services;
using Usuario.Domain.Interfaces.Services.GestaoDeAcesso.Recurso;

using Logs.Infra.Attributes;

namespace Usuario.Domain.Impl.Services.GestaoDeAcesso.Recurso
{
    [LogDomainClass]
    public class RecursoService : IRecursoService
    {
        public readonly static string DESCRICAO_ENTIDADE = "Recurso";

        private readonly IRecursoRepository _recursoRepository;
        private readonly IRecursoMenuFuncionalidadeSistemaRepository _recursoMenuFuncionalidadeSistemaRepository;
        private readonly IRecursoMenuRepository _recursoMenuRepository;
        private readonly IRecursoValidatorService _recursoValidatorService;
        private readonly IFuncionalidadeSistemaRepository _funcionalidadeSistemaRepository;
        private readonly IUsuarioCacheService _usuarioCacheService;
        private readonly IDBConnectionUnitOfWork _dbConnectionUnitOfWork;
        private readonly IBuscaParametroConfiguracaoService _buscaParametroConfiguracaoService;
        private readonly ILogDBCore _logDB; 

        public RecursoService(IRecursoRepository recursoRepository,
                                    IRecursoValidatorService recursoValidatorService,

                                    IRecursoMenuFuncionalidadeSistemaRepository recursoMenuFuncionalidadeSistemaRepository,
                                    IRecursoMenuRepository recursoMenuRepository,

                                    IUsuarioCacheService usuarioCacheService,

                                    IFuncionalidadeSistemaRepository funcionalidadeSistemaRepository,

                                    IBuscaParametroConfiguracaoService buscaParametroConfiguracaoService,

                                    IDBConnectionUnitOfWork dbConnectionUnitOfWork,
                                    ILogDBCore logDB
                              )
        {
            _recursoRepository = recursoRepository;
            _recursoValidatorService = recursoValidatorService;

            _recursoMenuFuncionalidadeSistemaRepository = recursoMenuFuncionalidadeSistemaRepository;
            _recursoMenuRepository = recursoMenuRepository;

            _usuarioCacheService = usuarioCacheService;

            _funcionalidadeSistemaRepository = funcionalidadeSistemaRepository;

            _buscaParametroConfiguracaoService = buscaParametroConfiguracaoService;

            _dbConnectionUnitOfWork = dbConnectionUnitOfWork;

            _logDB = logDB;
        }

        public async Task<ApiGenericResult<IEnumerable<RecursoResult>>> ListarRecursos(string cpfRequest, int orgId)
        {
            var apiGenericResult = new ApiGenericResult<IEnumerable<RecursoResult>>();
            try
            {
                ValidaAcessoRecurso(cpfRequest, orgId);

                var recursosResult = await _recursoRepository.ListarRecursosAsync();

                var listaMenu = await _recursoMenuRepository.ListarRecursoMenusAsync();

                foreach (var recurso in recursosResult)
                {
                    recurso.Menu = listaMenu.SingleOrDefault(m => m.CodigoRecursoMenu == recurso.CodigoRecursoMenuTemp);

                    var listaFuncSistema = await _recursoMenuFuncionalidadeSistemaRepository.ObterRecursoMenuFuncionalidadeSistemaPorCodigoRecursoMenuAsync(recurso.Menu.CodigoRecursoMenu);
                    foreach (var funcSistema in listaFuncSistema)
                    {
                        recurso.FuncionalidadesSistema.Add(funcSistema);
                    }
                }

                apiGenericResult.Retorno = recursosResult.OrderBy(r => r.Menu.TipoMenu.Contains("_sidebar") ? 1 :
                                                                       r.Menu.TipoMenu.Contains("_header") ? 2 :
                                                                       r.Menu.TipoMenu.Contains("_profile") ? 3 : 4)
                                                         .ThenBy(o => o.Menu.Ordenacao);
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, DESCRICAO_ENTIDADE);
            }

            return apiGenericResult;
        }

        public async Task<ApiGenericResult<IEnumerable<RecursoMenuAninhadoResult>>> ListarRecursosVisaoMenu(string cpfRequest, int orgId)
        {
            var apiGenericResult = new ApiGenericResult<IEnumerable<RecursoMenuAninhadoResult>>();
            try
            {
                var listaRecursos = _usuarioCacheService.ObterListarRecursosVisaoMenuCache(cpfRequest, orgId);

                if (listaRecursos != null)
                {
                    apiGenericResult.Retorno = listaRecursos;
                    return apiGenericResult;
                }

                var recursosResult = await _recursoRepository.ListarRecursosVisaoMenuAsync(cpfRequest, orgId);

                var recursosVisaoMenu = RecursoMenuAninhadoResult.MontarHierarquia(recursosResult);

                var labelColaboradores = _buscaParametroConfiguracaoService.GetParametroConfiguracao<string>(
                    ParametroOrgCodigoFrontEndEnum.LABEL_COLABORADORES_TIMESHEET,
                    orgId,
                    cpfRequest
                );

                AtualizarNomenclaturaComLabels(recursosVisaoMenu, orgId, cpfRequest);

                apiGenericResult.Retorno = recursosVisaoMenu;
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, DESCRICAO_ENTIDADE);
            }

            return apiGenericResult;
        }

        private void AtualizarNomenclaturaComLabels(List<RecursoMenuAninhadoResult> menus, int orgId, string cpfRequest)
        {
            // Busca as labels
            var labelColaboradores = _buscaParametroConfiguracaoService.GetParametroConfiguracao<string>(
                ParametroOrgCodigoFrontEndEnum.LABEL_COLABORADORES_TIMESHEET,
                orgId,
                cpfRequest
            );

            var labelColaborador = _buscaParametroConfiguracaoService.GetParametroConfiguracao<string>(
                ParametroOrgCodigoFrontEndEnum.LABEL_COLABORADOR_TIMESHEET,
                orgId,
                cpfRequest
            );

            if (labelColaboradores.IsNotEmpty() || labelColaborador.IsNotEmpty())
            {
                AtualizarNomenclaturaRecursiva(menus, labelColaboradores, labelColaborador);
            }
        }

        private void AtualizarNomenclaturaRecursiva(List<RecursoMenuAninhadoResult> menus, string labelColaboradores, string labelColaborador)
        {
            foreach (var menu in menus)
            {
                if (menu.NomeMenu.IndexOf("colaboradores", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    menu.NomeMenu = Regex.Replace(menu.NomeMenu, "(?i)colaboradores", labelColaboradores);
                }
                else if (menu.NomeMenu.IndexOf("colaborador", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    menu.NomeMenu = Regex.Replace(menu.NomeMenu, "(?i)colaborador", labelColaborador);
                }

                // Processa os submenus recursivamente
                if (menu.SubMenus.Any())
                {
                    AtualizarNomenclaturaRecursiva(menu.SubMenus, labelColaboradores, labelColaborador);
                }
            }
        }

        public async Task<ApiGenericResult<RecursoResult>> ObterRecursoPorCodigoRecurso(string codigoRecurso, string cpfRequest, int orgId)
        {
            var apiGenericResult = new ApiGenericResult<RecursoResult>();

            try
            {
                ValidaAcessoRecurso(cpfRequest, orgId);

                var recurso = await _recursoRepository.ObterRecursoPorCodigoRecursoAsync(codigoRecurso);

                recurso.Menu = await _recursoMenuRepository.ObterRecursoMenuPorCodigoRecursoAsync(recurso.CodigoRecurso);

                var listaFuncSistema = await _recursoMenuFuncionalidadeSistemaRepository.ObterRecursoMenuFuncionalidadeSistemaPorCodigoRecursoMenuAsync(recurso.Menu.CodigoRecursoMenu);

                foreach (var funcSistema in listaFuncSistema)
                {
                    recurso.FuncionalidadesSistema.Add(funcSistema);
                }

                if (recurso == null)
                {
                    ExceptionUtil.NaoEncontrado(DESCRICAO_ENTIDADE);
                }

                apiGenericResult.Retorno = recurso;

            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, DESCRICAO_ENTIDADE);
            }

            return apiGenericResult;
        }

        public async Task<ApiGenericResult<List<RecursoResult>>> InserirRecursos(List<RecursoInput> recursoInputs, bool forcarExclusao, string cpfRequest, int orgId)
        {
            var apiGenericResult = new ApiGenericResult<List<RecursoResult>> { Retorno = new List<RecursoResult>() };

            try
            {
                ValidaAcessoRecurso(cpfRequest, orgId);

                _dbConnectionUnitOfWork.BeginTransaction();

                if (forcarExclusao)
                {
                    await _recursoRepository.DeletarTodosRecursosAsync();
                }

                foreach (var recursoInput in recursoInputs)
                {
                    recursoInput.ConfigurarParaPersistencia(cpfRequest, recursoInput.CodigoRecurso);
                    await _recursoValidatorService.ValidaRecurso(recursoInput, CRUDEnum.Create, recursoInputs);
                };

                // Primeiro: inserir recursos do tipo group_sidebar
                var recursosGroupSidebar = recursoInputs.Where(r => r.Menu != null && r.Menu.TipoMenu == TipoMenuEnum.group_sidebar.ToString()).ToList();
                foreach (var recursoInput in recursosGroupSidebar)
                { 
                    var resultRecurso = await _recursoRepository.InserirRecursoAsync(recursoInput);

                    if (recursoInput.Menu != null)
                    {
                        recursoInput.Menu.ConfigurarParaPersistencia(orgId, cpfRequest, Guid.NewGuid());
                        resultRecurso.Menu = await _recursoMenuRepository.InserirRecursoMenuAsync(recursoInput.Menu);
                    }

                    if (resultRecurso == null)
                    {
                        ExceptionUtil.NaoInserido(DESCRICAO_ENTIDADE);
                    }

                    apiGenericResult.Retorno.Add(resultRecurso);
                }

                // Segundo: inserir demais recursos (exceto group_sidebar)
                var demaisRecursos = recursoInputs.Where(r => r.Menu == null || r.Menu.TipoMenu != TipoMenuEnum.group_sidebar.ToString()).ToList();
                foreach (var recursoInput in demaisRecursos)
                { 
                    var resultRecurso = await _recursoRepository.InserirRecursoAsync(recursoInput);

                    if (recursoInput.Menu != null)
                    {
                        recursoInput.Menu.ConfigurarParaPersistencia(orgId, cpfRequest, Guid.NewGuid());
                        resultRecurso.Menu = await _recursoMenuRepository.InserirRecursoMenuAsync(recursoInput.Menu);
                    }

                    if (resultRecurso == null)
                    {
                        ExceptionUtil.NaoInserido(DESCRICAO_ENTIDADE);
                    }

                    apiGenericResult.Retorno.Add(resultRecurso);
                }

                // Terceiro: inserir funcionalidades após todos os recursos e menus estarem inseridos
                foreach (var recursoInput in recursoInputs)
                {
                    var resultRecurso = apiGenericResult.Retorno.FirstOrDefault(r => r.Menu.CodigoRecursoMenu == recursoInput.Menu.CodigoRecursoMenu);
                    
                    if (recursoInput.FuncionalidadesSistema != null && resultRecurso != null)
                    {
                        foreach (var funcSistema in recursoInput.FuncionalidadesSistema)
                        {
                            //funcSistema.CodigoRecursoMenu;
                            var resultFunc = await _recursoMenuFuncionalidadeSistemaRepository.InserirRecursoMenuFuncionalidadeSistemaAsync(funcSistema);
                            resultRecurso.FuncionalidadesSistema.Add(resultFunc);
                        }
                    }
                }

                //antes de commitar faz log da última inserção que funcionou
                _logDB.SaveLogDefaultInDatabase("Recursos - Última inserção", JsonConvert.SerializeObject(apiGenericResult.Retorno), ProcessIdentifierEnum.MenuLastInsertedData, cpfRequest);

                _dbConnectionUnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                _dbConnectionUnitOfWork.SafeRollback();
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Create, DESCRICAO_ENTIDADE);
            }

            return apiGenericResult;
        }


        public async Task<ApiGenericResult<RecursoResult>> AtualizarRecurso(RecursoInput recursoInput, string codigoRecurso, string cpfRequest, int orgId)
        {
            var apiGenericResult = new ApiGenericResult<RecursoResult>();

            try
            {
                ValidaAcessoRecurso(cpfRequest, orgId);

                recursoInput.ConfigurarParaPersistencia(cpfRequest, codigoRecurso);

                await _recursoValidatorService.ValidaRecurso(recursoInput, CRUDEnum.Update);

                _dbConnectionUnitOfWork.BeginTransaction();

                var result = await _recursoRepository.AtualizarRecursoAsync(recursoInput);

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

        public async Task<ApiGenericResult> DeletarRecurso(string codigoRecurso, bool forcarExclusao, string cpfRequest, int orgId)
        {
            var apiGenericResult = new ApiGenericResult();

            try
            {
                ValidaAcessoRecurso(cpfRequest, orgId);

                _dbConnectionUnitOfWork.BeginTransaction();

                var recursoResult = (await ObterRecursoPorCodigoRecurso(codigoRecurso, cpfRequest, orgId)).Retorno;

                var recursoInput = new RecursoInput();
                recursoInput.AtualizarPropriedadesDaClasseBase(recursoResult);
                recursoInput.ConfigurarParaPersistencia(cpfRequest, codigoRecurso);

                await _recursoValidatorService.ValidaRecurso(recursoInput, CRUDEnum.Delete, new(), forcarExclusao);

                var sucesso = await _recursoRepository.DeletarRecursoAsync(codigoRecurso);

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

        public async Task<ApiGenericResult> DeletarTodosRecurso(string cpfRequest, int orgId)
        {
            var apiGenericResult = new ApiGenericResult();

            try
            {
                ValidaAcessoRecurso(cpfRequest, orgId);

                _dbConnectionUnitOfWork.BeginTransaction();

                var sucesso = await _recursoRepository.DeletarTodosRecursosAsync();

                if (!sucesso)
                {
                    ExceptionUtil.NaoExcluido(DESCRICAO_ENTIDADE);
                }

                _dbConnectionUnitOfWork.Commit();

                apiGenericResult.Sucesso = sucesso;
                apiGenericResult.Mensagem = $"Todos {DESCRICAO_ENTIDADE}s excluídos com sucesso.";

            }
            catch (Exception ex)
            {
                _dbConnectionUnitOfWork.SafeRollback();
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Delete, DESCRICAO_ENTIDADE);
            }
            return apiGenericResult;
        }

        public async Task<ApiGenericResult<List<RecursoResult>>> ObterUltimaInsercaoRecursoMenuLog(string cpfRequest, int orgId)
        {
            var apiGenericResult = new ApiGenericResult<List<RecursoResult>>();

            try
            {
                ValidaAcessoRecurso(cpfRequest, orgId);

                var log = await _logDB.ObterUltimoLogPorProcessIdentifierAsync(ProcessIdentifierEnum.MenuLastInsertedData);

                apiGenericResult.Retorno = LogUtil.MontarRetornoLog<List<RecursoResult>>(log);
                apiGenericResult.Mensagem = LogUtil.MontarMensagemLogFormatada(log);

            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, DESCRICAO_ENTIDADE);
            }

            return apiGenericResult;
        }


        private void ValidaAcessoRecurso(string cpfRequest, int orgId)
        {
            var isValid = _funcionalidadeSistemaRepository.ValidaAcessoFuncionalidade(
                cpfRequest, orgId, FuncionalidadeSistemaEnum.GESTAO_ACESSO);

            if (!isValid.Result)
            {
                throw new UnauthorizedAccessException($"Acesso negado para {DESCRICAO_ENTIDADE}"); 
            }
        }
   
    }
}