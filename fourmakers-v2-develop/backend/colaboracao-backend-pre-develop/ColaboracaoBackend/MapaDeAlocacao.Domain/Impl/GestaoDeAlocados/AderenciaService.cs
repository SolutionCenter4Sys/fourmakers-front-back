using Colaboracao.Helper;
using Colaboracao.Helper.Util;
using Core.Domain;
using Core.Domain.Colaborador;
using Core.Domain.MapaAlocacao;
using Core.Domain.MapaAlocacao.GestaoDeAlocados;
using Core.DomainModel.MapaAlocacao.GestaoDeAlocados;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.Endereco;
using DataTransferObject.Domain.Fourmakers;
using DataTransferObject.Domain.MapaDeAlocacao;
using DataTransferObject.Domain.MapaDeAlocacao.Aderencia;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.Aderencia;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfil;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfilSkill;
using DataTransferObject.Domain.ModeloTrabalho;
using DataTransferObject.Domain.Nivel;
using DataTransferObject.Domain.Util.Enum;
using Foursys.Domain.Interfaces.Services;
using MapaDeAlocacao.Domain.Interfaces.GestaoDeAlocados;
using MapaDeAlocacao.Domain.Interfaces.GestaoDeAlocados.GestorExternoPerfil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colaboracao.Helper.Enum;
using MapaDeAlocacao.Domain.Interfaces;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfilSkill.ItemPerfil;
using DataTransferObject.Domain.Match;
using Newtonsoft.Json;
using ApiClient.Domain.Interfaces;
using DataTransferObject.Domain.Vaga;
using DataTransferObject.Domain.Log;
using Colaboracao.Core;
using Competencia.Domain.Enums;
using ApiClient.Domain;

using Logs.Infra.Attributes;

namespace MapaDeAlocacao.Domain.Impl.GestaoDeAlocados
{
    [LogDomainClass]
    public class AderenciaService : IAderenciaService
    {
        private readonly IAderenciaRepository _aderenciaRepository;
        private readonly IGestorExternoPerfilService _gestorExternoPerfilService;
        private readonly IBuscaParametroConfiguracaoService _buscaParametroConfiguracaoService;
        private readonly IPerfilAlocacaoRepository _perfilAlocacaoRepository;
        private readonly IMapaAlocacaoRepository _mapaAlocacaoRepository;
        private readonly IBuscaColaboradorRepository _buscaColaboradorRepository;
        private readonly IGestaoAlocadosRepository _gestaoAlocadosRepository;
        private readonly IMapaDeAlocacaoService _mapaDeAlocacaoService;
        private readonly IMatchClient _matchClient;
        private readonly ILogCore _log;

        public AderenciaService(IAderenciaRepository aderenciaRepository,
                                IGestorExternoPerfilService gestorExternoPerfilService,
                                IBuscaParametroConfiguracaoService buscaParametroConfiguracaoService,
                                IPerfilAlocacaoRepository perfilAlocacaoRepository,
                                IMapaAlocacaoRepository mapaAlocacaoRepository,
                                IBuscaColaboradorRepository buscaColaboradorRepository,
                                IGestaoAlocadosRepository gestaoAlocadosRepository, IMapaDeAlocacaoService mapaDeAlocacaoService, IMatchClient matchClient = null, ILogCore log = null)
        {
            _aderenciaRepository = aderenciaRepository;
            _gestorExternoPerfilService = gestorExternoPerfilService;
            _buscaParametroConfiguracaoService = buscaParametroConfiguracaoService;
            _perfilAlocacaoRepository = perfilAlocacaoRepository;
            _mapaAlocacaoRepository = mapaAlocacaoRepository;
            _buscaColaboradorRepository = buscaColaboradorRepository;
            _gestaoAlocadosRepository = gestaoAlocadosRepository;
            _mapaDeAlocacaoService = mapaDeAlocacaoService;
            _matchClient = matchClient;
            _log = log;
        }

        public async Task<List<CalculoAderenciaComPerfilEColaboradorDTO>> GerarListaAderenciaSimplificada(List<PerfilEColaboradorAderenciaDTO> listaColaboradoresEPerfis, string cpfRequest, int orgId)
        {
            var ret = new List<CalculoAderenciaComPerfilEColaboradorDTO>();

            var configuracaoAderencia = _buscaParametroConfiguracaoService.GetParametroConfiguracao<string>(ParametroOrgCodigoEnum.CONFIGURACAO_PERCENTUAL_ADERENCIA, (int)EnumORG.FOURSYS_2, cpfRequest);
            var dicCredenciais = StringUtil.ExtrairPropriedadesSeparadasPontoEVirgula(configuracaoAderencia);

            var codigoPerfis = listaColaboradoresEPerfis.Select(x => x.PerfilId).ToList();
            var perfis = await _gestorExternoPerfilService.ObterGestoresExternoPerfilPorListaDeIds(codigoPerfis , orgId);

            var codigoColaboradores = listaColaboradoresEPerfis.Select(x => x.CodigoInternoColaborador).ToList();

            var colaboradores = await _aderenciaRepository.ListarColaboradoresAderentesPorListaId(codigoColaboradores, orgId);

            foreach (var item in listaColaboradoresEPerfis)
            {
                var perfil = perfis.Retorno.Where(x => x.Id == item.PerfilId).FirstOrDefault();

                //Somente com Perfil
                if (perfil != null)
                {
                    var colaborador = colaboradores.Where(x => x.CodigoInternoColaborador == item.CodigoInternoColaborador).FirstOrDefault();

                    var calculoAderencia = PreencherCalculoAderencia(colaborador.Ativo, colaborador.DataAdmissao, colaborador.PeriodoDTOs, colaborador.Localidade, colaborador.CustoHora, colaborador.Skills, perfil, dicCredenciais);

                    var calculo = new CalculoAderenciaComPerfilEColaboradorDTO()
                    {
                        PerfilId = perfil.Id,
                        NomePerfil = perfil.NomePerfil,
                        NomeColaborador = colaborador.NomeCompleto,
                        Cargo = colaborador.Cargo,
                        CalculoAderencia = calculoAderencia.CalculoAderencia,
                        CodigoInternoColaborador = colaborador.CodigoInternoColaborador
                    };

                    ret.Add(calculo);
                }
            }
            return ret;
        }

