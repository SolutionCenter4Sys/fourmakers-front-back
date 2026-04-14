using ApiClient.Domain;
using ApiClient.Domain.Interfaces;
using Colaboracao.Core.Exceptions;
using Colaboracao.Helper;
using Colaboracao.Helper.Enum;
using Core.Domain;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Foursys;
using DataTransferObject.Domain.Historico;
using DataTransferObject.Domain.Linkedin;
using DataTransferObject.Domain.SRS;
using DataTransferObject.Domain.SRS.Candidate;
using DataTransferObject.Domain.SRS.Cargos;
using DataTransferObject.Domain.SRS.LocalDeTrabalho;
using DataTransferObject.Domain.SRS.Offboarding;
using DataTransferObject.Domain.SRS.Onboarding;
using DataTransferObject.Domain.SRS.Tecnica;
using MessageQueue.Infra.Infrastructure;
using Newtonsoft.Json;
using SRS.Domain.Interfaces.Service;
using SRS.Infra.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Logs.Infra.Attributes;

namespace SRS.Domain.Impl.Service
{
    [LogDomainClass]
    public class SRSCandidateService : ISRSCandidateService
    {
        private readonly ISRSInfraClient _srsInfraClient;
        private readonly IHardSkillRepository _hardSkillRepository;
        private readonly ICompetenciaClient _competenciaClient;
        private readonly IIdiomaClient _idiomaClient;
        private readonly ICurriculoClient _curriculoClient;
        private readonly ISRSService _srsService;
        private readonly IMetodologiaClient _metodologiaClient;
        private readonly IDominioClient _dominioClient;
        private readonly ISoftskillClient _softskillClient;
        private readonly ISkillDesconhecidaClient _skillDesconhecidaClient;

        private const int CATEGORY_HARDSKILL = 1;
        private const int CATEGORY_IDIOMA = 3;

        private const int NIVEL_INDEFINIDO_HARDSKILL = 29;
        private const int NIVEL_INDEFINIDO_IDIOMA = 32;
        private const int NIVEL_INDEFINIDO_METODOLOGIA = 31;
        private const int NIVEL_INDEFINIDO_DOMINIO = 33;
        private const int NIVEL_INDEFINIDO_SOFTSKILL = 30;
        private const int NIVEL_INDEFINIDO_DESCONHECIDA = 37;

        public SRSCandidateService(ISRSInfraClient srsInfraClient, IHardSkillRepository hardSkillRepository, ICompetenciaClient competenciaClient,
            ICurriculoClient curriculoClient, IIdiomaClient idiomaClient, ISRSService srsService, IMetodologiaClient metodologiaClient,
            IDominioClient dominioClient, ISoftskillClient softskillClient, ISkillDesconhecidaClient skillDesconhecidaClient)
        {
            _srsInfraClient = srsInfraClient;
            _hardSkillRepository = hardSkillRepository;
            _competenciaClient = competenciaClient;
            _curriculoClient = curriculoClient;
            _idiomaClient = idiomaClient;
            _srsService = srsService;
            _metodologiaClient = metodologiaClient;
            _dominioClient = dominioClient;
            _softskillClient = softskillClient;
            _skillDesconhecidaClient = skillDesconhecidaClient;
        }

        public InsertCandidateDTO GetCandidate(string cpf)
        {
            var ret = _srsInfraClient.GetCandidate(cpf);
            return ret;
        }

        public bool ValidaSeJaExisteEmail(List<string> emails)
        {
            try
            {
                bool ret = false;
                foreach (var email in emails)
                {
                    ret = _srsInfraClient.ValidaSeJaExisteEmail(email);
                    if (ret == true)
                    {
                        break;
                    }
                }
                return ret;
            }
            catch
            {
                throw;
            }
        }

        void ISRSCandidateService.InsertCandidate(SRSInsertCandidateParam param)
        {
            try
            {
                if (param.first_name is not null && param.Fonte is not null && param.Skills != null && param.Cand_cpf is not null)
                {
                    if (param.EhAtivo >= 0 && param.EhFoursys >= 0)
                    {
                        if (param.dataNascimento.Value.Year > 1900 && param.uniresp != null && param.cand_rg != null && param.Cand_cpf != null && param.phone_cell != null && param.TipoCargo != null)
                        {
                            _srsInfraClient.InsertCandidate(param, param.Cand_cpf);
                        }
                        else
                        {
                            throw new Exception("Campos Foursys e Ativo estão marcados é necessário preencher: data de nascimento, unidade, RG, CPF, Celular e Cargo");
                        }
                    }
                    else
                    {
                        _srsInfraClient.InsertCandidate(param, param.Cand_cpf);
                    }
                }
                else
                {
                    throw new Exception("Alguns campos obrigatórios não foram preenchidos, confira novamente o: Nome, Fonte, competências/skills e CPF");
                }
            }
            catch
            {
                throw;
            }
        }

        public void InsertEntrevistaTecnica(EntrevistaParam param)
        {
            try
            {
                _srsInfraClient.EntrevistaTecnicaInsert(param);
            }
            catch
            {
                throw;
            }
        }

        public void EntrevistaRHInsert(EntrevistaParam param)
        {
            try
            {
                _srsInfraClient.EntrevistaRHInsert(param);
            }
            catch
            {
                throw;
            }
        }
        public List<EntrevistaDTO> GetEntrevista(int candidateId)
        {
            try
            {
                var ret = _srsInfraClient.GetEntrevista(candidateId);
                return ret;
            }
            catch
            {
                throw;
            }
        }

