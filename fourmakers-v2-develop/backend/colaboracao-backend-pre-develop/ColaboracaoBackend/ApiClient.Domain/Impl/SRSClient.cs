using ApiClient.Domain.Interfaces;
using ApiClient.Infra.Interfaces;
using Colaboracao.Helper;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.SRS;
using DataTransferObject.Domain.SRS.Candidate;
using DataTransferObject.Domain.VagasSRS;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SRS.Infra.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiClient.Domain.Impl
{
    public class SRSClient : ISRSClient
    {
        private IApiClient _apiClient;
        private IConfiguration _configuration;
        private ISRSInfraClient _srsInfraClient;
        private ILogger _log;
        private string _baseURL;
        private string _baseURLSRS;
        private string _usernamesrs;
        private string _passwordsrs;
        private string _baseURLRazao;
        private string _baseURLCandidatoInscritoVagas;
        private string _baseCandidatarSe;
        private string _baseDescandidatarSe;
        private string _baseUrlValidarEmail;
        private string _baseUrlSrsCandidate;
        private string _baseUrlSrsContatoEmergencia;
        private string _baseURLPath;
        private string _baseURLSRSInterno;

        public SRSClient(IApiClient apiClient, IConfiguration configuration, ISRSInfraClient sRSInfraClient,
            ILogger<UsuarioClient> logger)
        {
            _apiClient = apiClient;
            _configuration = configuration;
            _srsInfraClient = sRSInfraClient;
            _baseURL = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.BASE_URL_SRS_AUT);
            _baseURLSRS = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.BASE_URL_SRS);
            _usernamesrs = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.USER_NAME_SRS);
            _passwordsrs = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.PASSWORD_SRS);
            _baseURLRazao = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.BASE_URL_RAZAO);
            _baseURLCandidatoInscritoVagas =
                VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.BASE_URL_CANDIDATO_INSCRITO_VAGAS);
            _baseCandidatarSe =
                VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.BASE_URL_CANDIDATAR_SE);
            _baseDescandidatarSe =
                VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.BASE_URL_DESCANDIDATAR_SE);
            _baseUrlValidarEmail =
                VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.BASE_URL_VALIDAR_EMAIL);
            _baseUrlSrsCandidate =
                VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.BASE_URL_SRS_CANDIDATO);
            _baseURLPath = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.COLABORACAO_BASE) +
                           VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.SRS_API_PATH);
            _baseURLSRSInterno =
                VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.BASE_URL_SRS_SERVIDOR_INTERNO);
             
            _log = logger;
        }

        public async Task<TokenSRS> Autenticacao()
        {
            var baseUrl = _baseURL;
            var url = baseUrl;
            var objeto = new { username = _usernamesrs, password = _passwordsrs };
            var headers = new List<KeyValuePair<string, string>>();

            var responseMessage = await _apiClient.PostAsync<TokenSRS>(objeto, url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<List<JobOrderDTO>> Requisicao(string tokenUsuario)
        {
            var baseUrl = _baseURLSRS;
            var url = baseUrl;
            var headers = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>
                (
                    "Authorization",
                    String.Format("Bearer {0}", tokenUsuario)
                )
            };
            var responseMessage = await _apiClient.GetAsync<JobOrdersResponse>(url, headers);
            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta.Data;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<List<MotivosDTO>> RequisicaoRazao(string tokenUsuario)
        {
            var baseUrl = _baseURLRazao;
            var url = baseUrl;
            var headers = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>
                (
                    "Authorization",
                    String.Format("Bearer {0}", tokenUsuario)
                )
            };
            var responseMessage = await _apiClient.GetAsync<SRSResult>(url, headers);
            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta.Data;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<List<VagasInscritoDTO>> CandidatoInscritoNasVagas(string tokenUsuario, int candidate_id)
        {
            var baseUrl = _baseURLCandidatoInscritoVagas;
            var url = baseUrl + candidate_id;
            var headers = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>
                (
                    "Authorization",
                    String.Format("Bearer {0}", tokenUsuario)
                )
            };
            var responseMessage = await _apiClient.GetAsync<InscricoesCandidatoResult>(url, headers);
            if (responseMessage.Sucesso)
            {
                var listVagasInscritas = new List<VagasInscritoDTO>();
                foreach (var item in responseMessage.Resposta.Data)
                {
                    item.Jo_Stvaga = FormatStatusVagaSrs(item.Jo_Stvaga);
                    listVagasInscritas.Add(item);
                }

                return listVagasInscritas;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        private string FormatStatusVagaSrs(string statusSrs)
        {
            string statusFourmaker = null;
            switch (statusSrs)
            {
                case "STAND BY":
                    statusFourmaker = "CANCELADA";
                    break;

                case "FECHADA OC":
                    statusFourmaker = "CANCELADA";
                    break;

                case "FECHADA FOURSYS":
                    statusFourmaker = "CONCLUÍDA";
                    break;

                case "FECHADA RI":
                    statusFourmaker = "CONCLUÍDA";
                    break;

                case "EM APROVAÇÃO":
                    statusFourmaker = "EM AVALIAÇÃO";
                    break;

                case "INDICADO CONTRATADO":
                    statusFourmaker = "CONCLUÍDA";
                    break;

                case "DECURSO DE PRAZO":
                    statusFourmaker = "CANCELADA";
                    break;

                case "AGUARDANDO AJUSTES":
                    statusFourmaker = "EM AVALIAÇÃO";
                    break;

                default:
                    break;
            }

            return statusFourmaker != null ? statusFourmaker : statusSrs;
        }

        public async Task<CandidatarResult> CandidatarSeAUmaVaga(string tokenUsuario, long joborder_id,
            int candidate_id, string origem)
        {
            var baseUrl = _baseCandidatarSe;
            var url = baseUrl;
            var objeto = new { candidate_id = candidate_id, joborder_id = joborder_id, origem = origem };
            var headers = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>
                (
                    "Authorization",
                    String.Format("Bearer {0}", tokenUsuario)
                )
            };

            var responseMessage = await _apiClient.PostAsync<CandidatarResult>(objeto, url, headers);

            return responseMessage.Resposta;
        }

        public async Task<DescandidatarResult> DescandidatarSeDeUmaVaga(string tokenUsuario, long joborder_id,
            int candidate_id, string origem, int status_reason_cancellation, int reason_cancellation)
        {
            var baseUrl = _baseDescandidatarSe;
            var url = baseUrl + candidate_id;
            var objeto = new
            {
                joborder_id = joborder_id,
                origem = origem,
                status_reason_cancellation = status_reason_cancellation,
                reason_cancellation = reason_cancellation
            };
            var headers = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>
                (
                    "Authorization",
                    String.Format("Bearer {0}", tokenUsuario)
                )
            };

            var responseMessage = await _apiClient.PostAsync<DescandidatarResult>(objeto, url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<SRSColaboradorResult> GetColaboradorSRS(string email, string tokenUsuario)
        {
            var url = _baseUrlValidarEmail;
            var objeto = new { email };
            var headers = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>
                (
                    "Authorization",
                    String.Format("Bearer {0}", tokenUsuario)
                )
            };

            var responseMessage = await _apiClient.PostAsync<SRSColaboradorResult>(objeto, url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<SRSCandidatePostDTO> PostCandidate(string tokenUsuario, string cpf,
            SRSCandidateDTO colabInfoCandidate, List<SRSCandidateContatosEmergenciaDTO> contatos_emergencia)
        {
            var baseUrl = _baseUrlSrsCandidate + "/" + cpf;
            var contatosEmergenciaAux = contatos_emergencia != null
                ? contatos_emergencia.Select(contato => new
                {
                    name = contato.name,
                    degree_kinship = contato.degree_kinship,
                    phone = contato.phone,
                    contact_order = contato.contact_order
                })
                : null;
            var url = baseUrl;
            var objeto = new
            {
                first_name = colabInfoCandidate.first_name,
                phone_home = colabInfoCandidate.phone_home,
                phone_cell = colabInfoCandidate.phone_cell,
                address = colabInfoCandidate.address,
                address_number = colabInfoCandidate.address_number,
                address_complement = colabInfoCandidate.address_complement,
                district = colabInfoCandidate.district,
                city = colabInfoCandidate.city,
                state = colabInfoCandidate.state,
                zip = colabInfoCandidate.zip,
                source = colabInfoCandidate.source,
                key_skills = colabInfoCandidate.key_skills != null
                    ? String.Join(", ", colabInfoCandidate.key_skills)
                    : "",
                methodologies = colabInfoCandidate.key_skills != null
                    ? String.Join(", ", colabInfoCandidate.methodologies)
                    : "",
                email1 = colabInfoCandidate.email1,
                email2 = colabInfoCandidate.email2,
                emailFoursys = colabInfoCandidate.emailFoursys,
                desired_pay = colabInfoCandidate.desired_pay,
                current_pay = colabInfoCandidate.current_pay,
                cand_rg = colabInfoCandidate.cand_rg,
                cand_estCivil = colabInfoCandidate.cand_estCivil,
                cand_Lkdin = colabInfoCandidate.cand_Lkdin,
                cand_skype = colabInfoCandidate.cand_skype,
                cand_gruporisco = colabInfoCandidate.cand_gruporisco,
                instagram = colabInfoCandidate.instagram,
                facebook = colabInfoCandidate.facebook,
                twitter = colabInfoCandidate.twitter,
                cand_filhos = colabInfoCandidate.cand_filhos,
                dataNascimento = colabInfoCandidate.dataNascimento,
                disponibilidade = colabInfoCandidate.disponibilidade,
                zona = colabInfoCandidate.zona,
                pcd = colabInfoCandidate.pcd,
                genre = colabInfoCandidate.genre,
                sexual_orientation = colabInfoCandidate.sexual_orientation,
                ethnicity = colabInfoCandidate.ethnicity,
                school_level = colabInfoCandidate.school_level,
                refugee_person = colabInfoCandidate.refugee_person,
                candidate_emergency_contact = contatosEmergenciaAux != null
                    ? JsonConvert.SerializeObject(contatosEmergenciaAux)
                    : null,
                file = colabInfoCandidate.file ?? "",
            };
            var headers = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>
                (
                    "Authorization",
                    String.Format("Bearer {0}", tokenUsuario)
                )
            };

            var responseMessage = await _apiClient.PostAsync<SRSCandidatePostDTO>(objeto, url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<SRSCandidateResult> GetCandidate(string tokenUsuario, string cpf)
        {
            var baseUrl = _baseUrlSrsCandidate + "/" + cpf;
            var url = baseUrl;

            var headers = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>
                (
                    "Authorization",
                    String.Format("Bearer {0}", tokenUsuario)
                )
            };

            var responseMessage = await _apiClient.GetAsync<SRSCandidateResult>(url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<ApiGenericResult<bool>> AlterarCategoriaHabilidade(SRSAlterarCategoriaHabilidadeParam param, string tokenUsuario)
        {
            var _tokenSistema = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.TOKEN_SISTEMA_COLABORACAO);

            var url = _baseURLSRSInterno;
            var path = _configuration["Clients:SRS:AlterarCategoriaHabilidade"];
            
            var finalUrl = url + "/api/" + path;
            
            var headers = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>
                (
                    "Authorization",
                    String.Format("Bearer {0}", tokenUsuario)
                )
            };

            var responseMessage = await _apiClient.PostAsync<ApiGenericResult<bool>>(param, finalUrl, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }
    }
}