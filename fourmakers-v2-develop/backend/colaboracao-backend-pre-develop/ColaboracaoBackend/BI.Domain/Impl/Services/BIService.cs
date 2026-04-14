using ApiClient.Domain;
using ApiClient.Domain.Interfaces;
using Aws.Infra.Interfaces;
using BI.Domain.Interfaces.Services;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Competencia.Domain.Interfaces.Services;
using Core.Domain;
using Core.Domain.Apontamento;
using Core.Domain.BI;
using Core.Domain.Competencia.Metodologia;
using Core.Domain.Dominio;
using Core.Domain.IIdioma;
using Core.Domain.MapaAlocacao;
using Core.Domain.Usuario;
using Core.DomainModel.Org;
using Core.DomainModel.Softskill;
using DataTransferObject.Domain;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.BI;
using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.Dominio;
using DataTransferObject.Domain.Idioma;
using DataTransferObject.Domain.MapaDeAlocacao;
using DataTransferObject.Domain.Softskill;
using DataTransferObject.Domain.SRS;
using DataTransferObject.Domain.Util.Enum;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Colaboracao.Helper.Util.Competencia;
using Competencia.Domain.Enums;
using Microsoft.IdentityModel.Tokens;

using Logs.Infra.Attributes;

namespace BI.Domain.Impl.Services
{
    [LogDomainClass]
    public class BIService : IBIService
    {
        // Semaphore por orgId: garante 1 execução por vez por organização
        private static readonly ConcurrentDictionary<int, SemaphoreSlim> _semaphoresPorOrg = new ConcurrentDictionary<int, SemaphoreSlim>();

        private readonly IApontamentoRepository _apontamentoRepository;
        private readonly IExtracaoAlocacaoRepository _extracaoAlocacaoRepository;
        private readonly IColaboradorBIRepository _colaboradorBIRepository;
        private readonly IEscolaridadeColaboradorBIRepository _escolaridadeColaboradorBiRepository;
        private readonly IExperienciaProfissionalBIRepository _experienciaColaboradorBiRepository;
        private readonly IOrgRepository _orgRepository;
        private readonly ISRSColaboracaoClient _srsColaboracaoClient;
        private readonly IHardSkillRepository _hardSkillRepository;
        private readonly ISoftskillRepository _softskillRepository;
        private readonly ISoftskillNivelRepository _softskillNivelRepository;
        private readonly IMetodologiasRepository _metodologiaRepository;
        private readonly IDominioRepository _dominioRepository;
        private readonly IIdiomaRepository _idiomaRepository;
        private readonly IIdiomaNivelRepository _idiomaNivelRepository;
        private readonly IAwsCacheService _awsCacheService;
        private readonly IMapaAlocacaoRepository _mapaAlocacaoRepository;
        private readonly ITokenSistemaService _tokenSistemaService;
        private readonly IBICompetenciaRepository _bICompetenciaRepository;

        public BIService(IApontamentoRepository apontamentoRepository, ITokenSistemaRepository tokenRepository, IExtracaoAlocacaoRepository extracaoAlocacaoRepository,
            IColaboradorBIRepository colaboradorBIRepository, IEscolaridadeColaboradorBIRepository escolaridadeColaboradorBiRepository,
            IExperienciaProfissionalBIRepository experienciaColaboradorBiRepository, IOrgRepository orgRepository, ISRSColaboracaoClient srsColaboracaoClient,
            IHardSkillRepository hardSkillRepository, ISoftskillRepository softskillRepository, IMetodologiasRepository metodologiaRepository, IDominioRepository dominioRepository,
            IIdiomaRepository idiomaRepository, ISoftskillNivelRepository softskillNivelRepository, IIdiomaNivelRepository idiomaNivelRepository, IAwsCacheService awsCacheService,
            IMapaAlocacaoRepository mapaAlocacaoRepository, ITokenSistemaService tokenSistemaService, IBICompetenciaRepository bICompetenciaRepository)
        {
            _apontamentoRepository = apontamentoRepository;
            _extracaoAlocacaoRepository = extracaoAlocacaoRepository;
            _colaboradorBIRepository = colaboradorBIRepository;
            _escolaridadeColaboradorBiRepository = escolaridadeColaboradorBiRepository;
            _experienciaColaboradorBiRepository = experienciaColaboradorBiRepository;
            _orgRepository = orgRepository;
            _srsColaboracaoClient = srsColaboracaoClient;
            _hardSkillRepository = hardSkillRepository;
            _softskillRepository = softskillRepository;
            _softskillNivelRepository = softskillNivelRepository;
            _metodologiaRepository = metodologiaRepository;
            _dominioRepository = dominioRepository;
            _idiomaRepository = idiomaRepository;
            _idiomaNivelRepository = idiomaNivelRepository;
            _awsCacheService = awsCacheService;
            _mapaAlocacaoRepository = mapaAlocacaoRepository;
            _tokenSistemaService = tokenSistemaService;
            _bICompetenciaRepository = bICompetenciaRepository;
        }


