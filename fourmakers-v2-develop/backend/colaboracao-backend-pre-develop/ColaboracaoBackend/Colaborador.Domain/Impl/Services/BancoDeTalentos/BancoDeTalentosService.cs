using ApiClient.Domain.Interfaces;
using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Colaborador.Domain.Interfaces.Services.BancoDeTalentos;
using Core.Domain.Colaborador;
using Core.DomainModel;
using Core.DomainModel.BancoDeTalentos;
using DataTransferObject.Domain.BancoDeTalentos;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Log;
using DataTransferObject.Domain.Match;
using DataTransferObject.Domain.Util.Enum;
using DataTransferObject.Domain.Nivel;
using Core.Domain.Formacao;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataTransferObject.Domain.Vaga;
using System.Threading;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfil;
using DataTransferObject.Domain.Labs.MatchSemantico;
using Competencia.Domain.Enums;
using Labs.Domain.Interfaces;
using Logs.Infra.Attributes;

namespace Colaborador.Domain.Impl.Services.BancoDeTalentos
{
    [LogDomainClass]
    public class BancoDeTalentosService : IBancoDeTalentosService
    {
        private readonly IBancoDeTalentosRepository _bancoDeTalentosRepository;
        private readonly ICandidaturaRepository _candidaturaRepository;
        private readonly IProcessamentoCurriculoLoteRepository _processamentoCurriculoLoteRepository;
        private readonly ILogCore _log;
        private readonly IMatchClient _matchClient;
        private readonly IMatchService _matchService;
        private readonly IExtractorService _extractorService;
        private readonly IFormacaoNivelRepository _formacaoNivelRepository;
        private readonly IBuscaColaboradorRepository _buscaColaboradorRepository;
        private readonly IBancoDeTalentosValidatorService _bancoDeTalentosValidatorService;
        private readonly IClassificacaoService _classificacaoService;

        public BancoDeTalentosService(IBancoDeTalentosRepository bancoDeTalentosRepository, ICandidaturaRepository candidaturaRepository, IProcessamentoCurriculoLoteRepository processamentoCurriculoLoteRepository, ILogCore log, IMatchClient matchClient, IMatchService matchService, IExtractorService extractorService, IFormacaoNivelRepository formacaoNivelRepository, IBuscaColaboradorRepository buscaColaboradorRepository, IBancoDeTalentosValidatorService bancoDeTalentosValidatorService, IClassificacaoService classificacaoService)
        {
            _bancoDeTalentosRepository = bancoDeTalentosRepository;
            _candidaturaRepository = candidaturaRepository;
            _processamentoCurriculoLoteRepository = processamentoCurriculoLoteRepository;
            _log = log;
            _matchClient = matchClient;
            _matchService = matchService;
            _extractorService = extractorService;
            _formacaoNivelRepository = formacaoNivelRepository;
            _buscaColaboradorRepository = buscaColaboradorRepository;
            _bancoDeTalentosValidatorService = bancoDeTalentosValidatorService;
            _classificacaoService = classificacaoService;
        }

        public async Task DeletarBancoDeTalentos(string codInternoColaborador)
        {
            await _bancoDeTalentosRepository.RemoverBancoDeTalentos(codInternoColaborador);
        }

        public async Task<string> InserirBancoDeTalentos(string codInternoColaborador, int orgId, string origem)
        {
            var validar = await _bancoDeTalentosRepository.BuscarBancoDeTalentosPorColaborador(codInternoColaborador);
            if (validar != null)
            {
                throw new Exception("Já existe um banco de talentos com o Código Colaborador fornecido.");
            }
            var result = await _bancoDeTalentosRepository.InserirBancoDeTalentos(codInternoColaborador, orgId, origem);
            return result;
        }

        public async Task<string> InserirBancoDeTalentosFourmakers(string codInternoColaborador, int orgId, string origem, string codigoInternoColaboradorCadastrante, DateTime utcNow, int formaCadastro)
        {
            var validar = await _bancoDeTalentosRepository.BuscarBancoDeTalentosPorColaborador(codInternoColaborador);
            if (validar != null)
            {
                throw new Exception("Já existe um banco de talentos com o Código Colaborador fornecido.");
            }

            var idExterno = codInternoColaborador;

            var result = await _bancoDeTalentosRepository.InserirBancoDeTalentosIdExterno(codInternoColaborador, orgId, origem, idExterno, codigoInternoColaboradorCadastrante, utcNow, formaCadastro);
            return result;
        }