        public async Task<ApiGenericResult<List<CalculoAderenciaComPerfilEColaboradorDTO>>> ListarAderenciaAlocadosColabEPerfil(string cpfRequest, int orgId)
        {
            var ret = new ApiGenericResult<List<CalculoAderenciaComPerfilEColaboradorDTO>>();
            try
            {
                var alocados = await _aderenciaRepository.ListarAlocadosColabEPerfil(orgId);
                ret.Retorno = await GerarListaAderenciaSimplificada(alocados, cpfRequest, orgId);
            }
            catch (Exception e)
            {
               ExceptionUtil.GerenciarRetornoExcecao(e, CRUDEnum.Read, "Aderencia");
               
            }
            return ret;
        }

        public async Task<ApiGenericResult<List<ColaboradorAderenciaDTO>>> ListarAderenciaPerfilGestorExterno(Guid perfilId, string filtro, int cursor, int limite, int orgId, string cpfRequest)
        {
            var apiGenericResult = new ApiGenericResult<List<ColaboradorAderenciaDTO>>();

            try
            {
                var perfil = await _gestorExternoPerfilService.ObterGestorExternoPerfilPorId(perfilId, cpfRequest, orgId);
                //var skillsDescricao = perfil.Retorno.GestorExternoPerfilSkills.Where(x => x.Relevante == true).Select(x => x.Skill.Descricao).ToList();
                var skillsDescricao = perfil.Retorno.GestorExternoPerfilSkills.Select(x => x.Skill.Descricao).ToList();
                var configuracaoOrgAderencia = _buscaParametroConfiguracaoService.GetParametroConfiguracao<string>(ParametroOrgCodigoEnum.CONFIGURACAO_ADERENCIA_ORG, orgId, cpfRequest);

                var listaDeOrgs = string.IsNullOrEmpty(configuracaoOrgAderencia)
                    ? new List<string> { orgId.ToString() }
                    : configuracaoOrgAderencia.Split(',').ToList();

                var colaboradoresOrg = await _aderenciaRepository.ListarColaboradoresAderentesPorOrgIdLimite(listaDeOrgs, filtro, skillsDescricao, cursor, limite, null, perfilId);

                var configuracaoAderencia = _buscaParametroConfiguracaoService.GetParametroConfiguracao<string>(ParametroOrgCodigoEnum.CONFIGURACAO_PERCENTUAL_ADERENCIA, (int)EnumORG.FOURSYS_2, cpfRequest);
                var dicCredenciais = StringUtil.ExtrairPropriedadesSeparadasPontoEVirgula(configuracaoAderencia);

                colaboradoresOrg.ForEach(async (colaborador) =>
                {
                    var calculoAderencia = PreencherCalculoAderencia(colaborador.Ativo, colaborador.DataAdmissao, colaborador.PeriodoDTOs, colaborador.Localidade, colaborador.CustoHora, colaborador.Skills, perfil.Retorno, dicCredenciais);
                    colaborador.CalculoAderencia = calculoAderencia.CalculoAderencia;
                    colaborador.DisponibilidadeHoras = calculoAderencia.HorasDisponiveis;
                    //colaborador.DataDeDisponibilidade = calculoAderencia.DataDisponibilidade;
                    colaborador.RateCard = perfil.Retorno.RatecardPerfil;
                    colaborador.DataDeDisponibilidade = calculoAderencia.DataDisponibilidade == null ? CalcularDisponibilidade(colaborador.PeriodoDTOs, colaborador.DataAdmissao) : calculoAderencia.DataDisponibilidade;
                });

                foreach (var colab in colaboradoresOrg)
                {
                    try
                    {
                        colab.RetornoMatch = await BuscarMatchPerfilAsync(perfil, colab.Cpf, orgId);
                    }
                    catch (Exception ex)
                    {
                        _log.Log($"Erro ListarAderenciaPerfilGestorExterno ao buscar match do colab:{colab.Cpf}", LevelsEnum.Information);

                        colab.RetornoMatch = new CandidatosMatchResponse();
                        colab.RetornoMatch.DetalhamentoCalculo = new DetalhamentoCalculo();
                        colab.RetornoMatch.ComparativoPorSkill = new ComparativoPorSkill();
                        colab.RetornoMatch.DetalhamentoCalculo.HardSkills = new CategoriaScore();
                        colab.RetornoMatch.DetalhamentoCalculo.SoftSkills = new CategoriaScore();
                        colab.RetornoMatch.DetalhamentoCalculo.Idiomas = new CategoriaScore();
                        colab.RetornoMatch.DetalhamentoCalculo.Metodologias = new CategoriaScore();
                        colab.RetornoMatch.DetalhamentoCalculo.DominiosNegocio = new CategoriaScore();
                        colab.RetornoMatch.DetalhamentoCalculo.Disponibilidades = new CategoriaScore();
                        colab.RetornoMatch.ComparativoPorSkill.HardSkills = new List<SkillComparativa>();
                        colab.RetornoMatch.ComparativoPorSkill.SoftSkills = new List<SkillComparativa>();
                        colab.RetornoMatch.ComparativoPorSkill.Idiomas = new List<SkillComparativa>();
                        colab.RetornoMatch.ComparativoPorSkill.Metodologias = new List<SkillComparativa>();
                        colab.RetornoMatch.ComparativoPorSkill.DominiosNegocio = new List<SkillComparativa>();
                        colab.RetornoMatch.ComparativoPorSkill.Disponibilidades = new List<SkillComparativa>();
                    }
                }

                //apiGenericResult.Retorno = colaboradoresOrg.OrderByDescending(x => x.CalculoAderencia.TotalAderencia).Skip(cursor).Take(limite).ToList();
                apiGenericResult.Retorno = colaboradoresOrg.OrderByDescending(x => x.CalculoAderencia.TotalAderencia).ToList();
            }
            catch (Exception e)
            {
                throw;
            }
            return apiGenericResult;
        }

