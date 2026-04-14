using ApiClient.Domain.Interfaces;
using Labs.Domain.Interfaces;
using CargaCore.Domain.Interfaces;
using Colaboracao.Core;
using Colaboracao.Helper;
using Colaboracao.Helper.Enum;
using Core.Domain;
using Core.Domain.MapaAlocacao.GestaoDeAlocados;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.Aderencia;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.ClienteOrg;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.ColaboradoresAlocados;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoAreaAtuacao.Permanencia;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfil;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfil.ModeloTrabalho;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfil.ProfissionalLocalidade;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.RateCardsDosPerfis;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.SincronizarCRM;
using DataTransferObject.Domain.MapaDeAlocacao.Perfil;
using DataTransferObject.Domain.Pricing.Equipe;
using DataTransferObject.Domain.Match;
using DataTransferObject.Domain.Util.Enum;
using DataTransferObject.Domain.Vaga;
using Foursys.Domain.Interfaces.Services;
using MapaDeAlocacao.Domain.Interfaces.GestaoDeAlocados;
using MapaDeAlocacao.Domain.Interfaces.GestaoDeAlocados.GestaoAlocadosValidaAcesso;
using MapaDeAlocacao.Domain.Interfaces.GestaoDeAlocados.GestorExternoPerfil;
using MapaDeAlocacao.Domain.Interfaces.Perfil;
using MapaDeAlocacao.Domain.Interfaces.Perfil.PerfilValidaAcesso;
using Newtonsoft.Json;
using Sprache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataTransferObject.Domain.Log;

using Logs.Infra.Attributes;

namespace MapaDeAlocacao.Domain.Impl.Perfil
{
    [LogDomainClass]
    public class PerfilService : IPerfilService
    {
        private readonly IPerfilRepository _repository;
        private readonly IPerfilValidarAcessoService _perfilValidarAcessoService;
        private readonly ILogCore _log;
        private readonly IExtractorService _extractorService;
        private readonly IMatchService _matchService;

        public PerfilService(IPerfilRepository gestaoAlocadosRepository,
                                     IPerfilValidarAcessoService perfilValidarAcessoService,
                                     ILogCore log,
                                     IExtractorService extractorService,
                                     IMatchService matchService)
        {
            _repository = gestaoAlocadosRepository;
            _perfilValidarAcessoService = perfilValidarAcessoService;
            _log = log;
            _extractorService = extractorService;
            _matchService = matchService;
        }

        public async Task<BuscarQuantidadesPessoasMatchSkillResult> BuscarQuantidadesPessoasMatchSkill(BuscarQuantidadesPessoasMatchSkillInput input, int orgId)
        {
            try
            {
                // Criar uma requisição de match baseada na skill e senioridade fornecidas
                var candidatosMatchRequest = new CandidatosMatchRequest
                {
                    HardSkills = input.HardSkills is null ? new List<HabilidadeTecnica>() : input.HardSkills,
                    SoftSkills = input.SoftSkills is null ? new List<HabilidadeComportamental>() : input.SoftSkills,
                    Metodologias = input.Metodologias is null ? new List<Metodologia>() : input.Metodologias,
                    DominiosNegocio = input.DominiosNegocio is null ? new List<DominioNegocio>() : input.DominiosNegocio,
                    Idiomas = input.Idiomas is null ? new List<Idioma>() : input.Idiomas,
                    Disponibilidades = input.Disponibilidades is null ? new List<Disponibilidade>() : input.Disponibilidades,

                    PesoHardSkills = 1,
                    PesoSoftSkills = 1,
                    PesoDisponibilidades = 1,
                    PesoMetodologias = 1,
                    PesoDominiosNegocio = 1,
                    PesoIdiomas = 1,
                    VisibleToOrgIds = new List<int> { EnumORG.FOURMAKERS_1.ToInt(), EnumORG.FMU_7.ToInt(), orgId },
                    NumeroDeCandidatos = 100
                };

                //_log.Log("MATCH - Envio - BuscarQuantidadesPessoasMatchSkill", LevelsEnum.Information);
                //_log.Log(JsonConvert.SerializeObject(candidatosMatchRequest), LevelsEnum.Information);

                var candidatosMatchResponse = await _matchService.RankCandidates(candidatosMatchRequest);

                // Contar a quantidade de pessoas encontradas
                var quantidadePessoas = candidatosMatchResponse?.Count() ?? 0;

                // Criar o resultado com a quantidade de pessoas encontradas
                var result = new BuscarQuantidadesPessoasMatchSkillResult
                {
                    QuantidadePessoas = quantidadePessoas
                };

                //_log.Log($"MATCH - Resultado - Quantidade de pessoas encontradas: {quantidadePessoas}", LevelsEnum.Information);

                return result;
            }
            catch (Exception ex)
            {
                //_log.Log($"Erro ao buscar quantidades de pessoas match skill: {ex.Message}", LevelsEnum.Error);
                //_log.Log(ex.StackTrace, LevelsEnum.Error);
                throw new ApplicationException($"Erro ao buscar quantidades de pessoas match skill: {ex.Message}");
            }
        }