        public async Task<BancoDeTalentoDTO> BuscarBancoDeTalentosPorColaborador(string codInternoColaborador)
        {
            var result = await _bancoDeTalentosRepository.BuscarBancoDeTalentosPorColaborador(codInternoColaborador);
            return result;
        }

        public async Task<string> BuscarNomeCadastrante(string cpf, int orgId)
        {
            return await _bancoDeTalentosRepository.BuscarNomeCadastrante(cpf, orgId);
        }

        public async Task<IEnumerable<BuscarPessoasQueEuCadastrei>> BuscarPessoasQueEuCadastrei(string codInternoColaborador, int orgId, string busca, int cursor, int limite)
        {
            IEnumerable<BuscarPessoasQueEuCadastrei> colaboradores = await _bancoDeTalentosRepository.BuscarPessoasQueEuCadastrei(codInternoColaborador, orgId, busca, cursor, limite);

            foreach (var colaborador in colaboradores)
            {
                var candidaturas = await _candidaturaRepository.ListarCandidaturasPorCodCandidato(colaborador.CodigoInternoColaborador);
                if (candidaturas == null || candidaturas.Count() == 0)
                    continue;

                colaborador.PossuiCandidatura = true;
                colaborador.Candidaturas = candidaturas.Select(m => new CandidaturaComTituloDTO 
                { 
                    IdCandidatura = m.CandidaturaId, 
                    TituloVaga = m.Titulo,
                    CodigoCliente = m.CodigoCliente,
                    NomeCliente = m.NomeCliente,
                    CodigoGestor = m.CodigoGestor,
                    NomeGestor = m.NomeGestor
                }).ToList();
            }

            return colaboradores;
        }

        public async Task<IEnumerable<ProcessamentoCurriculoLoteDTO>> BuscarMeusLotes(string codInternoColaborador, int orgId)
        {
            return await _processamentoCurriculoLoteRepository.BuscarMeusLotes(codInternoColaborador, orgId);
        }

        public async Task<ApiGenericResult<BuscarInformacoesLoteResult>> BuscarInformacoesLote(string idLote)
        {
            var result = new ApiGenericResult<BuscarInformacoesLoteResult>();
            result.Retorno = new BuscarInformacoesLoteResult();

            var lote = _processamentoCurriculoLoteRepository.BuscarPorId(idLote);
            if (lote is null)
                throw new ArgumentNullException($"Nao existe lote sob este id {idLote}");

            result.Retorno.Lote = lote;

            result.Retorno.PessoasCadastradasNesteLote = await _processamentoCurriculoLoteRepository.BuscarPessoasCadastradasDesteLote(idLote);

            result.Retorno.Erros = await _processamentoCurriculoLoteRepository.BuscarErrosPorLoteInformacoesBasicas(idLote);

            result.Retorno.PdfsAProcessar = await _processamentoCurriculoLoteRepository.BuscarNomesArquivos(idLote);

            foreach (var colaborador in result.Retorno.PessoasCadastradasNesteLote)
            {
                var candidaturas = await _candidaturaRepository.ListarCandidaturasPorCodCandidato(colaborador.CodigoInternoColaborador);
                if (candidaturas == null || candidaturas.Count() == 0)
                    continue;

                colaborador.PossuiCandidatura = true;
                colaborador.Candidaturas = candidaturas.Select(m => new CandidaturaComTituloDTO 
                { 
                    IdCandidatura = m.CandidaturaId, 
                    TituloVaga = m.Titulo 
                }).ToList();
            }

            return result;
        }

        public async Task<IEnumerable<RecrutadoresQuantidadeCadastroBancoTalentos>> BuscarRecrutadoresQuantidadeCadastrada()
        {
            return await _bancoDeTalentosRepository.BuscarRecrutadoresQuantidadeCadastrada();
        }