        public async Task<CandidatosMatchResponse> BuscarMatchPerfilAsync(ApiGenericResult<GestorExternoPerfilResult> perfil, string cpf, int orgId)
        {
            var baseRequest = new ScoreSingleCandidateRequest
            {
                HardSkills = perfil.Retorno.GestorExternoPerfilSkills
                    .Where(s => s.ItemPerfil.Id == (int)ItemPerfilEnum.COMPETENCIA && 
                               !string.IsNullOrWhiteSpace(s.Skill?.Descricao) && 
                               !string.IsNullOrWhiteSpace(s.Nivel?.Descricao))
                    .Select(s => new SkillItem { Nome = s.Skill.Descricao, Nivel = s.Nivel.Descricao, Obrigatoriedade = s.Relevante ? "obrigatorio" : "desejavel" }).ToList(),
                SoftSkills = perfil.Retorno.GestorExternoPerfilSkills
                    .Where(s => s.ItemPerfil.Id == (int)ItemPerfilEnum.SOFTSKILL && 
                               !string.IsNullOrWhiteSpace(s.Skill?.Descricao) && 
                               !string.IsNullOrWhiteSpace(s.Nivel?.Descricao))
                    .Select(s => new SkillItem { Nome = s.Skill.Descricao, Nivel = s.Nivel.Descricao, Obrigatoriedade = s.Relevante ? "obrigatorio" : "desejavel" }).ToList(),
                Metodologias = perfil.Retorno.GestorExternoPerfilSkills
                    .Where(s => s.ItemPerfil.Id == (int)ItemPerfilEnum.METODOLOGIA && 
                               !string.IsNullOrWhiteSpace(s.Skill?.Descricao) && 
                               !string.IsNullOrWhiteSpace(s.Nivel?.Descricao))
                    .Select(s => new SkillItem { Nome = s.Skill.Descricao, Nivel = s.Nivel.Descricao, Obrigatoriedade = s.Relevante ? "obrigatorio" : "desejavel" }).ToList(),
                DominiosNegocio = perfil.Retorno.GestorExternoPerfilSkills
                    .Where(s => s.ItemPerfil.Id == (int)ItemPerfilEnum.DOMINIONEGOCIO && 
                               !string.IsNullOrWhiteSpace(s.Skill?.Descricao) && 
                               !string.IsNullOrWhiteSpace(s.Nivel?.Descricao))
                    .Select(s => new SkillItem { Nome = s.Skill.Descricao, Nivel = s.Nivel.Descricao, Obrigatoriedade = s.Relevante ? "obrigatorio" : "desejavel" }).ToList(),
                Idiomas = perfil.Retorno.GestorExternoPerfilSkills
                    .Where(s => s.ItemPerfil.Id == (int)ItemPerfilEnum.IDIOMA && 
                               !string.IsNullOrWhiteSpace(s.Skill?.Descricao) && 
                               !string.IsNullOrWhiteSpace(s.Nivel?.Descricao))
                    .Select(s => new SkillItem { Nome = s.Skill.Descricao, Nivel = s.Nivel.Descricao, Obrigatoriedade = s.Relevante ? "obrigatorio" : "desejavel" }).ToList(),
                PesoHardSkills = 1,
                PesoSoftSkills = 1,
                PesoMetodologias = 1,
                PesoDominiosNegocio = 1,
                PesoIdiomas = 1,
                PesoDisponibilidades = 1,
                VisibleToOrgIds = new List<int> { orgId, EnumORG.FOURMAKERS_1.ToInt(), EnumORG.FMU_7.ToInt() },
                Disponibilidades = new List<DisponibilidadeItem>(),
                NumeroDeCandidatos = 1,
                CodigoInternoColaborador = cpf
            };

            return await _matchClient.ScoreSingleCandidate(baseRequest);
        }