        public async Task<List<dynamic>> GeraRelatorioAlocacao(string tokenSistema)
        {
            try
            {
                var orgId = _tokenSistemaService.GetOrgTokenSistema(tokenSistema);
                var qtdGerenteProjetoPrioridade = "1";
                Console.WriteLine("Foi solicitado GeraRelatorioAlocacao pela org: " + orgId + " as " + DateTime.Now.ToString());
                var retChache = await _awsCacheService.GetAsync<List<dynamic>>("GeraRelatorioAlocacao_" + orgId);
                if (retChache != null)
                {
                    Console.WriteLine("Retornando cache GeraRelatorioAlocacao_" + orgId);
                    return retChache;
                }
                var (alocacoes, _) = await _mapaAlocacaoRepository.ListarAlocacoesColaboradoresETbdsDynamicAsync(null, null, orgId, null, null, null, null, TipoProfissionalEnum.Todos, null, null, false, null, null, qtdGerenteProjetoPrioridade, false, null, null);
                
                await _awsCacheService.SetAsync("GeraRelatorioAlocacao_" + orgId, alocacoes, TimeSpan.FromMinutes(15));
                return alocacoes;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public async Task<IEnumerable<RelatorioApontamentoDTO>> GeraRelatorioApontamento(string tokenSistema)
        {
            var orgId = _tokenSistemaService.GetOrgTokenSistema(tokenSistema);
            Console.WriteLine("Foi solicitado GeraRelatorioApontamento pela org: " + orgId + " as " + DateTime.Now.ToString());
            var semaphore = _semaphoresPorOrg.GetOrAdd(orgId, _ => new SemaphoreSlim(1, 1));

            var retChache = await _awsCacheService.GetAsync<List<RelatorioApontamentoDTO>>("GeraRelatorioApontamento_" + orgId);
            if (retChache != null)
            {
                Console.WriteLine("Retornando cache GeraRelatorioApontamento_" + orgId);
                return retChache;
            }

            await semaphore.WaitAsync();
            try
            {
                retChache = await _awsCacheService.GetAsync<List<RelatorioApontamentoDTO>>("GeraRelatorioApontamento_" + orgId);
                if (retChache != null)
                {
                    Console.WriteLine("Retornando cache GeraRelatorioApontamento_" + orgId);
                    return retChache;
                }

                var ret = await _apontamentoRepository.ListarApontamentoRelatorioBIAsync(orgId, orgId == 4 ? DateTime.Parse("2025-10-01").ToString("yyyy-MM-dd") : DateTime.Parse("2025-01-01").ToString("yyyy-MM-dd"));
                await _awsCacheService.SetAsync("GeraRelatorioApontamento_" + orgId, ret, TimeSpan.FromMinutes(15));
                return ret;
            }
            finally
            {
                semaphore.Release();
            }
        }

        public async Task<List<RelatorioApontamentoDTO>> GeraRelatorioApontamentoRecentes(string tokenSistema)
        {
            var orgId = _tokenSistemaService.GetOrgTokenSistema(tokenSistema);
            Console.WriteLine("Foi solicitado GeraRelatorioApontamentoRecentes pela org: " + orgId + " as " + DateTime.Now.ToString());
            var semaphore = _semaphoresPorOrg.GetOrAdd(orgId, _ => new SemaphoreSlim(1, 1));

            var retChache = await _awsCacheService.GetAsync<List<RelatorioApontamentoDTO>>("GeraRelatorioApontamentoRecentes_" + orgId);
            if (retChache != null)
            {
                Console.WriteLine("Retornando cache GeraRelatorioApontamentoRecentes_" + orgId);
                return retChache;
            }

            await semaphore.WaitAsync();
            try
            {
                retChache = await _awsCacheService.GetAsync<List<RelatorioApontamentoDTO>>("GeraRelatorioApontamentoRecentes_" + orgId);
                if (retChache != null)
                {
                    Console.WriteLine("Retornando cache GeraRelatorioApontamentoRecentes_" + orgId);
                    return retChache;
                }

                var ret = await _apontamentoRepository.ListarApontamentoRecenteRelatorioBI(orgId);
                await _awsCacheService.SetAsync("GeraRelatorioApontamentoRecentes_" + orgId, ret, TimeSpan.FromMinutes(15));
                return ret;
            }
            finally
            {
                semaphore.Release();
            }
        }

        public async Task<List<AlocacaoColaboradorOrgDTO>> GeraAlocacaoTodos(string tokenSistema)
        {
            try
            {
                var orgId = _tokenSistemaService.GetOrgTokenSistemaWithValidatingOrgs(tokenSistema, EnumORG.FOURSYS_2);
                
                var qtdGerenteProjetoPrioridade = "1";

                var retChache = await _awsCacheService.GetAsync<List<AlocacaoColaboradorOrgDTO>>("GeraAlocacaoTodos_" + orgId);
                if (retChache != null)
                {
                    Console.WriteLine("Retornando cache GeraAlocacaoTodos_" + orgId);
                    return retChache;
                }

                var orgsSistema = _orgRepository.GetAllOrgsId(true);
                var lstAlocacoes = new List<AlocacaoColaboradorOrgDTO>();
                foreach (var orgInfo in orgsSistema)
                {
                    var lstResult = await _mapaAlocacaoRepository.ListarAlocacoesColaboradoresETbds(null, null, (int)orgInfo.Id, null, null, null, null, TipoProfissionalEnum.Todos, null, null, false, null, null, qtdGerenteProjetoPrioridade, false, null, null);
                    var resultado = lstAlocacoes.Cast<dynamic>().ToList();
                    lstAlocacoes.Add(new AlocacaoColaboradorOrgDTO
                    {
                        OrgId = (int)orgInfo.Id,
                        OrgNome = orgInfo.Descricao,
                        Alocacoes = resultado
                    });
                }

                await _awsCacheService.SetAsync("GeraAlocacaoTodos_" + orgId, lstAlocacoes, TimeSpan.FromMinutes(15));
                return lstAlocacoes;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<ColaboradorCompletoResult> GetListaColaboradoresCompleto(string tokenSistema)
        {
            try
            {
                var orgId = _tokenSistemaService.GetOrgTokenSistemaWithValidatingOrgs(tokenSistema, EnumORG.FOURSYS_2);
                var ret = new ColaboradorCompletoResult
                {
                    Colaboradores = new List<ColaboradorCompletoDTO>()
                };

                var retChache = await _awsCacheService.GetAsync<ColaboradorCompletoResult>("GetListaColaboradoresCompleto_" + orgId);
                if (retChache != null)
                {
                    Console.WriteLine("Retornando cache GetListaColaboradoresCompleto_" + orgId);
                    return retChache;
                }

                Console.WriteLine("Foi solicitado GetListaColaboradoresCompleto pela org: " + orgId + " as " + DateTime.Now.ToString());
                var colaboradoresInfo = _colaboradorBIRepository.GetAllColaboradoresBI();
                var colaboradorOrgInfo = _colaboradorBIRepository.GetOrgInfoBI();
                var competenciaColaboradorInfo = _colaboradorBIRepository.GetCompetenciaColaboradorBI();
                var certificadoCompetenciaColaboradorInfo = _colaboradorBIRepository.GetCertificadoCompetenciaColaboradorBI();
                var softskillColaboradorInfo = _colaboradorBIRepository.GetSoftskillColaboradorBI();
                var metodologiaColaboradorInfo = _colaboradorBIRepository.GetMetodologiaColaboradorBI();
                var dominioColaboradorInfo = _colaboradorBIRepository.GetDominiColaboradorBI();
                var idiomaColaboradorInfo = _colaboradorBIRepository.GetIdiomaColaboradorBI();
                var escolaridadeColaboradorInfo = _escolaridadeColaboradorBiRepository.Listar();
                var experienciaColaboradorInfo = _experienciaColaboradorBiRepository.Listar();

                ret.Colaboradores.AddRange(colaboradoresInfo.Select(x => new ColaboradorCompletoDTO { Colaborador = x }));
                ret.Colaboradores = ret.Colaboradores.Select(x =>
                    x = new ColaboradorCompletoDTO
                    {
                        Colaborador = x.Colaborador,
                        ListaOrgColaborador = colaboradorOrgInfo.Where(y => y.Key == x.Colaborador.Cpf).Select(y => y.Value).ToList(),
                        Softskills = softskillColaboradorInfo.Where(y => y.Key == x.Colaborador.Cpf).Select(y => y.Value).ToList(),
                        Metodologias = metodologiaColaboradorInfo.Where(y => y.Key == x.Colaborador.Cpf).Select(y => y.Value).ToList(),
                        Dominios = dominioColaboradorInfo.Where(y => y.Key == x.Colaborador.Cpf).Select(y => y.Value).ToList(),
                        Idiomas = idiomaColaboradorInfo.Where(y => y.Key == x.Colaborador.Cpf).Select(y => y.Value).ToList(),
                        Competencias = competenciaColaboradorInfo.Where(y => y.Key == x.Colaborador.Cpf).Select(y => y.Value).ToList(),
                        Escolaridade = escolaridadeColaboradorInfo.Where(y => y.Key == x.Colaborador.Cpf).Select(y => y.Value).ToList(),
                        Experiencias = experienciaColaboradorInfo.Where(y => y.Key == x.Colaborador.Cpf).Select(y => y.Value).ToList(),
                    }
                ).ToList();
                ret.Colaboradores.ForEach(x =>
                    x.Competencias?.ForEach(y =>
                        y.Certificados = certificadoCompetenciaColaboradorInfo.Where(z => z.Key.Equals(new Tuple<string, long>(x.Colaborador.Cpf, y.Id))).Select(z => z.Value).ToList())
                );

                await _awsCacheService.SetAsync("GetListaColaboradoresCompleto_" + orgId, ret, TimeSpan.FromMinutes(15));
                return ret;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string BuildSkillInfo(
            string skill,
            Dictionary<(int, long), string> dictSkills,
            Dictionary<(int, long), string> dictNiveis)
        {
            var arr = skill.Split(',');

            int tipoSkill = int.Parse(arr[0].Trim('('));
            long idSkill = long.Parse(arr[1]);
            long nivelSkill = long.Parse(arr[2].Trim(')'));

            string descTipo = tipoSkill switch
            {
                1 => "HARDSKILL",
                2 => "SOFTSKILL",
                3 => "IDIOMA",
                4 => "METODOLOGIA",
                5 => "CONHECIMENTO_NEGOCIO",
                _ => "DESCONHECIDO"
            };

            dictSkills.TryGetValue((tipoSkill, idSkill), out var descSkill);
            dictNiveis.TryGetValue((tipoSkill, nivelSkill), out var descNivel);
            
            //Competencia Inativa no Fourmakers
            if (descSkill.IsNullOrEmpty())
            {
                return "";
            }
            return $"({descTipo},{descSkill ?? "-"}, {descNivel ?? "-"})";
        }

        public async Task<List<CandidateRelatorioBI>> GetRelatorioCandidate(string tokenSistema)
        {
            try
            {
                var orgId = _tokenSistemaService.GetOrgTokenSistemaWithValidatingOrgs(tokenSistema, EnumORG.FOURSYS_2);

                var retChache = await _awsCacheService.GetAsync<List<CandidateRelatorioBI>>("GetRelatorioCandidate_" + orgId);
                if (retChache != null)
                {
                    Console.WriteLine("Retornando cache GetRelatorioCandidate_" + orgId);
                    return retChache;
                }

                var candidates = await _srsColaboracaoClient.GetRelatorioCandidatosRaw(VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.TOKEN_SISTEMA_COLABORACAO));
                
                var listaDeSkillsRetornadasCandidates = candidates.Where(x => !string.IsNullOrEmpty(x.Skills)).Select(x => x.Skills).ToList();

                var dicionarioDeSkills = GerarListaDeIdsPorTipoPerfilId(listaDeSkillsRetornadasCandidates);

                var listaDeCompetencias = new List<CompetenciaGroupDTO>();

                foreach (var listaSkill in dicionarioDeSkills)
                {
                    var keyItemPerfilFourmakers = (int)CompetenciaUtils.ConverterTipoCompetenciaSRSToItemPerfil((TipoCompetenciaSRSEnum)listaSkill.Key);
                    var skills = await _bICompetenciaRepository.ObterCompetenciasPorTipoEListaDeIds(keyItemPerfilFourmakers, listaSkill.Value);
                    listaDeCompetencias.AddRange(skills);
                }

                var competenciasNivel = _hardSkillRepository.ListarNivelHardSkill();
                var sofskillsNivel = _softskillNivelRepository.ListarNivelSoftskill(8);
                var metodologiasNivel = _metodologiaRepository.ListarNivelMetodologia();
                var dominiosNivel = _softskillNivelRepository.ListarNivelSoftskill(4);
                var idiomasNivel = _idiomaNivelRepository.ListaNivelIdioma();

                var listaNiveisParaSRS = new List<Tuple<int, long, string>>();
                listaNiveisParaSRS.AddRange(competenciasNivel.Select(x => new Tuple<int, long, string>(1, (long)x.Id, x.Descricao)));
                listaNiveisParaSRS.AddRange(sofskillsNivel.Select(x => new Tuple<int, long, string>(2, (long)x.Id, x.Descricao)));
                listaNiveisParaSRS.AddRange(idiomasNivel.Select(x => new Tuple<int, long, string>(3, (long)x.Id, x.Descricao)));
                listaNiveisParaSRS.AddRange(metodologiasNivel.Select(x => new Tuple<int, long, string>(4, (long)x.Id, x.Descricao)));
                listaNiveisParaSRS.AddRange(dominiosNivel.Select(x => new Tuple<int, long, string>(5, (long)x.Id, x.Descricao)));
                
                var dictNiveis = listaNiveisParaSRS
                    .ToDictionary(x => (x.Item1, x.Item2), x => x.Item3);
                
                var dictSkills = listaDeCompetencias
                    .ToDictionary(
                        x => (x.TipoIdSRS.Value, x.Id),
                        x => x.Descricao
                );

                candidates.ForEach(x =>
                    x.Skills = string.IsNullOrEmpty(x.Skills)
                        ? null
                        : string.Join(";",
                            x.Skills.Split(";")
                                .Select(skill => BuildSkillInfo(
                                    skill,
                                    dictSkills,
                                    dictNiveis
                                ))
                        )
                );

                await _awsCacheService.SetAsync("GetRelatorioCandidate_" + orgId, candidates, TimeSpan.FromMinutes(15));
                return candidates;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private Dictionary<int, List<long>> GerarListaDeIdsPorTipoPerfilId(List<string> skills)
        {
            var resultado = new Dictionary<int, List<long>>();

            foreach (var skillString in skills)
            {
                var grupos = skillString.Split(';', StringSplitOptions.RemoveEmptyEntries);

                foreach (var grupo in grupos)
                {
                    var clean = grupo.Trim().Trim('(', ')');

                    var partes = clean.Split(',', StringSplitOptions.RemoveEmptyEntries);

                    if (partes.Length < 2)
                        continue;

                    int tipoPerfilId = int.Parse(partes[0]);
                    
                    long skillId = long.Parse(partes[1]);

                    if (!resultado.ContainsKey(tipoPerfilId))
                        resultado[tipoPerfilId] = new List<long>();

                    resultado[tipoPerfilId].Add(skillId);
                }
            }

            return resultado;
        }

        public async Task<List<RelatorioAprovadoresDTO>> RelatorioProjetosEAprovadores(string tokenSistema)
        {
            var orgId = _tokenSistemaService.GetOrgTokenSistemaWithValidatingOrgs(tokenSistema, EnumORG.NUMEN_4);

            var retChache = await _awsCacheService.GetAsync<List<RelatorioAprovadoresDTO>>("ListarProjetosEAprovadores" + orgId);
            if (retChache != null)
            {
                Console.WriteLine("Retornando cache GeraRelatorioApontamento_" + orgId);
                return retChache;
            }
            var ret = await _mapaAlocacaoRepository.ListarProjetosEAprovadores(orgId);
            await _awsCacheService.SetAsync<List<RelatorioAprovadoresDTO>>("ListarProjetosEAprovadores" + orgId, ret, TimeSpan.FromMinutes(15));
            return ret;
        }

    }
}