        public async Task<IEnumerable<BuscarPessoasQueEuCadastrei>> BuscarBancoTalentos(int orgId, string busca, int cursor, int limite)
        {
            if(limite == 0)
                limite = 10;

            var colaboradores = await _bancoDeTalentosRepository.BuscarBancoTalentosPorOrg(orgId, busca, cursor, limite);

            foreach (var colaborador in colaboradores)
            {
                var candidaturas = await _candidaturaRepository.ListarCandidaturasPorCodCandidato(colaborador.CodigoInternoColaborador);
                if (candidaturas == null || candidaturas.Count() == 0)
                    continue;

                colaborador.PossuiCandidatura = true;
                colaborador.Candidaturas = candidaturas.Select(m => new CandidaturaComTituloDTO
                {
                    IdCandidatura = m.CandidaturaId,
                    TituloVaga = m.Titulo,
                    CodigoCliente = m.CodigoCliente,
                    NomeCliente = m.NomeCliente,
                    CodigoGestor = m.CodigoGestor,
                    NomeGestor = m.NomeGestor
                }).ToList();
            }

            return colaboradores;
        }

        public async Task<IEnumerable<BuscarPessoasQueEuCadastrei>> BuscarBancoTalentosComMatch(GestorExternoPerfilInput gestorExternoPerfilInput,int orgId, int limite, int cursor)
        {
            if (limite == 0)
                limite = 10;

            var talentos = await BuscarBancoTalentos(orgId, "", cursor, limite);

            var baseRequest = new ScoreSingleCandidateRequest
            {
                HardSkills = gestorExternoPerfilInput.GestorExternoPerfilSkills
                    .Where(s => s.Skill.Id == (int)ItemPerfilEnum.COMPETENCIA)
                    .Select(s => new SkillItem { Nome = s.Skill.Descricao, Nivel = ObterDescricaoNivel(s.Nivel.Id), Obrigatoriedade = s.Relevante ? "obrigatorio" : "desejavel" }).ToList(),
                SoftSkills = gestorExternoPerfilInput.GestorExternoPerfilSkills
                    .Where(s => s.Skill.Id == (int)ItemPerfilEnum.SOFTSKILL)
                    .Select(s => new SkillItem { Nome = s.Skill.Descricao, Obrigatoriedade = s.Relevante ? "obrigatorio" : "desejavel" }).ToList(),
                Metodologias = gestorExternoPerfilInput.GestorExternoPerfilSkills
                    .Where(s => s.Skill.Id == (int)ItemPerfilEnum.METODOLOGIA)
                    .Select(s => new SkillItem { Nome = s.Skill.Descricao, Nivel = ObterDescricaoNivel(s.Nivel.Id), Obrigatoriedade = s.Relevante ? "obrigatorio" : "desejavel" }).ToList(),
                DominiosNegocio = gestorExternoPerfilInput.GestorExternoPerfilSkills
                    .Where(s => s.Skill.Id == (int)ItemPerfilEnum.DOMINIONEGOCIO)
                    .Select(s => new SkillItem { Nome = s.Skill.Descricao, Nivel = ObterDescricaoNivel(s.Nivel.Id), Obrigatoriedade = s.Relevante ? "obrigatorio" : "desejavel" }).ToList(),
                Idiomas = gestorExternoPerfilInput.GestorExternoPerfilSkills
                    .Where(s => s.Skill.Id == (int)ItemPerfilEnum.IDIOMA)
                    .Select(s => new SkillItem { Nome = s.Skill.Descricao, Nivel = ObterDescricaoNivel(s.Nivel.Id), Obrigatoriedade = s.Relevante ? "obrigatorio" : "desejavel" }).ToList(),
                PesoHardSkills = 1,
                PesoSoftSkills = 1,
                PesoMetodologias = 1,
                PesoDominiosNegocio = 1,
                PesoIdiomas = 1,
                PesoDisponibilidades = 1,
                VisibleToOrgIds = new List<int> { gestorExternoPerfilInput.OrgId.ToInt(), EnumORG.FOURMAKERS_1.ToInt(), EnumORG.FMU_7.ToInt() },
                Disponibilidades = new List<DisponibilidadeItem>(),
                NumeroDeCandidatos = 1
            };

            _log.Log("MATCH - Envio - Banco", LevelsEnum.Information);
            _log.Log(JsonConvert.SerializeObject(baseRequest), LevelsEnum.Information);

            var scoreTasks = talentos.Select(async talento =>
            {
                try
                {
                    var req = JsonConvert.DeserializeObject<ScoreSingleCandidateRequest>(JsonConvert.SerializeObject(baseRequest));
                    req.CodigoInternoColaborador = talento.CodigoInternoColaborador;
                    var scoreResult = await _matchClient.ScoreSingleCandidate(req);
                    talento.Match = scoreResult.Match;
                }
                catch (Exception ex)
                {
                    talento.Match = 0;
                }
                return talento;
            });

            var talentosComScore = await Task.WhenAll(scoreTasks);

            return talentosComScore;
        }

