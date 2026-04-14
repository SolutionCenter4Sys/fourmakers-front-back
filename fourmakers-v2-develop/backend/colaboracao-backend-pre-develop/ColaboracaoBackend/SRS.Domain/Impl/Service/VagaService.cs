using ApiClient.Domain;
using ApiClient.Domain.Interfaces;
using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Colaboracao.Helper.Enum;
using Colaboracao.Helper.Util;
using Colaboracao.Helper.Util.Competencia;
using ColaboracaoBridge.Domain.Interfaces.Services;
using Competencia.Domain.Enums;
using Core.Domain;
using Core.Domain.Colaborador;
using Core.Domain.MapaAlocacao.GestaoDeAlocados;
using Core.Domain.Social;
using Core.Domain.TemplateEmail;
using Core.Domain.Usuario.Permissao;
using Core.Domain.Vaga;
using Core.DomainModel;
using Core.DomainModel.Projeto;
using DataTransferObject.Domain;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.Curriculo;
using DataTransferObject.Domain.Log;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfil;
using DataTransferObject.Domain.Match;
using DataTransferObject.Domain.SRS;
using DataTransferObject.Domain.SRS.Candidate;
using DataTransferObject.Domain.SRS.Vagas;
using DataTransferObject.Domain.Usuario;
using DataTransferObject.Domain.Util.Enum;
using DataTransferObject.Domain.Vaga;
using DataTransferObject.Domain.Vaga.Enums;
using DataTransferObject.Domain.VagasSRS;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SRS.Domain.Impl.Service.Strategy;
using SRS.Domain.Impl.Util;
using SRS.Domain.Interfaces.Service;
using SRS.Domain.Interfaces.Service.Validadores;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TemplateOrg.Constantes;

using Labs.Domain.Interfaces;
using Logs.Infra.Attributes;

namespace SRS.Domain.Impl.Service
{
    [LogDomainClass]
    public class VagaService : IVagaService
    {
        private readonly ISRSRepository _srsRepository;
        private readonly ISRSColaboracaoClient _sRSColaboracaoClient;
        private readonly ISRSVagaClient _sRSVagaClient;
        private readonly IClienteOrgRepository _clienteOrgRepository;
        private readonly IPerfilAlocacaoRepository _perfilAlocacaoRepository;
        private readonly IAspNetUser _aspNetUser;
        private readonly IColaboracaoBridgeClient _colaboracaoBridgeClient;
        private readonly IVagaFourmakersRepository _vagaFourmakersRepository;
        private readonly IVagaValidatorService _vagaValidatorService;
        private readonly IDBConnection _dapperConnection;
        private readonly IFuncionalidadeSistemaRepository _funcionalidadeSistemaRepository;
        private readonly string _tokenSistema;
        private readonly UsuarioLogadoDTO _usuarioLogado;
        private readonly ITemplateRepository _templateRepository;
        private readonly IBuscaColaboradorRepository _buscaColaboradorRepository;
        private readonly IVagaFourmakersRepository _repository;
        private readonly ILogCore _log;
        private readonly IGestaoAlocadosRepository _gestaoAlocadosRepository;
        private readonly IGestorExternoRepository _gestorExternoRepository;
        private readonly Core.Domain.Candidato.ICandidatoRepository _candidatoRepository;
        private readonly ICandidaturaRepository _candidaturaRepository;
        private readonly IMatchClient _matchClient;
        private readonly IMatchService _matchService;
        private readonly IComentarioCandidaturaRepository _comentarioCandidaturaRepository;
        private readonly IComentarioVagaRepository _comentarioVagaRepository;
        private readonly ICRMBridgeService _colaboracaoBridgeService;
        private readonly IClassificacaoService _classificacaoService;
        private readonly IClassificacaoRepository _classificacaoRepository;
        private readonly string _urlRoboLinkedin;
        private readonly MudancaStatusCandidaturaStrategyFactory _strategyFactory;
        private readonly ICompetenciaDtoRepository _competenciaRepository;

        const int QUANTIDADE_CANDIDATOS_MINIMO_PARA_ENVIAR_VAGA_PARA_ENTREVISTA_COM_CLIENTE = 3;

        public VagaService(
            IAspNetUser aspNetUser,
            ISRSRepository sRSRepository,
            ISRSColaboracaoClient sRSColaboracaoClient,
            ISRSVagaClient sRSVagaClient,
            IClienteOrgRepository clienteOrgRepository,
            IPerfilAlocacaoRepository perfilAlocacaoRepository,
            IColaboracaoBridgeClient colaboracaoBridgeClient,
            IVagaFourmakersRepository vagaFourmakersRepository,
            IVagaValidatorService vagaValidatorService,
            IFuncionalidadeSistemaRepository funcionalidadeSistemaRepository,
            IDBConnection dapperConnection,
            ITemplateRepository templateRepository,
            IBuscaColaboradorRepository buscaColaboradorRepository,
            IVagaFourmakersRepository repository,
            ILogCore log,
            IGestaoAlocadosRepository gestaoAlocadosRepository,
            IGestorExternoRepository gestorExternoRepository,
            Core.Domain.Candidato.ICandidatoRepository candidatoRepository,
            ICandidaturaRepository candidaturaRepository,
            IMatchClient matchClient,
            IMatchService matchService,
            IComentarioCandidaturaRepository comentarioCandidaturaRepository,
            ICRMBridgeService colaboracaoBridgeService,
            IComentarioVagaRepository comentarioVagaRepository,
            IClassificacaoService classificacaoService,
            IClassificacaoRepository classificacaoRepository, ICompetenciaDtoRepository competenciaRepository)
        {
            _srsRepository = sRSRepository;
            _sRSColaboracaoClient = sRSColaboracaoClient;
            _sRSVagaClient = sRSVagaClient;
            _clienteOrgRepository = clienteOrgRepository;
            _perfilAlocacaoRepository = perfilAlocacaoRepository;
            _colaboracaoBridgeClient = colaboracaoBridgeClient;
            _vagaFourmakersRepository = vagaFourmakersRepository;
            _vagaValidatorService = vagaValidatorService;
            _funcionalidadeSistemaRepository = funcionalidadeSistemaRepository;
            _aspNetUser = aspNetUser;
            _tokenSistema = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.TOKEN_SISTEMA_COLABORACAO);
            _dapperConnection = dapperConnection;
            _templateRepository = templateRepository;
            _usuarioLogado = _aspNetUser.GetUsuarioLogado();
            _buscaColaboradorRepository = buscaColaboradorRepository;
            _repository = repository;
            _log = log;
            _gestaoAlocadosRepository = gestaoAlocadosRepository;
            _gestorExternoRepository = gestorExternoRepository;
            _candidatoRepository = candidatoRepository;
            _candidaturaRepository = candidaturaRepository;
            _matchClient = matchClient;
            _matchService = matchService;
            _comentarioCandidaturaRepository = comentarioCandidaturaRepository;
            _colaboracaoBridgeService = colaboracaoBridgeService;
            _classificacaoService = classificacaoService;
            _classificacaoRepository = classificacaoRepository;
            _competenciaRepository = competenciaRepository;
            _urlRoboLinkedin = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.URL_ROBO_CRIACAO_VAGA_LINKEDIN);