        public List<SoEntrevistaDTO> GetSoEntrevista(int candidateId)
        {
            try
            {
                var ret = _srsInfraClient.GetSoEntrevista(candidateId);
                return ret;
            }
            catch
            {
                throw;
            }
        }

        public List<SoEntrevistaIdDTO> GetSoEntrevistaId(int entrevistaId, int candidateId)
        {
            try
            {
                var ret = _srsInfraClient.GetSoEntrevistaId(entrevistaId, candidateId);
                return ret;
            }
            catch
            {
                throw;
            }
        }

        public void InsertEntrevistaClienteGestor(EntrevistaParam param)
        {
            try
            {
                _srsInfraClient.EntrevistaClienteGestorInsert(param);
            }
            catch
            {
                throw;
            }
        }

        public void EditarEntrevista(EntrevistaDTO param)
        {
            try
            {
                _srsInfraClient.EditarEntrevista(param);
            }
            catch
            {
                throw;
            }
        }

        public int insertOffboarding(OffboardingDTO param)
        {
            try
            {
                return _srsInfraClient.insertOffboarding(param);
            }
            catch
            {
                throw;
            }
        }

        public void EditarOffboarding(OffboardingDTO param)
        {
            try
            {
                _srsInfraClient.EditarOffboarding(param);
            }
            catch
            {
                throw;
            }
        }

        public List<OffboardingDTO> GetOffboarding(int candidateId)
        {
            try
            {
                return _srsInfraClient.GetOffboarding(candidateId);
            }
            catch
            {
                throw;
            }
        }

        public void InsertSkillCandidate(CandidateSkillParam param)
        {
            try
            {
                _srsInfraClient.InsertSkillCandidate(param);
            }
            catch
            {
                throw;
            }
        }

        public void InsertCandidateSkillEntrevista(EntrevistaSkillParam param)
        {
            try
            {
                _srsInfraClient.InsertCandidateSkillEntrevista(param);
            }
            catch
            {
                throw;
            }
        }

        public void EditarSkillCandidate(CandidateSkillDTO param)
        {
            try
            {
                _srsInfraClient.EditarSkillCandidate(param);
            }
            catch
            {
                throw;
            }
        }

        public void RemoveSkillCandidate(int skillId)
        {
            try
            {
                _srsInfraClient.RemoveSkillCandidate(skillId);
            }
            catch
            {
                throw;
            }
        }

        public List<CandidateSkillDTO> GetCandidateSkill(int candidate_id)
        {
            try
            {
                return _srsInfraClient.GetCandidateSkill(candidate_id);
            }
            catch
            {
                throw;
            }
        }

        public List<CandidateSkillDTO> GetCandidateSkillEntrevista(int candidateId, int entrevistaId)
        {
            try
            {
                return _srsInfraClient.GetCandidateSkillEntrevista(candidateId, entrevistaId);
            }
            catch
            {
                throw;
            }
        }

        public void InsertOnboarding(OnboardingParam param)
        {
            try
            {
                _srsInfraClient.InsertOnboarding(param);
            }
            catch
            {
                throw;
            }
        }

        public List<OnboardingDTO> GetOnboarding(int candidate_id)
        {
            try
            {
                return _srsInfraClient.GetOnboarding(candidate_id);
            }
            catch
            {
                throw;
            }
        }

        public List<HistoricoCandidateEntrevistaDTO> GetHistoricoCandidatoEntrevistas(int candidateId)
        {
            try
            {
                return _srsInfraClient.GetHistoricoCandidatoEntrevistas(candidateId);
            }
            catch
            {
                throw;
            }
        }

        public void HistoricoCandidatoEntrevista(HistoricoCandidateEntrevistaDTO historico)
        {
            try
            {
                _srsInfraClient.HistoricoCandidatoEntrevista(historico);
            }
            catch
            {
                throw;
            }
        }

        public HistoricoCandidateEntrevistaDTO GetHistoricoCandidatoEntrevistaEspecifica(int id)
        {
            try
            {
                return _srsInfraClient.GetHistoricoCandidatoEntrevistaEspecifica(id);
            }
            catch
            {
                throw;
            }
        }

        public void EditarOnboarding(OnboardingDTO param)
        {
            try
            {
                _srsInfraClient.EditarOnboarding(param);
            }
            catch
            {
                throw;
            }
        }

        public void RemoveOnboarding(int onboardingId)
        {
            try
            {
                _srsInfraClient.RemoveOnboarding(onboardingId);
            }
            catch
            {
                throw;
            }
        }

        public List<CargoSimplesDTO> GetCargoSimples()
        {
            try
            {
                return _srsInfraClient.GetCargoSimples();
            }
            catch
            {
                throw;
            }
        }

        public CargoDTO GetCargo(int cargoId)
        {
            try
            {
                return _srsInfraClient.GetCargo(cargoId);
            }
            catch
            {
                throw;
            }
        }

        public List<LocalDeTrabalhoDTO> GetLocalDeTrabalho()
        {
            try
            {
                return _srsInfraClient.GetLocalDeTrabalho();
            }
            catch
            {
                throw;
            }
        }

        public int UnidadeFourmakersToSRS(int idUnidade)
        {
            try
            {
                return _srsInfraClient.UnidadeFourmakersToSRS(idUnidade);
            }
            catch
            {
                throw;
            }
        }

        public async Task<AlterarStatusCandidaturaDTO> AlterarStatusCandidatura(AlterarStatusCandidaturaParam status, string email)
        {
            try
            {
                var nomeAnalista = _srsInfraClient.BuscaNomeAnalista(email);
                var ret = await _srsInfraClient.AlterarStatusCandidatura(status, nomeAnalista);
                return ret;
            }
            catch
            {
                throw;
            }
        }

