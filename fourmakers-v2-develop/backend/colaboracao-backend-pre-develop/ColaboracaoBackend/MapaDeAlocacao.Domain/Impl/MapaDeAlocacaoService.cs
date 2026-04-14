using ApiClient.Domain.Interfaces;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Competencia.Domain.Enums;
using Core.Domain;
using Core.Domain.Apontamento;
using Core.Domain.Colaborador;
using Core.Domain.MapaAlocacao;
using Core.Domain.Usuario;
using Core.Domain.Usuario.Permissao;
using Core.DomainModel.Projeto;
using DataTransferObject.Domain;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.CCH;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.Fourmakers;
using DataTransferObject.Domain.MapaDeAlocacao;
using DataTransferObject.Domain.MapaDeAlocacao.CargaMapaAlocacao;
using DataTransferObject.Domain.MapaDeAlocacao.ConsultaAlocacao;
using DataTransferObject.Domain.MapaDeAlocacao.DetalharColaborador;
using DataTransferObject.Domain.MapaDeAlocacao.EditarAlocacao;
using DataTransferObject.Domain.MapaDeAlocacao.ExcluirAlocacao;
using DataTransferObject.Domain.Notificacao;
using DataTransferObject.Domain.Org;
using DataTransferObject.Domain.Usuario;
using Firebase.Domain.Interfaces.Services;
using Foursys.Domain.Interfaces.Services;
using MapaDeAlocacao.Domain.Interfaces;
using Sprache;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Colaboracao.Helper.Extension;
using SRS.Infra.Constantes;

using Logs.Infra.Attributes;

namespace MapaDeAlocacao.Domain.Impl
{
    [LogDomainClass]
    public class MapaDeAlocacaoService : IMapaDeAlocacaoService
    {
        private readonly ICCHClient _cchClient;
        private readonly IColaboradorClient _colaboradorClient;
        private readonly ICompetenciaClient _competenciaClient;
        private readonly IIdiomaClient _idiomaClient;
        private readonly IDominioClient _dominioClient;
        private readonly IMetodologiaClient _metodologiaClient;
        private readonly ISoftskillClient _softSkillClient;
        private readonly IUsuarioColaboradorRepository _usuarioColaboradorRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapaAlocacaoRepository _mapaDeAlocacaoRepository;
        private readonly IMapaDeAlocacaoValidadorService _validadorService;
        private readonly IProjetoOrgRepository _projetoOrgRepository;
        private readonly IBuscaColaboradorRepository _buscaColaboradorRepository;
        private readonly IBuscaParametroConfiguracaoService _buscaParametroConfiguracaoService;
        private readonly IFuncionalidadeSistemaRepository _funcionalidadeSistemaRepository;
        private readonly ITbdRepository _tbdRepository;
        private readonly IProjetoMapaDeAlocacaoRepository _projetoMapaAlocacaoRepository;
        private readonly IPerfilAlocacaoRepository _perfilAlocacaoRepository;
        private readonly IDBConnectionUnitOfWork _dbUnitOfWork;
        private readonly INotificacaoService _notificacaoService;
        private readonly IRestricaoDeAcessoService _restricaoDeAcessoService;

        private const double HORAS_DIARIAS_COLABORADOR = 8;

        public MapaDeAlocacaoService(ICCHClient cchClient, IColaboradorClient colaboradorClient, IUsuarioColaboradorRepository
            usuarioColaboradorRepository,
            ICompetenciaClient competenciaClient, IDominioClient dominioClient,
            IMetodologiaClient metodologiaClient, ISoftskillClient softskillClient,
            IIdiomaClient idiomaClient, IUnitOfWork unitOfWork, IMapaAlocacaoRepository mapaAlocacaoRepository,
            IMapaDeAlocacaoValidadorService validadorService, IProjetoOrgRepository projetoOrgRepository,
            IBuscaColaboradorRepository buscaColaboradorRepository, IBuscaParametroConfiguracaoService buscaParametroConfiguracaoService,
            IFuncionalidadeSistemaRepository funcionalidadeSistemaRepository, ITbdRepository tbdRepository, IApontamentoRepository apontamentoRepository,
            IProjetoMapaDeAlocacaoRepository projetoMapaAlocacaoRepository, IPerfilAlocacaoRepository perfilAlocacaoRepository, IDBConnectionUnitOfWork dbUnitOfWork, INotificacaoService notificacaoService, IRestricaoDeAcessoService restricaoDeAcessoService)
        {
            _cchClient = cchClient;
            _colaboradorClient = colaboradorClient;
            _usuarioColaboradorRepository = usuarioColaboradorRepository;
            _competenciaClient = competenciaClient;
            _dominioClient = dominioClient;
            _softSkillClient = softskillClient;
            _metodologiaClient = metodologiaClient;
            _idiomaClient = idiomaClient;
            _unitOfWork = unitOfWork;
            _mapaDeAlocacaoRepository = mapaAlocacaoRepository;
            _validadorService = validadorService;
            _projetoOrgRepository = projetoOrgRepository;
            _buscaColaboradorRepository = buscaColaboradorRepository;
            _buscaParametroConfiguracaoService = buscaParametroConfiguracaoService;
            _funcionalidadeSistemaRepository = funcionalidadeSistemaRepository;
            _tbdRepository = tbdRepository;
            _projetoMapaAlocacaoRepository = projetoMapaAlocacaoRepository;
            _perfilAlocacaoRepository = perfilAlocacaoRepository;
            _dbUnitOfWork = dbUnitOfWork;
            _notificacaoService = notificacaoService;
            _restricaoDeAcessoService = restricaoDeAcessoService;
        }

        public async Task<CadastroMapaAlocacaoDTO> CadastroMapaAlocacao(CadastroMapaAlocacaoDTO cadastroMapaAlocacaoDTO, string cpfSolicitante, int orgId)
        {
            ValidaAcessoMapaAlocacao(cpfSolicitante, orgId, FuncionalidadeSistemaEnum.MAPA_DE_ALOCACAO_EDICAO);

            var cadastroMapaAlocacaoValidacao = cadastroMapaAlocacaoDTO.ToCadastroMapaAlocacaoValidacaoDTO(orgId, cpfSolicitante);
            bool isColaborador = cadastroMapaAlocacaoValidacao.IsColaborador;
            bool isTbd = cadastroMapaAlocacaoValidacao.IsTbd;

            await _validadorService.ValidaMapaAlocacaoCadastro(cadastroMapaAlocacaoValidacao);

            try
            {
                var associacaoAutomaticaColaboradorAoProjeto = _buscaParametroConfiguracaoService.GetParametroConfiguracao<bool>(ParametroOrgCodigoEnum.ASSOCIACAO_AUTOMATICA_COLABORADOR_PROJETO, orgId, cpfSolicitante);

                if (isColaborador && //associacao automatica somente para colaborador
                    associacaoAutomaticaColaboradorAoProjeto)
                {
                    await _mapaDeAlocacaoRepository.CadastrarAssociacaoAutomaticaNoProjeto(cadastroMapaAlocacaoDTO.CodigoColaborador, cadastroMapaAlocacaoDTO.CodigoProjeto, orgId);
                }

                var retorno = await _mapaDeAlocacaoRepository.CadastroMapaAlocacao(cadastroMapaAlocacaoDTO, orgId, isTbd, isColaborador);

                _dbUnitOfWork.BeginTransaction();
                try
                {
                    string idPerfil = null;
                    string idPerfilGestorExterno = null;
                    if (String.IsNullOrEmpty(cadastroMapaAlocacaoDTO.IdPerfilAlocacao) && !String.IsNullOrEmpty(cadastroMapaAlocacaoDTO.NomePerfilAlocacao))
                    {
                        idPerfil = _perfilAlocacaoRepository.InserirNovoPerfilAlocacao(cadastroMapaAlocacaoDTO.NomePerfilAlocacao, cadastroMapaAlocacaoDTO.CodigoProjeto, cadastroMapaAlocacaoDTO.PerfilSkills, cpfSolicitante, orgId);
                    }
                    else if (!String.IsNullOrEmpty(cadastroMapaAlocacaoDTO.IdPerfilAlocacao))
                    {
                        var arrIdPerfilAlocacao = cadastroMapaAlocacaoDTO.IdPerfilAlocacao.Split("|");
                        if (arrIdPerfilAlocacao[0] == "1")
                            idPerfil = arrIdPerfilAlocacao[1];
                        if (arrIdPerfilAlocacao[0] == "2")
                            idPerfilGestorExterno = arrIdPerfilAlocacao[1];
                    }
                    if (idPerfil != null || idPerfilGestorExterno != null)
                        _perfilAlocacaoRepository.VincularPerfilAlocacao(retorno.PeriodoAlocadoId, idPerfilGestorExterno, idPerfil, orgId);

                    _perfilAlocacaoRepository.InserirSkillsAlocacao(retorno.PeriodoAlocadoId, cadastroMapaAlocacaoDTO.PerfilSkills, cpfSolicitante);
                    _dbUnitOfWork.Commit();
                }
                catch (Exception e)
                {
                    _dbUnitOfWork.Rollback();
                    throw new Exception("Falha ao inserir o perfil da alocação. " + e.Message);
                }
                await this.ProcessarCalculoMensal(isTbd ? cadastroMapaAlocacaoDTO.CodigoTbd.ToString() : cadastroMapaAlocacaoDTO.CodigoColaborador, cadastroMapaAlocacaoValidacao.IsTbd, orgId);

                return retorno;
            }
            catch (Exception)
            {
                Console.WriteLine("Erro na Service: CadastroMapaAlocacao");
                throw;
            }
        }