        public async Task<ApiGenericResult<PerfisAderentesDTO>> ListarPerfisAderentesColaborador(ListasPerfisAderentesParams listaAderenciaColaboradorParam, string cpfRequest)
        {
            var apiGenericResult = new ApiGenericResult<PerfisAderentesDTO>();
            try
            {
                var colaborador = _buscaColaboradorRepository.GetColaborador(
                    listaAderenciaColaboradorParam.CodigoInternoColaborador, listaAderenciaColaboradorParam.OrgId);

                if (colaborador == null)
                {
                    throw new ArgumentException("Colaborador não encontrado."); // Retorna null se o colaborador não for encontrado
                }

                var orgId = new List<string>(){
                    listaAderenciaColaboradorParam.OrgId.ToString()
                };

                var colaboradoresOrg = await _aderenciaRepository.ListarColaboradoresAderentesPorOrgId(orgId, colaborador.NomeCompleto, new List<string>(), colaborador.Cpf);

                if (colaboradoresOrg.Count() == 0)
                {
                    apiGenericResult.Retorno = new();
                    return apiGenericResult;
                }

                var colaboradorSelected = colaboradoresOrg.FirstOrDefault(c => c.Cpf == colaborador.Cpf);

                if (colaboradorSelected == null)
                {
                    throw new Exception("Erro inesperado, ao menos uma linha de perfil aderente ao colaborador deve ser retornada para continuar.");
                }

                var perfisFiltrados = await _aderenciaRepository.ListarPerfisDaOrgID(
                    listaAderenciaColaboradorParam.OrgId, listaAderenciaColaboradorParam.Filtro);

                var configuracaoAderencia = _buscaParametroConfiguracaoService.GetParametroConfiguracao<string>(ParametroOrgCodigoEnum.CONFIGURACAO_PERCENTUAL_MATCH_REVERSO, (int)EnumORG.FOURSYS_2, cpfRequest);
                var dicCredenciais = StringUtil.ExtrairPropriedadesSeparadasPontoEVirgula(configuracaoAderencia);

                var listaPerfis = new List<PerfilAderenteDTO>();

                foreach (var perfil in perfisFiltrados)
                {
                    var gestorExternoPerfil = await _gestorExternoPerfilService.ObterGestorExternoPerfilPorId(perfil.IdPerfil, cpfRequest, listaAderenciaColaboradorParam.OrgId);
                    var gestorExternoPerfilResult = gestorExternoPerfil.Retorno;
                    var gestorExternoPerfilSkillsResult = gestorExternoPerfilResult.GestorExternoPerfilSkills.ToList();

                    var calculoAderencia = PreencherCalculoAderencia(colaboradorSelected.Ativo, colaboradorSelected.DataAdmissao, colaboradorSelected.PeriodoDTOs, colaboradorSelected.Localidade, colaboradorSelected.CustoHora, colaboradorSelected.Skills, gestorExternoPerfilResult, dicCredenciais);
                    perfil.CalculoAderencia = calculoAderencia.CalculoAderencia;
                    colaboradorSelected.DisponibilidadeHoras = calculoAderencia.HorasDisponiveis;
                    colaboradorSelected.DataDeDisponibilidade = calculoAderencia.DataDisponibilidade;

                    perfil.Habilidades = gestorExternoPerfilSkillsResult.Select(s => new SkillNivelDTO
                    {
                        Descricao = s.Skill.Descricao,
                        TipoSkill = s.ItemPerfil.Descricao,
                        Nivel = new NivelDTO { Descricao = s.Nivel.Descricao, Id = s.Nivel.Id },
                        Id = s.Skill.Id
                    }).ToList();

                    // só adiciona se valor > 0
                    if (perfil.CalculoAderencia.CalculosInformados.Where(ci => ci.Label == "Skills Desejáveis").Any(y => y.Valor > 0)
                       || perfil.CalculoAderencia.CalculosInformados.Where(ci => ci.Label == "Skills Imprescindíveis").Any(y => y.Valor > 0))
                    {
                        listaPerfis.Add(perfil);
                    }
                }

                // Monta o resultado final
                PerfisAderentesDTO result = new PerfisAderentesDTO
                {
                    CodigoInternoColaborador = listaAderenciaColaboradorParam.CodigoInternoColaborador,
                    NomeColaborador = colaborador.NomeCompleto,
                    ListaPerfisAderentes = listaPerfis.OrderByDescending(x => x.CalculoAderencia.TotalAderencia).Skip(listaAderenciaColaboradorParam.Cursor).Take(listaAderenciaColaboradorParam.Limite).ToList()
                };

                apiGenericResult.Retorno = result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return apiGenericResult;
        }

        private CalculoInformadoDTO CalcularPorcentagemSkillAderencia(IEnumerable<GestorExternoPerfilSkillResult> skillsPerfil, List<SkillNivelDTO> skillsColaborador, bool relevante, int valorAderencia)
        {
            string razao = relevante ? "Skills imprecindíveis: " : "Skills desejáveis: ";
            double percentual = 0;

            var skillsPerfilFiltrada = skillsPerfil.Where(skill => skill.Relevante == relevante).ToList();

            if (skillsPerfilFiltrada.Count == 0) return new CalculoInformadoDTO
            {
                Razao = razao + "Nenhuma",
                Valor = percentual,
            };

            double valorSkillDesejavel = valorAderencia / skillsPerfilFiltrada.Count;

            var skillsColaboradorRetorno = new List<string>();

            foreach (var skill in skillsPerfilFiltrada)
            {
                var habilidade = skillsColaborador.FirstOrDefault(h => h.Descricao == skill.Skill.Descricao);// procura nas habilidades do colaborador

                if (habilidade != null)// caso obtenha resultado
                {
                    skillsColaboradorRetorno.Add(habilidade.Descricao);

                    if (habilidade.Nivel.Id == skill.Nivel.Id) // compara o nivel da habilidade
                    {
                        percentual += valorSkillDesejavel; // senioridade igual retonar o percentual total
                    }
                    else
                    {
                        percentual += valorSkillDesejavel / 2; // caso a senioridade seja diferente retorna metade do percentual
                    }
                }
            }

            string concatenateSkills = skillsColaboradorRetorno.Count > 0 ? string.Join(", ", skillsColaboradorRetorno.OrderBy(x => x)) : "Nenhuma";

            return new CalculoInformadoDTO
            {
                Razao = razao + concatenateSkills,
                Valor = percentual,
            };
        }

        private CalculoInformadoDisponibilidadeDTO CalcularPorcentagemDisponibilidadeAderencia(bool ativo, DateTime? dataAdmissao, List<PeriodoDTO> periodos, int aderenciaLimite)
        {
            var retorno = new CalculoInformadoDisponibilidadeDTO();
            double aderenciaFinal = 0;
            string razaoDisponivel = "";

            var alocado = VerificarSeEstaAlocado(periodos);
            var dataDeDisponibilidade = CalcularDisponibilidade(periodos, dataAdmissao);

            if (!ativo)
            {
                aderenciaFinal = aderenciaLimite / 3;
                retorno.HorasDisponiveis = 0;
                razaoDisponivel = "Colaborador não ativo.";

                retorno.Calculo = new CalculoInformadoDTO
                {
                    Razao = razaoDisponivel,
                    Valor = aderenciaFinal,
                };

                return retorno;
            }

            if (!alocado)
            {
                aderenciaFinal = aderenciaLimite;
                retorno.HorasDisponiveis = HorasUtil.QuantidadeHorasDia;
                razaoDisponivel = "Não possui alocação.";

                retorno.Calculo = new CalculoInformadoDTO
                {
                    Razao = razaoDisponivel,
                    Valor = aderenciaFinal,
                };
                return retorno;
            }

            var disponibilidadeAderenciaDTO = CalcularDisponibilidadeAderencia(periodos, aderenciaLimite);

            retorno.DataDisponibilidade = dataDeDisponibilidade;

            if (disponibilidadeAderenciaDTO.TotalHorasAlocadasQueVaoFicarDisponiveis > 0 && disponibilidadeAderenciaDTO.TotalHorasAlocadasQueVaoFicarDisponiveis < 8)
            {
                retorno.HorasDisponiveis = disponibilidadeAderenciaDTO.TotalHorasAlocadasQueVaoFicarDisponiveis;
            }
            else
            {
                retorno.HorasDisponiveis = HorasUtil.QuantidadeHorasDia;
            }

            aderenciaFinal = disponibilidadeAderenciaDTO.Percentual;

            switch (disponibilidadeAderenciaDTO.DisponibilidadeEnum)
            {
                case DisponbilidadeAderenciaEnum.Proximos15Dias:
                    razaoDisponivel = "Disponibilidade PARCIAL ou TOTAL nos próximos 15 dias.";
                    break;

                case DisponbilidadeAderenciaEnum.Entre15e30Dias:
                    razaoDisponivel = "Disponibilidade PARCIAL ou TOTAL entre 16 e 30 dias.";
                    break;

                case DisponbilidadeAderenciaEnum.Acima30Dias:
                    razaoDisponivel = "Disponibilidade PARCIAL ou TOTAL somente após 30 dias.";
                    break;

                default:
                    break;
            }

            retorno.Calculo = new CalculoInformadoDTO
            {
                Razao = razaoDisponivel,
                Valor = aderenciaFinal,
            };

            return retorno;
        }

        private DisponibilidadeAderenciaDTO CalcularDisponibilidadeAderencia(List<PeriodoDTO> periodos, int aderenciaLimite)
        {
            if (periodos.Count() == 0)
            {
                throw new ArgumentException("Necessário períodos para executar o cálculo.");
            }

            var listaDisponibilidadeAderencia = new List<DisponibilidadeAderenciaDTO>();

            foreach (var diponibilidadeEnum in Enum.GetValues(typeof(DisponbilidadeAderenciaEnum)))
            {
                var inicioDias = 0;
                var fimDias = 0;

                switch (diponibilidadeEnum)
                {
                    case DisponbilidadeAderenciaEnum.Proximos15Dias:
                        inicioDias = 0;
                        fimDias = 15;
                        break;

                    case DisponbilidadeAderenciaEnum.Entre15e30Dias:
                        inicioDias = 16;
                        fimDias = 30;
                        break;

                    case DisponbilidadeAderenciaEnum.Acima30Dias:
                        inicioDias = 31;
                        fimDias = 9999;
                        break;

                    default:
                        break;
                }

                DateTime dataInicio = DateTime.Now.AddDays(inicioDias).Date;
                DateTime dataFim = DateTime.Now.AddDays(fimDias).Date.AddHours(23).AddMinutes(59).AddSeconds(59);

                var colaboradorFiltrado = periodos
                    .Where(p => (p.DataInicio <= dataFim && p.DataFim >= dataInicio));

                var quantidadeHorasAlocadas = (double)colaboradorFiltrado.Where(x => x.DataFim < dataFim).Sum(x => x.QuantidadeHoras);

                var disponibilidadeAdDTO = new DisponibilidadeAderenciaDTO()
                {
                    DisponibilidadeEnum = (DisponbilidadeAderenciaEnum)diponibilidadeEnum,
                    TotalHorasAlocadasQueVaoFicarDisponiveis = quantidadeHorasAlocadas
                };
                disponibilidadeAdDTO.Percentual = CalcularPercentualDisponibilidade(quantidadeHorasAlocadas, disponibilidadeAdDTO.DisponibilidadeEnum, aderenciaLimite);

                listaDisponibilidadeAderencia.Add(disponibilidadeAdDTO);
            }

            foreach (var dispo in listaDisponibilidadeAderencia)
            {
                if (dispo.DisponibilidadeEnum == DisponbilidadeAderenciaEnum.Proximos15Dias && dispo.TotalHorasAlocadasQueVaoFicarDisponiveis > 0)
                {
                    var sub = listaDisponibilidadeAderencia.Where(d => d.DisponibilidadeEnum != DisponbilidadeAderenciaEnum.Proximos15Dias).Sum(x => x.TotalHorasAlocadasQueVaoFicarDisponiveis);
                    dispo.TotalHorasAlocadasQueVaoFicarDisponiveis = HorasUtil.QuantidadeHorasDia - sub;
                    return dispo;
                }

                if (dispo.DisponibilidadeEnum == DisponbilidadeAderenciaEnum.Entre15e30Dias && dispo.TotalHorasAlocadasQueVaoFicarDisponiveis > 0)
                {
                    var sub = listaDisponibilidadeAderencia.Where(d => d.DisponibilidadeEnum != DisponbilidadeAderenciaEnum.Entre15e30Dias).Sum(x => x.TotalHorasAlocadasQueVaoFicarDisponiveis);
                    dispo.TotalHorasAlocadasQueVaoFicarDisponiveis = HorasUtil.QuantidadeHorasDia - sub;
                    return dispo;
                }

                if (dispo.DisponibilidadeEnum == DisponbilidadeAderenciaEnum.Acima30Dias && dispo.TotalHorasAlocadasQueVaoFicarDisponiveis > 0)
                {
                    var sub = listaDisponibilidadeAderencia.Where(d => d.DisponibilidadeEnum != DisponbilidadeAderenciaEnum.Acima30Dias).Sum(x => x.TotalHorasAlocadasQueVaoFicarDisponiveis);
                    dispo.TotalHorasAlocadasQueVaoFicarDisponiveis = HorasUtil.QuantidadeHorasDia - sub;
                    return dispo;
                }
            }

            return listaDisponibilidadeAderencia.FirstOrDefault();
        }

        private bool VerificarSeEstaAlocado(List<PeriodoDTO> periodos)
        {
            if (periodos.Count == 0)
            {
                return false;
            }
            return true;
        }

        public static DateTime? CalcularDisponibilidade(List<PeriodoDTO> periodos, DateTime? dataAdmissao)
        {
            DateTime? dataCompatibilidade = null;
            if (periodos != null && periodos.Any())
            {
                var alocacao = periodos
                                   .Where(p => p.QuantidadeHoras > 0)
                                   .OrderByDescending(p => p.DataFim)
                                   .FirstOrDefault();

                if (alocacao != null)
                {
                    dataCompatibilidade = alocacao.DataFim.AddDays(1);
                }
            }

            if (dataCompatibilidade == null)
            {
                dataCompatibilidade = dataAdmissao;
            }

            return dataCompatibilidade;
        }

        private double CalcularPercentualDisponibilidade(double horasAlocadas, DisponbilidadeAderenciaEnum disponibilidade, int aderenciaLimite)
        {
            double aderencia = 0;
            if (horasAlocadas >= 0)
            {
                if (disponibilidade == DisponbilidadeAderenciaEnum.Acima30Dias)
                    aderencia = 0;
                else if (disponibilidade == DisponbilidadeAderenciaEnum.Entre15e30Dias)
                    aderencia = aderenciaLimite / 3;
                else if (disponibilidade == DisponbilidadeAderenciaEnum.Proximos15Dias)
                    aderencia = (2 * aderenciaLimite) / 3;
            }
            return aderencia;
        }

        private CalculoInformadoDTO CalcularPorcentagemCustoAderencia(decimal? custoHora, decimal? rateCard, int totalAderencia)
        {
            if (custoHora.ToDecimalOuZero() == 0)
            {
                return new CalculoInformadoDTO
                {
                    Valor = 0,
                    Razao = "Custo Hora do Colaborador não informado",
                };
            }

            if (custoHora > rateCard)
            {
                return new CalculoInformadoDTO
                {
                    Valor = 0,
                    Razao = "Custo não aderente ao custo do perfil",
                };
            }

            return new CalculoInformadoDTO
            {
                Valor = totalAderencia,
                Razao = "Custo aderente ao custo do perfil",
            };
        }

        private CalculoInformadoDTO CalcularPorcentagemLocalidadeAderencia(LocalidadeDTO? localidade, GestorExternoPerfilResult perfil, int totalAderencia)
        {
            if (localidade == null && perfil.ModeloTrabalhoDescricao != ModeloTrabalhoConst.Remoto100Porcento) return new CalculoInformadoDTO
            {
                Valor = 0,
                Razao = "Modelo de Trabalho remoto.",
            };// caso seja nulo e o tipo do modelo seja diferente de remoto retona 0

            string localVagaCidade = perfil.Cidade;
            string localVagaEstado = perfil.Estado;

            string localColaboradorCidade = localidade?.Cidade;
            string localColaboradorEstado = localidade?.Estado;

            string localVaga = "Local da Vaga: " + localVagaCidade + "/" + localVagaEstado + ". ";
            string localColaborador = "Local do Colaborador: " + localColaboradorCidade + "/" + localColaboradorEstado + ". ";

            bool cidadeIgual = string.Equals(localidade?.Cidade, perfil.Cidade, StringComparison.OrdinalIgnoreCase);// compara a cidade
            string razaoCidadeIgual =
                cidadeIgual ? localVaga + localColaborador + "Reside na mesma região da vaga." : localVaga + localColaborador + "Não reside na mesma região da vaga.";
            double percentualModalidadeTrabalho = perfil.ModeloTrabalhoDescricao switch
            {
                ModeloTrabalhoConst.Remoto100Porcento => totalAderencia, // se for remoto retorna total de aderencia
                ModeloTrabalhoConst.Hibrido or ModeloTrabalhoConst.Presencial100Porcento when cidadeIgual => totalAderencia, // se for hibrido ou presencial retorna o percentual maximo havendo cidade iguais
                _ => 0 // caso não retorna 0
            };

            return new CalculoInformadoDTO
            {
                Razao = razaoCidadeIgual,
                Valor = percentualModalidadeTrabalho,
            };
        }

        private CalculoAderenciaResultadoDTO PreencherCalculoAderencia(bool ativo, DateTime? dataAdmissao, List<PeriodoDTO> periodos, LocalidadeDTO? localidade, decimal? custoHora, List<SkillNivelDTO> habilidadesColaborador, GestorExternoPerfilResult perfil, Dictionary<string, string> configuracaoAderencia)
        {
            try
            {
                var configuracao = new
                {
                    PercMaxSkillsRelevantes = configuracaoAderencia.TryGetValue("perc_max_skills_relevantes", out var percMaxSkillsRelevantes) ? percMaxSkillsRelevantes.ToIntOuZero() : 0,
                    PercMaxSkillsDesejaveis = configuracaoAderencia.TryGetValue("perc_max_skills_desejaveis", out var percMaxSkillsDesejaveis) ? percMaxSkillsDesejaveis.ToIntOuZero() : 0,
                    PercMaxDisponibilidade = configuracaoAderencia.TryGetValue("perc_max_disponibilidade", out var percMaxDisponibilidade) ? percMaxDisponibilidade.ToIntOuZero() : 0,
                    PercMaxLocalidadeColaboradorModeloTrabalho = configuracaoAderencia.TryGetValue("perc_max_localidade_colaborador_modelo_trabalho", out var percMaxLocalidade) ? percMaxLocalidade.ToIntOuZero() : 0,
                    PercMaxCustosPerfilColaborador = configuracaoAderencia.TryGetValue("perc_max_custos_perfil_colaborador", out var percMaxCustos) ? percMaxCustos.ToIntOuZero() : 0
                };

                var totalConfiguracao =
                    configuracao.PercMaxSkillsRelevantes +
                    configuracao.PercMaxSkillsDesejaveis +
                    configuracao.PercMaxDisponibilidade +
                    configuracao.PercMaxLocalidadeColaboradorModeloTrabalho +
                    configuracao.PercMaxCustosPerfilColaborador;

                // Validação da soma total dos percentuais
                if (totalConfiguracao != 100)
                {
                    throw new Exception($"A soma dos parâmetros deve ser 100, mas foi {totalConfiguracao}. Verifique a configuração.");
                }

                var percSkillsRelevantes = configuracao.PercMaxSkillsRelevantes > 0
                ? CalcularPorcentagemSkillAderencia(perfil.GestorExternoPerfilSkills, habilidadesColaborador, true, configuracao.PercMaxSkillsRelevantes)
                    : null;

                var percSkillsDesejaveis = configuracao.PercMaxSkillsDesejaveis > 0
                ? CalcularPorcentagemSkillAderencia(perfil.GestorExternoPerfilSkills, habilidadesColaborador, false, configuracao.PercMaxSkillsDesejaveis)
                    : null;

                var percDisponibilidade = configuracao.PercMaxDisponibilidade > 0
                ? CalcularPorcentagemDisponibilidadeAderencia(ativo, dataAdmissao, periodos, configuracao.PercMaxDisponibilidade)
                    : null;

                var percLocalidadeColaboradorModeloTrabalho = configuracao.PercMaxLocalidadeColaboradorModeloTrabalho > 0
                    ? CalcularPorcentagemLocalidadeAderencia(localidade, perfil, configuracao.PercMaxLocalidadeColaboradorModeloTrabalho)
                            : null;

                var percCustosPerfilColaborador = configuracao.PercMaxCustosPerfilColaborador > 0
                ? CalcularPorcentagemCustoAderencia(custoHora, perfil.RatecardPerfil, configuracao.PercMaxCustosPerfilColaborador)
                    : null;

                // Calculando o valor total de aderência
                var valorTotalAderencia =
                    (double)(
                        (percSkillsRelevantes?.Valor ?? 0) +
                        (percSkillsDesejaveis?.Valor ?? 0) +
                        (percDisponibilidade?.Calculo.Valor ?? 0) +
                        (percLocalidadeColaboradorModeloTrabalho?.Valor ?? 0) +
                        (percCustosPerfilColaborador?.Valor ?? 0));

                var listaCalculoInformado = new List<CalculoInformadoDTO>() {
                                                MontarCalculoInformado(percSkillsRelevantes, configuracao.PercMaxSkillsRelevantes, "Skills Imprescindíveis"),
                                                MontarCalculoInformado(percSkillsDesejaveis, configuracao.PercMaxSkillsDesejaveis, "Skills Desejáveis"),
                                                MontarCalculoInformado(percDisponibilidade?.Calculo, configuracao.PercMaxDisponibilidade, "Disponibilidade"),
                                                MontarCalculoInformado(percCustosPerfilColaborador, configuracao.PercMaxCustosPerfilColaborador, "Custos do Perfil X Custo do Colaborador"),
                                                MontarCalculoInformado(percLocalidadeColaboradorModeloTrabalho, configuracao.PercMaxLocalidadeColaboradorModeloTrabalho, "Localidade do Colaborador X Modelo de Trabalho do Perfil"),
                };


                var calculoAderencia = new CalculoAderenciaDTO
                {
                    CalculosInformados = listaCalculoInformado.Where(x => x is not null).ToList(),
                    TotalAderencia = valorTotalAderencia
                };

                return new CalculoAderenciaResultadoDTO
                {
                    CalculoAderencia = calculoAderencia,
                    DataDisponibilidade = percDisponibilidade?.DataDisponibilidade,
                    HorasDisponiveis = percDisponibilidade?.HorasDisponiveis ?? 0,
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao PreencherCalculoAderencia: {ex.Message}", ex);
            }
        }

        private CalculoInformadoDTO MontarCalculoInformado(CalculoInformadoDTO percCalculado, int percMaximo, string label)
        {
            return percCalculado != null
                ? new CalculoInformadoDTO
                {
                    Label = label,
                    Razao = percCalculado.Razao,
                    Valor = percCalculado.Valor,
                    ValorMaximo = percMaximo
                }
                : null;
        }

        public async Task<List<ListarCandidatosAderentesResult>> ListarAderenciaAlocadosViaMatch(string cpfRequest, Guid perfilId, int orgId, int cursor, int limite)
        {
            var perfil = await _gestorExternoPerfilService.ObterGestorExternoPerfilPorIdLimite(perfilId, cpfRequest, orgId, cursor, limite);
            
            if (perfil == null)
                throw new ApplicationException("Perfil não encontrado.");

            var candidatosMatchRequest = MapearObjetoDeRequisicaoMatch(perfil, orgId, limite);

            _log.Log("MATCH - Envio - Pefil", LevelsEnum.Information);
            _log.Log(JsonConvert.SerializeObject(candidatosMatchRequest), LevelsEnum.Information);
           
            var candidatosMatchResponse = await _matchClient.RankCandidates(candidatosMatchRequest);

            var retorno = new List<ListarCandidatosAderentesResult>();

            foreach (var candidato in candidatosMatchResponse)
            {
                try
                {
                    var dadosColaborador = await _buscaColaboradorRepository.GetColaboradorBasicoPorCodigo(candidato.CodigoInternoColaborador);
                    
                    var organizacoes = await _buscaColaboradorRepository.GetOrganizacoesColaborador(candidato.CodigoInternoColaborador);

                    var orgPrincipal = organizacoes.FirstOrDefault(o => o.AtivoNaOrg == true) ?? organizacoes.FirstOrDefault();

                    var candidatoAderente = new ListarCandidatosAderentesResult
                    {
                        Nome = candidato.Nome,
                        Codigo = candidato.CodigoInternoColaborador,
                        PercentualAderencia = candidato.Match,
                        Email = dadosColaborador?.EmailAlternativo,
                        RetornoMatch = candidato,
                        Qualificado = dadosColaborador?.Qualificado,
                        OrgId = orgPrincipal?.OrgId ?? 0,
                        OrgDescricao = orgPrincipal?.OrgDescricao ?? "",
                        AtivoNaOrg = orgPrincipal?.AtivoNaOrg,
                        Organizacoes = organizacoes,
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

        private CandidatosMatchRequest MapearObjetoDeRequisicaoMatch(ApiGenericResult<GestorExternoPerfilResult> perfil, int orgId, int limite)
        {
            var requisicao = new CandidatosMatchRequest
            {
                HardSkills = new List<HabilidadeTecnica>(),
                SoftSkills = new List<HabilidadeComportamental>(),
                Metodologias = new List<DataTransferObject.Domain.Match.Metodologia>(),
                DominiosNegocio = new List<DominioNegocio>(),
                Idiomas = new List<DataTransferObject.Domain.Match.Idioma>(),
                Disponibilidades = new List<Disponibilidade>(),

                PesoHardSkills = 1,
                PesoSoftSkills = 1,
                PesoDisponibilidades = 1,
                PesoMetodologias = 1,
                PesoDominiosNegocio = 1,
                PesoIdiomas = 1,
                VisibleToOrgIds = new List<int> { orgId, EnumORG.FOURMAKERS_1.ToInt(), EnumORG.FMU_7.ToInt() },
                NumeroDeCandidatos = limite == 0 ? 10 : limite
            };

            foreach (var skill in perfil.Retorno.GestorExternoPerfilSkills)
            {
                // Pular se contém "_INATIVO"
                if (skill.Skill?.Descricao?.ToUpper().Contains("INATIVO") == true)
                    continue;

                switch (skill.ItemPerfil.Id)
                {
                    case (int)ItemPerfilEnum.COMPETENCIA:
                        requisicao.HardSkills.Add(new HabilidadeTecnica
                        {
                            Nome = skill.Skill.Descricao,
                            Nivel = skill.Nivel.Descricao,
                            Obrigatoriedade = skill.Relevante ? "obrigatorio" : "desejavel"
                        });
                        break;

                    case (int)ItemPerfilEnum.SOFTSKILL:
                        requisicao.SoftSkills.Add(new HabilidadeComportamental
                        {
                            Nome = skill.Skill.Descricao,
                            Obrigatoriedade = skill.Relevante ? "obrigatorio" : "desejavel"
                        });
                        break;

                    case (int)ItemPerfilEnum.METODOLOGIA:
                        requisicao.Metodologias.Add(new DataTransferObject.Domain.Match.Metodologia
                        {
                            Nome = skill.Skill.Descricao,
                            Nivel = skill.Nivel.Descricao,
                            Obrigatoriedade = skill.Relevante ? "obrigatorio" : "desejavel"
                        });
                        break;

                    case (int)ItemPerfilEnum.DOMINIONEGOCIO:
                        requisicao.DominiosNegocio.Add(new DominioNegocio
                        {
                            Nome = skill.Skill.Descricao,
                            Nivel = skill.Nivel.Descricao,
                            Obrigatoriedade = skill.Relevante ? "obrigatorio" : "desejavel"
                        });
                        break;

                    case (int)ItemPerfilEnum.IDIOMA:
                        requisicao.Idiomas.Add(new DataTransferObject.Domain.Match.Idioma
                        {
                            Nome = skill.Skill.Descricao,
                            Nivel = skill.Nivel.Descricao,
                            Obrigatoriedade = skill.Relevante ? "obrigatorio" : "desejavel"
                        });
                        break;
                }
            }

            return requisicao;
        }

        //public Task<ApiGenericResult<PerfisAderentesDTO>> ListarPerfisAderentesColaborador(ListasPerfisAderentesParams listaAderenciaColaboradorParam, string cpfRequest)
        //{
        //    throw new NotImplementedException();
        //}
    }
}