        public async Task<int> CadastroCandidatoFourmakersLinkedin(CadastroCandidatoLinkedinInput request)
        {
            try
            {
                Root resultLinkedin;
                if (request.Email == null || request.Telefone == null || request.UrlLinkedin == null)
                {
                    throw new ValidationException("Email, telefone e url do linkedin são obrigatórios");
                }

                try
                {
                    resultLinkedin = await _curriculoClient.GetPerfilLinkedin(GetVanityName(request.UrlLinkedin), VariaveisDeAmbienteUtil.GetVariavelDeAmbiente("CURRICULO_API_TOKEN"));
                }
                catch (Exception)
                {
                    throw new ValidationException("Perfil inválido");
                }

                if (resultLinkedin.profile.entityUrn == null)
                    throw new ValidationException("Perfil não encontrado");

                var candidateIdAux = _srsInfraClient.GetCandidateId("https://www.linkedin.com/in/" + GetVanityName(request.UrlLinkedin));

                if (candidateIdAux == null)
                {
                    var candidateId = SaveAndReturnCandidateId(request, resultLinkedin);
                    SaveCandidateCurriculum(resultLinkedin, candidateId);
                    await SaveCandidateSkills(resultLinkedin, candidateId);
                    SaveCandidateHistory(candidateId);

                    await EnviarOuAtualizarCandidatoParaSQS(resultLinkedin, candidateId, request.Email, CRUDEnum.Create);

                    return candidateId;
                }
                else
                {
                    UpdateCandidateCurriculum(resultLinkedin, (int)candidateIdAux);
                    await UpdateCandidateSkills(resultLinkedin, (int)candidateIdAux);
                    return (int)candidateIdAux;
                }
            }
            catch (ValidationException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task EnviarOuAtualizarCandidatoParaSQS(Root resultLinkedin, int candidateId, string email, CRUDEnum cRUDEnum)
        {
            bool isUpdate = cRUDEnum == CRUDEnum.Update;
            var logExistente = _srsInfraClient.VerificaLogExistenteCandidateID(candidateId);

            if (isUpdate && logExistente.ToIntOuZero() <= 0)
            {
                isUpdate = false; // "rebaixa" de Update para Add
            }

            var candidateParamSQS = new SRSInsertCandidateParamSQS
            {
                CandidateId = candidateId,
                FirstName = resultLinkedin.profile.firstName + " " + resultLinkedin.profile.lastName,
                Email1 = email,
                PendingToSend = SqsPendingStatus.PendingTrue.ToCharString()
            };

            var listaHardskill = await ConstruirListCandidateSkillParam(resultLinkedin, candidateParamSQS.CandidateId, CATEGORY_HARDSKILL, NIVEL_INDEFINIDO_HARDSKILL, new List<CandidateSkillDTO>());
            var listaIdiomas = await ConstruirListCandidateSkillParam(resultLinkedin, candidateParamSQS.CandidateId, CATEGORY_IDIOMA, NIVEL_INDEFINIDO_IDIOMA, new List<CandidateSkillDTO>());

            candidateParamSQS.Hardskill = listaHardskill;
            candidateParamSQS.Idioma = listaIdiomas;
            candidateParamSQS.Softskill = new();
            candidateParamSQS.Dominio = new();
            candidateParamSQS.Methodologia = new();

            var idLog = _srsInfraClient.SendToLogSqsQueueLog(candidateParamSQS);

            var bodyToSend = JsonConvert.SerializeObject(new
            {
                nomeCompleto = candidateParamSQS.FirstName,
                dataNascimento = "",
                emailContato = candidateParamSQS.Email1,
                emailCorporativo = "",
                flagColaborador = false,
                id = candidateParamSQS.CandidateId.ToString(),
                cpf = "",
                cargo = "",
                hardskill = candidateParamSQS.Hardskill,
                softskill = candidateParamSQS.Softskill,
                metodologia = candidateParamSQS.Methodologia,
                dominio = candidateParamSQS.Dominio,
                idioma = candidateParamSQS.Idioma,
                observacao = isUpdate ? "Candidate update" : "Candidate add"
            });
            
            MessageDispatcher.To.AmazonSQS.SRS.Candidate.EnviaSQS(bodyToSend);            

            // Atualiza o status para 'F' (processado)
            _srsInfraClient.UpdateLogStatus(idLog, SqsPendingStatus.PendingFalse.ToCharString());
        }

        public async Task<CadastroCandidatoInputResponse> CadastrarAtualizarPreAprovarCandidadoService(CadastrarAtualizarPreAprovarCandidadoLinkedinInput model)
        {
            string operacao = "cadastrado";
            var cadastroInput = new CadastroCandidatoInput
            {
                UserId = model.UserId.ToString(),
                UrlLinkedin = model.UrlLinkedin,
            };
            var updateResult = new AtualizaCadastroCandidatoInputResponse();
            int candidateId = 0;
            var candId = _srsInfraClient.GetCandidateIdByVanityName(GetVanityName(model.UrlLinkedin));
            if (candId > 0)
            {
                operacao = "atualizado";
                var updateUser = new AtualizaCadastroCandidatoInput { CandidateId = candId };
                updateResult = await AtualizaCurriculoCandidate(updateUser);
            }
            else
            {
                var createResult = await CadastroCandidatoLinkedin(cadastroInput);
                if (createResult.Mensagem.Contains("existe"))
                {
                    operacao = "atualizado";
                    var updateUser = new AtualizaCadastroCandidatoInput { CandidateId = Convert.ToInt32(createResult.CandidateId) };
                    updateResult = await AtualizaCurriculoCandidate(updateUser);
                }
                candidateId = createResult.CandidateId.HasValue ? Convert.ToInt32(createResult.CandidateId) : Convert.ToInt32(updateResult.CandidateId);
            }

            if (model.CodigoDaVaga.HasValue && model.CodigoDaVaga.Value != 0)
            {
                var cadidatarResult = await _srsService.CandidatarSe(model.CodigoDaVaga.Value, "", "Linkedin", model.UserId);
                if (cadidatarResult != null)
                {
                    var candidate = _srsInfraClient.GetCandidate(candidateId);
                    return new CadastroCandidatoInputResponse
                    {
                        Mensagem = $"Candidato foi {operacao} e seu pré-cadastro foi realizado com sucesso",
                        CandidateId = candidateId,
                        UrlLinkedin = model.UrlLinkedin,
                        Email = candidate.email1 ?? candidate.email2 ?? "",
                        UserId = model.UserId.ToString()
                    };
                }
            }
            else 
            {
                return new CadastroCandidatoInputResponse
                {
                    Mensagem = $"Candidato foi {operacao} com sucesso",
                    CandidateId = candidateId,
                    UrlLinkedin = model.UrlLinkedin,
                    Email = "",
                    UserId = model.UserId.ToString()
                };
            }

            return new CadastroCandidatoInputResponse
            {
                Mensagem = $"Houve um erro ao cadastrar/atualizar candidato",
                UrlLinkedin = model.UrlLinkedin,
                UserId = model.UserId.ToString()
            };
        }

        public async Task<CadastroCandidatoInputResponse> CadastroCandidatoLinkedin(CadastroCandidatoInput request)
        {
            try
            {
                var userExists = _srsService.ValidarUsuarioLinkedinService(int.Parse(request.UserId), null);

                if (userExists != null)
                {
                    Root resultLinkedin;
                    try
                    {
                        resultLinkedin = await _curriculoClient.GetPerfilLinkedinRapidAPI(GetVanityName(request.UrlLinkedin), VariaveisDeAmbienteUtil.GetVariavelDeAmbiente("CURRICULO_API_TOKEN"));
                    }
                    catch (Exception)
                    {
                        throw new ValidationException("Perfil inválido");
                    }

                    if (resultLinkedin is null
                        || resultLinkedin.profile is null
                        || resultLinkedin.profile.entityUrn.IsEmpty())
                    {
                        throw new ValidationException("Perfil não encontrado");
                    }

                    var candId = _srsInfraClient.GetCandidateIdByVanityName(GetVanityName(request.UrlLinkedin));
                    if (candId > 0)
                        return new CadastroCandidatoInputResponse
                        {
                            Mensagem = "Candidato já existe na base de dados",
                            CandidateId = candId,
                            UserId = request.UserId,
                            Email = request.Email,
                            UrlLinkedin = request.UrlLinkedin,
                        };

                    var candidateId = SaveAndReturnCandidateId(request, resultLinkedin);
                    SaveCandidateCurriculum(resultLinkedin, candidateId);
                    await SaveCandidateSkills(resultLinkedin, candidateId);
                    SaveCandidateHistory(request, candidateId);

                    string jsonString = "", entityUrn = "";
                    if (resultLinkedin.profile is not null)
                    {
                        jsonString = JsonConvert.SerializeObject(resultLinkedin.profile);
                        entityUrn = JsonConvert.SerializeObject(resultLinkedin.profile?.entityUrn.ToStringOuVazio());
                    }

                    return new CadastroCandidatoInputResponse
                    {
                        Mensagem = $"Candidato cadastrado com sucesso!",
                        CandidateId = candidateId,
                        UserId = request.UserId,
                        Email = request.Email,
                        UrlLinkedin = request.UrlLinkedin,
                    };
                }

                throw new ValidationException("Perfil inválido");
            }
            catch (ValidationException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void EnviarBancoTalentoFourmakers(string urlLinkedin, string codVaga, string email, string telefone)
        {
            string vanityName =  "";
            try {
                vanityName = GetVanityName(urlLinkedin);
                if(String.IsNullOrEmpty(vanityName))
                    throw new Exception("Url inválida do linkedin.");
                
                MessageDispatcher.To.AmazonSQS.SRS.Candidate.EnviaSQS(JsonConvert.SerializeObject(new { perfilIN = vanityName, codVaga = codVaga, email = email, telefone = telefone }));    
            }
            catch(Exception e)
            {
                throw;
            }
        }

        private string GetVanityName(string linkedinUrl)
        {
            try
            {
                if(linkedinUrl.Contains("linkedin.com"))
                {
                    string pattern = @"(?<=/in/)[^/]+";
                    var match = Regex.Match(linkedinUrl, pattern);

                    if (!match.Success)
                        throw new ValidationException("Url inválida");

                    return match.Value;
                }
                else
                {
                    return linkedinUrl;
                }
                
            }
            catch (Exception)
            {
                throw new ValidationException("Url inválida");
            }
        }

        private int SaveAndReturnCandidateId(CadastroCandidatoInput request, Root result)
        {
            var srsInsertCandidateParam = BuildSRSInsertCandidateParam(request, result);
            _srsInfraClient.InsertCandidateLinkedin(srsInsertCandidateParam);
            var candidateId = _srsInfraClient.GetCandidateIdByVanityName(GetVanityName(request.UrlLinkedin));

            return candidateId;
        }

        private int SaveAndReturnCandidateId(CadastroCandidatoLinkedinInput request, Root result)
        {
            var srsInsertCandidateParam = BuildSRSInsertCandidateParam(request, result);
            _srsInfraClient.InsertCandidateLinkedin(srsInsertCandidateParam);
            var candidateId = _srsInfraClient.GetCandidateIdByVanityName(GetVanityName(request.UrlLinkedin));

            return candidateId;
        }

        private SRSInsertCandidateParam BuildSRSInsertCandidateParam(CadastroCandidatoLinkedinInput request, Root result)
        {
            return BuildSRSInsertCandidateParamBase("1", result.profile.firstName + " " + result.profile.lastName, request.Telefone, request.Email, request.UrlLinkedin, "4Makers");
        }

        private SRSInsertCandidateParam BuildSRSInsertCandidateParam(CadastroCandidatoInput request, Root result)
        {
            return BuildSRSInsertCandidateParamBase(request.UserId, result.profile.firstName + " " + result.profile.lastName, "", request.Email, request.UrlLinkedin, "Linkedin");
        }

        private SRSInsertCandidateParam BuildSRSInsertCandidateParamBase(string userId, string nome, string telefone, string email, string linkedin, string source)
        {            
            return new SRSInsertCandidateParam
            {
                site_id = 1,
                last_name = " ",
                first_name = nome,
                can_relocate = 0,
                entered_by = int.Parse(userId),
                owner = int.Parse(userId),
                date_created = DateTime.Now,
                date_modified = DateTime.Now,
                import_id = 0,
                is_hot = 0,
                best_time_to_call = " ",
                cand_rg = " ",
                cand_cpf = " ",
                cand_estCivil = " ",
                cand_Lkdin = "https://www.linkedin.com/in/" + GetVanityName(linkedin),
                cand_skype = " ",
                cand_filhos = 0,
                cand_fumante = " ",
                source = source,
                email1 = email,
                phone_cell = telefone,
                is_active = 0
            };
        }

        private Tuple<DateTime?, DateTime?> GetStartDateAndEndDate(TimePeriod timePeriod)
        {
            DateTime? startDate = null;
            DateTime? endDate = null;
            if (timePeriod != null)
            {
                if (timePeriod.startDate != null && timePeriod.startDate.year != null)
                {
                    startDate = new DateTime(timePeriod.startDate.year.Value , timePeriod.startDate.month ?? 1, 1);
                }
                if (timePeriod.endDate != null && timePeriod.endDate.year != null)
                {
                    endDate = new DateTime(timePeriod.endDate.year.Value , timePeriod.endDate.month ?? 1, 1);
                }
            }
            return new Tuple<DateTime?, DateTime?>(startDate, endDate);
        }

        private void SaveCandidateCurriculum(Root result, int candidateId)
        {
            if (result.profile != null)
            {
                _srsInfraClient.InsertCandidateCurriculum(candidateId, result.profile.headline, result.profile.summary, result.profile.geoLocationName, DateTime.Now, DateTime.Now);
            }

            if (result.profile.experience != null && result.profile.experience.Count() > 0)
            {
                foreach (var experience in result.profile.experience)
                {
                    var dates = GetStartDateAndEndDate(experience.timePeriod);
                    _srsInfraClient.InsertCandidateCurriculumCompany(candidateId, experience.companyName, experience.description, experience.title, experience.locationName, dates.Item1, dates.Item2, dates.Item2 == null, DateTime.Now, DateTime.Now);
                }
            }

            if (result.profile.certifications != null && result.profile.certifications.Count() > 0)
            {
                foreach (var certificate in result.profile.certifications)
                {
                    var dates = GetStartDateAndEndDate(certificate.timePeriod);
                    _srsInfraClient.InsertCandidateCurriculumCertification(candidateId, certificate.name, certificate.authority, dates.Item1, dates.Item2, DateTime.Now, DateTime.Now);
                }
            }

            if (result.profile.education != null && result.profile.education.Count() > 0)
            {
                foreach (var education in result.profile.education)
                {
                    var dates = GetStartDateAndEndDate(education.timePeriod);
                    _srsInfraClient.InsertCandidateCurriculumDegree(candidateId, education.degreeName, education.fieldOfStudy, education.schoolName, dates.Item1, dates.Item2, DateTime.Now, DateTime.Now);
                }
            }
        }

        private void UpdateCandidateCurriculum(Root result, int candidateId)
        {
            if (result.profile != null)
            {
                _srsInfraClient.UpdateCandidateCurriculum(candidateId, result.profile.headline, result.profile.summary, result.profile.geoLocationName, DateTime.Now);
            }

            if (result.profile.experience != null && result.profile.experience.Count() > 0)
            {
                var experienciasAnteriores = _srsInfraClient.GetCandidateCurriculumCompany(candidateId);
                foreach (var experience in result.profile.experience.Where(x => experienciasAnteriores.Where(y => y.Equals(x.companyName + "|" + x.title)).Any() == false))
                {
                    var dates = GetStartDateAndEndDate(experience.timePeriod);
                    _srsInfraClient.InsertCandidateCurriculumCompany(candidateId, experience.companyName, experience.description, experience.title, experience.locationName, dates.Item1, dates.Item2, dates.Item2 == null, DateTime.Now, DateTime.Now);
                }
            }

            if (result.profile.certifications != null && result.profile.certifications.Count() > 0)
            {
                var certificadosAnteriores = _srsInfraClient.GetCandidateCurriculumCertification(candidateId);
                foreach (var certificate in result.profile.certifications.Where(x => certificadosAnteriores.Where(y => y.Equals(x.name)).Any() == false))
                {
                    var dates = GetStartDateAndEndDate(certificate.timePeriod);
                    _srsInfraClient.InsertCandidateCurriculumCertification(candidateId, certificate.name, certificate.authority, dates.Item1, dates.Item2, DateTime.Now, DateTime.Now);
                }
            }

            if (result.profile.education != null && result.profile.education.Count() > 0)
            {
                var educacaoAnterior = _srsInfraClient.GetCandidateCurriculumDegree(candidateId);
                foreach (var education in result.profile.education.Where(x => educacaoAnterior.Where(y => y.Equals(x.schoolName + "|" + x.degreeName)).Any() == false))
                {
                    var dates = GetStartDateAndEndDate(education.timePeriod);
                    _srsInfraClient.InsertCandidateCurriculumDegree(candidateId, education.degreeName, education.fieldOfStudy, education.schoolName, dates.Item1, dates.Item2, DateTime.Now, DateTime.Now);
                }
            }
        }

        private async Task SaveCandidateSkills(Root result, int candidateId)
        {
            
            var skillsLinkedin = result.skills.Select(x => x.name).ToList();
            var idiomasLinkedin = result.profile.languages.Select(x => x.name).ToList();



            var skillsClassified = await _curriculoClient.SkillClassify(skillsLinkedin, VariaveisDeAmbienteUtil.GetVariavelDeAmbiente("CURRICULO_API_TOKEN"));
            var skillsFourmakers = skillsLinkedin.Count > 0 ? await _competenciaClient.GetHardSkillInfoByDescricao(skillsClassified.Hardskill, VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.TOKEN_SISTEMA_COLABORACAO)) : new List<SkillValuePair>();
            var metodologiasFourmakers = skillsClassified.Metodologia.Count > 0 ? await _metodologiaClient.GetMetodologiaInfoByDescricao(skillsClassified.Metodologia, VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.TOKEN_SISTEMA_COLABORACAO)) : new List<KeyValuePair<string, long>>();
            var dominiosFourmakers = skillsClassified.Dominio.Count > 0 ? await _dominioClient.GetDominioInfoByDescricao(skillsClassified.Dominio, VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.TOKEN_SISTEMA_COLABORACAO)) : new List<KeyValuePair<string, long>>();
            var softskillsFourmakers = skillsClassified.Softskill.Count > 0 ? await _softskillClient.GetSoftSkillInfoByDescricao(skillsClassified.Softskill, VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.TOKEN_SISTEMA_COLABORACAO)) : new List<KeyValuePair<string, long>>();
            var skillsDesconhecidasFourmakers = skillsClassified.Nao_classificada.Count > 0 ? await _skillDesconhecidaClient.GetSkillDesconhecidaInfoByDescricao(skillsClassified.Nao_classificada, VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.TOKEN_SISTEMA_COLABORACAO)) : new List<KeyValuePair<string, long>>();

