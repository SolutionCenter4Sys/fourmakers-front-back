using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Colaboracao.Helper.Enum;
using Colaboracao.Helper.Util.Competencia;
using Competencia.Domain.Enums;
using Core.Domain.MapaAlocacao.GestaoDeAlocados;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfil;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfilSkill;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfilSkill.ItemPerfil;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfilSkill.Nivel;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfilSkill.Skill;
using DataTransferObject.Domain.Vaga.Enums;
using MapaDeAlocacao.Domain.Interfaces.GestaoDeAlocados.GestaoAlocadosValidaAcesso;
using MapaDeAlocacao.Domain.Interfaces.GestaoDeAlocados.GestorExternoPerfil;
using MapaDeAlocacao.Domain.Interfaces.GestaoDeAlocados.GestorExternoPerfilSkill;
using Newtonsoft.Json;
using SRS.Domain.Interfaces.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Logs.Infra.Attributes;

namespace MapaDeAlocacao.Domain.Impl.GestaoDeAlocados.GestorExternoPerfil
{
    [LogDomainClass]
    public class GestorExternoPerfilService : IGestorExternoPerfilService
    {
        public static readonly string DESCRICAO_ENTIDADE = "Perfil";

        private readonly IGestorExternoPerfilRepository _gestorExternoPerfilRepository;
        private readonly IGestorExternoPerfilValidatorService _gestorExternoPerfilValidatorService;
        private readonly IGestorExternoPerfilSkillService _gestorExternoPerfilSkillService;
        private readonly IGestaoAlocadosValidarAcessoService _gestaoAlocadosValidarAcessoService;
        private readonly IVagaService _vagaService;
        private readonly IClassificacaoService _classificacaoService;
        private readonly ILogCore _log;
        private IDBConnectionUnitOfWork _dbConnectionUnitOfWork;

        public GestorExternoPerfilService(IGestorExternoPerfilRepository gestorExternoPerfilRepository,
                                IGestorExternoPerfilValidatorService gestorExternoPerfilValidatorService,
                                IGestorExternoPerfilSkillService gestorExternoPerfilSkillService,
                                IGestaoAlocadosValidarAcessoService gestaoAlocadosValidarAcessoService,
                                IDBConnectionUnitOfWork dbConnectionUnitOfWork,
                                IVagaService vagaService,
                                IClassificacaoService classificacaoService,
                                ILogCore log)
        {
            _gestorExternoPerfilRepository = gestorExternoPerfilRepository;
            _gestorExternoPerfilValidatorService = gestorExternoPerfilValidatorService;
            _gestorExternoPerfilSkillService = gestorExternoPerfilSkillService;
            _gestaoAlocadosValidarAcessoService = gestaoAlocadosValidarAcessoService;
            _dbConnectionUnitOfWork = dbConnectionUnitOfWork;
            _vagaService = vagaService;
            _classificacaoService = classificacaoService;
            _log = log;
        }

        public async Task<ApiGenericResult<IEnumerable<GestorExternoPerfilResult>>> ListarGestorExternoPerfis(string cpfRequest, int orgId)
        {
            var apiGenericResult = new ApiGenericResult<IEnumerable<GestorExternoPerfilResult>>();

            try
            {
                _gestaoAlocadosValidarAcessoService.ValidaAcessoCadastroGestaoAlocados(cpfRequest, orgId);

                var result = await _gestorExternoPerfilRepository.ListarGestorExternoPerfisAsync();

                foreach (var gestorExternoPerfil in result)
                {
                    var resultSkill = await _gestorExternoPerfilSkillService.ListarSkillsPerfilGestorExternoPorId(gestorExternoPerfil.Id);
                    gestorExternoPerfil.GestorExternoPerfilSkills = resultSkill.Retorno;
                }

                apiGenericResult.Retorno = result;
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, DESCRICAO_ENTIDADE);
            }

            return apiGenericResult;
        }

