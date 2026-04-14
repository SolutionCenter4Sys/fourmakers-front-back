using ApiClient.Domain.Interfaces;
using Colaboracao.Core;
using Colaboracao.Core.Exceptions;
using Colaboracao.Helper;
using Colaborador.Domain.Interfaces.Services;
using Competencia.Domain.Interfaces.Services;
using Competencia.Domain.Interfaces.Services.Metodologia;
using Core.Domain.Colaborador;
using Core.Domain.Usuario;
using Core.Domain.Usuario.Permissao;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Formacao;
using DataTransferObject.Domain.Idioma;
using DataTransferObject.Domain.Linkedin;
using DataTransferObject.Domain.Usuario;
using Formacao.Domain.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colaboracao.Helper.Util;
using Microsoft.AspNetCore.Http;
using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;

using Logs.Infra.Attributes;
using Microsoft.IdentityModel.Tokens;

namespace Colaborador.Domain
{
    [LogDomainClass]
    public class LinkedinService : ILinkedinService
    {
        private readonly ICurriculoClient _curriculoClient;
        private readonly IHardSkillService _hardskillService;
        private readonly ISoftskillService _softskillService;
        private readonly IMetodologiasService _metodologiaService;
        private readonly IDominioService _dominioService;
        private readonly IIdiomaService _idiomaService;
        private readonly IEscolaridadeColaboradorService _escolaridadeService;
        private readonly IExperienciaProfissionalService _experienciaService;
        private readonly IFormacaoService _formacaoService;
        private readonly IBuscaColaboradorRepository _buscaColaboradorRepository;
        private readonly IColaboradorService _colaboradorService;
        private readonly ILogCore _logCore;

        private readonly IFuncionalidadeSistemaRepository _funcionalidadeSistemaRepository;
        private readonly ISkillDesconhecidaService _skillDesconhecidaService;
        private readonly IUsuarioColaboradorRepository _usuarioColaboradorRepository;
        private readonly IColaboradorPerfilRepository _colaboradorPerfilRepository;
        private readonly IClassificacaoService _classificacaoService;
        private const int NIVEL_HARDSKILL = 29;
        private const int NIVEL_SOFTSKILL = 30;
        private const int NIVEL_METODOLOGIA = 31;
        private const int NIVEL_DOMINIO = 33;

        public LinkedinService(ICurriculoClient curriculoClient, IHardSkillService hardskillService, ISoftskillService softskillService, IMetodologiasService metodologiaService, IDominioService dominioService, IIdiomaService idiomaService,
        IEscolaridadeColaboradorService escolaridadeService, IExperienciaProfissionalService experienciaService, IFormacaoService formacaoService,
        IBuscaColaboradorRepository buscaColaboradorRepository, IFuncionalidadeSistemaRepository funcionalidadeSistemaRepository, ISkillDesconhecidaService skillDesconhecidaService, IColaboradorService colaboradorService, ILogCore logCore, [FromKeyedServices("Dapper")]IUsuarioColaboradorRepository usuarioColaboradorRepository, IColaboradorPerfilRepository colaboradorPerfilRepository, IClassificacaoService classificacaoService)
        {
            _curriculoClient = curriculoClient;
            _hardskillService = hardskillService;
            _softskillService = softskillService;
            _metodologiaService = metodologiaService;
            _dominioService = dominioService;
            _idiomaService = idiomaService;
            _escolaridadeService = escolaridadeService;
            _experienciaService = experienciaService;
            _formacaoService = formacaoService;
            _funcionalidadeSistemaRepository = funcionalidadeSistemaRepository;
            _buscaColaboradorRepository = buscaColaboradorRepository;
            _skillDesconhecidaService = skillDesconhecidaService;
            _colaboradorService = colaboradorService;
            _logCore = logCore;
            _usuarioColaboradorRepository = usuarioColaboradorRepository;
            _colaboradorPerfilRepository = colaboradorPerfilRepository;
            _classificacaoService = classificacaoService;
        }