            var idiomasFourmakers = idiomasLinkedin.Count > 0 ? await _idiomaClient.GetIdiomaInfoByDescricao(idiomasLinkedin, VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.TOKEN_SISTEMA_COLABORACAO)) : new List<SkillValuePair>();

            foreach (var item in skillsFourmakers)
            {
                var candidateSkillParam = BuildCandidateSkillParam(candidateId, (int)CategoriaSkillEnum.Hardskills, (int)item.Value, NIVEL_INDEFINIDO_HARDSKILL);
                _srsInfraClient.InsertSkillCandidate(candidateSkillParam);
            }

            foreach (var item in skillsDesconhecidasFourmakers)
            {
                var candidateSkillParam = BuildCandidateSkillParam(candidateId, (int)CategoriaSkillEnum.Desconhecida, (int)item.Value, NIVEL_INDEFINIDO_DESCONHECIDA);
                _srsInfraClient.InsertSkillCandidate(candidateSkillParam);
            }

            foreach (var item in idiomasFourmakers)
            {
                var candidateSkillParam = BuildCandidateSkillParam(candidateId, (int)CategoriaSkillEnum.Idiomas, (int)item.Value, NIVEL_INDEFINIDO_IDIOMA);
                _srsInfraClient.InsertSkillCandidate(candidateSkillParam);
            }