        private string ObterDescricaoNivel(long? nivelId)
        {
            if (nivelId == null || nivelId == 0)
                return string.Empty;

            try
            {
                var nivel = new NivelDTO { Id = nivelId };
                var nivelCompleto = _formacaoNivelRepository.GetNivel(nivel);
                return nivelCompleto?.Descricao ?? string.Empty;
            }
            catch (Exception ex)
            {
                _log.Log($"Erro ao buscar descrição do nível {nivelId}: {ex.Message}", LevelsEnum.Error);
                return string.Empty;
            }
        }

        public async Task<PromptMatchResult> BuscarBancoTalentosComPromptMatch(ExtrairPerfilDeUmPromptRequest request, int orgId, int limite, int cursor, string? codigoInternoColaborador = null, string? idVaga = null)
        {
            if (limite == 0)
                limite = 10;

            var logContextExtractor = new DataTransferObject.Domain.Labs.ExtrairPerfilLogContext { OrgId = orgId, VagaId = idVaga, CodigoInternoColaborador = codigoInternoColaborador };
            var perfil = await _extractorService.ExtrairPerfilDeUmPrompt(request, logContextExtractor);
            var comunidade = String.Empty;

            _log.Log($"BuscarBancoTalentosComPromptMatch - Perfil extraido {JsonConvert.SerializeObject(perfil.perfil_extraido)}", LevelsEnum.Information);

            // Classificar perfil antes de buscar candidatos no match
            if (perfil.perfil_extraido != null)
            {
                try
                {
					comunidade = await _classificacaoService.ClassificarPerfilExtraidoAsync(perfil.perfil_extraido);
					_log.Log($"BuscarBancoTalentosComPromptMatch - Comunidade classificada {comunidade}", LevelsEnum.Information);
				}
                catch (Exception ex)
                {
                    _log.Log($"Erro ao classificar perfil após inserção: {ex}", DataTransferObject.Domain.Log.LevelsEnum.Error);
                }
            }

            var candidatosMatchRequest = MapearMatchRequest(perfil.perfil_extraido, orgId, limite, comunidade);

            var logContext = codigoInternoColaborador != null
                ? new DataTransferObject.Domain.Labs.RankCandidatesIdsLogContext { OrgId = orgId, VagaId = idVaga, CodigoInternoColaborador = codigoInternoColaborador }
                : null;
            var rankResult = await _matchService.RankCandidatesIds(candidatosMatchRequest, logContext);
            var candidatosMatchResponse = rankResult.Candidates;

            var retorno = new PromptMatchResult { IdLogRankCandidatesIds = rankResult.IdLogRankCandidatesIds };

            foreach (var candidato in candidatosMatchResponse)
            {
                var colaborador = new PessoasPromptMatch();
                colaborador.CodigoInternoColaborador = candidato.CodigoInternoColaborador;
                colaborador.Nome = candidato.Nome;
                colaborador.Match = candidato.Match;
                colaborador.RetornoMatch = candidato;

                retorno.Colaboradores.Add(colaborador);
            }

            var semaphore = new SemaphoreSlim(10, 10);
            var candidaturasTasks = retorno.Colaboradores.Select(async colaborador =>
            {
                await semaphore.WaitAsync();
                try
                {
                    var candidaturas = await _candidaturaRepository.ListarCandidaturasPorCodCandidato(colaborador.CodigoInternoColaborador);
                    if (candidaturas != null && candidaturas.Any())
                    {
                        colaborador.PossuiCandidatura = true;
                        colaborador.Candidaturas = candidaturas.Select(m => new CandidaturaComTituloDTO
                        {
                            IdCandidatura = m.CandidaturaId,
                            TituloVaga = m.Titulo,
                            CodigoCliente = m.CodigoCliente,
                            NomeCliente = m.NomeCliente,
                            CodigoGestor = m.CodigoGestor,
                            NomeGestor = m.NomeGestor
                        }).ToList();
                    }

                    //TODO - Mesma logica de negocio aqui e no VagaService
                    // Buscar organizações do colaborador
                    var organizacoes = await _buscaColaboradorRepository.GetOrganizacoesColaborador(colaborador.CodigoInternoColaborador);

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
                            TipoCadastroBancoDeTalentos = c.TipoCadastroBancoDeTalentos
                        })
                        .GroupBy(o => o.OrgId) // Agrupa por OrgId para remover duplicatas
                        .Select(g => g.First()) // Pega a primeira ocorrência de cada OrgId
                        .ToList();

                    // Define a origem baseada nas organizações
                    string origem = "Banco de Talentos";
                    if (organizacoes.Any())
                    {
                        if (ColaboradorBancoTalentosNaoFoursys(organizacoes))
                            origem = "Banco de Talentos";
                        else if (organizacoes.FirstOrDefault(m => m.TipoCadastroBancoDeTalentos == "SRS_LINKEDIN") is not null)
                            origem = "Linkedin";
                        else
                        {
                            var orgAtiva = organizacoes.FirstOrDefault(o => o.AtivoNaOrg == true);
                            if (orgAtiva != null)
                                origem = "Colaborador";
                            else if (organizacoes.Any(o => o.AtivoNaOrg == false))
                                origem = "Inativo";
                        }
                    }

                    colaborador.Origem = origem;
                    colaborador.Organizacoes = organizacoes;
                }
                finally
                {
                    semaphore.Release();
                }
            });