            // Inicializar factory de estratégias
            _strategyFactory = new MudancaStatusCandidaturaStrategyFactory(
                buscaColaboradorRepository,
                vagaFourmakersRepository,
                candidaturaRepository,
                log,
                EnviarEmailParaGestoresExternos);
            _comentarioVagaRepository = comentarioVagaRepository;
        }

        public async Task<ApiGenericResult<CriarVagasSRSParam>> EditarCadastrarVaga(VagaFourmakersDTO vagaDTO, CRUDEnum cRUDEnum)
        {
            ValidaAcessoAoCadastroVaga(_aspNetUser.GetUsuarioLogado().Cpf, _aspNetUser.GetUsuarioLogado().OrgId);

            await _vagaValidatorService.ValidaVaga(vagaDTO.IdVaga.ToIntOuZero(), vagaDTO, _aspNetUser.GetUsuarioLogado().OrgId, cRUDEnum);

            CriarVagasSRSParam param = new CriarVagasSRSParam();

            if (param.Estado == "Estado")
            {
                throw new ArgumentException("Estado não pode ser nulo ou vazio.");
            }

            if (param.Cidade == "Cidade")
            {
                throw new ArgumentException("Cidade não pode ser nulo ou vazio.");
            }

            if (!String.IsNullOrEmpty(vagaDTO.PerfilId))
            {
                var codCliente = _perfilAlocacaoRepository.ObterCodClientePorPerfilId(vagaDTO.PerfilId, _aspNetUser.GetUsuarioLogado().OrgId);
                var cliente = _clienteOrgRepository.ObterClientePorCodigo(codCliente, _aspNetUser.GetUsuarioLogado().OrgId);

                if (cliente == null)
                {
                    throw new ArgumentException("Cliente não encontrado.");
                }

                if (!cliente.CodigoCliente.ToUpper().StartsWith("ACC"))
                {
                    throw new ArgumentException("Apenas clientes do CRM são permitidos para abertura da vaga.");
                }

                param.Company = new CompanySRSParam
                {
                    CodigoCRM = cliente.CodigoCliente,
                    Nome = cliente.NomeCliente
                };

            }

            param.IdVaga = vagaDTO.IdVaga;
            param.Unidade = vagaDTO.Unidade;
            param.Titulo = vagaDTO.Titulo;
            param.Tipo = vagaDTO.Tipo;
            param.NumeroDeVagas = vagaDTO.NumeroDeVagas;
            param.TaxaMaximaPorHora = vagaDTO.TaxaMaximaPorHora;
            param.Cargo = vagaDTO.Cargo;
            param.DescricaoCargo = vagaDTO.DescricaoCargo;
            param.DataCriacao = vagaDTO.DataCriacao;
            param.DataAtualizacao = vagaDTO.DataAtualizacao;
            param.TipoLocalizacao = vagaDTO.TipoLocalizacao;
            param.Estado = vagaDTO.Estado;
            param.Cidade = vagaDTO.Cidade;
            param.CrmInfo = vagaDTO.CrmInfo;
            param.CrmInfo.PropostaOportunidadeCCRM = vagaDTO.OportunidadePropostaCRM;
            param.Solicitante = vagaDTO.Solicitante;
            param.Aprovador = vagaDTO.Aprovador;
            param.Termometro = vagaDTO.Termometro;
            param.StackPrincipal = vagaDTO.StackPrincipal;
            param.ConfiguracaoMaquina = vagaDTO.ConfiguracaoMaquina;
            param.DuracaoContrato = vagaDTO.DuracaoContrato;
            param.DuracaoContratoDeterminado = vagaDTO.DuracaoContratoDeterminado;
            param.TipoContratacao = vagaDTO.TipoContratacao;
            param.CargaHoraria = vagaDTO.CargaHoraria;
            param.Frequencia = vagaDTO.Frequencia;
            param.DescricaoTecnica = vagaDTO.DescricaoTecnica;
            param.DataPrevistaInicio = vagaDTO.DataPrevistaInicio;
            param.GestorFoursys = vagaDTO.GestorFoursys;
            param.Acrescimo = vagaDTO.Acrescimo;
            param.MaquinaCliente = vagaDTO.MaquinaCliente;
            param.MaquinaFour = vagaDTO.MaquinaFour;
            param.Notes = vagaDTO.Notes;

            var skillCategories = new List<(IEnumerable<SkillNivelDTO> Skills, CategoriaSkillEnum Category)>
                                            {
                                                (vagaDTO.Hardskills, CategoriaSkillEnum.Hardskills),
                                                (vagaDTO.Softskills, CategoriaSkillEnum.Softskills),
                                                (vagaDTO.Metodologias, CategoriaSkillEnum.Metodologia),
                                                (vagaDTO.Dominios, CategoriaSkillEnum.ConhecimentoNegocio),
                                                (vagaDTO.Idiomas, CategoriaSkillEnum.Idiomas)
                                            };

            param.Skills = SRSUtil.ConverterParaListaSkillsVagasSrs(skillCategories);

            var token = _aspNetUser.GetUsuarioLogado().Token;
            var vagaCriadaSRS = await _sRSColaboracaoClient.EditarCriarVaga(token, param);

            var _connection = _dapperConnection.GetConnection();

            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    var vagaFourmakersSRSDTO = ConverterCriarVagasSRSParamParaVagaFourmakersSRSDTO(vagaCriadaSRS);
                    var listaVagaFourDTO = new List<VagaFourmakersSRSDTO>() { vagaFourmakersSRSDTO };

                    _vagaFourmakersRepository.SalvarVagasSRS(listaVagaFourDTO);

                    await _vagaFourmakersRepository.InserirVagaGestorExternoPerfil(_aspNetUser.GetUsuarioLogado().OrgId, _aspNetUser.GetUsuarioLogado().Cpf, (int)vagaCriadaSRS.IdVaga, vagaDTO.PerfilId);
                    await _vagaFourmakersRepository.InserirLogVaga(_aspNetUser.GetUsuarioLogado().OrgId, _aspNetUser.GetUsuarioLogado().Cpf, (int)vagaCriadaSRS.IdVaga, vagaDTO.PerfilId, AcaoLogVagaEnum.Criacao);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception("Vaga criada no SRS, porém não foi registrada no Fourmakers. Detalhe do erro: " + ex.Message);
                }
            }

            return new ApiGenericResult<CriarVagasSRSParam> { Sucesso = true, Mensagem = "Vaga cadastrada com sucesso.", Retorno = vagaCriadaSRS };
        }

        private VagaFourmakersSRSDTO ConverterCriarVagasSRSParamParaVagaFourmakersSRSDTO(CriarVagasSRSParam vagaParam)
        {
            var vaga = new VagaFourmakersSRSDTO();

            vaga.Id_vaga = vagaParam.IdVaga.ToInt();
            vaga.Titulo = vagaParam.Titulo;
            vaga.Vagas_abertas = vagaParam.NumeroDeVagas;
            vaga.Data_abertura = vagaParam.DataCriacao;
            vaga.Status_vaga = "EM APROVAÇÃO";
            vaga.Descricao = vagaParam.DescricaoTecnica;
            vaga.Cargo = vagaParam.DescricaoCargo;
            vaga.Data_criacao = vagaParam.DataCriacao;
            vaga.Data_alteracao = vagaParam.DataAtualizacao;
            vaga.LocTrabalho = vagaParam.TipoLocalizacao;
            vaga.Estado = vagaParam.Estado;
            vaga.Confidencial = 0;
            vaga.TipoVaga = vagaParam.Tipo.ToIntOuZero();
            vaga.Termometro = vagaParam.Termometro;
            vaga.Ativo = 1;
            vaga.GestorFoursys = vagaParam.GestorFoursys;
            vaga.NomeAprovador = vagaParam.Aprovador;
            vaga.MaquinaCliente = vagaParam.MaquinaCliente;
            vaga.MaquinaFour = vagaParam.MaquinaFour;
            vaga.Notes = vagaParam.Notes;

            vaga.Skills = new List<SkillsVagasDTO>();

            foreach (var skill in vagaParam.Skills)
            {
                var skillDTO = new SkillsVagasDTO();
                skillDTO.SkillId = skill.Id;
                skillDTO.TypeSkills = skill.CategoriaId.ToIntOuZero();
                skillDTO.SkillNivelId = skill.NivelId.ToIntOuZero();

                vaga.Skills.Add(skillDTO);
            }

            return vaga;
        }

        public async Task<ApiGenericResult<List<JobOrderSemanticaListagemDTO>>> ListarVagasSemantica(int cursor, int limite)
        {
            var result = await ListarVagasParaSemanticaComSkill(cursor, limite);
            if (!result.Sucesso || result.Retorno == null)
            {
                return new ApiGenericResult<List<JobOrderSemanticaListagemDTO>>
                {
                    Sucesso = result.Sucesso,
                    Mensagem = result.Mensagem,
                    Erros = result.Erros,
                    Retorno = null
                };
            }

            await EnriquecerSkillsSemanticaAsync(result.Retorno);

            return new ApiGenericResult<List<JobOrderSemanticaListagemDTO>>
            {
                Sucesso = true,
                Retorno = result.Retorno.Select(MapearJobOrderSemanticaParaListagem).ToList()
            };
        }

        private static JobOrderSemanticaListagemDTO MapearJobOrderSemanticaParaListagem(JobOrderSemanticaDTO x) =>
            new()
            {
                JoborderId = x.JoborderId,
                Title = x.Title,
                Description = x.Description,
                CargoId = x.CargoId,
                Cargo = x.Cargo,
                StackPrincipal = x.StackPrincipal,
                Skills = x.Skills
            };

        private static bool TryMapCategoryIdParaTipoCompetenciaSRS(int categoryId, out TipoCompetenciaSRSEnum tipo)
        {
            tipo = default;
            if (!Enum.IsDefined(typeof(ItemPerfilEnum), categoryId))
                return false;
            try
            {
                tipo = CompetenciaUtils.ConverterPerfilItemParaTipoCompetenciaSRS((ItemPerfilEnum)categoryId);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task EnriquecerSkillsSemanticaAsync(List<JobOrderSemanticaDTO> vagas)
        {
            var flatSkills = vagas
                .Where(v => v.JobOrderSkills != null && v.JobOrderSkills.Count > 0)
                .SelectMany(v => v.JobOrderSkills)
                .ToList();

            if (flatSkills.Count == 0)
                return;

            var idsPorTipo = new Dictionary<TipoCompetenciaSRSEnum, HashSet<int>>();
            foreach (var s in flatSkills)
            {
                if (!TryMapCategoryIdParaTipoCompetenciaSRS(s.CategoryId, out var tipo) || tipo == TipoCompetenciaSRSEnum.Desconhecida)
                    continue;
                if (!idsPorTipo.TryGetValue(tipo, out var set))
                {
                    set = new HashSet<int>();
                    idsPorTipo[tipo] = set;
                }
                set.Add(s.DescriptionId);
            }

            var lookup = new Dictionary<(int CategoryId, int DescriptionId), string>();

            foreach (var kv in idsPorTipo)
            {
                var tipo = kv.Key;
                var idList = kv.Value.ToList();
                var nomes = await _competenciaRepository.ObterNomeSkillsPorTipoEIds(tipo, idList);
                var byId = nomes
                    .Where(x => !string.IsNullOrEmpty(x.Descricao))
                    .ToDictionary(x => x.Id, x => x.Descricao);

                foreach (var s in flatSkills)
                {
                    if (!TryMapCategoryIdParaTipoCompetenciaSRS(s.CategoryId, out var t) || t != tipo)
                        continue;
                    if (byId.TryGetValue(s.DescriptionId, out var desc))
                        lookup[(s.CategoryId, s.DescriptionId)] = desc;
                }
            }

            foreach (var vaga in vagas)
            {
                if (vaga.JobOrderSkills == null || vaga.JobOrderSkills.Count == 0)
                    continue;

                var partes = vaga.JobOrderSkills
                    .OrderBy(s => s.Id)
                    .Select(s => lookup.TryGetValue((s.CategoryId, s.DescriptionId), out var d) ? d : null)
                    .Where(d => !string.IsNullOrEmpty(d))
                    .ToList();

                if (partes.Count > 0)
                    vaga.Skills = string.Join(";", partes);
            }
        }

        public async Task<ApiGenericResult<VagaFourmakersDTO>> ObterVagaPorIdEGestorExternoPerfilId(int vagaId, Guid? gestorExternoPerfilId)
        {
            ValidaAcessoAoCadastroVaga(_aspNetUser.GetUsuarioLogado().Cpf, _aspNetUser.GetUsuarioLogado().OrgId);

            VagaFourmakersDTO vagaDTO = new VagaFourmakersDTO();

            await _vagaValidatorService.ValidaVaga(vagaId, new VagaFourmakersDTO() { PerfilId = gestorExternoPerfilId.ToStringOuVazio() }, _aspNetUser.GetUsuarioLogado().OrgId, CRUDEnum.Read);

            var token = _aspNetUser.GetUsuarioLogado().Token;
            var vagaCriadaSRS = await _sRSColaboracaoClient.ObterVagaPorId(token, vagaId);

            // Mapear os campos do CriarVagasSRSParam de volta para o VagaFourmakersDTO
            vagaDTO.IdVaga = vagaCriadaSRS.IdVaga;
            vagaDTO.Unidade = vagaCriadaSRS.Unidade;
            vagaDTO.Titulo = vagaCriadaSRS.Titulo;
            vagaDTO.Tipo = vagaCriadaSRS.Tipo;
            vagaDTO.NumeroDeVagas = vagaCriadaSRS.NumeroDeVagas;
            vagaDTO.TaxaMaximaPorHora = vagaCriadaSRS.TaxaMaximaPorHora;
            vagaDTO.Cargo = vagaCriadaSRS.Cargo;
            vagaDTO.DescricaoCargo = vagaCriadaSRS.DescricaoCargo;
            vagaDTO.DataCriacao = vagaCriadaSRS.DataCriacao;
            vagaDTO.DataAtualizacao = vagaCriadaSRS.DataAtualizacao;
            vagaDTO.TipoLocalizacao = vagaCriadaSRS.TipoLocalizacao;
            vagaDTO.Estado = vagaCriadaSRS.Estado;
            vagaDTO.Cidade = vagaCriadaSRS.Cidade;
            vagaDTO.CrmInfo = vagaCriadaSRS.CrmInfo ?? null;
            vagaDTO.OportunidadePropostaCRM = vagaCriadaSRS.CrmInfo?.PropostaOportunidadeCCRM ?? null;
            vagaDTO.Solicitante = vagaCriadaSRS.Solicitante;
            vagaDTO.Aprovador = vagaCriadaSRS.Aprovador;
            vagaDTO.Termometro = vagaCriadaSRS.Termometro;
            vagaDTO.StackPrincipal = vagaCriadaSRS.StackPrincipal;
            vagaDTO.ConfiguracaoMaquina = vagaCriadaSRS.ConfiguracaoMaquina;
            vagaDTO.DuracaoContrato = vagaCriadaSRS.DuracaoContrato;
            vagaDTO.DuracaoContratoDeterminado = vagaCriadaSRS.DuracaoContratoDeterminado;
            vagaDTO.TipoContratacao = vagaCriadaSRS.TipoContratacao;
            vagaDTO.CargaHoraria = vagaCriadaSRS.CargaHoraria;
            vagaDTO.Frequencia = vagaCriadaSRS.Frequencia;
            vagaDTO.DescricaoTecnica = vagaCriadaSRS.DescricaoTecnica;
            vagaDTO.DataPrevistaInicio = vagaCriadaSRS.DataPrevistaInicio;
            vagaDTO.GestorFoursys = vagaCriadaSRS.GestorFoursys;
            vagaDTO.Acrescimo = vagaCriadaSRS.Acrescimo;
            vagaDTO.Notes = vagaCriadaSRS.Notes;
            vagaDTO.MaquinaCliente = vagaCriadaSRS.MaquinaCliente;
            vagaDTO.MaquinaFour = vagaCriadaSRS.MaquinaFour;

            var listaSkills = await _vagaFourmakersRepository.GetSkillsPorIds(vagaCriadaSRS.Skills.Select(s => s.DescricaoId.ToIntOuZero()).ToList());

            var listaTodosNiveis = await _vagaFourmakersRepository.GetTodosNiveis();

            var listaSkillsRetorno = new List<SkillNivelDTO>();

            foreach (var vagaCriadaSkill in vagaCriadaSRS.Skills)
            {
                var skillToAdd = listaSkills.FirstOrDefault(sdb => sdb.Id == vagaCriadaSkill.DescricaoId
                                                                && CompetenciaUtils.GetDescricaoCompetenciaById(vagaCriadaSkill.CategoriaIdFourmakers.ToIntOuZero()) == sdb.TipoSkill);
                if (skillToAdd.IsNotNull())
                {
                    var nivelToUpdate = listaTodosNiveis.FirstOrDefault(n => n.Id == vagaCriadaSkill.NivelId);
                    skillToAdd.Nivel = nivelToUpdate;
                    listaSkillsRetorno.Add(skillToAdd);
                }
            }

            // Mapear as Skills
            vagaDTO.Hardskills = listaSkillsRetorno.Where(s => s.TipoSkill == ItemPerfilEnum.COMPETENCIA.ToString()).ToList();
            vagaDTO.Softskills = listaSkillsRetorno.Where(s => s.TipoSkill == ItemPerfilEnum.SOFTSKILL.ToString()).ToList();
            vagaDTO.Metodologias = listaSkillsRetorno.Where(s => s.TipoSkill == ItemPerfilEnum.METODOLOGIA.ToString()).ToList();
            vagaDTO.Dominios = listaSkillsRetorno.Where(s => s.TipoSkill == ItemPerfilEnum.DOMINIONEGOCIO.ToString()).ToList();
            vagaDTO.Idiomas = listaSkillsRetorno.Where(s => s.TipoSkill == ItemPerfilEnum.IDIOMA.ToString()).ToList();

            var vaga = await _vagaFourmakersRepository.ObterVagaPorId(vagaDTO.IdVaga.ToIntOuZero());
            vagaDTO.GestorFoursys = vaga.GestorFoursys;

            return new ApiGenericResult<VagaFourmakersDTO> { Sucesso = true, Retorno = vagaDTO };
        }

        public async Task<List<VagaDTO>> BuscarVagaSRS(string token)
        {
            try
            {
                var vagas = await _sRSColaboracaoClient.BuscarVagaSRS(token);
                return vagas;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro : {ex.Message}");
            }
        }

        public async Task<IEnumerable<VagaOrquestracaoDTO>> ListarVagaOrquestracao(string email)
        {
            try
            {
                return await _srsRepository.ListarVagaOrquestracao(email);
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting job orders", ex);
            }
        }

        public async Task<ActionResult<IEnumerable<CanditatoVagaSrsDTO>>> ListarCandidatosPorVaga(long? idVaga)
        {
            try
            {
                var candidatosPorVaga = await _srsRepository.ListarCandidatosPorVaga(idVaga);
                return candidatosPorVaga;
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting job orders", ex);
            }
        }

        public async Task<IEnumerable<TotalizadoresVagaDTO>> TotalizadoresVaga(long? idVaga)
        {
            try
            {
                return await _srsRepository.TotalizadoresVaga(idVaga);
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting job orders", ex);
            }
        }

        public async Task<ApiGenericResult<List<DropDownItemDTO>>> ListarSolicitantes()
        {
            return await _sRSVagaClient.ListarSolicitantes(_tokenSistema);
        }

        public async Task<ApiGenericResult<List<DropDownItemDTO>>> ListarAprovadores()
        {
            return await _sRSVagaClient.ListarAprovadores(_tokenSistema);
        }

        public async Task<ApiGenericResult<List<DropDownItemDTO>>> ListarTermometroVagas()
        {
            return await _sRSVagaClient.ListarTermometroVagas(_tokenSistema);
        }

        public async Task<ApiGenericResult<List<DropDownItemDTO>>> ListarStackPrincipal()
        {
            return await _sRSVagaClient.ListarStackPrincipal(_tokenSistema);
        }

        public async Task<ApiGenericResult<List<DropDownItemDTO>>> ListarConfiguracaoMaquina(string idContaCrm, int hardskillId)
        {
            return await _sRSVagaClient.ListarConfiguracaoMaquina(_tokenSistema, idContaCrm, hardskillId);
        }

        public async Task<ApiGenericResult<List<DropDownItemDTO>>> ListarTipoVaga()
        {
            return await _sRSVagaClient.ListarTipoVaga(_tokenSistema);
        }

        public async Task<ApiGenericResult<List<DropDownItemDTO>>> ListarDuracaoContrato()
        {
            return await _sRSVagaClient.ListarDuracaoContrato(_tokenSistema);
        }

        public async Task<ApiGenericResult<List<CargoDropdownItemDTO>>> ListarCargos()
        {
            return await _sRSVagaClient.ListarCargos(_tokenSistema);
        }

        public async Task<ApiGenericResult<List<DropDownItemDTO>>> ListarTipoContratacao()
        {
            return await _sRSVagaClient.ListarTipoContratacao(_tokenSistema);
        }

        public async Task<ApiGenericResult<List<DropDownItemDTO>>> ListarCargaHoraria()
        {
            return await _sRSVagaClient.ListarCargaHoraria(_tokenSistema);
        }

        public async Task<ApiGenericResult<List<DropDownItemDTO>>> ListarLocalTrabalho()
        {
            return await _sRSVagaClient.ListarLocalTrabalho(_tokenSistema);
        }

        public async Task<ApiGenericResult<List<DropDownItemDTO>>> ListarUnidadesSRS()
        {
            return await _sRSVagaClient.ListarUnidadesSRS(_tokenSistema);
        }
        
        public async Task<ApiGenericResult<List<JobOrderSemanticaDTO>>> ListarVagasParaSemanticaComSkill(int cursor, int limite)
        {
            ValidacaoUtil.ObrigaCursorLimite(cursor, limite);
            return await _sRSVagaClient.ListarVagasParaSemanticaComSkill(_tokenSistema, cursor, limite);
        }

        public async Task<IEnumerable<ListarVagasCadastradasFourmakersResult>> ListarVagasCadastradas(int limite, int cursor, int orgId, string codCliente = null, string gestorExternoPerfilId = null, string busca = null)
        {
            ValidaAcessoAoCadastroVaga(_aspNetUser.GetUsuarioLogado().Cpf, _aspNetUser.GetUsuarioLogado().OrgId);

            ValidacaoUtil.ObrigaCursorLimite(cursor, limite);

            return await _vagaFourmakersRepository.ListarVagasCadastradas(limite, cursor, orgId, codCliente, gestorExternoPerfilId, busca);
        }

        public async Task<ApiGenericResult<List<DropDownItemDTO>>> ListarClientesComVagasVigentes(int orgId)
        {
            var result = new ApiGenericResult<List<DropDownItemDTO>>();
            result.Retorno = await _vagaFourmakersRepository.ListarClientesComVagasVigentes(orgId);
            return result;
        }

        public async Task<ApiGenericResult<List<DropDownItemDTO>>> ListarGestorExternoPerfilComVagasVigentesPorCliente(string codigoCliente, int orgId)
        {
            var result = new ApiGenericResult<List<DropDownItemDTO>>();
            result.Retorno = await _vagaFourmakersRepository.ListarGestorExternoPerfilComVagasVigentesPorCliente(codigoCliente, orgId);
            return result;
        }

        private void ValidaAcessoAoCadastroVaga(string cpf, int orgId)
        {
            var funcionalidadesAcesso = FuncionalidadeSistemaEnum.CADASTRO_VAGAS;

            var isValid = _funcionalidadeSistemaRepository.ValidaAcessoFuncionalidade(
                cpf, orgId, funcionalidadesAcesso);

            if (!isValid.Result)
            {
                throw new UnauthorizedAccessException("Acesso negado para Mapa de Alocação");
            }
        }

        public async Task<ApiGenericResult> RecomendarCandidatosPorEmail(RecomendarCandidatosParam request)
        {
            // Validações de acesso
            await ValidacaoAcessoAoCvDigital();
            await _vagaValidatorService.ValidarCamposEnvioCandidatosAderencia(request);
            var dadosUsuarioLogado = _buscaColaboradorRepository.GetColaborador(_usuarioLogado.Cpf, _usuarioLogado.OrgId);
            var template = _templateRepository.BuscaTemplateEmail(TemplateOrgParametroEnum.CV_EM_MASSA);
            var templateFinal = template.Template;

            // Preparar os itens da tabela de colaboradores
            var tabelaItems = new StringBuilder();

            foreach (var colaborador in request.Colaboradores)
            {
                var habilidadesCard = GerarHabilidadesCardEnvioEmail(colaborador.Habilidades);

                var htmlContent = string.Format(@"
                        <tr>
                            <td style='padding-left: 12px; background-color: #ffffff; border-bottom: 1px solid #eaecf0; font-family: Arial; font-size: 14px; font-weight: 500; color: #101828; height: 60px;'>{0}</td>
                            <td style='padding-left: 12px; background-color: #ffffff; border-bottom: 1px solid #eaecf0; font-family: Arial; font-size: 14px; font-weight: 500; color: #101828; height: 60px;'>{1}</td>
                            <td style='display: flex; height: 60px; align-items: center; gap: 5px; padding-left: 12px; background-color: #ffffff; border-bottom: 1px solid #eaecf0;'>
                                <img style='width: 17px; height: 17px;' src='https://fsys2-public.s3.us-east-1.amazonaws.com/cv_link_button_icon.png'/>
                                <a href='{2}' target='_blank' style='font-family: Arial; font-size: 14px; font-weight: 500; text-align: left; text-decoration-line: underline; text-decoration-style: solid; color: #525252; cursor: pointer;'>Ver</a>
                            </td>
                        </tr>",
                    colaborador.Nome,
                    habilidadesCard,
                    colaborador.Link);

                tabelaItems.Append(htmlContent);
            }

            var usuarioEnvioDados = $"{dadosUsuarioLogado.NomeCompleto} - {dadosUsuarioLogado.Diretoria.Diretoria}";

            templateFinal = templateFinal.Replace("${Email_Usuario_Logado}", _usuarioLogado.Email);
            templateFinal = templateFinal.Replace("${NOME_E_ORG}", usuarioEnvioDados);
            templateFinal = templateFinal.Replace("${ITENS_TABELA}", tabelaItems.ToString());

            var assuntoPadrao = "Seleção Personalizada de Profissionais para Sua Avaliação";

            if (request.Assunto.ToStringOuNull() != null)
            {
                assuntoPadrao = request.Assunto;
            }

            foreach (var email in request.Emails)
            {
                await _templateRepository.RegistraTemplateEmailAsync(_usuarioLogado.OrgId, email, assuntoPadrao, templateFinal);
            }

            BusinessLog(request);

            return new ApiGenericResult { Sucesso = true };
        }

        private async Task ValidacaoAcessoAoCvDigital()
        {
            if (!await _funcionalidadeSistemaRepository.ValidaAcessoFuncionalidade(_usuarioLogado.Cpf, _usuarioLogado.OrgId, FuncionalidadeSistemaEnum.CV_DIGITAL))
                throw new UnauthorizedAccessException("Acesso negado ao Cv Digital");
        }

        private async void BusinessLog(RecomendarCandidatosParam request)
        {
            try
            {
                var id = await _repository.InserirRecomendacaoProfissionalAderencia(_usuarioLogado.OrgId, _usuarioLogado.Cpf, request.VagaId, JsonConvert.SerializeObject(request), 1);

                _log.Log("Information", LevelsEnum.Information);
                _log.Log($"Log de negocio para recomendacao de candidato por aderencia gravado no banco com sucesso sob o ID {id}", LevelsEnum.Information);
            }
            catch (Exception ex)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log("Erro na gravacao do Log de negocio para recomendacao de candidato por aderencia", LevelsEnum.Critical);
                _log.Log(ex.StackTrace, LevelsEnum.Critical);
            }
        }

        private string GerarHabilidadesCardEnvioEmail(List<string> habilidades)
        {
            if (habilidades == null || habilidades.Count == 0)
            {
                return string.Empty;
            }

            var primeiraHabilidade = habilidades.FirstOrDefault();
            var habilidadesCard = $@"
                <span style='padding: 5px 10px; border-radius: 1000px; opacity: 0.8; background-color: #d0e2ff; font-family: Arial, sans-serif; font-size: 12px; font-weight: 500; letter-spacing: 0.3px; text-align: left;color: #0043ce;'>
                    {primeiraHabilidade}
                </span>";

            if (habilidades.Count > 1)
            {
                var cardSkillCount = $@"
                    <span style='padding: 5px 10px; border-radius: 1000px; opacity: 0.8; background-color: #d0e2ff; font-family: Arial, sans-serif; font-size: 12px; font-weight: 500; etter-spacing: 0.3px; text-align: left; color: #0043ce; margin-left: 5px;'>
                        +{habilidades.Count - 1}
                    </span>";
                habilidadesCard += cardSkillCount;
            }

            return habilidadesCard;
        }

        public async Task<ActionResult<ApiGenericResult<List<ListarVagasEmBancoDeTalentosResult>>>> ListarVagasPipeline(int cursor, int limite, string busca, string cliente, string dataInicio, string dataFim, int? orgId)
        {
            var result = new ApiGenericResult<List<ListarVagasEmBancoDeTalentosResult>>();

            if (String.IsNullOrEmpty(dataInicio))
                dataInicio = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");

            if (String.IsNullOrEmpty(dataFim))
                dataFim = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");

            await _vagaValidatorService.ValidarDatas(dataInicio, dataFim);

            DateTime.TryParse(dataInicio, out DateTime dtInicio);
            DateTime.TryParse(dataFim, out DateTime dtFim);

            if (orgId != null)
            {
                // Trial e Fourmakers: retorna todas as vagas (sem filtro de org)
                if (orgId.Value == EnumORG.TRIAL_8.ToInt() || orgId.Value == EnumORG.FOURMAKERS_1.ToInt())
                {
                    result.Retorno = await _vagaFourmakersRepository.ListarVagasPipeline(dtInicio, dtFim, cliente, cursor, limite, busca);
                    return result;
                }
                
                // FMU: retorna vagas de FourSys e Fourmakers
                if (orgId.Value == EnumORG.FMU_7.ToInt())
                {
                    var orgIds = new List<int> { EnumORG.FOURSYS_2.ToInt(), EnumORG.FOURMAKERS_1.ToInt() };
                    result.Retorno = await _vagaFourmakersRepository.ListarVagasPipelinePorOrgs(dtInicio, dtFim, cliente, cursor, limite, busca, orgIds);
                    return result;
                }
            }

            result.Retorno = await _vagaFourmakersRepository.ListarVagasPipelinePorOrg(dtInicio, dtFim, cliente, cursor, limite, busca, orgId);
            return result;
        }

        public async Task<Guid?> CriarVagaAutomaticamenteAPartirDePerfilDeAtuacao(GestorExternoPerfilInput gestorExternoPerfilInput, string codigoColaboradorLogado)
        {
            try
            {
                _log.Log("Fluxo de criacao automatica de vaga iniciado.", LevelsEnum.Information);
                var vaga = new InserirVagaRecrutamentoDTO();

                vaga.Titulo = gestorExternoPerfilInput.NomePerfil;
                vaga.Descricao = gestorExternoPerfilInput.InformacoesRelevantes;
                vaga.Cargo = gestorExternoPerfilInput.NomePerfil;
                vaga.Localizacao = gestorExternoPerfilInput.ModeloTrabalhoDescricao;
                vaga.Estado = gestorExternoPerfilInput.Estado;
                vaga.Cidade = gestorExternoPerfilInput.Cidade;
                vaga.Cep = gestorExternoPerfilInput.Cep;
                vaga.Pais = gestorExternoPerfilInput.Pais;
                vaga.CodigoGestor = gestorExternoPerfilInput.CodGestorExterno;
                vaga.OrgId = gestorExternoPerfilInput.OrgId.ToString();
                vaga.NumeroDeVagas = 1;
                vaga.CustoProfissional = gestorExternoPerfilInput.CustoPerfil is null ? 0 : gestorExternoPerfilInput.CustoPerfil.Value;
                vaga.IdPerfilGerador = gestorExternoPerfilInput.Id.ToString();
                vaga.RateCard = gestorExternoPerfilInput.RatecardPerfil is null ? 0 : gestorExternoPerfilInput.RatecardPerfil.Value;

                var modelosTrabalho = await _gestaoAlocadosRepository.ListarModelosTrabalhoAsync();
                var modeloTrabalho = modelosTrabalho?.Where(m => m.Id == gestorExternoPerfilInput.ModeloTrabalhoId).FirstOrDefault();

                vaga.ModeloTrabalhoCod = modeloTrabalho?.Codigo;
                vaga.ModeloTrabalhoId = gestorExternoPerfilInput.ModeloTrabalhoId;
                vaga.PermanenciaId = gestorExternoPerfilInput.PermanenciaId;
                vaga.Frequencia = gestorExternoPerfilInput.HibridoDias is null ? null : gestorExternoPerfilInput.HibridoDias.Value.ToString();

                var hardSkills = gestorExternoPerfilInput.GestorExternoPerfilSkills.Where(m => m.ItemPerfil.Id == ItemPerfilEnum.COMPETENCIA.ToInt()).ToList();
                var softSkills = gestorExternoPerfilInput.GestorExternoPerfilSkills.Where(m => m.ItemPerfil.Id == ItemPerfilEnum.SOFTSKILL.ToInt()).ToList();
                var metodologias = gestorExternoPerfilInput.GestorExternoPerfilSkills.Where(m => m.ItemPerfil.Id == ItemPerfilEnum.METODOLOGIA.ToInt()).ToList();
                var idiomas = gestorExternoPerfilInput.GestorExternoPerfilSkills.Where(m => m.ItemPerfil.Id == ItemPerfilEnum.IDIOMA.ToInt()).ToList();
                var conhecimentoNegocio = gestorExternoPerfilInput.GestorExternoPerfilSkills.Where(m => m.ItemPerfil.Id == ItemPerfilEnum.DOMINIONEGOCIO.ToInt()).ToList();

                vaga.Hardskills = new List<SkillNivelDTO>();
                foreach (var hardSkill in hardSkills)
                    vaga.Hardskills.Add(new SkillNivelDTO { Id = hardSkill.Skill.Id, Descricao = hardSkill.Skill.Descricao, Nivel = new DataTransferObject.Domain.Nivel.NivelDTO { Id = hardSkill.Nivel.Id }, Relevante = hardSkill.Relevante });

                vaga.Softskills = new List<SkillNivelDTO>();
                foreach (var softSkill in softSkills)
                    vaga.Softskills.Add(new SkillNivelDTO { Id = softSkill.Skill.Id, Descricao = softSkill.Skill.Descricao, Nivel = new DataTransferObject.Domain.Nivel.NivelDTO { Id = softSkill.Nivel.Id }, Relevante = softSkill.Relevante });

                vaga.Metodologias = new List<SkillNivelDTO>();
                foreach (var metodologia in metodologias)
                    vaga.Metodologias.Add(new SkillNivelDTO { Id = metodologia.Skill.Id, Descricao = metodologia.Skill.Descricao, Nivel = new DataTransferObject.Domain.Nivel.NivelDTO { Id = metodologia.Nivel.Id }, Relevante = metodologia.Relevante });

                vaga.Idiomas = new List<SkillNivelDTO>();
                foreach (var idioma in idiomas)
                    vaga.Idiomas.Add(new SkillNivelDTO { Id = idioma.Skill.Id, Descricao = idioma.Skill.Descricao, Nivel = new DataTransferObject.Domain.Nivel.NivelDTO { Id = idioma.Nivel.Id }, Relevante = idioma.Relevante });

                vaga.Dominios = new List<SkillNivelDTO>();
                foreach (var dominio in conhecimentoNegocio)
                    vaga.Dominios.Add(new SkillNivelDTO { Id = dominio.Skill.Id, Descricao = dominio.Skill.Descricao, Nivel = new DataTransferObject.Domain.Nivel.NivelDTO { Id = dominio.Nivel.Id }, Relevante = dominio.Relevante });

                vaga.TipoEmpregoLinkedin = gestorExternoPerfilInput.TipoEmpregoLinkedin;
                vaga.NivelExperienciaLinkedin = gestorExternoPerfilInput.NivelExperienciaLinkedin;

                var resultado = await InserirVagaRecrutamento(vaga, codigoColaboradorLogado, true);
                
                if (resultado?.Retorno != null && Guid.TryParse(resultado.Retorno.Id, out Guid vagaId))
                {
                    return vagaId;
                }
                
                return null;
            }
            catch (Exception ex)
            {
                _log.Log("Erro na criacao automatica de vaga.", LevelsEnum.Error);
                _log.Log($"{JsonConvert.SerializeObject(ex)}", LevelsEnum.Error);
                return null;
            }
        }

        private async Task EnviarEmails(VagaRecrutamentoDTO vaga, Exception ex, string codigoColaboradorLogado, string tituloEmail)
        {
            var emails = new List<string>();
            emails.Add("mauricio.ribeiro@foursys.com.br");
            emails.Add("nayara.alves@foursys.com.br");

            var assuntoPadrao = "ERRO LinkedIn Robo";
            var templateFinal = CriarTemplateEmailErro(vaga, ex, codigoColaboradorLogado, tituloEmail);

            foreach (var email in emails)
            {
                await _templateRepository.RegistraTemplateEmailAsync(_usuarioLogado.OrgId, email, assuntoPadrao, templateFinal);
            }
        }

        private string CriarTemplateEmailErro(VagaRecrutamentoDTO vaga, Exception ex, string codigoColaboradorLogado, string tituloEmail)
        {
            var dataHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            var stackTrace = ex.StackTrace?.Length > 1000 ? ex.StackTrace.Substring(0, 1000) + "..." : ex.StackTrace;

            return $@"
                    <!DOCTYPE html>
                    <html lang='pt-BR'>
                    <head>
                        <meta charset='UTF-8'>
                        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                        <title>{tituloEmail}</title>
                        <style>
                            body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; margin: 0; padding: 20px; background-color: #f5f5f5; }}
                            .container {{ max-width: 800px; margin: 0 auto; background-color: white; border-radius: 8px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); overflow: hidden; }}
                            .header {{ background: linear-gradient(135deg, #dc3545 0%, #c82333 100%); color: white; padding: 30px; text-align: center; }}
                            .header h1 {{ margin: 0; font-size: 28px; font-weight: 600; }}
                            .header .subtitle {{ margin: 10px 0 0 0; font-size: 16px; opacity: 0.9; }}
                            .content {{ padding: 30px; }}
                            .section {{ margin-bottom: 25px; }}
                            .section h2 {{ color: #dc3545; border-bottom: 2px solid #dc3545; padding-bottom: 8px; margin-bottom: 15px; font-size: 20px; }}
                            .info-grid {{ display: grid; grid-template-columns: 1fr 1fr; gap: 20px; margin-bottom: 20px; }}
                            .info-item {{ background-color: #f8f9fa; padding: 15px; border-radius: 6px; border-left: 4px solid #007bff; }}
                            .info-item strong {{ color: #495057; display: block; margin-bottom: 5px; font-size: 14px; text-transform: uppercase; letter-spacing: 0.5px; }}
                            .info-item span {{ color: #6c757d; font-size: 14px; }}
                            .error-details {{ background-color: #fff5f5; border: 1px solid #fed7d7; border-radius: 6px; padding: 20px; margin: 20px 0; }}
                            .error-details h3 {{ color: #c53030; margin-top: 0; }}
                            .error-message {{ background-color: #fed7d7; padding: 15px; border-radius: 4px; margin: 15px 0; font-family: 'Courier New', monospace; font-size: 13px; white-space: pre-wrap; }}
                            .stack-trace {{ background-color: #f7fafc; border: 1px solid #e2e8f0; border-radius: 4px; padding: 15px; margin: 15px 0; font-family: 'Courier New', monospace; font-size: 12px; max-height: 300px; overflow-y: auto; }}
                            .action-required {{ background: linear-gradient(135deg, #ffc107 0%, #e0a800 100%); color: #856404; padding: 20px; border-radius: 6px; margin: 20px 0; }}
                            .action-required h3 {{ margin-top: 0; color: #856404; }}
                            .action-steps {{ background-color: #fff3cd; padding: 15px; border-radius: 4px; margin: 15px 0; }}
                            .action-steps ol {{ margin: 0; padding-left: 20px; }}
                            .action-steps li {{ margin-bottom: 8px; }}
                            .footer {{ background-color: #f8f9fa; padding: 20px; text-align: center; color: #6c757d; font-size: 12px; border-top: 1px solid #dee2e6; }}
                            .priority {{ display: inline-block; background-color: #dc3545; color: white; padding: 4px 12px; border-radius: 20px; font-size: 12px; font-weight: 600; text-transform: uppercase; margin-bottom: 15px; }}
                            .vaga-data {{ background-color: #e3f2fd; border: 1px solid #bbdefb; border-radius: 6px; padding: 20px; margin: 20px 0; }}
                            .vaga-data h3 {{ color: #1976d2; margin-top: 0; }}
                            .vaga-grid {{ display: grid; grid-template-columns: 1fr 1fr; gap: 15px; }}
                            .vaga-item {{ background-color: white; padding: 12px; border-radius: 4px; border: 1px solid #e3f2fd; }}
                            .vaga-item strong {{ color: #1976d2; display: block; margin-bottom: 3px; font-size: 12px; text-transform: uppercase; }}
                            .vaga-item span {{ color: #424242; font-size: 13px; }}
                        </style>
                    </head>
                    <body>
                        <div class='container'>
                            <div class='header'>
                                <h1>🚨 ERRO CRÍTICO</h1>
                                <p class='subtitle'>Falha na Criação Automática de Vaga no LinkedIn</p>
                                <div class='priority'>ALTA PRIORIDADE</div>
                            </div>
        
                            <div class='content'>
                                <div class='section'>
                                    <h2>Informações do Erro</h2>
                                    <div class='info-grid'>
                                        <div class='info-item'>
                                            <strong>Data/Hora</strong>
                                            <span>{dataHora}</span>
                                        </div>
                                        <div class='info-item'>
                                            <strong>Ambiente</strong>
                                            <span>{VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.AMBIENTE)}</span>
                                        </div>
                                        <div class='info-item'>
                                            <strong>Módulo</strong>
                                            <span>VagaService</span>
                                        </div>
                                        <div class='info-item'>
                                            <strong>Método</strong>
                                            <span>InserirVagaRecrutamentoNoLinkedin</span>
                                        </div>
                                        <div class='info-item'>
                                            <strong>Usuário Logado</strong>
                                            <span>{codigoColaboradorLogado}</span>
                                        </div>
                                        <div class='info-item'>
                                            <strong>Org ID</strong>
                                            <span>{vaga?.OrgId}</span>
                                        </div>
                                    </div>
                                </div>

                                <div class='section'>
                                    <h2>Dados da Vaga</h2>
                                    <div class='vaga-data'>
                                        <h3>Informações da Vaga que Falhou</h3>
                                        <div class='vaga-grid'>
                                            <div class='vaga-item'>
                                                <strong>Título</strong>
                                                <span>{vaga.Titulo ?? "N/A"}</span>
                                            </div>
                                            <div class='vaga-item'>
                                                <strong>Cargo</strong>
                                                <span>{vaga.Cargo ?? "N/A"}</span>
                                            </div>
                                            <div class='vaga-item'>
                                                <strong>Descrição</strong>
                                                <span>{(vaga.Descricao?.Length > 100 ? vaga.Descricao.Substring(0, 100) + "..." : vaga.Descricao) ?? "N/A"}</span>
                                            </div>
                                            <div class='vaga-item'>
                                                <strong>Localização</strong>
                                                <span>{vaga.Localizacao ?? "N/A"}</span>
                                            </div>
                                            <div class='vaga-item'>
                                                <strong>Cidade</strong>
                                                <span>{vaga.Cidade ?? "N/A"}</span>
                                            </div>
                                            <div class='vaga-item'>
                                                <strong>Estado</strong>
                                                <span>{vaga.Estado ?? "N/A"}</span>
                                            </div>
                                            <div class='vaga-item'>
                                                <strong>Número de Vagas</strong>
                                                <span>{vaga.NumeroDeVagas}</span>
                                            </div>
                                            <div class='vaga-item'>
                                                <strong>ID Perfil Gerador</strong>
                                                <span>{vaga.IdPerfilGerador ?? "N/A"}</span>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class='section'>
                                    <h2>Detalhes do Erro</h2>
                                    <div class='error-details'>
                                        <h3>Exceção Capturada</h3>
                                        <div class='error-message'>{ex.Message}</div>
                    
                                        <h3>Stack Trace (Primeiros 1000 caracteres)</h3>
                                        <div class='stack-trace'>{stackTrace ?? "Stack trace não disponível"}</div>
                                    </div>
                                </div>

                                <div class='section'>
                                    <h2>Ação Requerida</h2>
                                    <div class='action-required'>
                                        <h3>⚠️ ATENÇÃO: Intervenção Manual Necessária</h3>
                                        <p><strong>Este erro requer ação imediata da equipe de desenvolvimento.</strong></p>
                                    </div>
                
                                    <div class='action-steps'>
                                        <h3>Passos para Resolução:</h3>
                                        <ol>
                                            <li><strong>Verificar logs do sistema</strong> para identificar a causa raiz do erro</li>
                                            <li><strong>Validar conectividade</strong> com a API do Azure Logic Apps</li>
                                            <li><strong>Verificar autenticação OAuth</strong> com Microsoft Azure</li>
                                            <li><strong>Validar dados da vaga</strong> antes do envio</li>
                                            <li><strong>Testar endpoint manualmente</strong> para confirmar o problema</li>
                                            <li><strong>Criar vaga manualmente</strong> se necessário para não impactar o usuário</li>
                                            <li><strong>Implementar correção</strong> e validar em ambiente de teste</li>
                                            <li><strong>Monitorar</strong> após a correção para evitar reincidência</li>
                                        </ol>
                                    </div>
                                </div>

                                <div class='section'>
                                    <h2>Informações Técnicas</h2>
                                    <div class='info-grid'>
                                        <div class='info-item'>
                                            <strong>API Endpoint</strong>
                                            <span>Azure Logic Apps - LinkedIn</span>
                                        </div>
                                        <div class='info-item'>
                                            <strong>Autenticação</strong>
                                            <span>OAuth 2.0 - Microsoft</span>
                                        </div>
                                        <div class='info-item'>
                                            <strong>Fluxo</strong>
                                            <span>Criação Automática → API LinkedIn</span>
                                        </div>
                                        <div class='info-item'>
                                            <strong>Impacto</strong>
                                            <span>Vaga não criada no LinkedIn</span>
                                        </div>
                                    </div>
                                </div>
                            </div>
        
                            <div class='footer'>
                                <p><strong>Este email foi gerado automaticamente pelo sistema de monitoramento de erros.</strong></p>
                                <p>Para suporte técnico, entre em contato com a equipe de desenvolvimento.</p>
                                <p>Gerado em: {dataHora}</p>
                            </div>
                        </div>
                    </body>
                    </html>";
        }

        private async Task InserirVagaRecrutamentoNoLinkedin(VagaRecrutamentoDTO vaga)
        {
            _log.Log("Iniciando inserção da vaga no LinkedIn via Azure Logic Apps", LevelsEnum.Information);

            var accessToken = await ObterTokenOAuthLinkedin();
            if (string.IsNullOrEmpty(accessToken))
                throw new Exception("Não foi possível obter o token de acesso OAuth para o LinkedIn");

            var vagaLinkedin = await PrepararVagaParaLinkedin(vaga);

            var resultado = await EnviarVagaParaAzureLogicApps(vagaLinkedin, accessToken);

            _log.Log($"Vaga enviada com sucesso para o LinkedIn. Resultado: {resultado}", LevelsEnum.Information);
        }

        private async Task<string> ObterTokenOAuthLinkedin()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var tokenRequest = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("client_id", "2df17512-a7d3-451f-b2eb-1afd505cc286"),
                        new KeyValuePair<string, string>("grant_type", "client_credentials"),
                        new KeyValuePair<string, string>("client_secret", "dMk8Q~NxW5II-.hgeo.Ogs6Rb0Ls-V_tamPZDdoo"),
                        new KeyValuePair<string, string>("scope", "https://service.flow.microsoft.com//.default")
                    });

                    var response = await httpClient.PostAsync(
                        "https://login.microsoftonline.com/8f0133fa-8efb-40b1-8ac6-37c78469f445/oauth2/v2.0/token",
                        tokenRequest);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var tokenResponse = JsonConvert.DeserializeObject<dynamic>(responseContent);
                        return tokenResponse.access_token;
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        _log.Log($"Erro ao obter token OAuth: {response.StatusCode} - {errorContent}", LevelsEnum.Error);
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Log($"Erro ao obter token OAuth: {ex.Message}", LevelsEnum.Error);
                return null;
            }
        }

        private async Task<VagaLinkedinRequestDTO> PrepararVagaParaLinkedin(VagaRecrutamentoDTO vaga)
        {
            return new VagaLinkedinRequestDTO
            {
                titulo = vaga.Titulo,
                cargo = vaga.Titulo,
                localTrabalho = vaga.ModeloTrabalhoCod,
                localidade = vaga.ModeloTrabalhoCod == 3 ? $"Brasil" : $"{vaga.Cidade}",
                tipoEmprego = await ObterTipoEmprego(vaga.TipoEmpregoLinkedin),
                senioridade = await ObterSenioridade(vaga.NivelExperienciaLinkedin),
                descricaoVaga = vaga.Descricao,
                competencias = ObterCompetencias(vaga),
                requererCurriculo = true,
                triagemEliminatoria = false,
                arquivarForaDoPais = false,
                codigoVaga = vaga.Id,
                urlVaga = String.Empty
            };
        }

        private async Task<int> ObterTipoEmprego(Guid? tipoEmpregoId)
        {
            if (!tipoEmpregoId.HasValue)
                return 1; // Tempo integral por padrão

            try
            {
                var tipoEmprego = await _vagaFourmakersRepository.ObterTipoEmpregoLinkedinPorId(tipoEmpregoId.Value);
                if (tipoEmprego != null)
                {
                    // Mapeia a descrição para o código numérico do LinkedIn
                    // 1.Tempo integral, 2.Meio período, 3.Contrato, 4.Temporário, 5.Outro, 6.Voluntário, 7.Estágio
                    switch (tipoEmprego.Descricao.ToLower())
                    {
                        case "tempo integral":
                        case "full-time":
                            return 1;
                        case "meio período":
                        case "part-time":
                            return 2;
                        case "contrato":
                        case "contract":
                            return 3;
                        case "temporário":
                        case "temporary":
                            return 4;
                        case "outro":
                        case "other":
                            return 5;
                        case "voluntário":
                        case "volunteer":
                            return 6;
                        case "estágio":
                        case "internship":
                            return 7;
                        default:
                            return 1; // Tempo integral por padrão
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Log($"Erro ao buscar tipo de emprego LinkedIn: {ex.Message}", LevelsEnum.Warning);
            }

            return 1; // Tempo integral por padrão
        }

        private async Task<int> ObterSenioridade(Guid? nivelExperienciaId)
        {
            if (!nivelExperienciaId.HasValue)
                return 4; // Pleno-sênior por padrão

            try
            {
                var nivelExperiencia = await _vagaFourmakersRepository.ObterNivelExperienciaLinkedinPorId(nivelExperienciaId.Value);
                if (nivelExperiencia != null)
                {
                    // Mapeia a descrição para o código numérico do LinkedIn
                    // 1.Estagio, 2.Assistente, 3.Júnior, 4.Pleno-sênior, 5.Diretor, 6.Executivo, 7.Não aplicável
                    switch (nivelExperiencia.Descricao.ToLower())
                    {
                        case "estágio":
                        case "internship":
                        case "estagiário":
                            return 1;
                        case "assistente":
                        case "assistant":
                            return 2;
                        case "júnior":
                        case "junior":
                        case "jr":
                            return 3;
                        case "pleno":
                        case "pleno-sênior":
                        case "senior":
                        case "sênior":
                            return 4;
                        case "diretor":
                        case "director":
                            return 5;
                        case "executivo":
                        case "executive":
                        case "c-level":
                            return 6;
                        case "não aplicável":
                        case "nao aplicavel":
                        case "not applicable":
                        case "n/a":
                            return 7;
                        default:
                            return 4; // Pleno-sênior por padrão
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Log($"Erro ao buscar nível de experiência LinkedIn: {ex.Message}", LevelsEnum.Warning);
            }

            return 4; // Pleno-sênior por padrão
        }

        private List<string> ObterCompetencias(VagaRecrutamentoDTO vaga)
        {
            var competencias = new List<string>();

            if (vaga.Skills != null)
                foreach (var skill in vaga.Skills)
                    if (!string.IsNullOrEmpty(skill.SkillDescription))
                        competencias.Add(skill.SkillDescription.Replace("\t", "").Replace("\n", "").Replace("\r", "").Trim());

            return competencias;
        }

        private async Task<string> EnviarVagaParaAzureLogicApps(VagaLinkedinRequestDTO vagaLinkedin, string accessToken)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                var jsonContent = JsonConvert.SerializeObject(vagaLinkedin);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                _log.Log($"Robo Linkedin Json: {jsonContent}", LevelsEnum.Information);
                _log.Log($"Robo Linkedin Url: {_urlRoboLinkedin}", LevelsEnum.Information);
                _log.Log($"Robo Linkedin Token: {accessToken}", LevelsEnum.Information);

                var response = await httpClient.PostAsync(
                    _urlRoboLinkedin,
                    content);

                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    _log.Log($"Vaga enviada com sucesso para Azure Logic Apps. Status: {response.StatusCode}", LevelsEnum.Information);
                    return responseContent;
                }
                else
                {
                    _log.Log($"Erro ao enviar vaga para Azure Logic Apps. Status: {response.StatusCode}, Conteúdo: {responseContent}", LevelsEnum.Error);
                    throw new Exception($"Erro ao enviar vaga para Azure Logic Apps: {response.StatusCode} - {responseContent}");
                }
            }
        }

        private async Task<string> EnviarCancelamentoParaAzureLogicApps(object requestBody, string accessToken)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                var jsonContent = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var urlCancelamento = "https://default8f0133fa8efb40b18ac637c78469f4.45.environment.api.powerplatform.com:443/powerautomate/automations/direct/workflows/40985e40c1ed48b1a1bd30a4f648e939/triggers/manual/paths/invoke?api-version=1";

                _log.Log($"Cancelamento Linkedin Json: {jsonContent}", LevelsEnum.Information);
                _log.Log($"Cancelamento Linkedin Url: {urlCancelamento}", LevelsEnum.Information);
                _log.Log($"Cancelamento Linkedin Token: {accessToken}", LevelsEnum.Information);

                var response = await httpClient.PostAsync(
                    urlCancelamento,
                    content);

                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    _log.Log($"Vaga cancelada com sucesso no Azure Logic Apps. Status: {response.StatusCode}", LevelsEnum.Information);
                    return responseContent;
                }
                else
                {
                    _log.Log($"Erro ao cancelar vaga no Azure Logic Apps. Status: {response.StatusCode}, Conteúdo: {responseContent}", LevelsEnum.Error);
                    throw new Exception($"Erro ao cancelar vaga no Azure Logic Apps: {response.StatusCode} - {responseContent}");
                }
            }
        }

        public async Task<ApiGenericResult<VagaRecrutamentoDTO>> ObterVagaRecrutamentoPorCodigo(int id)
        {
            var vaga = await _vagaFourmakersRepository.ObterVagaPorCodigo(id);
            if (vaga == null)
                return new ApiGenericResult<VagaRecrutamentoDTO> { Sucesso = true, Mensagem = "Vaga não encontrada." };

            return new ApiGenericResult<VagaRecrutamentoDTO> { Sucesso = true, Retorno = vaga };
        }

        public async Task<ApiGenericResult<VagaRecrutamentoDTO>> ObterVagaRecrutamentoPorId(string vagaId)
        {
            var vaga = await _vagaFourmakersRepository.ObterVagaRecrutamentoPorId(vagaId);
            if (vaga == null)
                return new ApiGenericResult<VagaRecrutamentoDTO> { Sucesso = true, Mensagem = "Vaga não encontrada." };

            return new ApiGenericResult<VagaRecrutamentoDTO> { Sucesso = true, Retorno = vaga };
        }

        public async Task<ApiGenericResult<IEnumerable<HistoricoStatusVagaItemDTO>>> ListarHistoricoStatusVaga(string vagaId)
        {
            if (string.IsNullOrWhiteSpace(vagaId))
                return new ApiGenericResult<IEnumerable<HistoricoStatusVagaItemDTO>> { Sucesso = false, Mensagem = "Id da vaga é obrigatório." };

            var existe = await _vagaFourmakersRepository.EstaVagaExistePorId(vagaId);
            if (!existe)
                return new ApiGenericResult<IEnumerable<HistoricoStatusVagaItemDTO>> { Sucesso = false, Mensagem = "Vaga não encontrada." };

            var historico = await _vagaFourmakersRepository.ListarHistoricoStatusVaga(vagaId);
            return new ApiGenericResult<IEnumerable<HistoricoStatusVagaItemDTO>> { Sucesso = true, Retorno = historico };
        }

        public async Task<ApiGenericResult<VagaRecrutamentoDTO>> AtualizarVagaRecrutamentoPorId(AtualizarVagaRecrutamentoDTO vagaAtualizada, string codColaborador)
        {
            try
            {
                // Buscar a vaga existente
                var vagaExistente = await _vagaFourmakersRepository.ObterVagaRecrutamentoPorId(vagaAtualizada.Id);
                if (vagaExistente == null)
                    return new ApiGenericResult<VagaRecrutamentoDTO> { Sucesso = false, Mensagem = "Vaga não encontrada." };

                // Atualizar os campos editáveis
                vagaExistente.Titulo = vagaAtualizada.Titulo;
                vagaExistente.NumeroDeVagas = vagaAtualizada.NumeroDeVagas;
                vagaExistente.CustoProfissional = vagaAtualizada.CustoProfissional;
                vagaExistente.RateCard = vagaAtualizada.RateCard;
                vagaExistente.Descricao = vagaAtualizada.Descricao;
                vagaExistente.Cargo = vagaAtualizada.Cargo;
                vagaExistente.Localizacao = vagaAtualizada.Localizacao;
                vagaExistente.Estado = vagaAtualizada.Estado;
                vagaExistente.Cidade = vagaAtualizada.Cidade;
                vagaExistente.Cep = vagaAtualizada.Cep;
                vagaExistente.Pais = vagaAtualizada.Pais;
                vagaExistente.CodigoGestor = vagaAtualizada.CodigoGestor;
                vagaExistente.StatusVagaCod = vagaAtualizada.StatusVagaCod;
                vagaExistente.OrigemVagaCod = vagaAtualizada.OrigemVagaCod;
                vagaExistente.Frequencia = vagaAtualizada.Frequencia;
                vagaExistente.ModeloTrabalhoCod = vagaAtualizada.ModeloTrabalhoCod;
                vagaExistente.NomeGestor = vagaAtualizada.NomeGestor;
                vagaExistente.NomeCliente = vagaAtualizada.NomeCliente;
                vagaExistente.CodigoCliente = vagaAtualizada.CodigoCliente;
                vagaExistente.PropostaCrm = vagaAtualizada.PropostaCrm;
                vagaExistente.TipoVagaId = vagaAtualizada.TipoVagaId;
                vagaExistente.TipoContratacaoId = vagaAtualizada.TipoContratacaoId;
                vagaExistente.UnidadeId = vagaAtualizada.UnidadeId;
                vagaExistente.CodColaboradoresEntrevistadores = vagaAtualizada.CodColaboradoresEntrevistadores;
                vagaExistente.Tracking = vagaAtualizada.Tracking;
                vagaExistente.NumeroVagaCliente = vagaAtualizada.NumeroVagaCliente;
                vagaExistente.CodigoClienteFourmakers = vagaAtualizada.CodigoClienteFourmakers;
                vagaExistente.TipoEmpregoLinkedin = vagaAtualizada.TipoEmpregoLinkedin;
                vagaExistente.NivelExperienciaLinkedin = vagaAtualizada.NivelExperienciaLinkedin;
                vagaExistente.ModeloTrabalhoId = vagaAtualizada.ModeloTrabalhoId;
                vagaExistente.PermanenciaId = vagaAtualizada.PermanenciaId;
                vagaExistente.Skills = vagaAtualizada.Skills;
                vagaExistente.RecrutadorVaga = vagaAtualizada.RecrutadorVaga;
                vagaExistente.MaquinaColaborador = vagaAtualizada.Maquina;
                vagaExistente.CandidatosContratados = vagaAtualizada.CandidatosContratados;

                // Atualizar data de alteração
                vagaExistente.DataUltimaAlteracao = DateTime.Now;

                // Salvar no banco de dados
                await _vagaFourmakersRepository.AtualizarVaga(vagaExistente, codColaborador);

                // Classificar vaga após atualização (apenas se não houver classificação)
                try
                {
                    var existeClassificacao = await _classificacaoRepository.ExisteClassificacaoVaga(vagaExistente.Id);
                    if (!existeClassificacao)
                    {
                        await _classificacaoService.AtualizarClassificacaoVagaAsync(vagaExistente);
                    }
                }
                catch (Exception ex)
                {
                    _log.Log($"Erro ao classificar vaga após atualização: {ex}", LevelsEnum.Error);
                }

                return new ApiGenericResult<VagaRecrutamentoDTO> { Sucesso = true, Mensagem = "Vaga atualizada com sucesso.", Retorno = vagaExistente };
            }
            catch (Exception ex)
            {
                _log.Log($"Erro ao atualizar vaga por ID: {ex.Message}", LevelsEnum.Error);
                return new ApiGenericResult<VagaRecrutamentoDTO> { Sucesso = false, Mensagem = "Erro interno ao atualizar vaga." };
            }
        }

        public async Task<VagaAnonymousDTO> ObterVagaRecrutamentoPorCodigoAnonymous(int codigo)
        {
            return await _vagaFourmakersRepository.ObterVagaRecrutamentoPorCodigoAnonymous(codigo);
        }

        public async Task<ApiGenericResult<VagaRecrutamentoDTO>> InserirVagaRecrutamento(InserirVagaRecrutamentoDTO inserirVagaDTO, string cpfUsuarioLogado, bool criadaAutomaticamente = false)
        {
            await _vagaValidatorService.ValidarCadastroVagaRecrutamento(inserirVagaDTO);

            var vagaDto = new VagaRecrutamentoDTO();
            vagaDto.Skills = new List<VagaSkillRecrutamentoDTO>();

            vagaDto.Id = Guid.NewGuid().ToString();
            vagaDto.StatusVagaCod = StatusVagaRecrutamento.BancoDeTalentos.ToInt().ToString();
            vagaDto.CpfUsuarioAprovador = cpfUsuarioLogado;
            vagaDto.CpfUsuarioCriador = cpfUsuarioLogado;
            vagaDto.OrigemVagaCod = criadaAutomaticamente ? OrigemVagaRecrutamento.CriadaAutomaticamente.ToInt().ToString() : OrigemVagaRecrutamento.Fourmakers.ToInt().ToString();

            vagaDto.Titulo = inserirVagaDTO.Titulo;
            vagaDto.NumeroDeVagas = inserirVagaDTO.NumeroDeVagas.Value;
            vagaDto.CustoProfissional = inserirVagaDTO.CustoProfissional;
            vagaDto.Descricao = inserirVagaDTO.Descricao;
            vagaDto.Cargo = inserirVagaDTO.Cargo;
            vagaDto.Localizacao = inserirVagaDTO.Localizacao;
            vagaDto.Estado = inserirVagaDTO.Estado;
            vagaDto.Cidade = inserirVagaDTO.Cidade;
            vagaDto.Cep = inserirVagaDTO.Cep;
            vagaDto.Pais = inserirVagaDTO.Pais;
            vagaDto.CodigoGestor = inserirVagaDTO.CodigoGestor;
            vagaDto.OrgId = inserirVagaDTO.OrgId;
            vagaDto.IdPerfilGerador = inserirVagaDTO.IdPerfilGerador;
            vagaDto.ModeloTrabalhoCod = inserirVagaDTO.ModeloTrabalhoCod.HasValue ? inserirVagaDTO.ModeloTrabalhoCod.Value : 0;
            vagaDto.ModeloTrabalhoId = inserirVagaDTO.ModeloTrabalhoId?.ToString();
            vagaDto.PermanenciaId = inserirVagaDTO.PermanenciaId?.ToString();
            vagaDto.Frequencia = inserirVagaDTO.Frequencia;
            vagaDto.TipoEmpregoLinkedin = inserirVagaDTO.TipoEmpregoLinkedin;
            vagaDto.NivelExperienciaLinkedin = inserirVagaDTO.NivelExperienciaLinkedin;
            vagaDto.RateCard = inserirVagaDTO.RateCard;

            var skillCategories = new List<(IEnumerable<SkillNivelDTO> Skills, ItemPerfilEnum Category)>
                                            {
                                                (inserirVagaDTO.Hardskills, ItemPerfilEnum.COMPETENCIA),
                                                (inserirVagaDTO.Softskills, ItemPerfilEnum.SOFTSKILL),
                                                (inserirVagaDTO.Metodologias, ItemPerfilEnum.METODOLOGIA),
                                                (inserirVagaDTO.Dominios, ItemPerfilEnum.DOMINIONEGOCIO),
                                                (inserirVagaDTO.Idiomas, ItemPerfilEnum.IDIOMA)
                                            };

            foreach (var (skills, category) in skillCategories)
            {
                foreach (var skill in skills)
                {
                    vagaDto.Skills.Add(new VagaSkillRecrutamentoDTO
                    {
                        TipoSkillId = category.ToInt(),
                        SkillId = skill.Id.ToInt(),
                        SkillNivelId = skill.Nivel.Id.Value.ToInt(),
                        Relevante = skill.Relevante,
                    });
                }
            }

            var vagaInserida = await _vagaFourmakersRepository.InserirVaga(vagaDto, cpfUsuarioLogado);

            _log.Log($"Vaga criada automaticamente. Id: {vagaInserida.Id}", LevelsEnum.Information);

            // Classificar vaga após inserção
            try
            {
                await _classificacaoService.AtualizarClassificacaoVagaAsync(vagaInserida);
            }
            catch (Exception ex)
            {
                _log.Log($"Erro ao classificar vaga após inserção: {ex}", LevelsEnum.Error);
            }

            return new ApiGenericResult<VagaRecrutamentoDTO> { Sucesso = true, Mensagem = "Vaga inserida com sucesso.", Retorno = vagaInserida };
        }

        public async Task<ApiGenericResult<VagaRecrutamentoDTO>> AtualizarVagaRecrutamento(VagaVindaDeGestorExternoPerfil atualizarVagaDTO, string cpfUsuarioLogado, bool edicaoViaTela = false)
        {
            await _vagaValidatorService.ValidarAtualizacaoVagaRecrutamento(atualizarVagaDTO);

            var vagaExistente = await _vagaFourmakersRepository.ObterVagaPorCodigo(atualizarVagaDTO.Codigo.ToInt());
            if (vagaExistente == null)
                return new ApiGenericResult<VagaRecrutamentoDTO> { Sucesso = false, Mensagem = "Vaga não encontrada." };

            vagaExistente.Skills = new List<VagaSkillRecrutamentoDTO>();

            vagaExistente.StatusVagaCod = edicaoViaTela ? vagaExistente.StatusVagaCod : atualizarVagaDTO.StatusVagaCod;
            vagaExistente.CpfUsuarioAprovador = cpfUsuarioLogado;
            vagaExistente.CpfUsuarioCriador = cpfUsuarioLogado;
            vagaExistente.Titulo = atualizarVagaDTO.NomePerfil;
            vagaExistente.CustoProfissional = atualizarVagaDTO.CustoPerfil;
            vagaExistente.Descricao = atualizarVagaDTO.InformacoesRelevantes;
            vagaExistente.Localizacao = atualizarVagaDTO.ProfissionalLocalidadeId.ToString();
            vagaExistente.Estado = atualizarVagaDTO.Estado;
            vagaExistente.Cidade = atualizarVagaDTO.Cidade;
            vagaExistente.Cep = atualizarVagaDTO.Cep;
            vagaExistente.CodigoGestor = atualizarVagaDTO.CodGestorExterno;
            vagaExistente.OrgId = atualizarVagaDTO.OrgId;
            vagaExistente.ModeloTrabalhoCod = atualizarVagaDTO.ModeloTrabalhoCod.HasValue ? atualizarVagaDTO.ModeloTrabalhoCod.Value : 0;
            vagaExistente.ModeloTrabalhoId = atualizarVagaDTO.ModeloTrabalhoId.ToString();
            vagaExistente.PermanenciaId = atualizarVagaDTO.PermanenciaId.ToString();
            vagaExistente.NumeroDeVagas = (int)atualizarVagaDTO.NumeroDeVagas;
            vagaExistente.RateCard = atualizarVagaDTO.RatecardPerfil;
            vagaExistente.DataUltimaAlteracao = DateTime.UtcNow;
            vagaExistente.Skills = atualizarVagaDTO.GestorExternoPerfilSkills;
            vagaExistente.RecrutadorVaga = atualizarVagaDTO.RecrutadorVaga;
            vagaExistente.MaquinaColaborador = atualizarVagaDTO.Maquina;
            vagaExistente.CandidatosContratados = atualizarVagaDTO.CandidatosContratados;

            // Atualizar informações complementares se fornecidas
            if (!string.IsNullOrEmpty(atualizarVagaDTO.ColaboradorCodigoInternoColaboradorGestorOrgLogada))
            {
                vagaExistente.ColaboradorCodigoInternoColaboradorGestorOrgLogada = atualizarVagaDTO.ColaboradorCodigoInternoColaboradorGestorOrgLogada;
            }

            if (!string.IsNullOrEmpty(atualizarVagaDTO.PropostaCrm))
            {
                vagaExistente.PropostaCrm = atualizarVagaDTO.PropostaCrm;
            }

            if (!string.IsNullOrEmpty(atualizarVagaDTO.TipoVagaId))
            {
                vagaExistente.TipoVagaId = atualizarVagaDTO.TipoVagaId;
            }

            if (atualizarVagaDTO.TipoContratacaoId.HasValue)
            {
                vagaExistente.TipoContratacaoId = atualizarVagaDTO.TipoContratacaoId;
            }

            if (!string.IsNullOrEmpty(atualizarVagaDTO.UnidadeId))
            {
                vagaExistente.UnidadeId = atualizarVagaDTO.UnidadeId;
            }

            if (atualizarVagaDTO.CodColaboradoresEntrevistadores != null && atualizarVagaDTO.CodColaboradoresEntrevistadores.Count > 0)
            {
                vagaExistente.CodColaboradoresEntrevistadores = atualizarVagaDTO.CodColaboradoresEntrevistadores;
            }

            vagaExistente.TipoEmpregoLinkedin = atualizarVagaDTO.TipoEmpregoLinkedin;
            vagaExistente.NivelExperienciaLinkedin = atualizarVagaDTO.NivelExperienciaLinkedin;
            vagaExistente.Frequencia = atualizarVagaDTO.HibridoDias is null ? null : atualizarVagaDTO.HibridoDias.Value.ToString();

            await _vagaFourmakersRepository.AtualizarVaga(vagaExistente, cpfUsuarioLogado);

            // Classificar vaga após atualização (apenas se não houver classificação)
            try
            {
                await _classificacaoService.AtualizarClassificacaoVagaAsync(vagaExistente);
            }
            catch (Exception ex)
            {
                _log.Log($"Erro ao classificar vaga após atualização: {ex}", LevelsEnum.Error);
            }

            return new ApiGenericResult<VagaRecrutamentoDTO> { Sucesso = true, Mensagem = "Vaga atualizada com sucesso." };
        }

        public async Task<ApiGenericResult<bool>> CancelarVagaRecrutamento(int codigo, string codColaborador)
        {
            var vagaExistente = await _vagaFourmakersRepository.ObterIdVagaPorCodigo(codigo);
            if (vagaExistente == null)
                return new ApiGenericResult<bool> { Sucesso = false, Mensagem = "Vaga não encontrada." };

            await _vagaFourmakersRepository.CancelarVagaRecrutamento(vagaExistente, codColaborador, StatusVagaRecrutamento.Cancelada.ToInt());
            return new ApiGenericResult<bool> { Sucesso = true, Mensagem = "Vaga cancelada com sucesso.", Retorno = true };
        }

        public async Task<ApiGenericResult<IEnumerable<VagaRecrutamentoDTO>>> ListarVagasRecrutamento(int limite, int cursor, string busca = null, List<int> status = null, int? orgId = null, string dataInicio = null, string dataFim = null)
        {
            if (String.IsNullOrEmpty(dataInicio))
                dataInicio = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");

            dataFim = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");

            IEnumerable<VagaRecrutamentoDTO> vagas;
            var vagasCount = 0;

            // Converter lista de status para string (separado por vírgula)
            string statusString = status != null && status.Any() ? string.Join(",", status) : null;

            // Obter CPF do usuário logado
            var usuarioLogado = _aspNetUser.GetUsuarioLogado();
            var cpf = usuarioLogado?.Cpf;
            var orgIdUsuario = orgId ?? usuarioLogado?.OrgId ?? 0;

            // Validar acesso e obter clientes permitidos
            List<string> clientesPermitidos = null;
            if (orgIdUsuario > 0 && !string.IsNullOrEmpty(cpf))
            {
                clientesPermitidos = await _vagaValidatorService.ValidarAcessoEListarClientesPermitidos(cpf, orgIdUsuario);
            }

            if (orgId != null)
                if (OrgsQueSaoBancoDeTalentos(orgId))
                {
                    vagas = await _vagaFourmakersRepository.ListarVagasRecrutamento(limite, cursor, busca, statusString, dataInicio, dataFim, clientesPermitidos);
                    vagas = await BuscarQuantidadeCandidatosPorEstagio(vagas);
                    return new ApiGenericResult<IEnumerable<VagaRecrutamentoDTO>> { Sucesso = true, Retorno = vagas };
                }

            vagas = await _vagaFourmakersRepository.ListarVagasRecrutamentoPorOrg(limite, cursor, busca, statusString, orgId, dataInicio, dataFim, clientesPermitidos);
            vagas = await BuscarQuantidadeCandidatosPorEstagio(vagas);
            return new ApiGenericResult<IEnumerable<VagaRecrutamentoDTO>> { Sucesso = true, Retorno = vagas };
        }

        public async Task<int> CountVagasRecrutamento(int? orgId)
        {
            if (orgId != null)
                if (OrgsQueSaoBancoDeTalentos(orgId))
                    return await _vagaFourmakersRepository.CountVagasRecrutamento();

            return await _vagaFourmakersRepository.CountVagasRecrutamentoPorOrg(orgId);
        }

        public async Task<ContadorVagasPorStatusResultDTO> CountVagasRecrutamentoPorStatus(int? orgId, List<int> status = null, string dataInicio = null, string dataFim = null)
        {
            // Aplicar valores padrão para dataInicio e dataFim, igual à listagem
            if (String.IsNullOrEmpty(dataInicio))
                dataInicio = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");

            if (String.IsNullOrEmpty(dataFim))
                dataFim = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");

            // Converter lista de status para string (separado por vírgula)
            string statusString = status != null && status.Any() ? string.Join(",", status) : null;

            // Obter CPF do usuário logado
            var usuarioLogado = _aspNetUser.GetUsuarioLogado();
            var cpf = usuarioLogado?.Cpf;
            var orgIdUsuario = orgId ?? usuarioLogado?.OrgId ?? 0;

            // Validar acesso e obter clientes permitidos
            List<string> clientesPermitidos = null;
            if (orgIdUsuario > 0 && !string.IsNullOrEmpty(cpf))
            {
                clientesPermitidos = await _vagaValidatorService.ValidarAcessoEListarClientesPermitidos(cpf, orgIdUsuario);
            }

            if (orgId != null)
                if (OrgsQueSaoBancoDeTalentos(orgId))
                    return await _vagaFourmakersRepository.CountVagasRecrutamentoPorStatus(statusString, dataInicio, dataFim, clientesPermitidos);

            return await _vagaFourmakersRepository.CountVagasRecrutamentoPorStatusPorOrg(orgId, statusString, dataInicio, dataFim, clientesPermitidos);
        }
        
        public async Task<int> CountInscritoVagas(int orgId, string codColaborador)
        {
            return await _candidaturaRepository.CountInscritoVagas(orgId, codColaborador);
        }
        private async Task<IEnumerable<VagaRecrutamentoDTO>> BuscarQuantidadeCandidatosPorEstagio(IEnumerable<VagaRecrutamentoDTO> vagas)
        {
            var vagasList = vagas.ToList();
            if (!vagasList.Any())
                return vagasList;

            try
            {
                // Buscar todos os dados de uma vez usando os IDs das vagas
                var vagaIds = vagasList.Select(v => v.Id).ToList();
                var resultadosPorVaga = await _candidaturaRepository.ObterQuantidadeCandidatosPorEstagioMultiplasVagas(vagaIds);

                // Popular cada vaga com seus resultados
                foreach (var vaga in vagasList)
                {
                    if (resultadosPorVaga.TryGetValue(vaga.Id, out var quantidadePorEstagio))
                    {
                        vaga.QuantidadeCandidatosPorEstagio = quantidadePorEstagio;
                    }
                    else
                    {
                        vaga.QuantidadeCandidatosPorEstagio = new List<QuantidadeCandidatosPorEstagioDTO>();
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Log($"Erro ao buscar quantidade de candidatos por estágio: {ex.Message}", LevelsEnum.Error);
                // Em caso de erro, inicializar lista vazia para todas as vagas
                foreach (var vaga in vagasList)
                {
                    vaga.QuantidadeCandidatosPorEstagio = new List<QuantidadeCandidatosPorEstagioDTO>();
                }
            }

            return vagasList;
        }

        private static bool OrgsQueSaoBancoDeTalentos(int? orgId)
        {
            return orgId.Value == EnumORG.FMU_7.ToInt()
                || orgId.Value == EnumORG.FOURMAKERS_1.ToInt()
                || orgId.Value == EnumORG.TRIAL_8.ToInt();
        }

        public async Task<ApiGenericResult<VagaRecrutamentoDTO>> ObterVagaPorIdEGestorExternoPerfilIdRecrutamento(Guid idPerfil)
        {
            var vaga = await _vagaFourmakersRepository.ObterVagaPorIdEGestorExternoPerfilIdRecrutamento(idPerfil);
            if (vaga == null)
                return new ApiGenericResult<VagaRecrutamentoDTO> { Sucesso = true, Mensagem = "Vaga não encontrada." };

            return new ApiGenericResult<VagaRecrutamentoDTO> { Sucesso = true, Retorno = vaga };
        }

        public async Task<ApiGenericResult<IEnumerable<VagaRecrutamentoDTO>>> ObterTodasVagasPorIdEGestorExternoPerfilIdRecrutamento(Guid idPerfil)
        {
            var vagas = await _vagaFourmakersRepository.ObterTodasVagasPorIdEGestorExternoPerfilIdRecrutamento(idPerfil);
            if (!vagas.Any())
                return new ApiGenericResult<IEnumerable<VagaRecrutamentoDTO>> { Sucesso = true, Mensagem = "Nenhuma vaga encontrada." };

            return new ApiGenericResult<IEnumerable<VagaRecrutamentoDTO>> { Sucesso = true, Retorno = vagas };
        }

        public async Task AtualizarVagaCriadaAutomaticamenteRecrutamento(VagaRecrutamentoDTO vagaDto, GestorExternoPerfilInput gestorExternoPerfilInput, string cpfRequest)
        {
            var vaga = new VagaVindaDeGestorExternoPerfil
            {
                Codigo = vagaDto.Codigo,
                StatusVagaCod = vagaDto.StatusVagaCod,
                OrgId = gestorExternoPerfilInput.OrgId.ToString(),
                NumeroDeVagas = 1,
                IdPerfilGerador = gestorExternoPerfilInput.Id.ToString(),
                CodGestorExterno = gestorExternoPerfilInput.CodGestorExterno,
                NomePerfil = gestorExternoPerfilInput.NomePerfil,
                CustoPerfil = gestorExternoPerfilInput.CustoPerfil ?? 0,
                RatecardPerfil = gestorExternoPerfilInput.RatecardPerfil ?? 0,
                InformacoesRelevantes = gestorExternoPerfilInput.InformacoesRelevantes,
                PermanenciaId = gestorExternoPerfilInput.PermanenciaId ?? Guid.Empty,
                ModeloTrabalhoId = gestorExternoPerfilInput.ModeloTrabalhoId ?? Guid.Empty,
                ProfissionalLocalidadeId = gestorExternoPerfilInput.ProfissionalLocalidadeId ?? Guid.Empty,
                Cidade = gestorExternoPerfilInput.Cidade,
                Estado = gestorExternoPerfilInput.Estado,
                Cep = gestorExternoPerfilInput.Cep,
                HibridoDias = gestorExternoPerfilInput.HibridoDias,
                GestorExternoPerfilSkills = gestorExternoPerfilInput.GestorExternoPerfilSkills?.Select(s => new VagaSkillRecrutamentoDTO
                {
                    SkillId = s.Skill.Id.ToInt(),
                    SkillDescription = s.Skill.Descricao,
                    SkillNivelId = s.Nivel.Id.ToInt(),
                    TipoSkillId = s.ItemPerfil.Id.ToInt(),
                    Ativo = true,
                    Relevante = s.Relevante
                }).ToList()
            };

            var modelosTrabalho = await _gestaoAlocadosRepository.ListarModelosTrabalhoAsync();
            var modeloTrabalho = modelosTrabalho?.Where(m => m.Id == gestorExternoPerfilInput.ModeloTrabalhoId).FirstOrDefault();

            vaga.ModeloTrabalhoCod = modeloTrabalho?.Codigo;
            vaga.ModeloTrabalhoId = gestorExternoPerfilInput.ModeloTrabalhoId ?? Guid.Empty;
            vaga.PermanenciaId = gestorExternoPerfilInput.PermanenciaId ?? Guid.Empty;
            vaga.Localizacao = modeloTrabalho.Descricao;
            vaga.TipoEmpregoLinkedin = vagaDto.TipoEmpregoLinkedin;
            vaga.NivelExperienciaLinkedin = vagaDto.NivelExperienciaLinkedin;
            vaga.HibridoDias = gestorExternoPerfilInput.HibridoDias;

            await AtualizarVagaRecrutamento(vaga, cpfRequest);
        }

        public async Task CandidatarSe(CandidatarSeRecrutamentoParam candidatarSeRecrutamentoParam)
        {
            await _vagaValidatorService.ValidarCandidatarSeRecrutamento(candidatarSeRecrutamentoParam);

            var usuarioLogado = _aspNetUser.GetUsuarioLogado();

            var vagaId = await _vagaFourmakersRepository.ObterIdVagaPorCodigo(candidatarSeRecrutamentoParam.CodigoVaga);
            if (vagaId == null)
                throw new ApplicationException("Vaga não encontrada.");

            var idsOpcoesContato = await _vagaFourmakersRepository.ListarIdsDasOpcoesContato();
            if (candidatarSeRecrutamentoParam.OpcoesContatoIds.Except(idsOpcoesContato).Any())
                throw new ApplicationException("Opcao contato invalida.");

            if (!string.IsNullOrWhiteSpace(candidatarSeRecrutamentoParam.ModeloTrabalhoId)
                && !await _candidaturaRepository.EstaModeloTrabalhoExiste(candidatarSeRecrutamentoParam.ModeloTrabalhoId))
                throw new ApplicationException("Modelo de trabalho não encontrado");

            if (await _candidaturaRepository.EstaCandidaturaExiste(usuarioLogado.Cpf, vagaId))
                throw new ApplicationException("Ja existe uma candidatura sua para esta vaga.");

            await _candidaturaRepository.CandidatarSeRecrutamento(
                usuarioLogado.Cpf,
                vagaId,
                usuarioLogado.OrgId,
                StatusCandidaturaRecrutamento.InscricaoRegistrada.ToInt(),
                candidatarSeRecrutamentoParam.OpcoesContatoIds,
                candidatarSeRecrutamentoParam.PretencaoSalarial,
                candidatarSeRecrutamentoParam.ModeloTrabalhoId,
                candidatarSeRecrutamentoParam.DisponibilidadeEntrevistaId,
                candidatarSeRecrutamentoParam.QuantidadeDiasPresencial);
        }

        public async Task<ApiGenericResult<IEnumerable<OpcaoContatoDTO>>> ListarOpcoesContato()
        {
            var result = new ApiGenericResult<IEnumerable<OpcaoContatoDTO>>();
            var opcoes = await _vagaFourmakersRepository.ListarOpcoesContato();
            result.Retorno = opcoes;
            result.Sucesso = true;
            return result;
        }

        public async Task<IEnumerable<ListarCandidatosInscritosResult>> ListarCandidatosInscritos(string vagaId, string busca, string dataInicio, string dataFim, int cursor, int limite, string codigoInternoColaboradorLogado, bool? qualificados, int? diasUltimaAlteracao, string? localizacaoCidade, string? localizacaoEstado)
        {
            if (String.IsNullOrEmpty(dataInicio))
                dataInicio = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");

            if (String.IsNullOrEmpty(dataFim))
                dataFim = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");

            var candidatos = await _candidaturaRepository.ListarCandidatosInscritos(vagaId, busca, dataInicio, dataFim, cursor, limite, codigoInternoColaboradorLogado, qualificados, diasUltimaAlteracao, localizacaoCidade, localizacaoEstado);

            // Agrupa candidatos por código para evitar duplicatas
            var candidatosAgrupados = candidatos
                .GroupBy(c => c.Codigo)
                .Select(grupo =>
                {
                    var primeiroCandidato = grupo.First();

                    // Cria a lista de organizações removendo duplicatas
                    var organizacoes = grupo
                        .Where(c => c.OrgId > 0) // Filtra apenas registros com organização válida
                        .Select(c => new OrganizacaoCandidatoDTO
                        {
                            OrgId = c.OrgId,
                            OrgDescricao = c.OrgDescricao,
                            AtivoNaOrg = c.AtivoNaOrg,
                            TipoCadastroBancoDeTalentos = c.TipoCadastroBancoDeTalentos,
                            CodDiretoria = c.CodDiretoria
                        })
                        .GroupBy(o => o.OrgId) // Agrupa por OrgId para remover duplicatas
                        .Select(g => g.First()) // Pega a primeira ocorrência de cada OrgId
                        .ToList();

                    // Define a origem baseada nas organizações
                    string origem = "Banco de Talentos";
                    if (organizacoes.Any())
                        origem = CandidatoOrigemUtil.AtribuirOrigem(organizacoes, origem);

                    // Mantém os campos individuais com os valores da primeira organização (para compatibilidade)
                    if (organizacoes.Any())
                    {
                        primeiroCandidato.OrgId = organizacoes.First().OrgId;
                        primeiroCandidato.OrgDescricao = organizacoes.First().OrgDescricao;
                        primeiroCandidato.AtivoNaOrg = organizacoes.First().AtivoNaOrg;
                    }

                    primeiroCandidato.Organizacoes = organizacoes;
                    primeiroCandidato.Origem = origem;

                    return primeiroCandidato;
                })
                .ToList();

            var vaga = await _vagaFourmakersRepository.ObterVagaRecrutamentoPorId(vagaId);
            if (vaga == null)
                throw new ApplicationException("Vaga não encontrada.");

            var baseRequest = new ScoreSingleCandidateRequest
            {
                HardSkills = vaga.Skills
                    .Where(s => s.TipoSkillId == (int)ItemPerfilEnum.COMPETENCIA)
                    .Select(s => new SkillItem { Nome = s.SkillDescription, Nivel = s.SkillNivelDescription, Obrigatoriedade = s.Relevante ? "obrigatorio" : "desejavel" }).ToList(),
                SoftSkills = vaga.Skills
                    .Where(s => s.TipoSkillId == (int)ItemPerfilEnum.SOFTSKILL)
                    .Select(s => new SkillItem { Nome = s.SkillDescription, Nivel = s.SkillNivelDescription, Obrigatoriedade = s.Relevante ? "obrigatorio" : "desejavel" }).ToList(),
                Metodologias = vaga.Skills
                    .Where(s => s.TipoSkillId == (int)ItemPerfilEnum.METODOLOGIA)
                    .Select(s => new SkillItem { Nome = s.SkillDescription, Nivel = s.SkillNivelDescription, Obrigatoriedade = s.Relevante ? "obrigatorio" : "desejavel" }).ToList(),
                DominiosNegocio = vaga.Skills
                    .Where(s => s.TipoSkillId == (int)ItemPerfilEnum.DOMINIONEGOCIO)
                    .Select(s => new SkillItem { Nome = s.SkillDescription, Nivel = s.SkillNivelDescription, Obrigatoriedade = s.Relevante ? "obrigatorio" : "desejavel" }).ToList(),
                Idiomas = vaga.Skills
                    .Where(s => s.TipoSkillId == (int)ItemPerfilEnum.IDIOMA)
                    .Select(s => new SkillItem { Nome = s.SkillDescription, Nivel = s.SkillNivelDescription, Obrigatoriedade = s.Relevante ? "obrigatorio" : "desejavel" }).ToList(),
                PesoHardSkills = 1,
                PesoSoftSkills = 1,
                PesoMetodologias = 1,
                PesoDominiosNegocio = 1,
                PesoIdiomas = 1,
                PesoDisponibilidades = 1,
                VisibleToOrgIds = new List<int> { vaga.OrgId.ToInt(), EnumORG.FOURMAKERS_1.ToInt(), EnumORG.FMU_7.ToInt() },
                Disponibilidades = new List<DisponibilidadeItem>(),
                NumeroDeCandidatos = 1
            };

            var scoreTasks = candidatosAgrupados.Select(async candidato =>
            {
                try
                {
                    if (candidato.OrigemSrsLinkedin)
                        candidato.NomeCompletoDeQuemCadastrou = "LINKEDIN";

                    var req = JsonConvert.DeserializeObject<ScoreSingleCandidateRequest>(JsonConvert.SerializeObject(baseRequest));
                    req.CodigoInternoColaborador = candidato.Codigo;
                    var scoreResult = await _matchClient.ScoreSingleCandidate(req);
                    candidato.Match = scoreResult?.Match;
                    candidato.RetornoMatch = scoreResult;

                    // Busca outras vagas inscritos
                    candidato.TotalInscritoOutrasVagas = await CountInscritoVagas(vaga.OrgId.ToInt(), candidato.Codigo);
                }
                catch (Exception ex)
                {
                    candidato.Match = null;
                }
                return candidato;
            });

            var candidatosComScore = await Task.WhenAll(scoreTasks);
            return candidatosComScore
                    .OrderByDescending(c => c.Match ?? 0)
                    .ToList();
        }


        public async Task<IEnumerable<ListarCandidaturasPorCodCandidatoResult>> ListarCandidaturasPorCodCandidato(string codColaborador)
        {
            return await _candidaturaRepository.ListarCandidaturasPorCodCandidato(codColaborador);
        }

        public async Task<IEnumerable<StatusVagaRecrutamentoDTO>> ListarStatusVagaRecrutamento(int? orgIdUsuarioLogado)
        {
            return await _vagaFourmakersRepository.ListarStatusVagaRecrutamento(orgIdUsuarioLogado);
        }

        public async Task<ApiGenericResult<bool>> AtualizarOrdemStatusVagaRecrutamento(AtualizarOrdemStatusVagaRecrutamentoParam param, string codColaborador)
        {
            try
            {
                // Validações
                if (param.StatusOrdem == null || !param.StatusOrdem.Any())
                {
                    return new ApiGenericResult<bool> { Sucesso = false, Mensagem = "A lista de status e ordens não pode estar vazia.", Retorno = false };
                }

                // Validar se há ordens repetidas
                var ordens = param.StatusOrdem.Select(s => s.Ordem).ToList();
                var ordensDuplicadas = ordens.GroupBy(o => o).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
                if (ordensDuplicadas.Any())
                {
                    return new ApiGenericResult<bool> { Sucesso = false, Mensagem = $"Não é permitido ter ordens repetidas. Ordens duplicadas encontradas: {string.Join(", ", ordensDuplicadas)}", Retorno = false };
                }

                // Validar se há códigos repetidos
                var codigos = param.StatusOrdem.Select(s => s.Codigo).ToList();
                var codigosDuplicados = codigos.GroupBy(c => c).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
                if (codigosDuplicados.Any())
                {
                    return new ApiGenericResult<bool> { Sucesso = false, Mensagem = $"Não é permitido ter códigos repetidos. Códigos duplicados encontrados: {string.Join(", ", codigosDuplicados)}", Retorno = false };
                }

                // Validar se todos os status estão sendo enviados
                var todosStatusExistentes = await _vagaFourmakersRepository.ListarIdsStatusVagaRecrutamento();
                var codigosEnviados = codigos.ToHashSet();
                var codigosFaltantes = todosStatusExistentes.Where(c => !codigosEnviados.Contains(c)).ToList();
                if (codigosFaltantes.Any())
                {
                    return new ApiGenericResult<bool> { Sucesso = false, Mensagem = $"É necessário enviar todos os status. Status faltando: {string.Join(", ", codigosFaltantes)}", Retorno = false };
                }

                await _vagaFourmakersRepository.AtualizarOrdemStatusVagaRecrutamento(param.StatusOrdem, codColaborador);
                return new ApiGenericResult<bool> { Sucesso = true, Mensagem = "Ordem atualizada com sucesso.", Retorno = true };
            }
            catch (Exception ex)
            {
                return new ApiGenericResult<bool> { Sucesso = false, Mensagem = ex.Message, Retorno = false };
            }
        }

        public async Task<IEnumerable<VagaRecrutamentoDTO>> ListarVagasRecrutamentoPorParent(string vagaIdParent, int orgId)
        {
            if (string.IsNullOrWhiteSpace(vagaIdParent))
                throw new ArgumentException("VagaIdParent é obrigatório.");

            return await _vagaFourmakersRepository.ListarVagasRecrutamentoPorParent(vagaIdParent, orgId);
        }

        public async Task<IEnumerable<VagaRecrutamentoDTO>> ListarVagasRecrutamentoPorParentEmAndamento(string vagaIdParent, int orgId)
        {
            if (string.IsNullOrWhiteSpace(vagaIdParent))
                throw new ArgumentException("VagaIdParent é obrigatório.");

            return await _vagaFourmakersRepository.ListarVagasRecrutamentoPorParentEmAndamento(vagaIdParent, orgId);
        }

        public async Task MudarStatusVaga(MudarStatusVagaRecrutamentoParam mudarStatusVagaRecrutamentoParam, string codColaborador, int orgIdColaborador)
        {
            var codigoStatusLinkedin = 0;
            codigoStatusLinkedin = mudarStatusVagaRecrutamentoParam.CodigoStatus;

            if (mudarStatusVagaRecrutamentoParam.CodigoStatus == StatusVagaRecrutamento.BancoDeTalentos.ToInt())
                throw new ApplicationException("Nao permitido");

            var vaga = await _vagaFourmakersRepository.ObterVagaPorCodigo(mudarStatusVagaRecrutamentoParam.CodigoVaga);
            if (vaga == null)
                throw new ApplicationException("Vaga não encontrada.");

            var possiveisStatus = await _vagaFourmakersRepository.ListarIdsStatusVagaRecrutamento();
            if (!possiveisStatus.Any(m => m == mudarStatusVagaRecrutamentoParam.CodigoStatus))
                throw new ApplicationException("Codigo status invalido.");

            if (vaga.StatusVagaCod == StatusVagaRecrutamento.BancoDeTalentos.ToInt().ToString())
            {
                vaga.IdVagaParent = vaga.Id;
                vaga.Id = Guid.NewGuid().ToString();
                vaga.StatusVagaCod = mudarStatusVagaRecrutamentoParam.CodigoStatus.ToString();
                await _vagaFourmakersRepository.InserirVaga(vaga, codColaborador);
                await _vagaFourmakersRepository.CopiarCandidatosEInformacoesComplementares(vaga.IdVagaParent, vaga.Id, codColaborador);

                if (!String.IsNullOrWhiteSpace(mudarStatusVagaRecrutamentoParam.ComentarioVaga))
                    await _comentarioVagaRepository.AddAsync(new ComentarioVagaDTO
                    {
                        Id = Guid.NewGuid().ToString(),
                        VagaId = Guid.Parse(vaga.Id),
                        CodigoInternoColaborador = codColaborador,
                        DataCriacao = DateTime.Now,
                        DataAlteracao = DateTime.Now,
                        Texto = mudarStatusVagaRecrutamentoParam.ComentarioVaga
                    });
            }
            else
            {
                if (mudarStatusVagaRecrutamentoParam.CodigoStatus == StatusVagaRecrutamento.EmFoco.ToInt())
                    await ValidarEmRefinamentoAsync(vaga);

                await _vagaFourmakersRepository.MudarStatusVaga(mudarStatusVagaRecrutamentoParam.CodigoVaga, mudarStatusVagaRecrutamentoParam.CodigoStatus, codColaborador);
            }

            if (!vaga.SlaContando)
                if (mudarStatusVagaRecrutamentoParam.CodigoStatus == StatusVagaRecrutamento.EmFoco.ToInt())
                    await _vagaFourmakersRepository.IniciarSla(vaga.Id);

            if (codigoStatusLinkedin == StatusVagaRecrutamento.ProcurandoCandidatos.ToInt() && orgIdColaborador == EnumORG.FOURSYS_2.ToInt() && VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.AMBIENTE) == "PRD")
            {
                try
                {
                    _log.Log("Enviando para fila de criacao de vaga no Linkedin", LevelsEnum.Information);
                    await InserirVagaRecrutamentoNoLinkedin(vaga);
                }
                catch (Exception ex)
                {
                    _log.Log("Erro ao enviar para fila de criacao de vaga no Linkedin", LevelsEnum.Error);
                    try
                    {
                        await EnviarEmails(vaga, ex, codColaborador, "Erro na Criação de Vaga LinkedIn");
                    }
                    catch (Exception)
                    {
                        _log.Log("Erro ao enviar email do erro da fila de criacao de vaga no Linkedin", LevelsEnum.Error);
                    }
                }
            }

            if ((codigoStatusLinkedin == StatusVagaRecrutamento.Cancelada.ToInt() 
                || codigoStatusLinkedin == StatusVagaRecrutamento.Contratacao.ToInt() 
                || codigoStatusLinkedin == StatusVagaRecrutamento.Perdida.ToInt()) 
                && orgIdColaborador == EnumORG.FOURSYS_2.ToInt())
            {
                try
                {
                    _log.Log("Cancelar para fila de criacao de vaga no Linkedin", LevelsEnum.Information);
                    await CancelarVagaRecrutamentoNoLinkedin(vaga.Id);
                }
                catch (Exception ex)
                {
                    _log.Log("Erro ao Cancelar para fila de criacao de vaga no Linkedin", LevelsEnum.Error);
                    try
                    {
                        await EnviarEmails(vaga, ex, codColaborador, "Erro no Cancelamento de Vaga LinkedIn");
                    }
                    catch (Exception)
                    {
                        _log.Log("Erro ao Cancelar email do erro da fila de criacao de vaga no Linkedin", LevelsEnum.Error);
                    }
                }
            }

            // Classificar vaga após mudança de status (apenas se não houver classificação)
            try
            {
                var existeClassificacao = await _classificacaoRepository.ExisteClassificacaoVaga(vaga.Id);
                if (!existeClassificacao)
                    await _classificacaoService.AtualizarClassificacaoVagaAsync(vaga);
            }
            catch (Exception ex)
            {
                _log.Log($"Erro ao classificar vaga após mudança de status: {ex}", LevelsEnum.Error);
            }
        }

        private async Task CancelarVagaRecrutamentoNoLinkedin(string id)
        {
            _log.Log("Iniciando cancelamento da vaga no LinkedIn via Azure Logic Apps", LevelsEnum.Information);

            var accessToken = await ObterTokenOAuthLinkedin();
            if (string.IsNullOrEmpty(accessToken))
                throw new Exception("Não foi possível obter o token de acesso OAuth para o LinkedIn");

            var requestBody = new
            {
                id_vaga = id
            };

            var resultado = await EnviarCancelamentoParaAzureLogicApps(requestBody, accessToken);

            _log.Log($"Vaga cancelada com sucesso no LinkedIn. Resultado: {resultado}", LevelsEnum.Information);
        }

        private async Task ValidarEmRefinamentoAsync(VagaRecrutamentoDTO vaga)
        {
            await _vagaValidatorService.ValidaMudancaVagaParaEmRefinamento(vaga);
        }

        public async Task GravarPerdaVaga(int codigoVaga, Guid? idMotivoPerda, string comentario, string codColaborador, int orgIdColaborador)
        {
            var vagaExiste = await _vagaFourmakersRepository.EstaVagaExiste(codigoVaga);
            if (!vagaExiste)
                throw new ApplicationException("Vaga não encontrada.");

            var motivosPerda = await _vagaFourmakersRepository.ListarMotivosPerdaVaga();
            if (!motivosPerda.Any(m => m.Id == idMotivoPerda.Value))
                throw new ApplicationException("Id do motivo de perda inválido.");

            await MudarStatusVaga(new MudarStatusVagaRecrutamentoParam
            {
                CodigoVaga = codigoVaga,
                CodigoStatus = StatusVagaRecrutamento.Perdida.ToInt(),
                ComentarioVaga = comentario
            }, codColaborador, orgIdColaborador);

            await _vagaFourmakersRepository.AtualizarMotivoPerdaVaga(codigoVaga, idMotivoPerda.Value);
        }

        public async Task<(string, string)> MudarStatusCandidatura(string idCandidatura, int codigoStatus, string codColaborador, string comentario = null)
        {
            var candidatura = await _candidaturaRepository.ObterCandidaturaPorId(idCandidatura);
            if (candidatura == null)
                throw new ApplicationException("Candidatura não encontrada.");

            var vaga = await _repository.ObterVagaRecrutamentoPorId(candidatura.IdVaga) ?? throw new ApplicationException("Vaga não encontrada.");

            // Validação de status válido (validação de sistema, não de negócio)
            var possiveisStatus = await _candidaturaRepository.ListarIdsStatusCandidaturaRecrutamento();
            if (!possiveisStatus.Any(m => m == codigoStatus))
                throw new ApplicationException("Codigo status invalido.");

            var statusAnteriorId = candidatura.StatusId.ToInt();
            var msgRetorno = "Sucesso";

            var context = new MudancaStatusCandidaturaContext
            {
                Candidatura = candidatura,
                Vaga = vaga,
                NovoStatusId = codigoStatus,
                StatusAnteriorId = statusAnteriorId,
                CodColaborador = codColaborador,
                Comentario = comentario,
                MensagemRetorno = msgRetorno
            };

            var validacoes = _strategyFactory.ObterValidacoes(codigoStatus);
            foreach (var validacao in validacoes)
            {
                var mensagemErro = await validacao.ValidarAsync(context);
                if (!string.IsNullOrWhiteSpace(mensagemErro))
                    throw new ApplicationException(mensagemErro);
            }

            var comentarioId = await _candidaturaRepository.MudarStatusCandidatura(idCandidatura, codigoStatus, codColaborador, comentario);
            context.ComentarioId = comentarioId;

            context.Vaga.QuantidadeCandidatosPorEstagio = (await _candidaturaRepository.ObterQuantidadeCandidatosPorEstagio(vaga.Id)).ToList();

            var estrategias = _strategyFactory.ObterEstrategias(statusAnteriorId, codigoStatus, vaga);
            foreach (var estrategia in estrategias)
            {
                var mensagemEstrategia = await estrategia.ExecutarAsync(context);
                if (!string.IsNullOrWhiteSpace(mensagemEstrategia))
                {
                    msgRetorno = string.IsNullOrWhiteSpace(msgRetorno) || msgRetorno == "Sucesso"
                        ? mensagemEstrategia
                        : msgRetorno + "\n" + mensagemEstrategia;
                }
            }

            return (comentarioId, msgRetorno);
        }

        private string ObterIniciaisNome(string nomeCompleto)
        {
            if (string.IsNullOrWhiteSpace(nomeCompleto))
                return "";

            var nomes = nomeCompleto.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (nomes.Length == 0)
                return "";

            if (nomes.Length == 1)
                return nomes[0].Substring(0, 1).ToUpper();

            // Pegar todas as iniciais separadas por ponto
            var iniciais = nomes.Select(nome => nome.Substring(0, 1).ToUpper());

            return string.Join(".", iniciais);
        }

        private async Task<Dictionary<string, string>> ObterLinksCurriculos(List<string> codigosCandidatos, int orgId, string token)
        {
            var linksCurriculo = new Dictionary<string, string>();

            if (codigosCandidatos == null || !codigosCandidatos.Any())
                return linksCurriculo;

            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", token);
                    httpClient.DefaultRequestHeaders.Add("x-data-source", VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.AMBIENTE) == "PRD" ? "live" : "homolog");

                    var requestBody = new
                    {
                        ambiente = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.AMBIENTE) == "PRD" ? "producao" : "dev",
                        cpfs = codigosCandidatos,
                        orgid = orgId,
                        emails = new string[0],
                        parametro = 3,
                        assunto = ""
                    };

                    var json = System.Text.Json.JsonSerializer.Serialize(requestBody);
                    var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                    var response = await httpClient.PostAsync("https://xare-axod-hky2.b2.xano.io/api:MLkFdmWD/mycv/addCvEmMassa", content);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        _log.Log($"Resposta da API de currículos: {responseContent}", LevelsEnum.Information);

                        var responseData = System.Text.Json.JsonSerializer.Deserialize<CurriculoXanoResponseDTO>(responseContent);

                        if (responseData?.Colaboradores != null && responseData.Colaboradores.Any())
                        {
                            // Mapear colaboradores retornados pelos códigos enviados (assumindo ordem)
                            for (int i = 0; i < responseData.Colaboradores.Count && i < codigosCandidatos.Count; i++)
                            {
                                var colaborador = responseData.Colaboradores[i];
                                var codigoCandidato = codigosCandidatos[i];

                                if (!string.IsNullOrWhiteSpace(colaborador.Link))
                                {
                                    linksCurriculo[codigoCandidato] = colaborador.Link;
                                    _log.Log($"Link de currículo obtido para {codigoCandidato}: {colaborador.Link}", LevelsEnum.Information);
                                }
                            }

                            _log.Log($"Total de links de currículo obtidos: {linksCurriculo.Count}", LevelsEnum.Information);
                        }
                        else
                        {
                            _log.Log("Lista de colaboradores não encontrada ou vazia na resposta da API", LevelsEnum.Warning);
                        }
                    }
                    else
                    {
                        _log.Log($"Erro ao obter links de currículo: {response.StatusCode} - {response.ReasonPhrase}", LevelsEnum.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Log($"Erro ao obter links de currículo: {ex.Message}", LevelsEnum.Error);
            }

            return linksCurriculo;
        }

        private async Task EnviarEmailParaGestoresExternos(CandidaturaRecrutamentoDTO candidatura, string codColaborador)
        {
            try
            {
                // Buscar emails de análise de gestor para a vaga
                var emails = await _vagaFourmakersRepository.ObterEmailsAnaliseGestorPorVagaId(candidatura.IdVaga);

                if (emails == null || !emails.Any())
                {
                    _log.Log($"Nenhum email de análise de gestor encontrado para a vaga {candidatura.IdVaga}", LevelsEnum.Warning);
                    return;
                }

                // Buscar informações completas da vaga
                var vaga = await _vagaFourmakersRepository.ObterVagaRecrutamentoPorId(candidatura.IdVaga);
                if (vaga == null)
                {
                    _log.Log($"Vaga não encontrada: {candidatura.IdVaga}", LevelsEnum.Error);
                    return;
                }

                // Buscar candidatos em análise de gestor
                var inscritos = await _candidaturaRepository.ListarCandidatosInscritos(candidatura.IdVaga, null, DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd"), DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"), 0, 100, codColaborador, null, null, null, null);
                var inscritosAnaliseGestor = inscritos.Where(m => m.IdStatusCandidatura == StatusCandidaturaRecrutamento.AnaliseDoCvPeloGestor.ToInt().ToString());
                var inscritosOutrosStatus = inscritos.Except(inscritosAnaliseGestor).ToList();

                // Buscar candidatos que já passaram pelo status AnaliseGestor (código 4)
                var codigosOutrosStatus = inscritosOutrosStatus.Select(c => c.Codigo).ToList();

                // Obter links de currículo (Xano) para candidatos em análise e já enviados
                var cpfsAnaliseGestor = inscritosAnaliseGestor.Select(c => c.Codigo).Distinct().ToList();
                var cpfsJaEnviados = inscritosOutrosStatus.Select(c => c.Codigo).Distinct().ToList();
                var linksCurriculoAnalise = await ObterLinksCurriculos(cpfsAnaliseGestor, _usuarioLogado.OrgId, _usuarioLogado.Token);
                var linksCurriculoJaEnviados = await ObterLinksCurriculos(cpfsJaEnviados, _usuarioLogado.OrgId, _usuarioLogado.Token);
                var candidatosQuePassaramPorAnaliseGestor = new List<CandidatoQuePassouPorAnaliseGestorDTO>();

                foreach (var inscritoOutroStatus in inscritosOutrosStatus)
                {
                    var candidato = await _vagaFourmakersRepository.ObterCandidatosQuePassaramPorAnaliseGestor(inscritoOutroStatus.Codigo, inscritoOutroStatus.IdCandidatura);
                    if (candidato is not null)
                        candidatosQuePassaramPorAnaliseGestor.Add(candidato);
                }

                // Montar assunto e mensagem do email
                var assunto = $"Candidatos para Análise - {vaga.Titulo}";

                // Montar descrição da vaga
                var descricaoVaga = $"{vaga.Cargo}";

                // Montar lista de skills da vaga
                var skillsVaga = "";
                if (vaga.Skills != null && vaga.Skills.Any())
                {
                    var skillsList = vaga.Skills.Where(s => s.Ativo)
                        .Select(s => $"{s.SkillDescription} ({s.SkillNivelDescription})")
                        .ToList();
                    skillsVaga = string.Join("<br/>", skillsList);
                }
                else
                {
                    skillsVaga = "Nenhuma skill definida";
                }

                // Montar tabela de candidatos em análise
                var candidatosHtml = "";
                if (inscritosAnaliseGestor.Any())
                {
                    foreach (var candidato in inscritosAnaliseGestor)
                    {
                        var linkCv = linksCurriculoAnalise != null && linksCurriculoAnalise.TryGetValue(candidato.Codigo, out var urlCv) && !string.IsNullOrWhiteSpace(urlCv)
                            ? $"<a href='{urlCv}' target='_blank' style='color:#0d6efd;text-decoration:none;'>Abrir</a>"
                            : "N/A";

                        var iniciaisNome = ObterIniciaisNome(candidato.Nome);

                        candidatosHtml += $@"
                            <tr>
                                <td>{candidato.Candidatura:dd/MM/yyyy}</td>
                                <td>{iniciaisNome}</td>
                                <td>{linkCv}</td>
                                <td class='status-cell-analysis'>Aguardando análise</td>
                            </tr>";
                    }
                }

                // Montar tabela de candidatos que já passaram por análise de gestor
                var candidatosJaEnviadosHtml = "";
                if (candidatosQuePassaramPorAnaliseGestor.Any())
                {
                    foreach (var candidato in candidatosQuePassaramPorAnaliseGestor)
                    {
                        var linkCv = linksCurriculoJaEnviados != null && linksCurriculoJaEnviados.TryGetValue(candidato.CodigoColaborador, out var urlCv) && !string.IsNullOrWhiteSpace(urlCv)
                            ? $"<a href='{urlCv}' target='_blank' style='color:#0d6efd;text-decoration:none;'>Abrir</a>"
                            : "N/A";

                        var iniciaisNome = ObterIniciaisNome(candidato.NomeCandidato);
                        var isDeclined = candidato.StatusAtual.ToLower().Contains("reprovado") ||
                                        candidato.StatusAtual.ToLower().Contains("declinou");

                        candidatosJaEnviadosHtml += $@"
                            <tr>
                                <td>{DateTime.Parse(candidato.DataCandidatura):dd/MM/yyyy}</td>
                                <td class='{(isDeclined ? "declined" : "")}'>{iniciaisNome}</td>
                                <td>{linkCv}</td>
                                <td class='{(isDeclined ? "status-cell-declined" : "status-cell-analysis")}'>{candidato.StatusAtual}</td>
                            </tr>";
                    }
                }

                var mensagem = $@"
                    <!DOCTYPE html>
                    <html lang='pt-BR'>
                    <head>
                        <meta charset='UTF-8'>
                        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                        <title>Relatório de Candidatos - {vaga.Titulo}</title>
                        <style>
                            body {{
                                font-family: Arial, sans-serif;
                                margin: 0;
                                padding: 0;
                                color: #333;
                            }}
                            .container {{
                                max-width: 800px;
                                margin: 0 auto;
                                padding: 20px;
                            }}
                            .header-footer {{
                                width: 100%;
                                max-width: 800px;
                                margin: 0 auto;
                            }}
                            .text-section {{
                                margin-bottom: 20px;
                                line-height: 1.5;
                            }}
                            table {{
                                width: 100%;
                                border-collapse: collapse;
                                margin-bottom: 20px;
                            }}
                            th, td {{
                                border: 1px solid #ddd;
                                padding: 8px;
                                text-align: left;
                            }}
                            .header-table th {{
                                background-color: #2c3e50;
                                color: white;
                                text-align: center;
                            }}
                            .header-table th:nth-child(5),
                            .header-table th:nth-child(6) {{
                                background-color: #e67e22;
                            }}
                            .data-table th {{
                                background-color: #2c3e50;
                                color: white;
                                text-align: center;
                            }}
                            .data-table .status-header {{
                                background-color: #e74c3c;
                            }}
                            .data-table .declined {{
                                color: #e74c3c;
                                font-weight: bold;
                            }}
                            .data-table .status-cell-declined {{
                                background-color: #e74c3c;
                                color: white;
                                text-align: center;
                            }}
                            .data-table .status-cell-analysis {{
                                text-align: center;
                            }}
                            .requirements-cell {{
                                padding-left: 20px;
                            }}
                            .empty-row td {{
                                height: 30px;
                                background-color: white;
                            }}
                            .footer-text {{
                                margin-top: 20px;
                            }}
                        </style>
                    </head>
                    <body>
                        <!-- Header Image -->
                        <div class='header-footer' style='text-align: center; margin-bottom: 20px;'>
                            <img src='https://fsys2-public.s3.amazonaws.com/HeaderEmail.png' alt='Header' style='width: 100%; height: auto;' />
                        </div>
                        
                        <div class='container'>
                            
                            <div class='text-section'>
                                <h1>Olá, tudo bem?</h1>
                                <p>É um grande prazer atuar com <strong>{vaga.NomeCliente ?? "o cliente"}</strong> nessa demanda.</p>
                                <p>A equipe de recrutamento e seleção da <strong>Foursys</strong> recebeu missão a busca de candidatos com o perfil.</p>
                            </div>

                            <div class='vaga-info'>
                                <p><strong>Perfil da vaga:</strong><br>
                                    {vaga.Titulo}
                                </p>

                                <p><strong>Requisitos:</strong><br>
                                    {skillsVaga}
                                </p>

                                <p><strong>Quantidade de CVs enviados:</strong><br>
                                    {(inscritosAnaliseGestor.Count() + candidatosQuePassaramPorAnaliseGestor.Count())}
                                </p>
                            </div>

                            <div class='text-section'>
                                <p>Seguem currículos em anexos para apreciação:</p>
                            </div>

                            <table class='data-table'>
                                <thead>
                                    <tr>
                                        <th style='width: 20%;'>Data Candidatura</th>
                                        <th style='width: 30%;'>Nome do Candidato</th>
                                        <th style='width: 15%;'>Curriculo</th>
                                        <th class='status-header' style='width: 20%;'>Status</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {candidatosHtml}
                                    {candidatosJaEnviadosHtml}
                                </tbody>
                            </table>

                            <div class='footer-text'>
                                <p>Conte conosco para qualquer dúvida e/ou esclarecimento.</p>
                                <p>Estamos à disposição.</p>
                                <p><strong>Obrigado!</strong></p>
                            </div>
                        </div>
                        
                        <!-- Footer Image -->
                        <div class='header-footer' style='text-align: center; margin-top: 20px;'>
                            <img src='https://fsys2-public.s3.amazonaws.com/FooterEmail.png' alt='Footer' style='width: 100%; height: auto;' />
                        </div>
                    </body>
                    </html>";

                // Enviar email para cada gestor
                foreach (var email in emails)
                {
                    if (!string.IsNullOrWhiteSpace(email))
                    {
                        await _templateRepository.RegistraTemplateEmailAsync(_usuarioLogado.OrgId, email, assunto, mensagem);

                        _log.Log($"Email enviado para gestor: {email} - Vaga: {vaga.Codigo} - Candidatos: {inscritosAnaliseGestor.Count()}", LevelsEnum.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Log($"Erro ao enviar email para gestores externos - Candidatura: {candidatura.Id} - Erro: {ex.Message}", LevelsEnum.Error);
                throw;
            }
        }

        public async Task<IEnumerable<StatusCandidaturaRecrutamentoDTO>> ListarStatusCandidaturaRecrutamento()
        {
            return await _candidaturaRepository.ListarStatusCandidaturaRecrutamento();
        }

        public async Task<IEnumerable<MotivoDescandidatarDTO>> ListarMotivosDescandidatura()
        {
            return await _candidaturaRepository.ListarMotivosDescandidatura();
        }

        public async Task<ApiGenericResult<IEnumerable<MotivoPerdaVagaDTO>>> ListarMotivosPerdaVaga()
        {
            var result = new ApiGenericResult<IEnumerable<MotivoPerdaVagaDTO>>();
            var motivos = await _vagaFourmakersRepository.ListarMotivosPerdaVaga();
            result.Retorno = motivos;
            result.Sucesso = true;
            return result;
        }

        public async Task DescandidatarSe(string idCandidatura, string idMotivoDescandidatura)
        {
            var motivos = (await _candidaturaRepository.ListarMotivosDescandidatura()).Select(m => m.Id.ToString());
            if (!motivos.Any(m => m == idMotivoDescandidatura))
                throw new ApplicationException("Id Motivo invalido");

            if (!await _candidaturaRepository.EstaCandidaturaExiste(idCandidatura))
                throw new ApplicationException("Id Candidatura invalido");

            await _candidaturaRepository.DescandidatarSe(idCandidatura, idMotivoDescandidatura);
        }

        public async Task CandidatarOutraPessoa(CandidatarOutraPessoaRecrutamentoParam candidatarOutraPessoaRecrutamentoParam)
        {
            var usuarioLogado = _aspNetUser.GetUsuarioLogado();

            var vagaId = await _vagaFourmakersRepository.ObterIdVagaPorCodigo(candidatarOutraPessoaRecrutamentoParam.CodigoVaga);
            if (vagaId == null)
                throw new ApplicationException("Vaga não encontrada.");

            if (await _candidaturaRepository.EstaCandidaturaExiste(candidatarOutraPessoaRecrutamentoParam.CodigoColaborador, vagaId))
                throw new ApplicationException("Ja existe uma candidatura desta pessoa para esta vaga.");

            await _candidaturaRepository.CandidatarOutraPessoa(
                candidatarOutraPessoaRecrutamentoParam.CodigoColaborador,
                vagaId,
                usuarioLogado.OrgId,
                StatusCandidaturaRecrutamento.InscricaoRegistrada.ToInt(),
                usuarioLogado.Cpf,
                candidatarOutraPessoaRecrutamentoParam.PretencaoSalarial,
                candidatarOutraPessoaRecrutamentoParam.ModeloTrabalhoId,
                candidatarOutraPessoaRecrutamentoParam.DisponibilidadeEntrevistaId,
                candidatarOutraPessoaRecrutamentoParam.QuantidadeDiasPresencial);
        }

        private string RemoverAcentos(string texto)
        {
            if (string.IsNullOrEmpty(texto))
                return texto;

            var normalizedString = texto.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(c);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

        public async Task<IEnumerable<ListarCandidatosAderentesResult>> ListarCandidatosAderentes(ListarCandidatosAderentesParam param)
        {
            var vaga = await _vagaFourmakersRepository.ObterVagaRecrutamentoPorId(param.VagaId);
            if (vaga == null)
                throw new ApplicationException("Vaga não encontrada.");

            var candidatosMatchRequest = await MapearObjetoDeRequisicaoMatch(vaga, _usuarioLogado.OrgId, param);

            _log.Log("MATCH - Envio", LevelsEnum.Information);
            _log.Log(JsonConvert.SerializeObject(candidatosMatchRequest), LevelsEnum.Information);

            var codigoInternoColaborador = !string.IsNullOrEmpty(_usuarioLogado.CodColaborador) ? _usuarioLogado.CodColaborador : _usuarioLogado.Cpf;
            var logContext = new DataTransferObject.Domain.Labs.RankCandidatesIdsLogContext
            {
                OrgId = _usuarioLogado.OrgId,
                VagaId = param.VagaId,
                CodigoInternoColaborador = codigoInternoColaborador
            };
            var rankResult = await _matchService.RankCandidatesIds(candidatosMatchRequest, logContext);
            var candidatosMatchResponse = rankResult.Candidates;

            var retorno = new List<ListarCandidatosAderentesResult>();

            foreach (var candidato in candidatosMatchResponse)
            {
                try
                {
                    var dadosColaborador = await _buscaColaboradorRepository.GetColaboradorBasicoPorCodigo(candidato.CodigoInternoColaborador);
                    var ehCandidato = false;

                    if (dadosColaborador is not null)
                        if (await _candidaturaRepository.EstaCandidaturaExiste(candidato.CodigoInternoColaborador, param.VagaId))
                            ehCandidato = true;

                    // Buscar organizações do colaborador
                    var organizacoes = await _buscaColaboradorRepository.GetOrganizacoesColaborador(candidato.CodigoInternoColaborador);

                    // Definir organização principal (prioridade para organização ativa)
                    var orgPrincipal = organizacoes.FirstOrDefault(o => o.AtivoNaOrg == true) ?? organizacoes.FirstOrDefault();

                    // Cria a lista de organizações removendo duplicatas
                    organizacoes = organizacoes
                        .Where(c => c.OrgId > 0) // Filtra apenas registros com organização válida
                        .Select(c => new OrganizacaoCandidatoDTO
                        {
                            OrgId = c.OrgId,
                            OrgDescricao = c.OrgDescricao,
                            AtivoNaOrg = c.AtivoNaOrg,
                            TipoCadastroBancoDeTalentos = c.TipoCadastroBancoDeTalentos,
                            OrgIdTbBancoTalento = c.OrgIdTbBancoTalento,
                            OrgIdTbColaboradorOrg = c.OrgIdTbColaboradorOrg
                        })
                        .GroupBy(o => o.OrgId) // Agrupa por OrgId para remover duplicatas
                        .Select(g => g.First()) // Pega a primeira ocorrência de cada OrgId
                        .ToList();

                    var candidatoAderente = new ListarCandidatosAderentesResult
                    {
                        Nome = candidato.Nome,
                        Codigo = candidato.CodigoInternoColaborador,
                        PercentualAderencia = candidato.Match,
                        Email = dadosColaborador?.EmailAlternativo,
                        EhCandidato = ehCandidato,
                        RetornoMatch = candidato,
                        Qualificado = dadosColaborador?.Qualificado,
                        OrgId = orgPrincipal?.OrgId ?? 0,
                        OrgDescricao = orgPrincipal?.OrgDescricao ?? "",
                        AtivoNaOrg = orgPrincipal?.AtivoNaOrg,
                        Organizacoes = organizacoes,
                        Origem = candidato.Origem,
                        Comunidade = candidato.Comunidade
                    };

                    retorno.Add(candidatoAderente);
                }
                catch (Exception ex)
                {
                    _log.Log("Erro ao buscar colaborador", LevelsEnum.Error);
                    _log.Log(ex.Message, LevelsEnum.Error);
                    retorno.Add(new ListarCandidatosAderentesResult { Nome = candidato.Nome, Codigo = candidato.CodigoInternoColaborador, PercentualAderencia = candidato.Match, Comunidade = candidato.Comunidade });
                }
            }

            return retorno;
        }

        private async Task<CandidatosMatchRequestIds> MapearObjetoDeRequisicaoMatch(VagaRecrutamentoDTO vaga, int orgId, ListarCandidatosAderentesParam param)
        {
            // Buscar categoria da vaga se não foram informadas comunidades
            List<string> comunidades = param.Comunidades;
            if (comunidades == null || !comunidades.Any())
            {
                var categoriaVaga = await _classificacaoRepository.ObterCategoriaVagaPorId(vaga.Id);
                if (!string.IsNullOrWhiteSpace(categoriaVaga))
                {
                    comunidades = new List<string> { categoriaVaga };
                }
                else
                {
                    comunidades = new List<string>();
                }
            }

            var requisicao = new CandidatosMatchRequestIds
            {
                NomeCandidato = param.Busca,
                LocalizacaoCidade = string.IsNullOrWhiteSpace(param.LocalizacaoCidade) ? vaga.Cidade : param.LocalizacaoCidade,
                LocalizacaoEstado = string.IsNullOrWhiteSpace(param.LocalizacaoEstado) ? vaga.Estado : param.LocalizacaoEstado,
                UltimaAtualizacaoPerfil = null,
                DataDisponibilidade = null,
                Origem = await ObterDescricoesOrigensPorIds(param.Origens),
                HardSkills = new List<HabilidadeTecnicaIds>(),
                SoftSkills = new List<HabilidadeComportamentalIds>(),
                Metodologias = new List<DataTransferObject.Domain.Match.MetodologiaIds>(),
                DominiosNegocio = new List<DominioNegocioIds>(),
                Idiomas = new List<DataTransferObject.Domain.Match.IdiomaIds>(),
                Disponibilidades = new List<Disponibilidade>(),

                PesoHardSkills = param.PesoHardSkills,
                PesoSoftSkills = param.PesoSoftSkills,
                PesoDisponibilidades = param.PesoDisponibilidades,
                PesoMetodologias = param.PesoMetodologias,
                PesoDominiosNegocio = param.PesoDominiosNegocio,
                PesoIdiomas = param.PesoIdiomas,
                VisibleToOrgIds = new List<int> { orgId, EnumORG.FOURMAKERS_1.ToInt(), EnumORG.FMU_7.ToInt() },
                NumeroDeCandidatos = param.Limite == 0 ? 100 : param.Limite,
                Comunidades = comunidades
            };

            foreach (var skill in vaga.Skills)
            {
                switch (skill.TipoSkillId)
                {
                    case (int)ItemPerfilEnum.COMPETENCIA:
                        requisicao.HardSkills.Add(new HabilidadeTecnicaIds
                        {
                            Id = skill.SkillId,
                            NivelId = skill.SkillNivelId,
                            Obrigatoriedade = skill.Relevante ? "obrigatorio" : "desejavel"
                        });
                        break;

                    case (int)ItemPerfilEnum.SOFTSKILL:
                        requisicao.SoftSkills.Add(new HabilidadeComportamentalIds
                        {
                            Id = skill.SkillId,
                            NivelId = skill.SkillNivelId,
                            Obrigatoriedade = skill.Relevante ? "obrigatorio" : "desejavel"
                        });
                        break;

                    case (int)ItemPerfilEnum.METODOLOGIA:
                        requisicao.Metodologias.Add(new DataTransferObject.Domain.Match.MetodologiaIds
                        {
                            Id = skill.SkillId,
                            NivelId = skill.SkillNivelId,
                            Obrigatoriedade = skill.Relevante ? "obrigatorio" : "desejavel"
                        });
                        break;

                    case (int)ItemPerfilEnum.DOMINIONEGOCIO:
                        requisicao.DominiosNegocio.Add(new DominioNegocioIds
                        {
                            Id = skill.SkillId,
                            NivelId = skill.SkillNivelId,
                            Obrigatoriedade = skill.Relevante ? "obrigatorio" : "desejavel"
                        });
                        break;

                    case (int)ItemPerfilEnum.IDIOMA:
                        requisicao.Idiomas.Add(new DataTransferObject.Domain.Match.IdiomaIds
                        {
                            Id = skill.SkillId,
                            NivelId = skill.SkillNivelId,
                            Obrigatoriedade = skill.Relevante ? "obrigatorio" : "desejavel"
                        });
                        break;
                }
            }

            return requisicao;
        }
        private async Task<List<string>> ObterDescricoesOrigensPorIds(List<string> origemIds)
        {
            if (origemIds == null || !origemIds.Any())
                return new List<string>();

            var origens = await _buscaColaboradorRepository.ListarOrigensColaborador();
            return origens
                .Where(o => origemIds.Contains(o.Id.ToString()))
                .Select(o => o.Descricao)
                .ToList();
        }

        public async Task<IEnumerable<DisponibilidadeEntrevistaDTO>> ListarDisponibilidadesEntrevista()
        {
            return await _candidaturaRepository.ListarDisponibilidadesEntrevista();
        }

        public async Task<ListarCandidaturasPorCodCandidatoResult> ObterUltimaCandidaturaPorCodColaborador(string codColaborador)
        {
            var candidaturas = await ListarCandidaturasPorCodCandidato(codColaborador);
            var ultimaCandidatura = candidaturas
                .OrderByDescending(c => c.DataCriacao)
                .FirstOrDefault();

            return ultimaCandidatura;
        }

        public async Task InserirInformacoesComplementaresVagaRecrutamento(InserirInformacoesComplementaresVagaRecrutamentoParam param, string codColaboradorLogado)
        {
            if (string.IsNullOrEmpty(param.IdVaga))
                throw new ArgumentException("ID da vaga é obrigatório");

            var vaga = await _vagaFourmakersRepository.ObterVagaRecrutamentoPorId(param.IdVaga);
            if (vaga == null)
                throw new ArgumentException("Vaga não encontrada");

            if (!string.IsNullOrEmpty(param.ColaboradorCodigoInternoColaboradorGestorOrgLogada))
            {
                var colaborador = await _buscaColaboradorRepository.GetColaboradorBasicoPorCodigo(param.ColaboradorCodigoInternoColaboradorGestorOrgLogada);
                if (colaborador == null)
                    throw new ArgumentException("Colaborador gestor não encontrado");
            }

            // Foi pedido por Edvaldo para tirar a obrigatoriedade junto com os outros campos do template.
            //if (!string.IsNullOrEmpty(param.TipoVagaId))
            //{
            //    var tipoVagaExiste = await ValidarTipoVagaExiste(param.TipoVagaId);
            //    if (!tipoVagaExiste)
            //        throw new ArgumentException("Tipo de vaga não encontrado");
            //}

            if (param.TipoContratacaoId.HasValue)
            {
                var tipoContratacaoExiste = await ValidarTipoContratacaoExiste(param.TipoContratacaoId.Value);
                if (!tipoContratacaoExiste)
                    throw new ArgumentException("Tipo de contratação não encontrado");
            }

            if (!string.IsNullOrEmpty(param.UnidadeId))
            {
                var unidadeExiste = await ValidarUnidadeExiste(param.UnidadeId);
                if (!unidadeExiste)
                    throw new ArgumentException("Unidade não encontrada");
            }

            if (param.NumeroDeVagas == 0)
                param.NumeroDeVagas = 1;

            // Validação dos emails de análise de gestor
            if (param.EmailsAnaliseGestor != null && param.EmailsAnaliseGestor.Any())
            {
                foreach (var email in param.EmailsAnaliseGestor)
                {
                    if (string.IsNullOrWhiteSpace(email))
                        continue;

                    // Validação básica de email
                    if (!IsValidEmail(email))
                        throw new ArgumentException($"Email inválido: {email}");
                }
            }

            //try
            //{
            //    _colaboracaoBridgeService.BuscarContatoResponsavel(param.PropostaCrm);
            //}
            //catch (Exception)
            //{
            //    throw new ArgumentException("Proposta não encontrada");
            //}

            await _vagaFourmakersRepository.InserirInformacoesComplementaresVagaRecrutamento(param, vaga, codColaboradorLogado);
        }

        private async Task<bool> ValidarTipoVagaExiste(string tipoVagaId)
        {
            var tiposVaga = await ListarTiposVaga();
            if (tiposVaga.Any(m => m.Id == tipoVagaId))
                return true;

            return false;
        }

        private async Task<bool> ValidarTipoContratacaoExiste(int tipoContratacaoId)
        {
            var tiposContratacao = await ListarTiposContratacao();
            if (tiposContratacao.Any(m => m.Id == tipoContratacaoId))
                return true;

            return false;
        }

        private async Task<bool> ValidarUnidadeExiste(string unidadeId)
        {
            var unidades = await ListarUnidades();
            if (unidades.Any(m => m.Id == unidadeId))
                return true;

            return false;
        }

        public async Task<IEnumerable<TipoVagaDTO>> ListarTiposVaga()
        {
            return await _vagaFourmakersRepository.ListarTiposVaga();
        }

        public async Task<IEnumerable<NivelVagaDTO>> ListarNiveisVaga()
        {
            return await _vagaFourmakersRepository.ListarNiveisVaga();
        }

        public async Task<IEnumerable<TipoContratacaoDTO>> ListarTiposContratacao()
        {
            return await _vagaFourmakersRepository.ListarTiposContratacao();
        }

        public async Task<IEnumerable<UnidadeDTO>> ListarUnidades()
        {
            return await _vagaFourmakersRepository.ListarUnidades();
        }

        public async Task<DataTransferObject.Domain.Vaga.CountCandidatosInscritosResult> CountCandidatosInscritos(string vagaId)
        {
            return await _candidaturaRepository.CountCandidatosInscritos(vagaId);
        }

        public async Task<ObterTotaisInscritosResult> ObterTotaisInscritos(string vagaId)
        {
            try
            {
                var totalCandidatosInscritos = await CountCandidatosInscritos(vagaId);

                return new ObterTotaisInscritosResult
                {
                    TotalCandidatosInscritos = totalCandidatosInscritos.TotalCandidatosInscritos
                };
            }
            catch (Exception ex)
            {
                _log.Log($"Erro ao obter totais de candidatos para vaga {vagaId}: {ex.Message}", LevelsEnum.Error);
                throw;
            }
        }

        public async Task<List<DataTransferObject.Domain.Match.CandidatosMatchResponse>> RankCandidates(DataTransferObject.Domain.Match.CandidatosMatchRequest request)
        {
            try
            {
                _log.Log("MATCH - Envio - RankCandidates", LevelsEnum.Information);
                _log.Log(JsonConvert.SerializeObject(request), LevelsEnum.Information);

                var candidatosMatchResponse = await _matchClient.RankCandidates(request);

                _log.Log($"MATCH - Resultado - Total de candidatos encontrados: {candidatosMatchResponse?.Count() ?? 0}", LevelsEnum.Information);

                return candidatosMatchResponse;
            }
            catch (Exception ex)
            {
                _log.Log($"Erro ao executar RankCandidates: {ex.Message}", LevelsEnum.Error);
                throw new Exception($"Erro ao executar RankCandidates: {ex.Message}");
            }
        }

        public async Task<IEnumerable<TipoEmpregoLinkedin>> ListarTiposEmpregosLinkedin()
        {
            return await _vagaFourmakersRepository.ListarTiposEmpregosLinkedin();
        }

        public async Task<IEnumerable<NivelExperienciaLinkedin>> ListarNiveisExperienciaLinkedin()
        {
            return await _vagaFourmakersRepository.ListarNiveisExperienciaLinkedin();
        }

        public async Task<ApiGenericResult<FileContentResult>> RelatorioVagas(int orgId, DateTime dataInicio, DateTime dataFim)
        {
            var ret = new ApiGenericResult<FileContentResult>();
            try
            {

                var relatorioVagasResult = await _vagaFourmakersRepository.BuscaRelatorioVagas(orgId, dataInicio, dataFim);

                if (!relatorioVagasResult.Any())
                {
                    ret.Mensagem = "Não existem vagas para gerar o arquivo.";
                    ret.Sucesso = false;
                    return ret;
                }

                var fileBytes = ExcelFileUtil.CreateExcelFile(relatorioVagasResult);
                var fileName = "Relatorio_Vagas_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".xlsx";

                ret.Retorno = new FileContentResult(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                ret.Retorno.FileDownloadName = fileName;
                ret.Sucesso = true;
            }
            catch (Exception ex)
            {
                ret.Sucesso = false;
                ret.Mensagem = ex.Message;
            }

            return ret;
        }

        public async Task<ApiGenericResult<FileContentResult>> RelatorioProdutividade(int orgId, DateTime dataInicio, DateTime dataFim)
        {
            var ret = new ApiGenericResult<FileContentResult>();
            try
            {

                var relatorioProdutividadeResult = await _vagaFourmakersRepository.BuscaRelatorioProdutividade(orgId, dataInicio, dataFim);

                if (!relatorioProdutividadeResult.Any())
                {
                    ret.Mensagem = "Não existem registros para gerar o arquivo.";
                    ret.Sucesso = false;
                    return ret;
                }

                var fileBytes = ExcelFileUtil.CreateExcelFile(relatorioProdutividadeResult);
                var fileName = "Relatorio_Produtividade_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".xlsx";

                ret.Retorno = new FileContentResult(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = fileName
                };
                ret.Sucesso = true;
            }
            catch (Exception ex)
            {
                ret.Sucesso = false;
                ret.Mensagem = ex.Message;
            }

            return ret;
        }

        public async Task<ApiGenericResult<FileContentResult>> RelatorioVagasCandidaturas(DateTime dataInicio, DateTime dataFim, int orgIdUsuarioLogado)
        {
            var ret = new ApiGenericResult<FileContentResult>();
            try
            {

                var relatorioProdutividadeResult = await _vagaFourmakersRepository.RelatorioVagasCandidaturas(dataInicio, dataFim, orgIdUsuarioLogado);

                if (!relatorioProdutividadeResult.Any())
                {
                    ret.Mensagem = "Não existem registros para gerar o arquivo.";
                    ret.Sucesso = false;
                    return ret;
                }

                var fileBytes = ExcelFileUtil.CreateExcelFile(relatorioProdutividadeResult);
                var fileName = "Relatorio_Vagas_Candidaturas_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".xlsx";

                ret.Retorno = new FileContentResult(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = fileName
                };
                ret.Sucesso = true;
            }
            catch (Exception ex)
            {
                ret.Sucesso = false;
                ret.Mensagem = ex.Message;
            }

            return ret;
        }

        public async Task<TemplateDescricaoVagaDTO> BuscarTemplateDescricaoVaga(int orgId)
        {
            return await _vagaFourmakersRepository.BuscarTemplateDescricaoVaga(orgId);
        }

        public async Task AdicionarTemplateDescricaoVaga(int orgId, string textoIntroducao, string textoFinalizacao)
        {
            await _vagaFourmakersRepository.AdicionarTemplateDescricaoVaga(orgId, textoIntroducao, textoFinalizacao);
        }

        public async Task AtualizarTemplateDescricaoVaga(int orgId, string textoIntroducao, string textoFinalizacao)
        {
            await _vagaFourmakersRepository.AtualizarTemplateDescricaoVaga(orgId, textoIntroducao, textoFinalizacao);
        }

        public async Task ExcluirTemplateDescricaoVaga(int orgId)
        {
            await _vagaFourmakersRepository.ExcluirTemplateDescricaoVaga(orgId);
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public async Task<ApiGenericResult<string>> AdicionarRecrutadorVaga(string vagaId, string codInternoColaboradorRecrutador)
        {
            var retorno = await _vagaFourmakersRepository.AdicionarRecrutadorVaga(vagaId, codInternoColaboradorRecrutador);

            if (!retorno)
                return new ApiGenericResult<string> { Sucesso = false, Retorno = "Erro ao cadastrar recrutador na vaga." };

            return new ApiGenericResult<string> { Sucesso = true, Retorno = "Recrutador cadastrado com sucesso." };
        }
    }
}