        public async Task<int> CountPerfis(int orgId)
        {
            return await _repository.CountPerfis(orgId);
        }

        public async Task<ApiGenericResult<IEnumerable<ListarPerfisResult>>> ListarPerfis(string dataInicio, string dataFim, string cliente, string cpf, int limite, int cursor, string busca, int orgId)
        {
            var apiGenericResult = new ApiGenericResult<IEnumerable<ListarPerfisResult>>();
            try
            {
                _perfilValidarAcessoService.ValidaAcessoCadastroGestaoAlocados(cpf, orgId);
                ValidacaoUtil.ObrigaCursorLimite(cursor, limite);

                await ValidarCampos(dataInicio, dataFim);

                DateTime.TryParse(dataInicio, out DateTime dtInicio);
                DateTime.TryParse(dataFim, out DateTime dtFim);

                if (String.IsNullOrEmpty(dataFim))
                    dtFim = DateTime.Now.AddDays(1);

                var result = await _repository.ListarPerfis(dtInicio, dtFim, cliente, cpf, limite, cursor, busca, orgId);

                if (result == null)
                    throw new ApplicationException("Erro ao Listar Perfis.");

                apiGenericResult.Retorno = result;
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, nameof(ListarPerfis));
            }

            return apiGenericResult;
        }

        public async Task<ObterEstatisticasMatchSkillSenioridadeResult> ObterEstatisticasMatchSkillSenioridade(ObterEstatisticasMatchSkillSenioridadeInput input)
        {
            try
            {
                // Criar uma requisição de match baseada no input fornecido
                var candidatosMatchRequest = new CandidatosMatchRequest
                {
                    HardSkills = input.HardSkills ?? new List<HabilidadeTecnica>(),
                    SoftSkills = input.SoftSkills ?? new List<HabilidadeComportamental>(),
                    Metodologias = input.Metodologias ?? new List<DataTransferObject.Domain.Match.Metodologia>(),
                    DominiosNegocio = input.DominiosNegocio ?? new List<DominioNegocio>(),
                    Idiomas = input.Idiomas ?? new List<DataTransferObject.Domain.Match.Idioma>(),
                    Disponibilidades = input.Disponibilidades ?? new List<Disponibilidade>(),

                    PesoHardSkills = 1,
                    PesoSoftSkills = 1,
                    PesoDisponibilidades = 1,
                    PesoMetodologias = 1,
                    PesoDominiosNegocio = 1,
                    PesoIdiomas = 1,
                    VisibleToOrgIds = new List<int> { EnumORG.FOURMAKERS_1.ToInt(), EnumORG.FMU_7.ToInt() },
                    NumeroDeCandidatos = 100
                };

                //_log.Log("MATCH - Envio - ObterEstatisticasMatchSkillSenioridade", LevelsEnum.Information);
                //_log.Log(JsonConvert.SerializeObject(candidatosMatchRequest), LevelsEnum.Information);

                var candidatosMatchResponse = await _matchService.RankCandidates(candidatosMatchRequest);

                // Processar estatísticas por faixa de match
                var estatisticas = ProcessarEstatisticasPorFaixa(candidatosMatchResponse);

                //_log.Log($"MATCH - Resultado - Total de pessoas encontradas: {candidatosMatchResponse?.Count() ?? 0}", LevelsEnum.Information);

                return estatisticas;
            }
            catch (Exception ex)
            {
                //_log.Log($"Erro ao obter estatísticas match skill senioridade: {ex.Message}", LevelsEnum.Error);
                //_log.Log(ex.StackTrace, LevelsEnum.Error);
                throw new ApplicationException($"Erro ao obter estatísticas match skill senioridade: {ex.Message}");
            }
        }