            foreach (var item in metodologiasFourmakers)
            {
                var candidateSkillParam = BuildCandidateSkillParam(candidateId, (int)CategoriaSkillEnum.Metodologia, (int)item.Value, NIVEL_INDEFINIDO_METODOLOGIA);
                _srsInfraClient.InsertSkillCandidate(candidateSkillParam);
            }

            foreach (var item in dominiosFourmakers)
            {
                var candidateSkillParam = BuildCandidateSkillParam(candidateId, (int)CategoriaSkillEnum.ConhecimentoNegocio, (int)item.Value, NIVEL_INDEFINIDO_DOMINIO);
                _srsInfraClient.InsertSkillCandidate(candidateSkillParam);
            }

            foreach (var item in softskillsFourmakers)
            {
                var candidateSkillParam = BuildCandidateSkillParam(candidateId, (int)CategoriaSkillEnum.Softskills, (int)item.Value, NIVEL_INDEFINIDO_SOFTSKILL);
                _srsInfraClient.InsertSkillCandidate(candidateSkillParam);
            }
        }

        private async Task UpdateCandidateSkills(Root result, int candidateId)
        {
            var skillsLinkedin = result.skills.Select(x => x.name).ToList();
            var idiomasLinkedin = result.profile.languages.Select(x => x.name).ToList();

            var skillsAnteriores = _srsInfraClient.GetCandidateSkill(candidateId)?.Where(x => x.Category_id == (int)CategoriaSkillEnum.Hardskills).ToList();
            var IdiomasAnteriores = _srsInfraClient.GetCandidateSkill(candidateId)?.Where(x => x.Category_id == (int)CategoriaSkillEnum.Idiomas).ToList();
            var metodologiasAnteriores = _srsInfraClient.GetCandidateSkill(candidateId)?.Where(x => x.Category_id == (int)CategoriaSkillEnum.Metodologia).ToList();
            var dominiosAnteriores = _srsInfraClient.GetCandidateSkill(candidateId)?.Where(x => x.Category_id == (int)CategoriaSkillEnum.ConhecimentoNegocio).ToList();
            var softskillsAnteriores = _srsInfraClient.GetCandidateSkill(candidateId)?.Where(x => x.Category_id == (int)CategoriaSkillEnum.Softskills).ToList();
            var skillsDesconhecidasAnteriores = _srsInfraClient.GetCandidateSkill(candidateId)?.Where(x => x.Category_id == (int)CategoriaSkillEnum.Desconhecida).ToList();

            var skillsClassified = await _curriculoClient.SkillClassify(skillsLinkedin, VariaveisDeAmbienteUtil.GetVariavelDeAmbiente("CURRICULO_API_TOKEN"));
            var idiomasFourmakers = idiomasLinkedin.Count > 0 ? await _idiomaClient.GetIdiomaInfoByDescricao(idiomasLinkedin, VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.TOKEN_SISTEMA_COLABORACAO)) : new List<SkillValuePair>();

            // HardSkills
            var skillsFourmakers = skillsClassified.Hardskill.Count > 0 ? await _competenciaClient.GetHardSkillInfoByDescricao(skillsClassified.Hardskill, VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.TOKEN_SISTEMA_COLABORACAO)) : new List<SkillValuePair>();
            foreach (var item in skillsFourmakers.Where(x => skillsAnteriores.Where(y => y.Description_id == x.Value).Any() == false))
            {
                var candidateSkillParam = BuildCandidateSkillParam(candidateId, (int)CategoriaSkillEnum.Hardskills, (int)item.Value, NIVEL_INDEFINIDO_HARDSKILL);
                _srsInfraClient.InsertSkillCandidate(candidateSkillParam);
            }

            // Idiomas
            foreach (var item in idiomasFourmakers.Where(x => IdiomasAnteriores.Where(y => y.Description_id == x.Value).Any() == false))
            {
                var candidateSkillParam = BuildCandidateSkillParam(candidateId, (int)CategoriaSkillEnum.Idiomas, (int)item.Value, NIVEL_INDEFINIDO_IDIOMA);
                _srsInfraClient.InsertSkillCandidate(candidateSkillParam);
            }

            // Metodologias
            var metodologiasFourmakers = skillsClassified.Metodologia.Count > 0 ? await _metodologiaClient.GetMetodologiaInfoByDescricao(skillsClassified.Metodologia, VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.TOKEN_SISTEMA_COLABORACAO)) : new List<KeyValuePair<string, long>>();
            foreach (var item in metodologiasFourmakers.Where(x => metodologiasAnteriores.Where(y => y.Description_id == x.Value).Any() == false))
            {
                var candidateSkillParam = BuildCandidateSkillParam(candidateId, (int)CategoriaSkillEnum.Metodologia, (int)item.Value, NIVEL_INDEFINIDO_METODOLOGIA);
                _srsInfraClient.InsertSkillCandidate(candidateSkillParam);
            }

            // Domínios
            var dominiosFourmakers = skillsClassified.Dominio.Count > 0 ? await _dominioClient.GetDominioInfoByDescricao(skillsClassified.Dominio, VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.TOKEN_SISTEMA_COLABORACAO)) : new List<KeyValuePair<string, long>>();
            foreach (var item in dominiosFourmakers.Where(x => dominiosAnteriores.Where(y => y.Description_id == x.Value).Any() == false))
            {
                var candidateSkillParam = BuildCandidateSkillParam(candidateId, (int)CategoriaSkillEnum.ConhecimentoNegocio, (int)item.Value, NIVEL_INDEFINIDO_DOMINIO);
                _srsInfraClient.InsertSkillCandidate(candidateSkillParam);
            }

            // Softskills
            var softskillsFourmakers = skillsClassified.Softskill.Count > 0 ? await _softskillClient.GetSoftSkillInfoByDescricao(skillsClassified.Softskill, VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.TOKEN_SISTEMA_COLABORACAO)) : new List<KeyValuePair<string, long>>();
            foreach (var item in softskillsFourmakers.Where(x => softskillsAnteriores.Where(y => y.Description_id == x.Value).Any() == false))
            {
                var candidateSkillParam = BuildCandidateSkillParam(candidateId, (int)CategoriaSkillEnum.Softskills, (int)item.Value, NIVEL_INDEFINIDO_SOFTSKILL);
                _srsInfraClient.InsertSkillCandidate(candidateSkillParam);
            }

            // Skills Desconhecidas
            var skillsDesconhecidasFourmakers = skillsClassified.Nao_classificada.Count > 0 ? await _skillDesconhecidaClient.GetSkillDesconhecidaInfoByDescricao(skillsClassified.Nao_classificada, VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.TOKEN_SISTEMA_COLABORACAO)) : new List<KeyValuePair<string, long>>();
            foreach (var item in skillsDesconhecidasFourmakers.Where(x => skillsDesconhecidasAnteriores.Where(y => y.Description_id == x.Value).Any() == false))
            {
                var candidateSkillParam = BuildCandidateSkillParam(candidateId, (int)CategoriaSkillEnum.Desconhecida, (int)item.Value, NIVEL_INDEFINIDO_DESCONHECIDA);
                _srsInfraClient.InsertSkillCandidate(candidateSkillParam);
            }
        }

        private async Task<List<SRSInsertCandidateSkillSQS>> ConstruirListCandidateSkillParam(Root result, int candidateId, int category, int nivelIndefinido, List<CandidateSkillDTO> skillsAnteriores)
        {
            // Lista que armazenará os parâmetros do candidato
            var lista = new List<CandidateSkillParam>();

            // Verificar a categoria
            if (category == CATEGORY_HARDSKILL) // Para Habilidades
            {
                // Obter as habilidades do LinkedIn
                var skillsLinkedin = result.skills.Select(x => x.name).ToList();

                // Buscar as habilidades na Fourmakers, caso existam habilidades
                if (skillsLinkedin.Count > 0)
                {
                    var skillsFourmakers = await _competenciaClient.GetHardSkillInfoByDescricao(skillsLinkedin, VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.TOKEN_SISTEMA_COLABORACAO));

                    // Adicionar as habilidades que não estão em skillsAnteriores
                    foreach (var item in skillsFourmakers.Where(x => skillsAnteriores.All(y => y.Description_id != x.Value)))
                    {
                        var candidateSkillParam = BuildCandidateSkillParam(candidateId, category, (int)item.Value, nivelIndefinido);
                        lista.Add(candidateSkillParam);
                    }
                }
            }
            else if (category == CATEGORY_IDIOMA) // Para Idiomas
            {
                // Obter os idiomas do LinkedIn
                var idiomasLinkedin = result.profile.languages.Select(x => x.name).ToList();

                // Buscar os idiomas na Fourmakers, caso existam idiomas
                if (idiomasLinkedin.Count > 0)
                {
                    var idiomasFourmakers = await _idiomaClient.GetIdiomaInfoByDescricao(idiomasLinkedin, VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.TOKEN_SISTEMA_COLABORACAO));

                    // Adicionar os idiomas que não estão em skillsAnteriores
                    foreach (var item in idiomasFourmakers.Where(x => skillsAnteriores.All(y => y.Description_id != x.Value)))
                    {
                        var candidateSkillParam = BuildCandidateSkillParam(candidateId, category, (int)item.Value, NIVEL_INDEFINIDO_IDIOMA);
                        lista.Add(candidateSkillParam);
                    }
                }
            }

            var resultado =
                lista.Select(skill => new SRSInsertCandidateSkillSQS
                {
                    Id = skill.Description_id.ToString(),
                    Nivel = skill.Nivel_id.ToString()
                })
                .ToList();

            return resultado;
        }

        private static CandidateSkillParam BuildCandidateSkillParam(int candidateId, int categoryId, int hardSkillId, int nivelId)
        {
            return new CandidateSkillParam
            {
                Candidate_id = candidateId,
                Category_id = categoryId,
                Description_id = hardSkillId,
                Nivel_id = nivelId,
                EntrevistaId = 0,
                DateCreated = DateTime.Now,
                DateModified = DateTime.Now
            };
        }

        private void SaveCandidateHistory(CadastroCandidatoInput request, int candidateId)
        {
            var historicoDTO = BuildHistoricoDTO(request.UserId, candidateId);
            _srsInfraClient.InsertHistoricoCandidato(historicoDTO);
        }

        private void SaveCandidateHistory(int candidateId)
        {
            var historicoDTO = BuildHistoricoDTO("1", candidateId);
            _srsInfraClient.InsertHistoricoCandidato(historicoDTO);
        }

        private static HistoricoDTO BuildHistoricoDTO(string userId, long candidateId)
        {
            return new HistoricoDTO
            {
                data_item_type = 100,
                data_item_id = candidateId,
                the_field = "!newEntry!",
                previous_value = null,
                new_value = null,
                description = "(USER) created entry.",
                set_date = DateTime.Now,
                entered_by = userId,
                site_id = 1,
                flag = 0
            };
        }

        public async Task<int> InsertCandidateEntrevista(EntrevistaParam param)
        {
            try
            {
                return await _srsInfraClient.InsertCandidateEntrevistaAsync(param);
            }
            catch
            {
                throw;
            }
        }

        public List<CandidateRelatorioBI> GetRelatorioCandidate(string token)
        {
            try
            {
                return _srsInfraClient.GetRelatorioCandidatos();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<AtualizaCadastroCandidatoInputResponse> AtualizaCurriculoCandidate(AtualizaCadastroCandidatoInput candidate)
        {
            try
            {
                Root resultLinkedin;
                var candInfo = _srsInfraClient.GetCandidate(candidate.CandidateId);

                if (candInfo == null)
                    throw new ValidationException("Candidato não encontrado");

                try
                {
                    resultLinkedin = await _curriculoClient.GetPerfilLinkedinRapidAPI(GetVanityName(candInfo.cand_Lkdin), VariaveisDeAmbienteUtil.GetVariavelDeAmbiente("CURRICULO_API_TOKEN"));
                }
                catch (Exception)
                {
                    throw new ValidationException("Perfil inválido");
                }

                if (resultLinkedin is null
                        || resultLinkedin.profile is null
                        || resultLinkedin.profile.entityUrn.IsEmpty())
                throw new ValidationException("Perfil não encontrado");                

                UpdateCandidateCurriculum(resultLinkedin, candidate.CandidateId);
                await UpdateCandidateSkills(resultLinkedin, candidate.CandidateId);      
                
                // if (string.IsNullOrEmpty(candInfo.emailFoursys))
                // {
                //     await EnviarOuAtualizarCandidatoParaSQS(resultLinkedin, candidate.CandidateId, candInfo.email1, CRUDEnum.Update);
                // }

                return new AtualizaCadastroCandidatoInputResponse
                {
                    Mensagem = "Candidato Atualizado com sucesso!",
                    CandidateId = candidate.CandidateId
                };
            }
            catch (ValidationException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        
    }
}