        public async Task<ApiGenericResult<GestorExternoPerfilResult>> ObterGestorExternoPerfilPorId(Guid id, string cpfRequest, int orgId)
        {
            var apiGenericResult = new ApiGenericResult<GestorExternoPerfilResult>();

            try
            {
                _gestaoAlocadosValidarAcessoService.ValidaAcessoCadastroGestaoAlocados(cpfRequest, orgId);

                var result = await _gestorExternoPerfilRepository.ObterGestorExternoPerfilPorIdAsync(id);

                if (result == null)
                {
                    ExceptionUtil.NaoEncontrado(DESCRICAO_ENTIDADE);
                }

                var resultSkill = await _gestorExternoPerfilSkillService.ListarSkillsPerfilGestorExternoPorId(result.Id);
                result.GestorExternoPerfilSkills = resultSkill.Retorno;

                apiGenericResult.Retorno = result;
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, DESCRICAO_ENTIDADE);
            }

            return apiGenericResult;
        }

        public async Task<ApiGenericResult<IEnumerable<GestorExternoPerfilSkillResult>>> ObterListarDeSkillsGestorExternoPorGestorExternoIdAsync(Guid id)
        {
            var apiGenericResult = new ApiGenericResult<IEnumerable<GestorExternoPerfilSkillResult>>();

            try
            {

                var resultSkill = await _gestorExternoPerfilSkillService.ListarSkillsPerfilGestorExternoPorId(id);
                apiGenericResult.Retorno = resultSkill.Retorno;
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, DESCRICAO_ENTIDADE);
            }

            return apiGenericResult;
        }
        
        public async Task<ApiGenericResult<GestorExternoPerfilResult>> ObterGestorExternoPerfilPorIdLimite(Guid id, string cpfRequest, int orgId, int cursor, int limite)
        {
            var apiGenericResult = new ApiGenericResult<GestorExternoPerfilResult>();

            try
            {
                _gestaoAlocadosValidarAcessoService.ValidaAcessoCadastroGestaoAlocados(cpfRequest, orgId);

                var result = await _gestorExternoPerfilRepository.ObterGestorExternoPerfilPorIdLimiteAsync(id, cursor, limite);

                if (result == null)
                {
                    ExceptionUtil.NaoEncontrado(DESCRICAO_ENTIDADE);
                }

                var resultSkill = await _gestorExternoPerfilSkillService.ListarSkillsPerfilGestorExternoPorId(result.Id);
                result.GestorExternoPerfilSkills = resultSkill.Retorno;

                apiGenericResult.Retorno = result;
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, DESCRICAO_ENTIDADE);
            }

            return apiGenericResult;
        }

        public async Task<ApiGenericResult<List<GestorExternoPerfilResult>>> ObterGestoresExternoPerfilPorListaDeIds(List<Guid> ids, int orgId)
        {
            var apiGenericResult = new ApiGenericResult<List<GestorExternoPerfilResult>>();

            try
            {

                var results = await _gestorExternoPerfilRepository.ObterGestorExternoPerfilPorListaIdsAsync(ids);

                if (results == null)
                {
                    ExceptionUtil.NaoEncontrado(DESCRICAO_ENTIDADE);
                }

                foreach (var result in results)
                {
                    var resultSkill = await _gestorExternoPerfilSkillService.ListarSkillsPerfilGestorExternoPorId(result.Id);
                    result.GestorExternoPerfilSkills = resultSkill.Retorno;
                }

                apiGenericResult.Retorno = results.ToList();
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, DESCRICAO_ENTIDADE);
            }

            return apiGenericResult;
        }

        public async Task<ApiGenericResult<GestorExternoPerfilResult>> InserirGestorExternoPerfil(GestorExternoPerfilInput gestorExternoPerfilInput, string cpfRequest, int orgId)
        {
            var apiGenericResult = new ApiGenericResult<GestorExternoPerfilResult>();

            try
            {
                _gestaoAlocadosValidarAcessoService.ValidaAcessoCadastroGestaoAlocados(cpfRequest, orgId);

                gestorExternoPerfilInput.ConfigurarParaPersistencia(orgId, cpfRequest, Guid.NewGuid());
                await _gestorExternoPerfilValidatorService.ValidaGestorExternoPerfil(gestorExternoPerfilInput, CRUDEnum.Create);

                _dbConnectionUnitOfWork.BeginTransaction();

                var result = await _gestorExternoPerfilRepository.InserirGestorExternoPerfilAsync(gestorExternoPerfilInput);

                if (result == null)
                {
                    ExceptionUtil.NaoInserido(DESCRICAO_ENTIDADE);
                }

                result.GestorExternoPerfilSkills = await ProcessarGestorExternoPerfilSkillsAsync(gestorExternoPerfilInput.GestorExternoPerfilSkills, result.Id, cpfRequest, orgId);

                _dbConnectionUnitOfWork.Commit();

                var idVaga = await CriarVagaRecrutamento(gestorExternoPerfilInput, cpfRequest);
                result.IdVaga = idVaga;

                // Classificar perfil após inserção
                try
                {
                    await _classificacaoService.AtualizarClassificacaoPerfilAsync(result.Id);
                }
                catch (Exception ex)
                {
                    _log.Log($"Erro ao classificar perfil após inserção: {ex}", DataTransferObject.Domain.Log.LevelsEnum.Error);
                }

                apiGenericResult.Retorno = result;
            }
            catch (Exception ex)
            {
                _dbConnectionUnitOfWork.SafeRollback();
                _log.Log($"Erro ao criar Perfil: {ex}", DataTransferObject.Domain.Log.LevelsEnum.Information);
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Create, DESCRICAO_ENTIDADE);
            }

            return apiGenericResult;
        }

        private async Task<Guid?> CriarVagaRecrutamento(GestorExternoPerfilInput gestorExternoPerfilInput, string cpfRequest)
        {
            //TODO esta estrutura serah substituida por uma chamada num novo servico
            try
            {
                if (AtendeOsCriteriosDeAutomatizacao(gestorExternoPerfilInput))
                    return await _vagaService.CriarVagaAutomaticamenteAPartirDePerfilDeAtuacao(gestorExternoPerfilInput, cpfRequest);
            }
            catch (Exception ex)
            {
                _log.Log("Erro ao criar uma vaga automaticamente a partir de perfil", DataTransferObject.Domain.Log.LevelsEnum.Error);
                _log.Log(JsonConvert.SerializeObject(ex), DataTransferObject.Domain.Log.LevelsEnum.Error);
            }
            
            return null;
        }

        private static bool AtendeOsCriteriosDeAutomatizacao(GestorExternoPerfilInput gestorExternoPerfilInput)
            => GestorExternoPerfilCriteriosAutomatizacao.Atende(gestorExternoPerfilInput);

        public async Task<ApiGenericResult<GestorExternoPerfilResult>> AtualizarGestorExternoPerfil(GestorExternoPerfilInput gestorExternoPerfilInput, Guid id, string cpfRequest, int orgId)
        {
            var apiGenericResult = new ApiGenericResult<GestorExternoPerfilResult>();

            try
            {
                _gestaoAlocadosValidarAcessoService.ValidaAcessoCadastroGestaoAlocados(cpfRequest, orgId);

                gestorExternoPerfilInput.ConfigurarParaPersistencia(orgId, cpfRequest, id);
                await _gestorExternoPerfilValidatorService.ValidaGestorExternoPerfil(gestorExternoPerfilInput, CRUDEnum.Update);

                _dbConnectionUnitOfWork.BeginTransaction();

                var result = await _gestorExternoPerfilRepository.AtualizarGestorExternoPerfilAsync(gestorExternoPerfilInput);

                if (result == null)
                {
                    ExceptionUtil.NaoAtualizado(DESCRICAO_ENTIDADE);
                }

                result.GestorExternoPerfilSkills = await ProcessarGestorExternoPerfilSkillsAsync(gestorExternoPerfilInput.GestorExternoPerfilSkills, result.Id, cpfRequest, orgId);

                _dbConnectionUnitOfWork.Commit();

                await ModificarVagaAssociada(gestorExternoPerfilInput, cpfRequest);

                // Classificar perfil após atualização
                try
                {
                    await _classificacaoService.AtualizarClassificacaoPerfilAsync(result.Id);
                }
                catch (Exception ex)
                {
                    _log.Log($"Erro ao classificar perfil após atualização: {ex}", DataTransferObject.Domain.Log.LevelsEnum.Error);
                }

                apiGenericResult.Retorno = result;
            }
            catch (Exception ex)
            {
                _dbConnectionUnitOfWork.SafeRollback();
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Update, DESCRICAO_ENTIDADE);
            }

            return apiGenericResult;
        }

        private async Task<string> ModificarVagaAssociada(GestorExternoPerfilInput gestorExternoPerfilInput, string cpfRequest)
        {
            //TODO esta estrutura serah substituida por uma chamada num novo servico
            try
            {
                _log.Log("Fluxo de edicao ou criacao de vaga automatico iniciado (a partir de um perfil)", DataTransferObject.Domain.Log.LevelsEnum.Information);

                var vagasRecrutamento = await _vagaService.ObterTodasVagasPorIdEGestorExternoPerfilIdRecrutamento(gestorExternoPerfilInput.Id);
                var atendeOsCriteriosDeAutomatizacao = AtendeOsCriteriosDeAutomatizacao(gestorExternoPerfilInput);
                var temVagasAssociadas = vagasRecrutamento.Retorno is not null && vagasRecrutamento.Retorno.Any();

                _log.Log($"Perfil {gestorExternoPerfilInput.Id} - atendeOsCriteriosDeAutomatizacao: {atendeOsCriteriosDeAutomatizacao} - temVagasAssociadas: {temVagasAssociadas} - quantidade: {vagasRecrutamento.Retorno?.Count() ?? 0}", DataTransferObject.Domain.Log.LevelsEnum.Information);

                if (!atendeOsCriteriosDeAutomatizacao & !temVagasAssociadas)
                    return "NAO atende aos criterios e NAO tem vagas associadas";

                if (!atendeOsCriteriosDeAutomatizacao & temVagasAssociadas)
                {
                    // Cancela todas as vagas associadas ao perfil
                    foreach (var vaga in vagasRecrutamento.Retorno)
                    {
                        await _vagaService.CancelarVagaRecrutamento(vaga.Codigo.ToInt(), cpfRequest);
                    }
                    return $"NAO atende aos criterios e SIM tem {vagasRecrutamento.Retorno.Count()} vagas associadas - todas canceladas";
                }

                if (temVagasAssociadas)
                {
                    // Atualiza todas as vagas associadas ao perfil
                    foreach (var vaga in vagasRecrutamento.Retorno)
                    {
                        if (vaga.StatusVagaCod == StatusVagaRecrutamento.Cancelada.ToInt().ToString())
                        {
                            vaga.StatusVagaCod = StatusVagaRecrutamento.BancoDeTalentos.ToInt().ToString();
                            await _vagaService.AtualizarVagaCriadaAutomaticamenteRecrutamento(vaga, gestorExternoPerfilInput, cpfRequest);
                        }

                        if (vaga.StatusVagaCod == StatusVagaRecrutamento.BancoDeTalentos.ToInt().ToString() ||
                            vaga.StatusVagaCod == StatusVagaRecrutamento.EmFoco.ToInt().ToString())
                        {
                            await _vagaService.AtualizarVagaCriadaAutomaticamenteRecrutamento(vaga, gestorExternoPerfilInput, cpfRequest);
                        }
                    }
                    return $"SIM atende aos criterios e SIM tem {vagasRecrutamento.Retorno.Count()} vagas associadas - todas atualizadas";
                }
                else
                {
                    var _ = await _vagaService.CriarVagaAutomaticamenteAPartirDePerfilDeAtuacao(gestorExternoPerfilInput, cpfRequest);

                    return "SIM atende aos criterios e NAO tem vagas associadas - nova vaga criada";
                }
            }
            catch (Exception ex)
            {
                _log.Log("Erro ao criar uma vaga automaticamente a partir de perfil", DataTransferObject.Domain.Log.LevelsEnum.Error);
                _log.Log(JsonConvert.SerializeObject(ex), DataTransferObject.Domain.Log.LevelsEnum.Error);

                return $"ERRO - {ex}";
            }
        }

        public async Task<ApiGenericResult> DeletarGestorExternoPerfil(Guid id, string cpfRequest, int orgId)
        {
            var apiGenericResult = new ApiGenericResult();

            try
            {
                _gestaoAlocadosValidarAcessoService.ValidaAcessoCadastroGestaoAlocados(cpfRequest, orgId);

                _dbConnectionUnitOfWork.BeginTransaction();

                var gestorExternoPerfilResult = (await ObterGestorExternoPerfilPorId(id, cpfRequest, orgId)).Retorno;

                var gestorExternoPerfilInput = new GestorExternoPerfilInput();
                gestorExternoPerfilInput.AtualizarPropriedadesDaClasseBase(gestorExternoPerfilResult);
                gestorExternoPerfilInput.ConfigurarParaPersistencia(orgId, cpfRequest, id);

                await _gestorExternoPerfilValidatorService.ValidaGestorExternoPerfil(gestorExternoPerfilInput, CRUDEnum.Delete);

                var sucesso = await _gestorExternoPerfilRepository.DeletarGestorExternoPerfilAsync(id);

                if (!sucesso)
                {
                    ExceptionUtil.NaoExcluido(DESCRICAO_ENTIDADE);
                }

                _dbConnectionUnitOfWork.Commit();

                await CancelarVagaAssociada(gestorExternoPerfilInput, cpfRequest);

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

        private async Task CancelarVagaAssociada(GestorExternoPerfilInput gestorExternoPerfilInput, string cpfRequest)
        {
            try
            {
                _log.Log("Fluxo de cancelamento automatico de vaga iniciado (a partir de um perfil)", DataTransferObject.Domain.Log.LevelsEnum.Information);

                var vagasRecrutamento = await _vagaService.ObterTodasVagasPorIdEGestorExternoPerfilIdRecrutamento(gestorExternoPerfilInput.Id);
                var atendeOsCriteriosDeAutomatizacao = AtendeOsCriteriosDeAutomatizacao(gestorExternoPerfilInput);
                var temVagasAssociadas = vagasRecrutamento.Retorno is not null && vagasRecrutamento.Retorno.Any();

                if (!temVagasAssociadas)
                    return;

                _log.Log($"Cancelando {vagasRecrutamento.Retorno.Count()} vagas associadas ao perfil {gestorExternoPerfilInput.Id}", DataTransferObject.Domain.Log.LevelsEnum.Information);

                // Cancela todas as vagas associadas ao perfil
                foreach (var vaga in vagasRecrutamento.Retorno)
                {
                    vaga.StatusVagaCod = StatusVagaRecrutamento.Cancelada.ToInt().ToString();
                    await _vagaService.AtualizarVagaCriadaAutomaticamenteRecrutamento(vaga, gestorExternoPerfilInput, cpfRequest);
                }
            }
            catch (Exception ex)
            {
                _log.Log("Erro ao cancelar vagas automaticamente a partir de perfil", DataTransferObject.Domain.Log.LevelsEnum.Error);
                _log.Log(JsonConvert.SerializeObject(ex), DataTransferObject.Domain.Log.LevelsEnum.Error);
            }
        }

        private async Task<IEnumerable<GestorExternoPerfilSkillResult>> ProcessarGestorExternoPerfilSkillsAsync(List<GestorExternoPerfilSkillInput> listaGestorExternoPerfilSkills, Guid gestorExternoPerfilId, string cpfRequest, int orgId)
        {
            var result = Enumerable.Empty<GestorExternoPerfilSkillResult>();

            try
            {
                await _gestorExternoPerfilSkillService.DeletarGestorExternoPerfilSkillPorGestorExternoPerfilIdAsync(gestorExternoPerfilId);

                if (listaGestorExternoPerfilSkills != null && listaGestorExternoPerfilSkills.Any())
                {
                    foreach (var skill in listaGestorExternoPerfilSkills)
                    {
                        if (skill.Skill.Id != 0)
                        {
                            await _gestorExternoPerfilSkillService.InserirGestorExternoPerfilSkill(skill, gestorExternoPerfilId, cpfRequest);
                        }
                        else
                        {
                            var novaCompetencia = await _gestorExternoPerfilSkillService.InserirCompetenciaAPartirDeUmPerfil(skill.Skill.Descricao, CompetenciaUtils.ConverterPerfilItemParaTipoCompetenciaSRS((ItemPerfilEnum)skill.ItemPerfil.Id), cpfRequest);
                            skill.Skill.Id = novaCompetencia.IdCompetencia;
                            await _gestorExternoPerfilSkillService.InserirGestorExternoPerfilSkill(skill, gestorExternoPerfilId, cpfRequest);
                        }
                    }
                }

                var skillsResult = await _gestorExternoPerfilSkillService.ListarSkillsPerfilGestorExternoPorId(gestorExternoPerfilId);
                result = skillsResult.Retorno;
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, $"Skills do {DESCRICAO_ENTIDADE}");
            }

            return result;
        }
        public async Task CadastrarPerfisLegado(CadastrarPerfisLegado param, string cpf, int orgId)
        {
            _log.Log($"Iniciando processo de criacao de perfis legados para {param.Quantidade} perfis", DataTransferObject.Domain.Log.LevelsEnum.Information);

            var idsPerfis = await _gestorExternoPerfilRepository.BuscarIdsPerfis(param.Quantidade);

            foreach (var idPerfil in idsPerfis)
            {
                _log.Log($"Buscando perfil {idPerfil.TbGestorExternoPerfilId}", DataTransferObject.Domain.Log.LevelsEnum.Information);
                var perfil = await ObterGestorExternoPerfilPorId(Guid.Parse(idPerfil.TbGestorExternoPerfilId), cpf, orgId);

                _log.Log($"Mapeando {idPerfil.TbGestorExternoPerfilId}", DataTransferObject.Domain.Log.LevelsEnum.Information);
                var mapeado = MapToInput(perfil.Retorno);

                await _gestorExternoPerfilRepository.IniciarProcessamento(idPerfil);

                _log.Log($"Enviando para validacaoes de automatizacao {idPerfil.TbGestorExternoPerfilId}", DataTransferObject.Domain.Log.LevelsEnum.Information);
                var retorno = await ModificarVagaAssociada(mapeado, cpf);

                await _gestorExternoPerfilRepository.FinalizarProcessamento(idPerfil, retorno);
            }
        }

        private static GestorExternoPerfilInput MapToInput(GestorExternoPerfilResult result)
        {
            return new GestorExternoPerfilInput
            {
                Id = result.Id,
                CodGestorExterno = result.CodGestorExterno,
                NomePerfil = result.NomePerfil,
                CustoPerfil = result.CustoPerfil,
                RatecardPerfil = result.RatecardPerfil,
                InformacoesRelevantes = result.InformacoesRelevantes,
                PermanenciaId = result.PermanenciaId,
                ModeloTrabalhoId = result.ModeloTrabalhoId,
                ModeloTrabalhoDescricao = result.ModeloTrabalhoDescricao,
                ProfissionalLocalidadeId = result.ProfissionalLocalidadeId,
                Cidade = result.Cidade,
                Estado = result.Estado,
                HibridoDias = result.HibridoDias,
                Cep = result.Cep,
                GestorExternoPerfilSkills = result.GestorExternoPerfilSkills?
                    .Select(MapSkillToInput)
                    .ToList() ?? new List<GestorExternoPerfilSkillInput>()
            };
        }

        private static GestorExternoPerfilSkillInput MapSkillToInput(GestorExternoPerfilSkillResult result)
        {
            return new GestorExternoPerfilSkillInput
            {
                Relevante = result.Relevante,

                ItemPerfil = result.ItemPerfil != null
                    ? new ItemPerfilInput
                    {
                        Id = result.ItemPerfil.Id
                    }
                    : null,

                Skill = result.Skill != null
                    ? new SkillInput
                    {
                        Id = result.Skill.Id,
                        Descricao = result.Skill.Descricao
                    }
                    : null,

                Nivel = result.Nivel != null
                    ? new NivelInput
                    {
                        Id = result.Nivel.Id
                    }
                    : null
            };
        }
    }
}