        public async Task<CargaMapaAlocacaoResult> InserirCargaMapaAlocacao(string cpf, int orgId)
        {
            int cursor = 0;
            int totalRegistros = 0;

            int qtdColaboradorInserido = 0;
            int qtdColaboradoresExistentes = 0;
            int qtdMapaAlocacaoInseridos = 0;

            CargaMapaAlocacaoResult cargaMapaResult = null;
            var cargaInseridaList = new List<DataTransferObject.Domain.MapaDeAlocacao.CargaMapaAlocacao.Result>();
            var conflitoAlocacaoList = new List<ConflitoAlocacao>();

            do
            {
                try
                {
                    ValidaAcessoMapaAlocacao(cpf, orgId, FuncionalidadeSistemaEnum.MAPA_DE_ALOCACAO_EDICAO);

                    cargaMapaResult = await _colaboradorClient.BuscarCargaMapaAlocacao(cursor, 0);

                    if (cargaMapaResult.Response.results.Count == 0)
                        return cargaMapaResult;

                    if (cursor == 0)
                        totalRegistros = cargaMapaResult.Response.remaining + 100;

                    cursor = totalRegistros - cargaMapaResult.Response.remaining;

                    var cargaMapaList = cargaMapaResult.Response.results;

                    var tokenCCH = (await _cchClient.Autenticacao()).token;

                    if (tokenCCH == null)
                        throw new ApplicationException("Token CCH inválido.");

                    var associacaoAutomaticaColaboradorAoProjeto = _buscaParametroConfiguracaoService.GetParametroConfiguracao<bool>(ParametroOrgCodigoEnum.ASSOCIACAO_AUTOMATICA_COLABORADOR_PROJETO, orgId, cpf);

                    using (var dbtrans = _unitOfWork.BeginTransaction())
                    {
                        RecursoCCH recurso = null;

                        try
                        {
                            foreach (var colaborador in cargaMapaList)
                            {
                                if (colaborador.cdColaboradorCCH != 0)
                                {
                                    recurso = (await _cchClient.Recurso(colaborador.cdColaboradorCCH.ToString(), tokenCCH));

                                    try
                                    {
                                        if (!_usuarioColaboradorRepository.ExisteColaborador(recurso.cdCpf.RemoveMascaraCpf()))
                                        {
                                            _usuarioColaboradorRepository.UpsertUsuarioColaborador(new UsuarioColaboradorDTO
                                            {
                                                Cpf = recurso.cdCpf.RemoveMascaraCpf(),
                                                Email = recurso.nmEnderecoEletronico
                                            }, new ColaboradorDTO
                                            {
                                                NomeCompleto = recurso.nmProfissional,
                                                Cpf = recurso.cdCpf.RemoveMascaraCpf()
                                            }, null);

                                            _usuarioColaboradorRepository.UpsertColaboradorOrg(
                                                new ColaboradorDTO
                                                {
                                                    NomeCompleto = recurso.nmProfissional,
                                                    Cpf = recurso.cdCpf
                                                },
                                                new ColaboradorOrgDTO
                                                {
                                                    Cargo = recurso.nmCargo,
                                                    CodCargo = recurso.cdCargo.ToString(),
                                                    CodColaborador = colaborador.cdColaboradorCCH.ToString(),
                                                    CodDepartamento = "",
                                                    Departamento = "",
                                                    CodDiretoria = recurso.cdDivisao.ToString(),
                                                    Diretoria = recurso.nmDivisao,
                                                    DataAdmissao = recurso.dtContratacao,
                                                    ModeloContratacao = null,
                                                    EmpresaRelacionada = null,
                                                    ModeloTrabalho = null,
                                                    DiasPorSemana = null,
                                                    ValorHora = null,
                                                    CustoHora = null,
                                                    BaseHoraMes = null,
                                                    OrgId = orgId,
                                                    CodigoModeloContratacao = null
                                                }
                                            );
                                            qtdColaboradorInserido++;
                                        }
                                        else
                                        {
                                            qtdColaboradoresExistentes++;
                                        }

                                        var periodos = new List<PeriodoDTO>();

                                        var hierarquia = (await _cchClient.Hierarquia(tokenCCH));

                                        HierarquiaResult manager = null;
                                        if (hierarquia != null)
                                            manager = hierarquia.Where(x => x.NomeProfissional.ToLower() == colaborador.managername.ToLower()).FirstOrDefault();

                                        var cadastroMapaAlocacao = new CadastroMapaAlocacaoDTO()
                                        {
                                            DataFim = colaborador.datefinal,
                                            DataInicio = colaborador.datestart,
                                            IncluiFimDeSemana = false,
                                            QuantidadeHoras = colaborador.dailyhours,
                                            CpfColaborador = recurso.cdCpf.RemoveMascaraCpf(),
                                            CodigoColaborador = colaborador.cdColaboradorCCH.ToString(),
                                            ColaboradorNome = colaborador.collaborator,
                                            CodigoGestor = manager != null ? manager.CodigoProfissional.ToString() : "",
                                            NomeGestor = manager != null ? manager.NomeProfissional : colaborador.managername,
                                            CodigoProjeto = colaborador.cdProjeto.ToString(),
                                            NomeProjeto = colaborador.projectname,
                                        };

                                        if (associacaoAutomaticaColaboradorAoProjeto)
                                        {
                                            await _mapaDeAlocacaoRepository.CadastrarAssociacaoAutomaticaNoProjeto(cadastroMapaAlocacao.CodigoColaborador, cadastroMapaAlocacao.CodigoProjeto, orgId);
                                        }

                                        await _mapaDeAlocacaoRepository.CadastroMapaAlocacao(cadastroMapaAlocacao, orgId);

                                        cargaInseridaList.Add(colaborador);
                                        qtdMapaAlocacaoInseridos++;
                                    }
                                    catch (Exception ex)
                                    {
                                        dbtrans.Rollback();
                                        throw new ApplicationException(ex.Message);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            dbtrans.Rollback();
                            throw new ApplicationException(ex.Message);
                        }

                        dbtrans.Commit();
                    }
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(ex.Message);
                }
            }
            while (cargaMapaResult.Response.remaining > 0 && cargaMapaResult.Response.count > 0);

            cargaMapaResult.CargaInserida = cargaInseridaList;
            cargaMapaResult.ConflitoAlocacao = conflitoAlocacaoList;
            cargaMapaResult.MapaAlocacaoInseridos = qtdMapaAlocacaoInseridos;
            cargaMapaResult.ColaboradoresExistentes = qtdColaboradoresExistentes;
            cargaMapaResult.ColaboradoresInseridos = qtdColaboradorInserido;

            return cargaMapaResult;
        }

        public async Task<List<BuscarCargaMapaAlocacaoDTO>> BuscarCargaMapaLocacao()
        {
            try
            {
                return await _mapaDeAlocacaoRepository.BuscarCargaPeriodoAlocacaoFoursysBI();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<NomeRecursoDTO> ListarColaboradoresGestor(string codGestor, string cpfSolicitante, int orgId)
        {
            try
            {
                ValidaAcessoMapaAlocacao(cpfSolicitante, orgId, FuncionalidadeSistemaEnum.MAPA_DE_ALOCACAO);
                return _projetoOrgRepository.GetColaboradoresGestor(codGestor, orgId);
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<GestorDTO>> ListarNomesGestores(string cpfSolicitante, int orgId, string? codDiretoria, string codDepartamento)
        {
            try
            {
                ValidaAcessoMapaAlocacao(cpfSolicitante, orgId, FuncionalidadeSistemaEnum.LISTA_GESTORES);
                var restricaoDiretoria = await _restricaoDeAcessoService.ListarMinhasRestricoesDeAcessoPorCodigoInternoETipo(cpfSolicitante, orgId, RestricaoDeAcessoTipoConstants.DIRETORIA, codDiretoria);
                return await _projetoOrgRepository.GetGestoresOrg(orgId, restricaoDiretoria, codDepartamento.ToNullSeTextoNullOuZero());
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<ProjetosColaboradorDTO>> ListarProjetosColaborador(string codColaborador, bool ehTbd, string cpfSolicitante, int orgId, string codigoGerenteProjeto, List<string> listaCodigoCliente, string status, FiltroProjetosPrioritariosEnum prioritarioFiltroEnum)
        {
            try
            {
                //Validar com o Frontend a utilização desse service, já que este está sendo utilizado no timesheet
                //ValidaAcessoMapaAlocacao(cpfSolicitante, orgId, FuncionalidadeSistemaEnum.MAPA_DE_ALOCACAO);
                var associacaoAutomaticaColaboradorAoProjeto = _buscaParametroConfiguracaoService.GetParametroConfiguracao<bool>(ParametroOrgCodigoEnum.ASSOCIACAO_AUTOMATICA_COLABORADOR_PROJETO, orgId, cpfSolicitante);

                return await _projetoOrgRepository.GetProjetosCodColaboradorOrg(codColaborador.ToNullSeTextoNullOuZero(), orgId, associacaoAutomaticaColaboradorAoProjeto, ehTbd, codigoGerenteProjeto.ToNullSeTextoNullOuZero(), listaCodigoCliente, status.ToNullSeTextoNullOuZero(), prioritarioFiltroEnum);
            }
            catch
            {
                throw;
            }
        }

        public async Task<GetMapaAlocacaoRecursoOutputDTO> GetMapaAlocacaoRecurso(GetMapaAlocacaoRecursoInputParam inputParam, string cpfSolicitante, int orgId)
        {
            try
            {
                ValidaAcessoMapaAlocacao(cpfSolicitante, orgId, FuncionalidadeSistemaEnum.MAPA_DE_ALOCACAO);
                ValidacaoUtil.ObrigaCursorLimite(inputParam.Cursor, inputParam.Limite);
                StatusHorasEnum? statusHorasFiltroEnum = ValidarStatusHorasFiltro(inputParam.StatusHorasFiltro);

                ValidaParametrosInformados(inputParam);
                PreencherDataInicialEFinalCasoInformadoApenasTrimestral(inputParam);

                var mapaAlocacaoInputDTO = new GetMapaAlocacaoInputDTO(inputParam);

                var recursos = await _mapaDeAlocacaoRepository.FiltroMapaAlocacao(mapaAlocacaoInputDTO, orgId);

                var ret = new GetMapaAlocacaoRecursoOutputDTO
                {
                    Recurso = recursos.OrderBy(x => x.Nome).ToList()
                };

                return ret;
            }
            catch
            {
                throw;
            }
        }

        public async Task<GetMapaAlocacaoResumoOutputDTO> GetMapaAlocacaoResumo(GetMapaAlocacaoInputParam inputParam, string cpfSolicitante, int orgId)
        {
            try
            {
                ValidaAcessoMapaAlocacao(cpfSolicitante, orgId, FuncionalidadeSistemaEnum.MAPA_DE_ALOCACAO);

                ValidaParametrosInformados(inputParam);
                PreencherDataInicialEFinalCasoInformadoApenasTrimestral(inputParam);

                HorasTotaisDTO resumo = await CalcularResumoAsync(inputParam.DataInicial, inputParam.DataFinal, orgId);

                var ret = new GetMapaAlocacaoResumoOutputDTO
                {
                    Resumo = resumo,
                };

                return ret;
            }
            catch
            {
                throw;
            }
        }

        private void PreencherDataInicialEFinalCasoInformadoApenasTrimestral(GetMapaAlocacaoInputParam inputParam)
        {
            DateTime dataAtual = DateTime.Now;

            if (!inputParam.MesInicial.IsValidInteger() || !inputParam.AnoInicial.IsValidInteger())
            {
                inputParam.MesInicial = dataAtual.Month.ToString();
                inputParam.AnoInicial = dataAtual.Year.ToString();
            }

            if (!inputParam.MesFinal.IsValidInteger() || !inputParam.AnoFinal.IsValidInteger())
            {
                DateTime dataFinal = dataAtual.AddMonths(inputParam.Trimestral.Value ? 2 : 11);
                inputParam.MesFinal = dataFinal.Month.ToString();
                inputParam.AnoFinal = dataFinal.Year.ToString();
            }
        }

        public void ValidaParametrosInformados(GetMapaAlocacaoInputParam inputParam)
        {
            if ((!inputParam.MesFinal.IsValidInteger() || !inputParam.AnoFinal.IsValidInteger()) && inputParam.Trimestral is null)
            {
                throw new ArgumentException("Campos Obrigatórios: Mes Final e Ano Final ou a flag Trimestral.");
            }
        }

        private StatusHorasEnum? ValidarStatusHorasFiltro(int? statusHorasFiltro)
        {
            if (statusHorasFiltro.HasValue)
            {
                if (!Enum.IsDefined(typeof(StatusHorasEnum), statusHorasFiltro))
                {
                    var enumValues = Enum.GetValues(typeof(StatusHorasEnum))
                                         .Cast<StatusHorasEnum>()
                                         .Select(e => $"[{(int)e}] {e}");

                    var errorMessage = $"Parâmetro {nameof(statusHorasFiltro)} incorreto. O valor (inteiro) deve estar dentro das seguintes opções: " + string.Join(", ", enumValues);

                    throw new ApplicationException(errorMessage);
                }
                return (StatusHorasEnum)statusHorasFiltro;
            }
            return null;
        }

        private async Task<HorasTotaisDTO> CalcularResumoAsync(DateTime dataInicial, DateTime dataFinal, int orgId)
        {
            var horasTotaisAlocada = await GetHorasTotalAsync(dataInicial, dataFinal, orgId, null, null);

            var quantidadeColaboradores = await _mapaDeAlocacaoRepository.GetQuantidadeColaboradoresAsync(orgId);

            var feriados = await _projetoMapaAlocacaoRepository.GetFeriadosPorOrgId(orgId);

            var listHorasMensalDTOs = await MapaUtil.GetQuantidadeHorasTotalPrevistas(dataInicial, dataFinal, feriados.Select(x => x.Data).ToArray());

            var mesesRetornoResumo = new List<MesesDTO>();
            var index = 0;
            var horasTotaisAlocadasTotal = 0.0;
            var horasXqtdColaboradoresTotal = 0.0;

            foreach (var horaMensalDTO in listHorasMensalDTOs)
            {
                var horasTotaisAlocadas = horasTotaisAlocada.ElementAt(index).Horas;
                var horasXqtdColaboradoresUn2 = horaMensalDTO.Horas * quantidadeColaboradores;

                horasTotaisAlocadasTotal += horasTotaisAlocadas;
                horasXqtdColaboradoresTotal += horasXqtdColaboradoresUn2;

                mesesRetornoResumo.Add(new MesesDTO
                {
                    NomeMesAtual = new DateTime(horaMensalDTO.Ano, horaMensalDTO.Mes, 1).ToString("MMM/yyyy"),
                    Forca = horasXqtdColaboradoresUn2 != 0 ? horasTotaisAlocadas / horasXqtdColaboradoresUn2 : 0,
                    OciosidadeMes = ((horaMensalDTO.Horas * quantidadeColaboradores) - horasTotaisAlocada.ElementAt(index).Horas).ToStringHoraFormatada()
                });
                index++;
            }

            var resumo = new HorasTotaisDTO
            {
                OciosidadeResumo = horasXqtdColaboradoresTotal != 0 ? 1 - horasTotaisAlocadasTotal / horasXqtdColaboradoresTotal : 0,
                Meses = mesesRetornoResumo
            };

            return resumo;
        }

        public async Task<List<HorasMensalDTO>> GetHorasTotalAsync(DateTime dataInicio, DateTime dataFim, int orgId, string codigoColaborador = null, int? codTbdAlocado = null)
        {
            var calculosMensais = await _mapaDeAlocacaoRepository.GetCalculosMensaisAsync(orgId, codigoColaborador, codTbdAlocado);

            var ret = new List<HorasMensalDTO>();
            var mesesBase = new List<HorasMensalDTO>();
            var dataAtual = dataInicio;

            while (dataAtual <= dataFim)
            {
                mesesBase.Add(new HorasMensalDTO
                {
                    Mes = dataAtual.Month,
                    Ano = dataAtual.Year,
                    Horas = 0
                });
                dataAtual = dataAtual.AddMonths(1);
            }

            foreach (var mes in mesesBase)
            {
                var horasMes = calculosMensais
                    .Where(x => x.Ano == mes.Ano && x.Mes == mes.Mes)
                    .Sum(x => x.Horas);

                mes.Horas = horasMes;
                ret.Add(mes);
            }

            return ret;
        }

        private async Task ProcessarCalculoMensal(string codigoColaboradorOuTbd, bool ehTbd, int orgId, bool forcarRecalculoHorasPrevistas = false)
        {
            var recursos = new List<RecursoMapaDTO>() { };

            var listaAlocacoesColaborador = _mapaDeAlocacaoRepository.ConsultaDatasPeriodoAlocacaoColabOuTbd("", codigoColaboradorOuTbd, null, ehTbd, orgId).Result;

            var codigoColaboradorParaCalcular = !ehTbd ? codigoColaboradorOuTbd : null;
            var codTbdAlocadoParaCalcular = ehTbd ? codigoColaboradorOuTbd.ToIntOuNull() : null;
            var calculosMensal = new List<CalculoMensalDTO>();

            var feriados = _projetoMapaAlocacaoRepository.GetFeriadosPorOrgId(orgId).Result.Select(x => x.Data).ToArray();

            if (listaAlocacoesColaborador.Count() > 0)
            {
                var listaHorasMensalDTO = CalcularHorasAlocadasPorMes(listaAlocacoesColaborador, orgId);

                StatusHorasEnum? statusHorasRecursoFiltro = null;

                foreach (var mesAlocado in listaHorasMensalDTO)
                {
                    var totalHorasMes = MapaUtil.GetHorasPrevistasMesStaticCache(mesAlocado.Mes, mesAlocado.Ano, feriados, forcarRecalculoHorasPrevistas);

                    if (mesAlocado.Horas == totalHorasMes)
                        statusHorasRecursoFiltro = StatusHorasEnum.TodasAsHorasAlocadas;
                    else if (mesAlocado.Horas > totalHorasMes)
                        statusHorasRecursoFiltro = StatusHorasEnum.HorasAlocadasAMais;
                    else
                        statusHorasRecursoFiltro = StatusHorasEnum.HorasPendentes;

                    calculosMensal.Add(
                        new CalculoMensalDTO()
                        {
                            CodigoColaborador = codigoColaboradorParaCalcular,
                            CodTbdAlocado = codTbdAlocadoParaCalcular,
                            Mes = mesAlocado.Mes,
                            Ano = mesAlocado.Ano,
                            Horas = mesAlocado.Horas,
                            StatusColaboradorPeriodoAlocacao = statusHorasRecursoFiltro.Value.ToString(),
                            OrgId = orgId
                        });
                }
            }

            await _mapaDeAlocacaoRepository.RemoverCalculosMensaisAsync(codigoColaboradorParaCalcular, codTbdAlocadoParaCalcular, orgId);
            await _mapaDeAlocacaoRepository.PersistirCalculosMensaisAsync(calculosMensal);
        }

        public List<HorasMensalDTO> CalcularHorasAlocadasPorMes(List<ListaConsultaDatasDTO> periodos, int orgId)
        {
            var mesesContemplados = new List<HorasMensalDTO>();

            var feriados = _projetoMapaAlocacaoRepository.GetFeriadosPorOrgId(orgId).Result.Select(x => x.Data).ToArray();

            foreach (var periodo in periodos)
            {
                DateTime dataInicio = periodo.DataInicioPeriodo;
                DateTime dataFim = periodo.DataFimPeriodo;

                int anoInicio = dataInicio.Year;
                int mesInicio = dataInicio.Month;

                int anoFim = dataFim.Year;
                int mesFim = dataFim.Month;

                for (int ano = anoInicio; ano <= anoFim; ano++)
                {
                    int mesInicioAtual = (ano == anoInicio) ? mesInicio : 1;
                    int mesFimAtual = (ano == anoFim) ? mesFim : 12;

                    for (int mes = mesInicioAtual; mes <= mesFimAtual; mes++)
                    {
                        // Verifica se já existe o mês contemplado na lista
                        var mesExistente = mesesContemplados.FirstOrDefault(m => m.Ano == ano && m.Mes == mes);
                        if (mesExistente == null)
                        {
                            // Se não existe, adiciona na lista com horas inicializadas em zero
                            mesesContemplados.Add(new HorasMensalDTO { Horas = 0, Ano = ano, Mes = mes });
                        }
                    }
                }
            }

            // Calcula as horas alocadas para cada mês contemplado
            foreach (var mes in mesesContemplados)
            {
                // Filtra os períodos de alocação que contemplam o mês atual
                var periodosNoMes = periodos.Where(p =>
                                    (p.DataInicioPeriodo.Year < mes.Ano || (p.DataInicioPeriodo.Year == mes.Ano && p.DataInicioPeriodo.Month <= mes.Mes)) &&
                                    (p.DataFimPeriodo.Year > mes.Ano || (p.DataFimPeriodo.Year == mes.Ano && p.DataFimPeriodo.Month >= mes.Mes)));

                // Calcula as horas alocadas somando os períodos que cobrem o mês
                foreach (var periodo in periodosNoMes)
                {
                    DateTime dataInicioPeriodo = new DateTime(mes.Ano, mes.Mes, 1);
                    DateTime dataFimPeriodo = new DateTime(mes.Ano, mes.Mes, DateTime.DaysInMonth(mes.Ano, mes.Mes));

                    DateTime dataInicio = periodo.DataInicioPeriodo > dataInicioPeriodo ? periodo.DataInicioPeriodo : dataInicioPeriodo;
                    DateTime dataFim = periodo.DataFimPeriodo < dataFimPeriodo ? periodo.DataFimPeriodo : dataFimPeriodo;

                    // Calcula a quantidade de dias úteis no intervalo de datas
                    int diasUteis = DateTimeUtil.CountBusinessDays(dataInicio, dataFim, periodo.IncluiFinalDeSemana, feriados);

                    // Adiciona as horas alocadas considerando os dias úteis e a quantidade de horas por período
                    mes.Horas += diasUteis * periodo.QuantidadeHoras;
                }
            }

            return mesesContemplados.OrderBy(m => m.Ano)
                                    .ThenBy(m => m.Mes)
                                    .ToList();
        }

        public async Task<DetalharColaboradorDTO> GetAlocacaoColaborador(string cpf, int? tbdId, string codigoProjeto, int mes, int ano, string cpfSolicitante, int orgId)
        {
            try
            {
                ValidaAcessoMapaAlocacao(cpfSolicitante, orgId, FuncionalidadeSistemaEnum.MAPA_DE_ALOCACAO);

                var result = await _mapaDeAlocacaoRepository.GetPeriodoAlocacaoColaboradorComFiltro(cpf, tbdId, mes, ano, codigoProjeto, orgId, false);

                var ret = new DetalharColaboradorDTO();

                List<ProjetosDetalhadosDTO> listaProjetos = new List<ProjetosDetalhadosDTO>();

                List<DisponibilidadeHoras> listaDisponibilidadeHoras = new List<DisponibilidadeHoras>();

                double horasAlocadas = 0;
                var listPeriodos = new List<PeriodoAlocadoColaboradorDTO>();

                foreach (var periodo in result)
                {
                    var periodos = new PeriodoAlocadoColaboradorDTO()
                    {
                        IncluiFimDeSemana = periodo.IncluiFimDeSemana,
                        DataInicio = periodo.DataInicio,
                        DataFim = periodo.DataFim,
                        HorasPorDia = periodo.QuantidadeHoras,
                        PeriodoAlocadoId = periodo.PeriodoAlocadoId,
                        DataAlteracao = periodo.DataAlteracao,
                        Observacao = periodo.Observacao,
                        Oportunidade = periodo.Oportunidade,
                        Percentual = periodo.Percentual,
                        Prioritario = periodo.Prioritario,
                        ColaboradorAlocadoId = periodo.ColaboradorAlocadoId,
                    };
                    listPeriodos.Add(periodos);
                }

                foreach (var item in result)
                {
                    var projetoDetalhado = new ProjetosDetalhadosDTO();

                    projetoDetalhado.PeriodoAlocadoId = item.PeriodoAlocadoId;
                    projetoDetalhado.ColaboradorAlocadoId = item.ColaboradorAlocadoId;
                    projetoDetalhado.CpfColaborador = item.CpfColaborador;
                    projetoDetalhado.CodigoTBD = item.CodigoTBD;
                    projetoDetalhado.NomeProjeto = item.NomeProjeto;
                    projetoDetalhado.CodigoCliente = item.CodigoCliente;
                    projetoDetalhado.NomeCliente = item.NomeCliente;
                    projetoDetalhado.LabelProjetoCliente = item.LabelProjetoCliente;
                    projetoDetalhado.CodigoProjeto = item.CodigoProjeto;
                    projetoDetalhado.NomeGestor = item.NomeGestor;
                    projetoDetalhado.CodigoGestor = item.CodigoGestor;
                    projetoDetalhado.IncluiFimDeSemana = item.IncluiFimDeSemana;
                    projetoDetalhado.DetalhesPeriodo.DataInicio = item.DataInicio;
                    projetoDetalhado.DetalhesPeriodo.DataFim = item.DataFim;
                    projetoDetalhado.DetalhesPeriodo.HorasPorDia = item.QuantidadeHoras;
                    projetoDetalhado.DetalhesPeriodo.HorasPeriodo = new List<HorasPeriodoDTO>();

                    projetoDetalhado.PeriodoAlocacao.AddRange(listPeriodos.Where(x => x.ColaboradorAlocadoId == item.ColaboradorAlocadoId));

                    for (int dia = 1; dia <= DateTime.DaysInMonth(ano, mes); dia++)
                    {
                        //validar dataIterador para pegar entre um mês e outro exemplo: 30/06 até 02/07
                        var dataIterador = new DateTime(ano, mes, dia);

                        if (dataIterador.Date >= item.DataInicio.Date && dataIterador.Date <= item.DataFim.Date)
                        {
                            var horaPeriodo = new HorasPeriodoDTO();

                            horaPeriodo.DiaPeriodo = dataIterador.ToString("dd/MMM");

                            if (MapaUtil.EhFinalDeSemana(dataIterador))
                            {
                                if (item.IncluiFimDeSemana)
                                    horaPeriodo.HorasPeriodo = item.QuantidadeHoras;
                            }
                            else
                            {
                                horaPeriodo.HorasPeriodo = item.QuantidadeHoras;
                            }

                            horaPeriodo.PeriodoId = projetoDetalhado.PeriodoAlocadoId;
                            horaPeriodo.isFinalDeSemana = MapaUtil.EhFinalDeSemana(dataIterador);
                            horaPeriodo.Data = dataIterador;

                            projetoDetalhado.DetalhesPeriodo.HorasPeriodo.Add(horaPeriodo);
                        }
                    }

                    listaProjetos.Add(projetoDetalhado);
                }

                for (int dia = 1; dia <= DateTime.DaysInMonth(ano, mes); dia++)
                {
                    var disponibilidadeHoras = new List<DisponibilidadeHoras>();
                    double somaHorasDiarias = 0;
                    var dataIterador = new DateTime(ano, mes, dia);

                    foreach (var item in listaProjetos)
                    {
                        var retDia = item.DetalhesPeriodo.HorasPeriodo.Where(x => Int32.Parse(x.DiaPeriodo.Split('/')[0]) == dia).FirstOrDefault();
                        if (retDia is not null)
                        {
                            somaHorasDiarias += retDia.HorasPeriodo ?? 0;
                        }
                    }

                    var horasDiariasDisponiveis = MapaUtil.EhFinalDeSemana(dataIterador) && somaHorasDiarias == 0 ? 0 : HORAS_DIARIAS_COLABORADOR - somaHorasDiarias;
                    horasAlocadas += somaHorasDiarias;

                    var finalDeSemana = MapaUtil.EhFinalDeSemana(dataIterador);

                    listaDisponibilidadeHoras.Add(new DisponibilidadeHoras
                    {
                        DiaDisponibilidade = dataIterador.ToString("dd/MMM"),
                        HorasDisponiveis = horasDiariasDisponiveis,
                        EhFinalDeSemana = finalDeSemana,
                        Data = dataIterador
                    });
                }

                DateTime data = new DateTime(ano, mes, 1);
                double horasDisponiveis = 0;
                int diasUteisMes = 0;

                foreach (var item in listaDisponibilidadeHoras)
                {
                    if (item.EhFinalDeSemana == false)
                    {
                        diasUteisMes++;
                    }
                }

                horasDisponiveis = HORAS_DIARIAS_COLABORADOR * diasUteisMes;
                string email;

                if (cpf is not null)
                {
                    var dadosColaborador = _buscaColaboradorRepository.GetColaborador(cpf, orgId);
                    email = _usuarioColaboradorRepository.GetUserByCPFEOrgId(dadosColaborador.Cpf, orgId).Email;
                    var colaboradorOrg = _buscaColaboradorRepository.GetColaboradorOrg(cpf, orgId);
                    ret.CodigoColaborador = !string.IsNullOrEmpty(colaboradorOrg.CodColaborador) ? colaboradorOrg.CodColaborador : null;
                    ColaboradorOrgHierarquiaDTO colaboradorOrgHierarquia = _buscaColaboradorRepository.BuscaColaboradorOrgHierarquia(colaboradorOrg.CodColaborador, orgId);

                    ret.CodProfissionalSuperior = colaboradorOrgHierarquia?.CodProfissionalSuperior;
                    ret.NomeProfissionalSuperior = colaboradorOrgHierarquia?.NomeProfissionalSuperior;

                    ret.NomeColaborador = dadosColaborador.NomeCompleto;
                    ret.FotoUrl = dadosColaborador.UrlFoto;
                    ret.Projetos = listaProjetos;
                    ret.HorasAlocadas = horasAlocadas;
                    ret.HorasDisponiveis = horasDisponiveis - horasAlocadas;
                    ret.DisponibilidadeDeHoras = listaDisponibilidadeHoras;
                    ret.Periodo = data.ToString("MMM/yyyy");
                }
                else if (tbdId is not null)
                {
                    var dadosTBD = await _tbdRepository.ObterTbdPorCodigo(orgId, tbdId.Value);

                    ret.NomeColaborador = dadosTBD.Descricao;
                    ret.CodigoTBD = dadosTBD.CodTbdAlocado;
                    ret.Projetos = listaProjetos;
                    ret.HorasAlocadas = horasAlocadas;
                    ret.HorasDisponiveis = horasDisponiveis - horasAlocadas;
                    ret.DisponibilidadeDeHoras = listaDisponibilidadeHoras;
                    ret.Periodo = data.ToString("MMM/yyyy");
                }
                else
                {
                    throw new Exception("Erro ao obter alocação. Cpf ou Codigo TBD não encontrado.");
                }

                if (ret.Projetos != null)
                {
                    var duplicados = ret.Projetos.GroupBy(x => x.NomeProjeto)
                            .SelectMany(g => g.Skip(1))
                            .Distinct()
                            .ToList();

                    foreach (var item in duplicados)
                    {
                        ret.Projetos.Remove(item);

                        ret.Projetos
                        .First(x => x.NomeProjeto == item.NomeProjeto
                        && x.PeriodoAlocadoId != item.PeriodoAlocadoId).DetalhesPeriodo.HorasPeriodo.AddRange(item.DetalhesPeriodo.HorasPeriodo);
                    }
                }

                return ret;
            }
            catch (Exception e)
            {
                if (e.Message != "cpf inválido")
                {
                    throw;
                }
                else
                {
                    throw new Exception("cpf inválido", e);
                }
            }
        }

        private async Task ProcessarCalculoMensalDeTodosColaboradoresETbdsAlocados(bool forcarRecalculoHorasPrevistas = false)
        {
            var todasAlocacoes = _mapaDeAlocacaoRepository.GetColaboradorPeriodoAlocacaoTodos();

            var agrupamentos = todasAlocacoes
                .GroupBy(alocacao => new
                {
                    Identificador = String.IsNullOrEmpty(alocacao.CodigoTbd) ? alocacao.CodigoTbd.ToString() : alocacao.CodigoColaborador,
                    alocacao.OrgId
                });

            foreach (var grupo in agrupamentos)
            {
                // Pega o primeiro elemento do grupo para os dados comuns
                var alocacao = grupo.First();
                await ProcessarCalculoMensal(grupo.Key.Identificador, alocacao.CodigoTbd.HasValue(), grupo.Key.OrgId, forcarRecalculoHorasPrevistas);
            }
        }

        public async Task<DetalharColaboradorEditarDTO> GetPeriodoAlocacaoColaboradorVisaoEdicao(string cpf, int? tbdId, string codigoProjeto, string mesParam, string anoParam, string cpfSolicitante, int orgId)
        {
            try
            {
                ValidaAcessoMapaAlocacao(cpfSolicitante, orgId, FuncionalidadeSistemaEnum.MAPA_DE_ALOCACAO);

                int mes = mesParam.ToIntOuZero();
                int ano = anoParam.ToIntOuZero();

                if (!cpf.HasValue() && tbdId.ToIntOuZero() == 0)
                {
                    throw new Exception("CPF ou TBD são necessários.");
                }

                if (cpf.HasValue() && tbdId.ToIntOuZero() > 0)
                {
                    throw new Exception("Apenas um dos campos entre CPF e TBD ID devem ser preenchidos.");
                }

                if ((mes > 0 && ano == 0) || (mes == 0 && ano > 0))
                {
                    var message = "Para informar uma data, mes e ano precisam ser preenchidos";
                    throw new ApplicationException(message);
                }

                if (mes == 0 && ano == 0)
                {
                    mes = DateTime.Now.Month;
                    ano = DateTime.Now.Year;
                }

                bool filtrarPorMesAtualEFuturas = true;

                var listaPeriodoAlocado = await _mapaDeAlocacaoRepository.GetPeriodoAlocacaoColaboradorComFiltro(cpf, tbdId, mes, ano, codigoProjeto, orgId, filtrarPorMesAtualEFuturas);

                var ret = new DetalharColaboradorEditarDTO();

                List<ProjetosDetalhadosEditarDTO> listaProjetos = new List<ProjetosDetalhadosEditarDTO>();

                var listPeriodos = new List<PeriodoAlocadoColaboradorDTO>();

                foreach (var periodo in listaPeriodoAlocado)
                {
                    var periodos = new PeriodoAlocadoColaboradorDTO()
                    {
                        IncluiFimDeSemana = periodo.IncluiFimDeSemana,
                        DataInicio = periodo.DataInicio,
                        DataFim = periodo.DataFim,
                        HorasPorDia = periodo.QuantidadeHoras,
                        PeriodoAlocadoId = periodo.PeriodoAlocadoId,
                        DataAlteracao = periodo.DataAlteracao,
                        Observacao = periodo.Observacao,
                        Oportunidade = periodo.Oportunidade,
                        Percentual = periodo.Percentual,
                        Prioritario = periodo.Prioritario,
                        ColaboradorAlocadoId = periodo.ColaboradorAlocadoId,
                    };
                    listPeriodos.Add(periodos);
                }

                foreach (var item in listaPeriodoAlocado)
                {
                    var projetoDetalhado = new ProjetosDetalhadosEditarDTO();

                    projetoDetalhado.PeriodoAlocadoId = item.PeriodoAlocadoId;
                    projetoDetalhado.ColaboradorAlocadoId = item.ColaboradorAlocadoId;
                    projetoDetalhado.CodigoTBD = item.CodigoTBD;
                    projetoDetalhado.NomeProjeto = item.NomeProjeto;
                    projetoDetalhado.CodigoCliente = item.CodigoCliente;
                    projetoDetalhado.NomeCliente = item.NomeCliente;
                    projetoDetalhado.LabelProjetoCliente = item.LabelProjetoCliente;
                    projetoDetalhado.CodigoProjeto = item.CodigoProjeto;
                    projetoDetalhado.NomeGerenteProjeto = item.NomeGestor;
                    projetoDetalhado.CodigoGerenteProjeto = item.CodigoGestor;
                    projetoDetalhado.IncluiFimDeSemana = item.IncluiFimDeSemana;

                    projetoDetalhado.PeriodoAlocacao.AddRange(listPeriodos.Where(x => x.ColaboradorAlocadoId == item.ColaboradorAlocadoId));

                    listaProjetos.Add(projetoDetalhado);
                }

                DateTime data = new DateTime(ano, mes, 1);

                string email;

                if (cpf is not null)
                {
                    var dadosColaborador = _buscaColaboradorRepository.GetColaborador(cpf, orgId);
                    email = _usuarioColaboradorRepository.GetUserByCPFEOrgId(dadosColaborador.Cpf, orgId).Email;
                    var colaboradorOrg = _buscaColaboradorRepository.GetColaboradorOrg(cpf, orgId);
                    ret.CodigoColaborador = !string.IsNullOrEmpty(colaboradorOrg.CodColaborador) ? colaboradorOrg.CodColaborador : null;
                    ret.CpfColaborador = colaboradorOrg.Cpf.ToStringOuNull();

                    ret.Nome = dadosColaborador.NomeCompleto;
                    ret.Projetos = listaProjetos;
                    ret.periodo = data.ToString("MMM/yyyy");
                }
                else if (tbdId is not null)
                {
                    var dadosTBD = await _tbdRepository.ObterTbdPorCodigo(orgId, tbdId.Value);

                    ret.Nome = dadosTBD.Descricao;
                    ret.CodigoTBD = dadosTBD.CodTbdAlocado;
                    ret.Projetos = listaProjetos;
                    ret.periodo = data.ToString("MMM/yyyy");
                }
                else
                {
                    throw new Exception("Erro ao obter alocação. Cpf ou Codigo TBD não encontrado.");
                }

                return ret;
            }
            catch (Exception e)
            {
                if (e.Message != "cpf inválido")
                {
                    throw;
                }
                else
                {
                    throw new Exception("cpf inválido", e);
                }
            }
        }

        public async Task<bool> RecalcularColaboradorNoPeriodoMensal(string codigoColaborador, bool ehTbd, string cpfSolicitante, int orgId, bool forcarRecalculoHorasPrevistas)
        {
            ValidaAcessoMapaAlocacao(cpfSolicitante, orgId, FuncionalidadeSistemaEnum.MAPA_DE_ALOCACAO_EDICAO);

            if (codigoColaborador == null)
            {
                await this.ProcessarCalculoMensalDeTodosColaboradoresETbdsAlocados();
            }
            else
            {
                await ProcessarCalculoMensal(codigoColaborador, ehTbd, orgId);
            }

            return true;
        }
        public async Task<List<RemoverAlocacoesEmLoteResult>> RemoverAlocacoesEmLote(List<string> idsPeriodoAlocacao, string cpfSolicitante, int orgId)
        {
            ValidaAcessoMapaAlocacao(cpfSolicitante, orgId, FuncionalidadeSistemaEnum.MAPA_DE_ALOCACAO_EDICAO);

            if (idsPeriodoAlocacao.Count == 0)
            {
                throw new Exception("Nenhuma alocação informada.");
            }

            var associacaoAutomaticaColaboradorAoProjeto = _buscaParametroConfiguracaoService.GetParametroConfiguracao<bool>(ParametroOrgCodigoEnum.ASSOCIACAO_AUTOMATICA_COLABORADOR_PROJETO, orgId, cpfSolicitante);

            long[] idsPeriodoAlocacaoArray = idsPeriodoAlocacao.Select(id => long.Parse(id)).ToArray();

            var listaCadastroMapaAlocacaoDTO = _mapaDeAlocacaoRepository.GetColaboradorPeriodoAlocacaoByIds(idsPeriodoAlocacaoArray);

            await _mapaDeAlocacaoRepository.RemoverAlocacaoPorIds(idsPeriodoAlocacaoArray);

            _ = Task.Run(async () =>
            {
                foreach (var alocacao in listaCadastroMapaAlocacaoDTO)
                {
                    var ehTbd = alocacao.CodigoTbd.HasValue();
                    await ProcessarCalculoMensal((ehTbd ? alocacao.CodigoTbd.ToString() : alocacao.CodigoColaborador), ehTbd, orgId);
                    if (associacaoAutomaticaColaboradorAoProjeto)
                    {
                        await RemoverAssociacaoColaboradorProjetoCasoNaoHajaMaisAlocacao(alocacao, ehTbd, cpfSolicitante, orgId);
                    }
                }
            });

            var retorno = idsPeriodoAlocacao.Select(x => new RemoverAlocacoesEmLoteResult() { idPeriodoAlocacao = x }).ToList();

            return retorno;
        }

        private async Task RemoverAssociacaoColaboradorProjetoCasoNaoHajaMaisAlocacao(CadastroMapaAlocacaoDTO alocacao, bool ehTbd, string cpfSolicitante, int orgId)
        {
            var qtdAlocacoes = await _mapaDeAlocacaoRepository.ObterQuantidadeAlocacoesColaboradorNumProjeto(alocacao.CpfColaborador, alocacao.CodigoProjeto, orgId);
            if (qtdAlocacoes == 0)
            {
                await _projetoOrgRepository.RemoverAssociacaoProjetoColaborador(alocacao.CodigoColaborador, orgId, alocacao.CodigoProjeto);
            }
        }

        public async Task<StatusResult> RemoverAlocacao(long idPeriodoAlocacao, string cpfSolicitante, int orgId)
        {
            await RemoverAlocacoesEmLote(new List<string> { idPeriodoAlocacao.ToString() }, cpfSolicitante, orgId);
            return new StatusResult() { Mensagem = $"Período Alocação com id: [{idPeriodoAlocacao}] excluído com sucesso." }; ;
        }

        public async Task<AlocacaoColabETbdDTO> EditarAlocacao(long periodoAlocacaoId, DateTime? dataInicio, DateTime? dataFim, bool? incluiFimDeSemana, double? quantidadeDeHoras, string cpfSolicitante, int orgId, sbyte? prioritario, string observacao, string oportunidade, double? percentual, bool? flagRetroalimentaCV)
        {
            try
            {
                ValidaAcessoMapaAlocacao(cpfSolicitante, orgId, FuncionalidadeSistemaEnum.MAPA_DE_ALOCACAO_EDICAO);

                await _validadorService.ValidaMapaAlocacaoEditar(periodoAlocacaoId, dataInicio, dataFim, quantidadeDeHoras, cpfSolicitante, orgId);

                var ret = await _mapaDeAlocacaoRepository.EditarAlocacao(periodoAlocacaoId, dataInicio, dataFim, incluiFimDeSemana, quantidadeDeHoras, prioritario, observacao, oportunidade, percentual, null, null, null, null, flagRetroalimentaCV);

                if (ret != null)
                {
                    await ProcessarCalculoMensal(ret.CodColaborador, ret.Tbd, orgId);
                }
                return ret;
            }
            catch
            {
                throw;
            }
        }

        public List<ColaboradorAlocadoDTO> GetColaboradoresAlocadosNoProjeto(string projetoId, string cpfSolicitante, int orgId)
        {
            try
            {
                ValidaAcessoMapaAlocacao(cpfSolicitante, orgId, FuncionalidadeSistemaEnum.MAPA_DE_ALOCACAO_EDICAO);
                var colaboradoresAlocados = _mapaDeAlocacaoRepository.ColaboradoresAlocadosNoProjeto(projetoId, orgId);
                return colaboradoresAlocados;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<List<ColaboradorCchDTO>> ListarColaboradoresOrgAsync(string busca, int cursor, int limite, int orgId, string cpfSolicitante)
        {
            try
            {
                var restricaoDiretorias = await _restricaoDeAcessoService.ListarMinhasRestricoesDeAcessoPorCodigoInternoETipo(cpfSolicitante, orgId, RestricaoDeAcessoTipoConstants.DIRETORIA);
                return _mapaDeAlocacaoRepository.ListarColaboradoresOrg(busca, cursor, limite, orgId, restricaoDiretorias);
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public List<ProjetosCchDTO> ListarProjetosOrg(string busca, int cursor, int limite, int orgId)
        {
            try
            {
                return _mapaDeAlocacaoRepository.ListarProjetosOrg(busca, cursor, limite, orgId);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        private void ValidaAcessoMapaAlocacao(string cpf, int orgId, FuncionalidadeSistemaEnum funcionalidadesAcesso)
        {
            var isValid = _funcionalidadeSistemaRepository.ValidaAcessoFuncionalidade(
                cpf, orgId, funcionalidadesAcesso);

            if (!isValid.Result)
            {
                throw new UnauthorizedAccessException("Acesso negado para Mapa de Alocação");
            }
        }
        
        private void ValidaAcessosListarColaboradoresETbds(string cpf, int orgId)
        {
            var isValidRubricas = _funcionalidadeSistemaRepository.ValidaAcessoFuncionalidade(
                cpf, orgId, FuncionalidadeSistemaEnum.RUBRICAS);
            
            var isValidMapaAlocacao = _funcionalidadeSistemaRepository.ValidaAcessoFuncionalidade(
                cpf, orgId, FuncionalidadeSistemaEnum.MAPA_DE_ALOCACAO);

            if (!isValidRubricas.Result && !isValidMapaAlocacao.Result)
            {
                throw new UnauthorizedAccessException("Acesso negado para Listar ColaboradoresETbds");
            }
        }

        public async Task<IEnumerable<AlocacaoColabETbdDTO>> ListarAlocacoesColaboradoresETbds(ListarAlocacoesColabETbdInput dto, string cpfSolicitante, int orgId, string tokenUsuario, string cpfUsuarioLogado)
        {
            if (cpfSolicitante != cpfUsuarioLogado && _buscaColaboradorRepository.ValidaAcesso(cpfUsuarioLogado, orgId, tokenUsuario) == false)
            {
                throw new UnauthorizedAccessException("Acesso negado");
            }
            ValidaAcessoMapaAlocacao(cpfSolicitante, orgId, FuncionalidadeSistemaEnum.MAPA_DE_ALOCACAO);
            string qtdGerenteProjetoPrioridade = "0";

            var incluiInativos = _buscaParametroConfiguracaoService.GetParametroConfiguracao<bool>(ParametroOrgCodigoEnum.DEVE_INCLUIR_INATIVOS_MAPA_ALOCACAO, orgId, cpfSolicitante); ;
            
            var restricoesDiretoria = await _restricaoDeAcessoService.ListarMinhasRestricoesDeAcessoPorCodigoInternoETipo(cpfSolicitante, orgId, RestricaoDeAcessoTipoConstants.DIRETORIA, dto.CodigoUnidade);
            
            var ret = await _mapaDeAlocacaoRepository.ListarAlocacoesColaboradoresETbds(
                dto.Pesquisa.ToNullSeTextoNullOuZero(),
                null,
                orgId,
                restricoesDiretoria,
                dto.CodigoDepartamento.ToNullSeTextoNullOuZero(),
                dto.CodigoGestorAdm.ToNullSeTextoNullOuZero(),
                dto.ListaCodigoColabOuTbd, 
                dto.FiltroTipoProfissional,
                dto.CodigoGestorProjeto.ToNullSeTextoNullOuZero(),
                dto.ListaCodigoClientes,
                dto.ApenasProjetosPrioritarios,
                dto.ListaCodigoProjetos,
                dto.CodigoStatusProjeto.ToNullSeTextoNullOuZero(),
                qtdGerenteProjetoPrioridade,
                incluiInativos,
                null,
                null
            );

            return ret;
        }

        public async Task<IEnumerable<ColaboradorETbdDTO>> ListarColaboradoresETbds(int orgId, string codigoDiretoria, string codigoGestor, string cpfSolicitante, string codigoDepartamento, TipoProfissionalEnum filtroTipoProfissional)
        {
            ValidaAcessosListarColaboradoresETbds(cpfSolicitante, orgId);
            var restricoesDiretoria = await _restricaoDeAcessoService.ListarMinhasRestricoesDeAcessoPorCodigoInternoETipo(cpfSolicitante, orgId, RestricaoDeAcessoTipoConstants.DIRETORIA, codigoDiretoria);
            return await _mapaDeAlocacaoRepository.ListarColaboradoresETbds(orgId, restricoesDiretoria, codigoGestor.ToNullSeTextoNullOuZero(), codigoDepartamento.ToNullSeTextoNullOuZero(), filtroTipoProfissional);
        }

        public async Task<IEnumerable<AlocacaoColabETbdDTO>> SubstituirDadosAlocacaoesPorPeriodo(SubstituirDadosAlocacaoesPorPeriodoParam param, string cpfSolicitante, int orgId)
        {
            try
            {
                ValidaAcessoMapaAlocacao(cpfSolicitante, orgId, FuncionalidadeSistemaEnum.MAPA_DE_ALOCACAO_EDICAO);
                var colaboradorPeriodosAlocacao = await ValidarSubstituirDadosAlocacoes(param, orgId);

                string codigoColaboradorNovo = null;
                string colaboradorCpfNovo = null;
                int? codTbdAlocadoNovo = null;
                string codigoProjetoNovo = null;

                if (!param.EhTbd && param.CodigoColaboradorNovo.HasValue())
                {
                    codigoColaboradorNovo = param.CodigoColaboradorNovo;
                    colaboradorCpfNovo = _buscaColaboradorRepository.BuscaColaboradorPorCodigoExterno(param.CodigoColaboradorNovo, orgId);
                }

                if (param.EhTbd && param.CodigoColaboradorNovo.HasValue())
                {
                    codTbdAlocadoNovo = param.CodigoColaboradorNovo.ToInt();
                }

                if (param.CodigoProjetoNovo.HasValue())
                {
                    codigoProjetoNovo = param.CodigoProjetoNovo;
                }

                var listaAlocacaoColabETbdDTO = new List<AlocacaoColabETbdDTO>();

                foreach (var colaboradorPeriodoAlocacao in colaboradorPeriodosAlocacao)
                {
                    var associacaoAutomaticaColaboradorAoProjeto = _buscaParametroConfiguracaoService.GetParametroConfiguracao<bool>(ParametroOrgCodigoEnum.ASSOCIACAO_AUTOMATICA_COLABORADOR_PROJETO, orgId, cpfSolicitante);

                    var alocacaoColabETbdDTO = await _mapaDeAlocacaoRepository.EditarAlocacao(
                        colaboradorPeriodoAlocacao.PeriodoAlocadoId,
                        null, null, null, null, null, null, null, null,
                        codigoColaboradorNovo, colaboradorCpfNovo, codTbdAlocadoNovo, codigoProjetoNovo, null);

                    if (associacaoAutomaticaColaboradorAoProjeto)
                    {
                        bool alocacaoNovaEhColaborador = !alocacaoColabETbdDTO.Tbd; //se não é tbd, é colaborador
                        bool alocacaoAntigaEraColaborador = !string.IsNullOrEmpty(colaboradorPeriodoAlocacao.CodigoColaborador);

                        if (alocacaoNovaEhColaborador)
                        {
                            await _mapaDeAlocacaoRepository.CadastrarAssociacaoAutomaticaNoProjeto(alocacaoColabETbdDTO.CodColaborador, (codigoProjetoNovo.HasValue() ? codigoProjetoNovo : alocacaoColabETbdDTO.Projeto.CodProjeto), orgId);
                        }

                        if (alocacaoAntigaEraColaborador)
                        {
                            await RemoverAssociacaoColaboradorProjetoCasoNaoHajaMaisAlocacao(colaboradorPeriodoAlocacao, false, cpfSolicitante, orgId);
                        }
                    }

                    listaAlocacaoColabETbdDTO.Add(alocacaoColabETbdDTO);
                }

                ProcessarCalculoMensalAposSubstituir(param.EhTbd, codigoColaboradorNovo, codTbdAlocadoNovo, colaboradorPeriodosAlocacao, orgId);
                return listaAlocacaoColabETbdDTO;
            }
            catch
            {
                throw;
            }
        }

        private void ProcessarCalculoMensalAposSubstituir(bool ehTbd, string codigoColaboradorNovo, int? codTbdAlocadoNovo, List<CadastroMapaAlocacaoDTO> listaCadastroMapaAlocacaoDTO, int orgId)
        {
            _ = Task.Run(async () =>
            {
                //se mudou apenas o projeto, não precisa recalcular, pois o profissional é o mesmo
                if (codigoColaboradorNovo != null || codTbdAlocadoNovo != null)
                {
                    // calcular o profissional teve alocação associada
                    await ProcessarCalculoMensal(ehTbd ? codTbdAlocadoNovo.ToString() : codigoColaboradorNovo, ehTbd, orgId);

                    // calcular os profissionais que tiveram a alocação desassociada
                    foreach (var alocacao in listaCadastroMapaAlocacaoDTO)
                    {
                        bool ehTbdAlocacao = alocacao.CodigoTbd.HasValue();
                        await ProcessarCalculoMensal(ehTbdAlocacao ? alocacao.CodigoTbd.ToString() : alocacao.CodigoColaborador, ehTbdAlocacao, orgId);
                    }
                }
            });
        }

        private async Task<List<CadastroMapaAlocacaoDTO>> ValidarSubstituirDadosAlocacoes(SubstituirDadosAlocacaoesPorPeriodoParam param, int orgId)
        {
            if (param.PeriodosIdSubstituidos is null || !param.PeriodosIdSubstituidos.Any())
            {
                throw new ArgumentException("Para realizar a substituição, ao menos um período deve ser informado.");
            }

            if (string.IsNullOrEmpty(param.CodigoColaboradorNovo.ToStringOuVazio()) && string.IsNullOrEmpty(param.CodigoProjetoNovo.ToStringOuVazio()))
            {
                throw new ArgumentException("Para realizar a substituição, deve ser informado 'CodigoColaboradorNovo' ou 'CodigoProjetoNovo'.");
            }

            var colaboradorPeriodosAlocacao = _mapaDeAlocacaoRepository.GetColaboradorPeriodoAlocacaoByIds(param.PeriodosIdSubstituidos);

            if (param.PeriodosIdSubstituidos.Length != colaboradorPeriodosAlocacao.Count)
            {
                throw new ApplicationException("Um ou mais períodos informados não foram encontrados.");
            }

            foreach (var colaboradorPeriodoAlocacao in colaboradorPeriodosAlocacao)
            {
                if (colaboradorPeriodoAlocacao.OrgId != orgId)
                {
                    throw new ApplicationException($"Alocação {colaboradorPeriodoAlocacao.PeriodoAlocadoId} não pertence a esta Org.");
                }

                if (param.CodigoColaboradorNovo.HasValue())
                {
                    if (param.EhTbd)
                    {
                        if (_tbdRepository.ObterTbdPorCodigo(orgId, param.CodigoColaboradorNovo.ToIntOuZero()) is null)
                        {
                            throw new ApplicationException($"CodigoTbd {param.CodigoColaboradorNovo} não encontrado para esta Org.");
                        }
                    }
                    else
                    {
                        if (_buscaColaboradorRepository.BuscaColaboradorPorCodigoExterno(param.CodigoColaboradorNovo, orgId) is null)
                        {
                            throw new ApplicationException($"CodigoColaborador {param.CodigoColaboradorNovo} não encontrado para esta Org.");
                        }

                        var codigoProjetoParaValidacao = param.CodigoProjetoNovo.HasValue() ? param.CodigoProjetoNovo : colaboradorPeriodoAlocacao.CodigoProjeto;
                        var codigoColaboradorParaValidacao = param.CodigoColaboradorNovo.HasValue() ? param.CodigoColaboradorNovo : colaboradorPeriodoAlocacao.CodigoColaborador;

                        var validaConflitoERetornaDadosConflito = await _validadorService.ValidarConflitoAlocacaoCadastrar(colaboradorPeriodoAlocacao.DataInicio, colaboradorPeriodoAlocacao.DataFim, codigoProjetoParaValidacao, codigoColaboradorParaValidacao, param.EhTbd, orgId);

                        if (!validaConflitoERetornaDadosConflito)
                        {
                            throw new ApplicationException("Já há uma alocação para o colaborador que conflita com o periodo selecionado");
                        }
                    }
                }

                if (param.CodigoProjetoNovo.HasValue())
                {
                    if (!_projetoOrgRepository.ProjetoExiste(param.CodigoProjetoNovo, orgId))
                    {
                        throw new ApplicationException($"CodigoProjeto {param.CodigoProjetoNovo} não encontrado para esta Org.");
                    }
                }
            }

            return colaboradorPeriodosAlocacao;
        }

        public List<PerfilAlocacaoDTO> ListarPerfilAlocacao(string codProjeto, string codInternoColaborador, int orgId, bool ocultarSkill = false)
        {
            ValidaAcessoMapaAlocacao(codInternoColaborador, orgId, FuncionalidadeSistemaEnum.MAPA_DE_ALOCACAO);
            return _perfilAlocacaoRepository.ListarPerfilAlocacao(codProjeto, orgId, ocultarSkill);
        }

        public async Task<AlocacaoColabETbdDTO> RemoveSkillAlocacao(string codInternoColaborador, long periodoAlocacaoId, ItemSkillPerfilAlocacaoDTO skill, int orgId)
        {
            ValidaAcessoMapaAlocacao(codInternoColaborador, orgId, FuncionalidadeSistemaEnum.MAPA_DE_ALOCACAO_EDICAO);
            if (skill.NivelId == 0)
                skill.NivelId = null;
            var skills = _perfilAlocacaoRepository.ListarSkillsAlocacao(periodoAlocacaoId);
            if (!skills.Any(x => x.Id == skill.IdSkill && x.TipoSkill == skill.TipoSkill))
                throw new Exception("Essa skill não existe na alocação");
            _perfilAlocacaoRepository.RemoverSkillAlocacao(periodoAlocacaoId, skill);
            var ret = (await _mapaDeAlocacaoRepository.ListarAlocacoesColaboradoresETbds(
                null, // pesquisa
                periodoAlocacaoId.ToIntOuZero(), // periodoAlocacaoId
                orgId,
                null, // codigoUnidade
                null, // codigoDepartamento
                null, // codigoGestorAdm
                null, // listaCodigoColabOuTbd
                TipoProfissionalEnum.Todos, // filtroTipoProfissional
                null, // codigoGestorProjeto
                null, // listaCodigoClientes
                false, // apenasProjetosPrioritarios
                null, // listaCodigoProjetos
                null, // codigoStatusProjeto
                null, // qtdGerenteProjetoPrioridade
                false, // incluiInativos
                null,
                null
            )).FirstOrDefault();
            return ret;
        }

        public async Task<AlocacaoColabETbdDTO> AdicionaSkillAlocacao(string codInternoColaborador, long periodoAlocacaoId, ItemSkillPerfilAlocacaoDTO skill, int orgId)
        {
            ValidaAcessoMapaAlocacao(codInternoColaborador, orgId, FuncionalidadeSistemaEnum.MAPA_DE_ALOCACAO_EDICAO);
            var verificaPerfil = _perfilAlocacaoRepository.BuscarPerfilAlocacao(periodoAlocacaoId);
            if (verificaPerfil == null)
            {
                throw new Exception("Não é possível adicionar habilidades a este alocado, pois ele não possui um perfil associado.");
            }
            if (skill.NivelId == 0)
                skill.NivelId = null;
            var skills = _perfilAlocacaoRepository.ListarSkillsAlocacao(periodoAlocacaoId);
            if (skills.Any(x => x.Id == skill.IdSkill && x.TipoSkill == skill.TipoSkill))
                throw new Exception("Essa skill já existe na alocação");
            _perfilAlocacaoRepository.InserirSkillsAlocacao(periodoAlocacaoId, new List<ItemSkillPerfilAlocacaoDTO> { skill }, codInternoColaborador);
            var ret = (await _mapaDeAlocacaoRepository.ListarAlocacoesColaboradoresETbds(
                null, // pesquisa
                periodoAlocacaoId.ToIntOuZero(), // periodoAlocacaoId
                orgId,
                null, // codigoUnidade
                null, // codigoDepartamento
                null, // codigoGestorAdm
                null, // listaCodigoColabOuTbd
                TipoProfissionalEnum.Todos, // filtroTipoProfissional
                null, // codigoGestorProjeto
                null, // listaCodigoClientes
                false, // apenasProjetosPrioritarios
                null, // listaCodigoProjetos
                null, // codigoStatusProjeto
                null, // qtdGerenteProjetoPrioridade
                false, // incluiInativos
                null,
                null
            )).FirstOrDefault();
            return ret;
        }

        public async Task<AlocacaoColabETbdDTO> AlteraPerfilAlocacao(string codInternoColaborador, long periodoAlocacaoId, string perfilId, int orgId)
        {
            ValidaAcessoMapaAlocacao(codInternoColaborador, orgId, FuncionalidadeSistemaEnum.MAPA_DE_ALOCACAO_EDICAO);
            _dbUnitOfWork.BeginTransaction();
            try
            {
                var skills = _perfilAlocacaoRepository.ListarSkillsAlocacao(periodoAlocacaoId);
                if (skills.Any())
                    skills.ForEach(x => _perfilAlocacaoRepository.RemoverSkillAlocacao(periodoAlocacaoId, new ItemSkillPerfilAlocacaoDTO { IdSkill = x.Id, TipoSkill = x.TipoSkill }));
                var skillsPerfil = _perfilAlocacaoRepository.ListarSkillsPerfilAlocacao(perfilId);
                _perfilAlocacaoRepository.InserirSkillsAlocacao(periodoAlocacaoId, skillsPerfil.Select(x => new ItemSkillPerfilAlocacaoDTO { IdSkill = x.Id, NivelId = (int?)x.Nivel?.Id ?? null, TipoSkill = x.TipoSkill }).ToList(), codInternoColaborador);
                var tipo = perfilId.Split("|")[0];
                var id = perfilId.Split("|")[1];
                var perfilAlocacao = _perfilAlocacaoRepository.BuscarPerfilAlocacao(periodoAlocacaoId);
                if (perfilAlocacao != null)
                    _perfilAlocacaoRepository.AlteraVinculoPerfilAlocacao(periodoAlocacaoId, tipo == "2" ? id : null, tipo == "1" ? id : null);
                else
                    _perfilAlocacaoRepository.VincularPerfilAlocacao(periodoAlocacaoId, tipo == "2" ? id : null, tipo == "1" ? id : null, orgId);
                _dbUnitOfWork.Commit();
                var ret = (await _mapaDeAlocacaoRepository.ListarAlocacoesColaboradoresETbds(
                    null, // pesquisa
                    periodoAlocacaoId.ToIntOuZero(), // periodoAlocacaoId
                    orgId,
                    null, // codigoUnidade
                    null, // codigoDepartamento
                    null, // codigoGestorAdm
                    null, // listaCodigoColabOuTbd
                    TipoProfissionalEnum.Todos, // filtroTipoProfissional
                    null, // codigoGestorProjeto
                    null, // listaCodigoClientes
                    false, // apenasProjetosPrioritarios
                    null, // listaCodigoProjetos
                    null, // codigoStatusProjeto
                    null, // qtdGerenteProjetoPrioridade
                    false, // incluiInativos
                    null,
                    null
                )).FirstOrDefault();
                return ret;
            }
            catch (Exception e)
            {
                _dbUnitOfWork.Rollback();
                throw new Exception("Falha ao alterar o perfil da alocacao: " + e.Message);
            }
        }

        public async Task<List<SkillNivelDTO>> BuscarHabilidadesNaoDefinidasDoColaborador(string cpfColaborador, string token, int orgId)
        {
            var hardSkillsTask = _competenciaClient.ListarCompetenciasColaborador(cpfColaborador, token);
            var metodologiasTask = _metodologiaClient.ListarMetodologiasColaborador(cpfColaborador, token);
            var softSkillsTask = _softSkillClient.ListarSoftskillsColaborador(cpfColaborador, token);
            var dominiosTask = _dominioClient.ListarDominiosColaborador(cpfColaborador, token);
            var idiomasTask = _idiomaClient.ListarIdiomaColaborador(cpfColaborador, token);

            Task.WaitAll(hardSkillsTask, metodologiasTask, softSkillsTask, dominiosTask, idiomasTask);

            var habilidadesTecnicas = new List<SkillNivelDTO>();

            var hardSkillRes = hardSkillsTask.Result.Select(skill => new SkillNivelDTO
            {
                Id = skill.Competencia.Id,
                TipoSkill = TipoCompetenciaSRSEnum.HardSkill.ToString(),
                Descricao = skill.Competencia.Descricao,
                Nivel = skill.Nivel
            });

            var metodologiaRes = metodologiasTask.Result.Select(skill => new SkillNivelDTO
            {
                Id = skill.Metodologia.Id,
                TipoSkill = TipoCompetenciaSRSEnum.Metodologia.ToString(),
                Descricao = skill.Metodologia.Descricao,
                Nivel = skill.Nivel
            });

            var softSkillRes = softSkillsTask.Result.Select(skill => new SkillNivelDTO
            {
                Id = skill.SoftSkill.Id,
                TipoSkill = TipoCompetenciaSRSEnum.SoftSkill.ToString(),
                Descricao = skill.SoftSkill.Descricao,
                Nivel = skill.Nivel
            });

            var dominioRes = dominiosTask.Result.Select(skill => new SkillNivelDTO
            {
                Id = skill.Dominio.Id,
                TipoSkill = TipoCompetenciaSRSEnum.Dominio.ToString(),
                Descricao = skill.Dominio.Descricao,
                Nivel = skill.Nivel
            });

            var idiomaRes = idiomasTask.Result.Select(skill => new SkillNivelDTO
            {
                Id = skill.Idioma.Id,
                TipoSkill = TipoCompetenciaSRSEnum.Idioma.ToString(),
                Descricao = skill.Idioma.Descricao,
                Nivel = skill.Nivel
            });

            habilidadesTecnicas.AddRange(idiomaRes);
            habilidadesTecnicas.AddRange(hardSkillRes);
            habilidadesTecnicas.AddRange(softSkillRes);
            habilidadesTecnicas.AddRange(metodologiaRes);
            habilidadesTecnicas.AddRange(dominioRes);

            habilidadesTecnicas = habilidadesTecnicas.FindAll(x => x.Nivel?.Descricao.ToLower() == "a definir").ToList();

            if (habilidadesTecnicas.Count > 0)
            {
                var notificacao = new NotificacaoDTO
                {
                    Id = null,
                    ColaboradorCpf = cpfColaborador,
                    Titulo = "Habilidades perfil 360",
                    Mensagem = "Por favor, classifique suas habilidades de acordo com o nível de senioridade que você acredita possuir.",
                    OrgId = orgId,
                };
                await _notificacaoService.InserirNotificacaoColaborador(notificacao);
            }

            return habilidadesTecnicas;
        }
    }
}