        public async Task<StatusResult> ColetarLinkedinColaborador(string linkedin, string codInternoColaborador)
        {
            await Task.Run(() =>
            {
                _buscaColaboradorRepository.AtualizaInfoLinkedin(codInternoColaborador, linkedin);
            });
            return new StatusResult()
            {
                Sucesso = true
            };
        }
        public async Task<ApiGenericResult<string>> SincronizarPerfilLinkedinPorDocumento(byte[] documento, string codInternoColaborador)
        {
            try
            {
                RootIA ret;
                try
                {
                    ret = await _curriculoClient.GetProfileByDocumentContent(documento, VariaveisDeAmbienteUtil.GetVariavelDeAmbiente("CURRICULO_API_TOKEN"));

                }
                catch (Exception)
                {
                    throw new ValidationException("Não foi possivel identificar o documento");
                }

                if (ret.profile == null)
                    throw new ValidationException("Perfil não encontrado");
                
                await CadastrarSkillsComRetornoIA(codInternoColaborador, ret);

                if (ret.UrlLinkedin != null)
                {
                    _buscaColaboradorRepository.AtualizaInfoLinkedin(codInternoColaborador, ret.UrlLinkedin);
                }
                await _colaboradorService.InsereCurriculoColaborador(documento, codInternoColaborador);
                
                await _classificacaoService.AtualizarClassificacaoProfissionalPorCodigoInternoColaboradorAsync(codInternoColaborador);

                return new ApiGenericResult<string>()
                {
                    Retorno = ret.UrlLinkedin,
                    Sucesso = true
                };
            }
            catch (ValidationException e)
            {
                throw;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task CadastrarSkillsComRetornoIA(string codInternoColaborador, RootIA ret, bool importacaoLote = false)
        {
            if (!String.IsNullOrEmpty(ret.profile.summary))
                _experienciaService.AdicionarSobre(codInternoColaborador, ret.profile.summary, OrigemAlteracaoCVEnum.LINKEDIN);

            if (ret.profile.education != null && ret.profile.education.Any())
                await SyncEscolaridade(ret.profile.education, codInternoColaborador);

            if (ret.profile.experience != null && ret.profile.experience.Any())
                await SyncExperiencias(ret.profile.experience, codInternoColaborador);

            if (ret.profile.certifications != null && ret.profile.certifications.Any())
                await SyncCertificados(ret.profile.certifications, codInternoColaborador);

            if (ret.profile.languages != null && ret.profile.languages != null)
                await SyncLanguages(ret.profile.languages, codInternoColaborador);

            if (ret.habilidades != null && ret.habilidades != null)
                await SyncSkillsModeloIA(ret.habilidades, codInternoColaborador, importacaoLote);
        }

        public async Task<StatusResult> SincronizarPerfilLinkedin(string profileUrl, string codInternoColaborador, bool useRapidAPI = false, string fullUrl = "")
        {
            try
            {
                Root ret;
                try
                {
                    if (!useRapidAPI)
                        ret = await _curriculoClient.GetPerfilLinkedin(profileUrl, VariaveisDeAmbienteUtil.GetVariavelDeAmbiente("CURRICULO_API_TOKEN"));
                    else
                        ret = await _curriculoClient.GetPerfilLinkedinRapidAPI(profileUrl, VariaveisDeAmbienteUtil.GetVariavelDeAmbiente("CURRICULO_API_TOKEN"));
                }
                catch (Exception)
                {
                    throw new ValidationException("Perfil inválido");
                }

                if (ret.profile == null || ret.profile.entityUrn == null)
                    throw new ValidationException("Perfil não encontrado");

                await CadastrarSkillsComRetornoIA(codInternoColaborador, ret);

                var urlToSave = profileUrl;

                if (!fullUrl.IsNullOrEmpty())
                {
                    urlToSave =  fullUrl;
                }
                await _buscaColaboradorRepository.AtualizaInfoLinkedinAsync(codInternoColaborador, urlToSave);
                return new StatusResult
                {
                    Sucesso = true
                };
            }
            catch (ValidationException e)
            {
                throw;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task CadastrarSkillsComRetornoIA(string codInternoColaborador, Root ret)
        {

            // Atualizar endereço do colaborador baseado no geoLocationName
            if (!String.IsNullOrEmpty(ret.profile.geoLocationName))
            {
                var (cidade, estado) = ExtrairCidadeEEstado(ret.profile.geoLocationName);
                if (!String.IsNullOrEmpty(cidade) && !String.IsNullOrEmpty(estado))
                {
                    try
                    {
                        _usuarioColaboradorRepository.AtualizarEnderecoColaborador(codInternoColaborador, cidade, estado);
                    }
                    catch (Exception ex)
                    {
                        _logCore.Log($"Erro ao atualizar endereço do colaborador {codInternoColaborador}: {ex.Message}", DataTransferObject.Domain.Log.LevelsEnum.Warning);
                    }
                }
            }
            if (!String.IsNullOrEmpty(ret.profile.summary))
                await _colaboradorPerfilRepository.AtualizarSobreColaboradorAsync(codInternoColaborador, ret.profile.summary);

            if (ret.profile.education.Any())
                await SyncEscolaridade(ret.profile.education, codInternoColaborador);

            if (ret.profile.experience.Any())
                await SyncExperiencias(ret.profile.experience, codInternoColaborador);

            // Não esta sendor retornado pelo Linkedin
            // if (ret.profile.certifications.Any())
            //     await SyncCertificados(ret.profile.certifications, codInternoColaborador);

            if (ret.profile.languages != null)
                await SyncLanguages(ret.profile.languages, codInternoColaborador);

            if (ret.skills != null)
                await SyncSkills(ret.skills, codInternoColaborador);

        }

        private async Task SyncCertificados(List<Certification> certificados, string codInternoColaborador)
        {
            var certificadosColaborador = _hardskillService.ListaCertificadoColaborador(codInternoColaborador)
                                            .Select(x => x.Certificado.descricao);
            foreach (var certificado in certificados)
            {
                if (!certificadosColaborador.Where(x => x.Equals(certificado.name, StringComparison.InvariantCultureIgnoreCase) == true).Any())
                {
                    DateTime startDate = DateTime.Now;
                    if (certificado.timePeriod != null && certificado.timePeriod.startDate != null && certificado.timePeriod.startDate.year != null)
                        startDate = new DateTime(certificado.timePeriod.startDate.year.Value, certificado.timePeriod.startDate.month ?? 1, 1);

                    await _hardskillService.InserirCertificadoHardSkillColaborador(codInternoColaborador, null, null,
                            DataTransferObject.Domain.Colaborador.TipoCertificadoEnum.NULL, startDate, certificado.name, certificado.authority, 0, OrigemAlteracaoCVEnum.LINKEDIN);
                }
            }
        }

        private async Task SyncEscolaridade(List<Education> escolaridades, string codInternoColaborador)
        {
            if (escolaridades == null || !escolaridades.Any())
                return;

            // Buscar todas as escolaridades do colaborador de uma vez
            var escolaridadeColaborador = await _colaboradorPerfilRepository.ListarEscolaridadeColaboradorAsync(codInternoColaborador);
            
            // Preparar lista de descrições de formações para buscar em lote
            var descricoesFormacoes = new List<string>();
            var escolaridadesProcessadas = new List<(Education escolaridade, string escolaridadeField, string strValue)>();
            
            foreach (var escolaridade in escolaridades)
            {
                if(String.IsNullOrEmpty(escolaridade.schoolName))
                    escolaridade.schoolName = "Não informada";
                    
                var escolaridadeField = ((escolaridade.fieldOfStudy ?? escolaridade.degreeName) ?? escolaridade.schoolName).Trim().ToUpper();
                var strValue = StringUtil.RemoveDiacritics(escolaridadeField);
                
                descricoesFormacoes.Add(escolaridadeField);
                escolaridadesProcessadas.Add((escolaridade, escolaridadeField, strValue));
            }
            
            // Buscar todas as formações existentes em uma única query
            var formacoesExistentes = await _colaboradorPerfilRepository.BuscarFormacoesEmLoteAsync(descricoesFormacoes);
            
            // Identificar formações que precisam ser criadas
            var formacoesParaCriar = escolaridadesProcessadas
                .Where(e => !formacoesExistentes.ContainsKey(e.escolaridadeField.ToUpper()))
                .Select(e => e.escolaridadeField)
                .Distinct()
                .ToList();
            
            // Criar todas as formações que faltam em lote
            Dictionary<string, FormacaoDTO> novasFormacoes = new Dictionary<string, FormacaoDTO>();
            if (formacoesParaCriar.Any())
            {
                try
                {
                    novasFormacoes = await _colaboradorPerfilRepository.CriarFormacoesEmLoteAsync(formacoesParaCriar, codInternoColaborador);
                }
                catch (Exception err)
                {
                    Console.WriteLine("Falha ao inserir formações em lote: " + err.Message + "\n" + err.StackTrace);
                }
            }
            
            // Mesclar formações existentes com novas formações
            var todasFormacoes = new Dictionary<string, FormacaoDTO>(formacoesExistentes, StringComparer.OrdinalIgnoreCase);
            foreach (var formacao in novasFormacoes)
            {
                todasFormacoes[formacao.Key] = formacao.Value;
            }
            
            // Preparar lista de escolaridades para inserir em lote
            var escolaridadesParaAdicionar = new List<DataTransferObject.Domain.Escolaridade.EscolaridadeDTO>();
            
            foreach (var (escolaridade, escolaridadeField, strValue) in escolaridadesProcessadas)
            {
                // Verificar se a escolaridade já existe no colaborador
                var escolaridadeJaExiste = escolaridadeColaborador.Any(x =>
                    StringUtil.RemoveDiacritics(x.FormacaoDescricao ?? "").Equals(strValue, StringComparison.InvariantCultureIgnoreCase));
                
                if (escolaridadeJaExiste)
                    continue;
                
                // Obter o ID da formação
                if (!todasFormacoes.TryGetValue(escolaridadeField.ToUpper(), out var formacao))
                    continue;
                
                // Processar datas
                DateTime startDate = DateTime.Now;
                DateTime? endDate = null;
                if (escolaridade.timePeriod != null)
                {
                    var startDateAux = escolaridade.timePeriod.startDate;
                    var endDateAux = escolaridade.timePeriod.endDate;
                    if (startDateAux != null && startDateAux.year != null)
                    {
                        startDate = new DateTime(startDateAux.year.Value, startDateAux.month ?? 1, 1);
                    }
                    if (endDateAux != null && endDateAux.year != null)
                    {
                        endDate = new DateTime(endDateAux.year.Value, endDateAux.month ?? 1, 1);
                    }
                }
                
                // Adicionar à lista para inserção em lote
                escolaridadesParaAdicionar.Add(new DataTransferObject.Domain.Escolaridade.EscolaridadeDTO
                {
                    FormacaoId = formacao.Id,
                    Instituicao = escolaridade.schoolName,
                    DataInicio = startDate,
                    DataTermino = endDate,
                    Descricao = escolaridade.degreeName,
                    ColaboradorCpf = codInternoColaborador
                });
            }
            
            // Inserir todas as escolaridades de uma vez
            if (escolaridadesParaAdicionar.Any())
            {
                await _colaboradorPerfilRepository.AdicionarEscolaridadesEmLoteAsync(
                    escolaridadesParaAdicionar, 
                    codInternoColaborador, 
                    OrigemAlteracaoCVEnum.LINKEDIN
                );
            }
        }

        private async Task SyncExperiencias(List<Experience> experiencias, string codInternoColaborador)
        {
            if (experiencias == null || !experiencias.Any())
                return;

            // Buscar todas as experiências do colaborador de uma vez
            var experienciasProfissional = await _colaboradorPerfilRepository.ListarExperienciasColaboradorAsync(codInternoColaborador);
            
            // Preparar lista de experiências para inserir em lote
            var experienciasParaAdicionar = new List<DataTransferObject.Domain.Experiencia.ExperienciaDTO>();
            
            foreach (var experiencia in experiencias)
            {
                if (experiencia.companyName == null || experiencia.title == null)
                    continue;

                // Verificar se a experiência já existe
                var chaveExperiencia = experiencia.companyName + "|" + experiencia.title;
                var experienciaJaExiste = experienciasProfissional.Any(x => 
                    (x.Empresa + "|" + x.Funcao).Equals(chaveExperiencia, StringComparison.InvariantCultureIgnoreCase));

                if (experienciaJaExiste)
                    continue;

                // Processar datas
                DateTime startDate = DateTime.Now;
                DateTime? endDate = null;
                if (experiencia.timePeriod != null)
                {
                    if (experiencia.timePeriod.startDate != null && experiencia.timePeriod.startDate.year != null)
                    {
                        startDate = new DateTime(experiencia.timePeriod.startDate.year.Value, experiencia.timePeriod.startDate.month ?? 1, 1);
                    }
                    if (experiencia.timePeriod.endDate != null && experiencia.timePeriod.endDate.year != null)
                    {
                        endDate = new DateTime(experiencia.timePeriod.endDate.year.Value, experiencia.timePeriod.endDate.month ?? 1, 1);
                    }
                }

                // Adicionar à lista para inserção em lote
                experienciasParaAdicionar.Add(new DataTransferObject.Domain.Experiencia.ExperienciaDTO
                {
                    Funcao = experiencia.title,
                    Empresa = experiencia.companyName,
                    Atividades = experiencia.description ?? "",
                    DataInicio = startDate,
                    DataSaida = endDate,
                    Atual = endDate == null,
                    ColaboradorCpf = codInternoColaborador
                });
            }
            
            // Inserir todas as experiências de uma vez
            if (experienciasParaAdicionar.Any())
            {
                await _colaboradorPerfilRepository.AdicionarExperienciasEmLoteAsync(
                    experienciasParaAdicionar, 
                    codInternoColaborador, 
                    OrigemAlteracaoCVEnum.LINKEDIN
                );
            }
        }

        private async Task SyncSkills(List<Skill> skills, string codInternoColaborador)
        {
            try
            {
                var skillsClassified = await _curriculoClient.SkillClassify(skills.Select(x => x.name).ToList(), VariaveisDeAmbienteUtil.GetVariavelDeAmbiente("CURRICULO_API_TOKEN"));
                await SyncHardskills(skillsClassified.Hardskill, codInternoColaborador);
                await SyncSoftskills(skillsClassified.Softskill, codInternoColaborador);
                await SyncMetodologias(skillsClassified.Metodologia, codInternoColaborador, true);
                await SyncDominios(skillsClassified.Dominio, codInternoColaborador, true);
                await SyncSkillDesconhecidas(skillsClassified.Nao_classificada, codInternoColaborador);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task SyncSkillsModeloIA(SkillsHabilidades habilidades, string codInternoColaborador, bool importacaoLote = false)
        {
            try
            {
                if (habilidades.hardSkills != null)
                {
                    await SyncHardskills(habilidades.hardSkills, codInternoColaborador);
                }

                if (habilidades.softSkills != null)
                {
                    await SyncSoftskills(habilidades.softSkills, codInternoColaborador);
                }

                if (habilidades.metodologias != null)
                {
                    await SyncMetodologias(habilidades.metodologias, codInternoColaborador, importacaoLote);
                }

                if (habilidades.dominioNegocios != null)
                {
                    await SyncDominios(habilidades.dominioNegocios, codInternoColaborador, importacaoLote);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Método genérico para sincronizar skills (hardskill, softskill, metodologia, domínio, skill desconhecida)
        /// </summary>
        private async Task SyncSkillsGenerico(List<string> skills, string codInternoColaborador, TipoSkillEnum tipoSkill, long? nivelPadrao)
        {
            if (skills == null || !skills.Any())
                return;

            // Normalizar e remover duplicatas
            var skillsNormalizadas = skills
                .Select(s => StringUtil.RemoveDiacritics(s.Trim().ToUpper()))
                .Distinct()
                .ToList();
            
            // Buscar todas as skills do colaborador de uma vez
            var skillsColaborador = await _colaboradorPerfilRepository.ListarSkillsColaboradorAsync(codInternoColaborador, tipoSkill);
            
            // Buscar todas as skills existentes no sistema
            var skillsExistentes = await _colaboradorPerfilRepository.BuscarSkillsEmLoteAsync(skillsNormalizadas, tipoSkill);
            
            // Identificar skills que precisam ser criadas
            var skillsParaCriar = skillsNormalizadas
                .Where(s => !skillsExistentes.ContainsKey(s))
                .ToList();
            
            // Criar todas as skills que faltam em lote
            var novasSkills = new Dictionary<string, SkillGenericaDTO>();
            if (skillsParaCriar.Any())
            {
                try
                {
                    novasSkills = await _colaboradorPerfilRepository.CriarSkillsEmLoteAsync(skillsParaCriar, tipoSkill, codInternoColaborador);
                }
                catch (Exception err)
                {
                    Console.WriteLine($"Falha ao inserir {tipoSkill} em lote: {err.Message}\n{err.StackTrace}");
                }
            }
            
            // Mesclar skills existentes com novas skills
            var todasSkills = new Dictionary<string, SkillGenericaDTO>(skillsExistentes, StringComparer.OrdinalIgnoreCase);
            foreach (var skill in novasSkills)
            {
                todasSkills[skill.Key] = skill.Value;
            }
            
            // Preparar lista de skills para adicionar ao colaborador
            var skillsParaAdicionar = new List<(long skillId, long? nivelId)>();
            
            foreach (var skillNormalizada in skillsNormalizadas)
            {
                // Verificar se a skill já está atribuída ao colaborador
                var skillJaExiste = skillsColaborador.Any(x =>
                    StringUtil.RemoveDiacritics(x.Descricao).ToUpper().Equals(skillNormalizada, StringComparison.InvariantCultureIgnoreCase));
                
                if (skillJaExiste)
                    continue;
                
                // Obter o ID da skill
                if (!todasSkills.TryGetValue(skillNormalizada, out var skill))
                    continue;
                
                // Adicionar à lista para inserção em lote
                skillsParaAdicionar.Add((skill.Id, nivelPadrao));
            }
            
            // Inserir todas as skills do colaborador de uma vez
            if (skillsParaAdicionar.Any())
            {
                await _colaboradorPerfilRepository.AdicionarSkillsColaboradorEmLoteAsync(
                    skillsParaAdicionar, 
                    codInternoColaborador, 
                    tipoSkill,
                    OrigemAlteracaoCVEnum.LINKEDIN
                );
            }
        }

        private async Task SyncHardskills(List<string> skills, string codInternoColaborador)
        {
            await SyncSkillsGenerico(skills, codInternoColaborador, TipoSkillEnum.HARDSKILL, NIVEL_HARDSKILL);
        }

        private async Task SyncSoftskills(List<string> skills, string codInternoColaborador)
        {
            await SyncSkillsGenerico(skills, codInternoColaborador, TipoSkillEnum.SOFTSKILL, NIVEL_SOFTSKILL);
        }

        private async Task SyncMetodologias(List<string> skills, string codInternoColaborador, bool importacaoLote = false)
        {
            await SyncSkillsGenerico(skills, codInternoColaborador, TipoSkillEnum.METODOLOGIA, NIVEL_METODOLOGIA);
        }

        private async Task SyncDominios(List<string> skills, string codInternoColaborador, bool importacaoLote = false)
        {
            await SyncSkillsGenerico(skills, codInternoColaborador, TipoSkillEnum.DOMINIO, NIVEL_DOMINIO);
        }

        private async Task SyncLanguages(List<Language> languages, string codInternoColaborador)
        {
            if (languages == null || !languages.Any())
                return;

            // Buscar todos os idiomas do colaborador de uma vez
            var idiomasColaborador = await _colaboradorPerfilRepository.ListarIdiomasColaboradorAsync(codInternoColaborador);
            
            // Preparar lista de descrições de idiomas para buscar em lote
            var descricoesIdiomas = languages.Select(l => l.name.Trim().ToUpper()).ToList();
            var idiomasProcessados = new List<(Language language, string strValue)>();
            
            foreach (var language in languages)
            {
                var strValue = StringUtil.RemoveDiacritics(language.name);
                idiomasProcessados.Add((language, strValue));
            }
            
            // Buscar todos os idiomas existentes em uma única query
            var idiomasExistentes = await _colaboradorPerfilRepository.BuscarIdiomasEmLoteAsync(descricoesIdiomas);
            
            // Identificar idiomas que precisam ser criados
            var idiomasParaCriar = idiomasProcessados
                .Where(i => !idiomasExistentes.ContainsKey(i.language.name.Trim().ToUpper()))
                .Select(i => i.language.name.Trim().ToUpper())
                .Distinct()
                .ToList();
            
            // Criar todos os idiomas que faltam em lote
            var novosIdiomas = new Dictionary<string, IdiomaDTO>();
            if (idiomasParaCriar.Any())
            {
                novosIdiomas = await _colaboradorPerfilRepository.CriarIdiomasEmLoteAsync(idiomasParaCriar);
            }
            
            // Mesclar idiomas existentes com novos idiomas
            var todosIdiomas = new Dictionary<string, IdiomaDTO>(idiomasExistentes, StringComparer.OrdinalIgnoreCase);
            foreach (var idioma in novosIdiomas)
            {
                todosIdiomas[idioma.Key] = idioma.Value;
            }
            
            // Preparar lista de idiomas para adicionar ao colaborador
            var idiomasParaAdicionar = new List<(int idiomaId, long? nivelId)>();
            
            foreach (var (language, strValue) in idiomasProcessados)
            {
                // Verificar se o idioma já está atribuído ao colaborador
                var idiomaJaExiste = idiomasColaborador.Any(x =>
                    StringUtil.RemoveDiacritics(x.Idioma.Descricao).Equals(strValue, StringComparison.InvariantCultureIgnoreCase));
                
                if (idiomaJaExiste)
                    continue;
                
                // Obter o ID do idioma
                if (!todosIdiomas.TryGetValue(language.name.ToUpper(), out var idioma))
                    continue;
                
                // Adicionar à lista para inserção em lote
                idiomasParaAdicionar.Add((idioma.Id, null)); // null = nível "A definir"
            }
            
            // Inserir todos os idiomas do colaborador de uma vez
            if (idiomasParaAdicionar.Any())
            {
                await _colaboradorPerfilRepository.AdicionarIdiomasColaboradorEmLoteAsync(
                    idiomasParaAdicionar, 
                    codInternoColaborador, 
                    OrigemAlteracaoCVEnum.LINKEDIN
                );
            }
        }

        private async Task SyncSkillDesconhecidas(List<string> skills, string codInternoColaborador)
        {
            await SyncSkillsGenerico(skills, codInternoColaborador, TipoSkillEnum.SKILL_DESCONHECIDA, null);
        }

        public async Task<StatusResult> SincronizarPerfilLinkedinServicoExterno(string cpfRequest, string profileUrl, string codInternoColaborador, int orgId)
        {
            var result = new StatusResult();

            ValidaAcessoServicoRequerente(cpfRequest, orgId); //mudar para correlation id ???

            return await SincronizarPerfilLinkedin(profileUrl, codInternoColaborador);
        }

        private void ValidaAcessoServicoRequerente(string cpfRequest, int orgId)
        {
            var isValid = _funcionalidadeSistemaRepository.ValidaAcessoFuncionalidade(
                cpfRequest, orgId, FuncionalidadeSistemaEnum.CADASTRO_GESTAO_ALOCADOS);

            if (!isValid.Result)
            {
                throw new UnauthorizedAccessException("Acesso negado para Gestão Alocados (Sincronizar Linkedin).");
            }
        }

        private (string cidade, string estado) ExtrairCidadeEEstado(string geoLocationName)
        {
            if (String.IsNullOrEmpty(geoLocationName))
                return (null, null);

            // Formato esperado: "Goiânia, Goiás, Brazil"
            var partes = geoLocationName.Split(',').Select(p => p.Trim()).ToArray();
            
            if (partes.Length >= 2)
            {
                var cidade = partes[0];
                var nomeEstado = partes[1];
                
                // Converte o nome do estado para abreviação de 2 letras
                var estadoAbreviado = EstadoUtil.ConverterParaAbreviacao(nomeEstado);
                
                return (cidade, estadoAbreviado);
            }

            return (null, null);
        }
    }
}