            await Task.WhenAll(candidaturasTasks);

            retorno.Prompt = perfil;

            return retorno;
        }

        private static bool ColaboradorBancoTalentosNaoFoursys(List<OrganizacaoCandidatoDTO> organizacoes)
        {
            return (organizacoes.FirstOrDefault(o => o.OrgId == EnumORG.FMU_7.ToInt()) is not null || organizacoes.FirstOrDefault(o => o.OrgId == EnumORG.FOURMAKERS_1.ToInt()) is not null)
                    && organizacoes.FirstOrDefault(o => o.OrgId == EnumORG.FOURSYS_2.ToInt()) is null;
        }

        public async Task<PromptMatchResultSnakeCase> BuscarBancoTalentosComPromptMatchSnakeCase(ExtrairPerfilDeUmPromptRequest request, int orgId, int limite, int cursor, string? codigoInternoColaborador = null, string? idVaga = null)
        {
            if (request == null)
                throw new ArgumentException("O corpo da requisição é obrigatório.");
            if (string.IsNullOrWhiteSpace(request.texto_vaga))
                throw new ArgumentException("O texto da vaga não pode ser vazio.");

            var resultadoOriginal = await BuscarBancoTalentosComPromptMatch(request, orgId, limite, cursor, codigoInternoColaborador, idVaga);
            return MapearParaSnakeCase(resultadoOriginal);
        }

        private PromptMatchResultSnakeCase MapearParaSnakeCase(PromptMatchResult resultadoOriginal)
        {
            var resultadoSnakeCase = new PromptMatchResultSnakeCase
            {
                prompt = resultadoOriginal.Prompt,
                IdLogRankCandidatesIds = resultadoOriginal.IdLogRankCandidatesIds,
                colaboradores = resultadoOriginal.Colaboradores.Select(c => new PessoasPromptMatchSnakeCase
                {
                    nome = c.Nome,
                    codigo_interno_colaborador = c.CodigoInternoColaborador,
                    possui_candidatura = c.PossuiCandidatura,
                    match = c.Match,
                    candidaturas = c.Candidaturas?.Select(cand => new CandidaturaComTituloDTOSnakeCase
                    {
                        id_candidatura = cand.IdCandidatura,
                        titulo_vaga = cand.TituloVaga,
                        codigo_cliente = cand.CodigoCliente,
                        nome_cliente = cand.NomeCliente,
                        codigo_gestor = cand.CodigoGestor,
                        nome_gestor = cand.NomeGestor
                    }).ToList(),
                    retornoMatch = c.RetornoMatch != null ? new CandidatosMatchResponseSnakeCase
                    {
                        codigo_interno_colaborador = c.RetornoMatch.CodigoInternoColaborador,
                        nome = c.RetornoMatch.Nome,
                        orgs = c.RetornoMatch.Orgs,
                        match = c.RetornoMatch.Match,
                        score_candidato = c.RetornoMatch.ScoreCandidato,
                        score_vaga = c.RetornoMatch.ScoreVaga,
                        detalhamento_calculo = c.RetornoMatch.DetalhamentoCalculo != null ? new DetalhamentoCalculoSnakeCase
                        {
                            hard_skills = MapearCategoriaScore(c.RetornoMatch.DetalhamentoCalculo.HardSkills),
                            soft_skills = MapearCategoriaScore(c.RetornoMatch.DetalhamentoCalculo.SoftSkills),
                            metodologias = MapearCategoriaScore(c.RetornoMatch.DetalhamentoCalculo.Metodologias),
                            dominios_negocio = MapearCategoriaScore(c.RetornoMatch.DetalhamentoCalculo.DominiosNegocio),
                            idiomas = MapearCategoriaScore(c.RetornoMatch.DetalhamentoCalculo.Idiomas),
                            disponibilidades = MapearCategoriaScore(c.RetornoMatch.DetalhamentoCalculo.Disponibilidades)
                        } : null,
                        comparativo_por_skill = c.RetornoMatch.ComparativoPorSkill != null ? new ComparativoPorSkillSnakeCase
                        {
                            hard_skills = MapearSkillComparativa(c.RetornoMatch.ComparativoPorSkill.HardSkills),
                            soft_skills = MapearSkillComparativa(c.RetornoMatch.ComparativoPorSkill.SoftSkills),
                            metodologias = MapearSkillComparativa(c.RetornoMatch.ComparativoPorSkill.Metodologias),
                            dominios_negocio = MapearSkillComparativa(c.RetornoMatch.ComparativoPorSkill.DominiosNegocio),
                            idiomas = MapearSkillComparativa(c.RetornoMatch.ComparativoPorSkill.Idiomas),
                            disponibilidades = MapearSkillComparativa(c.RetornoMatch.ComparativoPorSkill.Disponibilidades)
                        } : null
                    } : null,
                    origem = c.Origem,
                    organizacoes = c.Organizacoes
                }).ToList()
            };

            return resultadoSnakeCase;
        }

        private CategoriaScoreSnakeCase MapearCategoriaScore(CategoriaScore categoria)
        {
            if (categoria == null) return null;
            
            return new CategoriaScoreSnakeCase
            {
                score_bruto_categoria = categoria.ScoreBrutoCategoria,
                score_bruto_obrigatorio = categoria.ScoreBrutoObrigatorio,
                score_bruto_desejavel = categoria.ScoreBrutoDesejavel
            };
        }

        private List<SkillComparativaSnakeCase> MapearSkillComparativa(List<SkillComparativa> skills)
        {
            if (skills == null) return new List<SkillComparativaSnakeCase>();
            
            return skills.Select(s => new SkillComparativaSnakeCase
            {
                skill_requisitada = s.SkillRequisitada,
                nivel_requerido = s.NivelRequerido,
                obrigatoriedade = s.Obrigatoriedade,
                skill_do_candidato = s.SkillDoCandidato,
                nivel_do_candidato = s.NivelDoCandidato,
                pontuacao_da_skill = s.PontuacaoDaSkill
            }).ToList();
        }

        private CandidatosMatchRequestIds MapearMatchRequest(PerfilExtraido perfilExtraido, int orgId, int limite, string comunidade)
        {
            List<string> origens = null;
            if(!String.IsNullOrEmpty(perfilExtraido.origem))
            {
                origens = new List<string>();
                origens.Add(perfilExtraido.origem);
            }

            return new CandidatosMatchRequestIds
			{
                HardSkills = perfilExtraido.gestorExternoPerfilSkills?
                    .Where(s => s.itemPerfil?.Id == (int)ItemPerfilEnum.COMPETENCIA)
                    .Select(s => new HabilidadeTecnicaIds 
                    { 
                        Id = s.skill?.Id ?? 0, 
                        NivelId = s.nivel?.Id ?? 0,
                        Nivel = s.nivel?.Descricao ?? string.Empty, 
                        Obrigatoriedade = s.relevante ? "obrigatorio" : "desejavel" 
                    }).ToList() ?? new List<HabilidadeTecnicaIds>(),

                SoftSkills = perfilExtraido.gestorExternoPerfilSkills?
                    .Where(s => s.itemPerfil?.Id == (int)ItemPerfilEnum.SOFTSKILL)
                    .Select(s => new HabilidadeComportamentalIds 
                    { 
                        Id = s.skill?.Id ?? 0,
                        NivelId = s.nivel?.Id ?? 0,
                        Obrigatoriedade = s.relevante ? "obrigatorio" : "desejavel" 
                    }).ToList() ?? new List<HabilidadeComportamentalIds>(),

                Metodologias = perfilExtraido.gestorExternoPerfilSkills?
                    .Where(s => s.itemPerfil?.Id == (int)ItemPerfilEnum.METODOLOGIA)
                    .Select(s => new MetodologiaIds 
                    { 
                        Id = s.skill?.Id ?? 0,
                        NivelId = s.nivel?.Id ?? 0,
                        Nivel = s.nivel?.Descricao ?? string.Empty, 
                        Obrigatoriedade = s.relevante ? "obrigatorio" : "desejavel" 
                    }).ToList() ?? new List<MetodologiaIds>(),

                DominiosNegocio = perfilExtraido.gestorExternoPerfilSkills?
                    .Where(s => s.itemPerfil?.Id == (int)ItemPerfilEnum.DOMINIONEGOCIO)
                    .Select(s => new DominioNegocioIds 
                    { 
                        Id = s.skill?.Id ?? 0,
                        NivelId = s.nivel?.Id ?? 0,
                        Nivel = s.nivel?.Descricao ?? string.Empty, 
                        Obrigatoriedade = s.relevante ? "obrigatorio" : "desejavel" 
                    }).ToList() ?? new List<DominioNegocioIds>(),

                Idiomas = perfilExtraido.gestorExternoPerfilSkills?
                    .Where(s => s.itemPerfil?.Id == (int)ItemPerfilEnum.IDIOMA)
                    .Select(s => new IdiomaIds 
                    { 
                        Id = s.skill?.Id ?? 0,
                        NivelId = s.nivel?.Id ?? 0,
                        Nivel = s.nivel?.Descricao ?? string.Empty, 
                        Obrigatoriedade = s.relevante ? "obrigatorio" : "desejavel" 
                    }).ToList() ?? new List<IdiomaIds>(),

                Disponibilidades = new List<Disponibilidade>(),
                Origem = origens,
                PesoHardSkills = 1,
                PesoSoftSkills = 1,
                PesoMetodologias = 1,
                PesoDominiosNegocio = 1,
                PesoIdiomas = 1,
                PesoDisponibilidades = 1,
                VisibleToOrgIds = new List<int> { orgId, EnumORG.FOURMAKERS_1.ToInt(), EnumORG.FMU_7.ToInt() },
                NumeroDeCandidatos = limite,
                LocalizacaoEstado = perfilExtraido.estado,
                LocalizacaoCidade = perfilExtraido.cidade,
                Comunidades = new List<string> { comunidade }
			};
        }

        public async Task<IEnumerable<BuscarPessoasCadastradasPorOrgResult>> BuscarPessoasCadastradasPorOrg(string cpf, int orgId, BuscarPessoasCadastradasPorOrgInput input)
        {
            // Validar acesso: grupo 37
            _bancoDeTalentosValidatorService.ValidaAcessoBuscarPessoasCadastradasPorOrg(cpf, orgId);

            if (input.Limite == 0)
                input.Limite = 10;

            var resultados = await _bancoDeTalentosRepository.BuscarPessoasCadastradasPorOrg(
                orgId,
                input.Busca,
                input.StatusVaga,
                input.StatusCandidatura,
                input.DataInicio,
                input.DataFim,
                input.Cursor,
                input.Limite
            );

            return resultados;
        }
    }
}