        private ObterEstatisticasMatchSkillSenioridadeResult ProcessarEstatisticasPorFaixa(IEnumerable<CandidatosMatchResponse> candidatosMatchResponse)
        {
            var result = new ObterEstatisticasMatchSkillSenioridadeResult();

            if (candidatosMatchResponse == null || !candidatosMatchResponse.Any())
            {
                return result;
            }

            var faixas = new[]
            {
                new { Minimo = 90.0, Maximo = 100.0, Descricao = "90% a 100%" },
                new { Minimo = 80.0, Maximo = 89.99, Descricao = "80% a 90%" },
                new { Minimo = 70.0, Maximo = 79.99, Descricao = "70% a 80%" },
                new { Minimo = 60.0, Maximo = 69.99, Descricao = "60% a 70%" },
                new { Minimo = 50.0, Maximo = 59.99, Descricao = "50% a 60%" },
                new { Minimo = 40.0, Maximo = 49.99, Descricao = "40% a 50%" },
                new { Minimo = 40.0, Maximo = 49.99, Descricao = "30% a 40%" },
                new { Minimo = 40.0, Maximo = 49.99, Descricao = "20% a 30%" },
                new { Minimo = 40.0, Maximo = 49.99, Descricao = "10% a 20%" },
                new { Minimo = 40.0, Maximo = 49.99, Descricao = "0% a 10%" }
            };

            foreach (var faixa in faixas)
            {
                var quantidade = candidatosMatchResponse.Count(c =>
                    c.Match >= faixa.Minimo && c.Match <= faixa.Maximo);

                result.EstatisticasPorFaixa.Add(new EstatisticaFaixaMatch
                {
                    Faixa = faixa.Descricao,
                    QuantidadePessoas = quantidade,
                    PercentualMinimo = faixa.Minimo,
                    PercentualMaximo = faixa.Maximo
                });
            }

            result.TotalPessoas = candidatosMatchResponse.Count();

            return result;
        }

        private async Task ValidarCampos(string dataInicio, string dataFim)
        {
            var campos = new List<CampoValidacao>();

            campos.Add(new("Data Inicio", dataInicio, TipoValidacaoEnum.ValidarData));
            campos.Add(new("Data Fim", dataFim, TipoValidacaoEnum.ValidarData));

            await ValidadorCamposUtil.ValidaCampos(campos);
        }

        public async Task<ExtrairPerfilDeUmPromptResponse> ExtrairPerfilDeUmPrompt(ExtrairPerfilDeUmPromptRequest request, int orgId, string? codigoInternoColaborador = null)
        {
            try
            {
                var logContext = new DataTransferObject.Domain.Labs.ExtrairPerfilLogContext
                {
                    OrgId = orgId,
                    CodigoInternoColaborador = codigoInternoColaborador
                };
                return await _extractorService.ExtrairPerfilDeUmPrompt(request, logContext);
            }
            catch (Exception ex)
            {
                _log.Log($"Erro ao extrair perfil de um prompt: {ex.Message}", LevelsEnum.Error);
                _log.Log(ex.StackTrace, LevelsEnum.Error);
                throw;
            }
        }
    }
}