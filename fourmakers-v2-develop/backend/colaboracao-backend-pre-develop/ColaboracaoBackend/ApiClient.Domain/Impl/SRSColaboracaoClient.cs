using ApiClient.Domain.Interfaces;
using ApiClient.Infra.Interfaces;
using Colaboracao.Helper;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.SRS;
using DataTransferObject.Domain.SRS.Candidate;
using DataTransferObject.Domain.SRS.Vagas;
using DataTransferObject.Domain.Vaga;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SRS.Infra.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace ApiClient.Domain.Impl
{
    public class SRSColaboracaoClient : ISRSColaboracaoClient
    {
        private IApiClient _apiClient;
        private IConfiguration _configuration;
        private ISRSInfraClient _srsInfraClient;
        private string _baseURLPath;
        private string _baseURLColaboradorPath;

        public SRSColaboracaoClient(IApiClient apiClient, IConfiguration configuration, ISRSInfraClient sRSInfraClient, ILogger<UsuarioClient> logger)
        {
            _apiClient = apiClient;
            _configuration = configuration;
            _srsInfraClient = sRSInfraClient;
            _baseURLPath = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.COLABORACAO_BASE) + VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.SRS_API_PATH);
            _baseURLColaboradorPath = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.COLABORACAO_BASE) + VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.COLABORADOR_API_PATH);
        }

        public async Task<List<VagaDTO>> BuscarVagaSRS(string token)
        {
            try
            {
                var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>
                    (
                        "Authorization",
                        String.Format("Bearer {0}", token)
                    )
                };

                var responseMessage = await _srsInfraClient.GetVagaInfra();

                if (responseMessage != null)
                {
                    return responseMessage;
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception ex) { throw new Exception(); }
        }

        public async Task<CriarVagasSRSParam> EditarCriarVaga(string token, CriarVagasSRSParam param)
        {
            var baseUrl = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.BASE_URL_SRS_SERVIDOR_INTERNO);
            var cadastrarVaga = "/api/" + _configuration["Clients:SRS:EditarCriarVagaFourmakersSRS"];
            var url = baseUrl + cadastrarVaga;
            var headers = new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("Authorization", $"Bearer {token}") };

            var responseMessage = await _apiClient.PostAsync<ApiGenericResult<CriarVagasSRSParam>>(param, url, headers);

            if (!responseMessage.Sucesso)
            {
                ExceptionUtil.TratarHttpStatusException(
                    Enum.Parse<HttpStatusCode>(responseMessage.HttpStatus),
                    responseMessage.Mensagem,
                    responseMessage.Resposta?.Mensagem
                );
            }
            return responseMessage.Resposta.Retorno;
        }

        public async Task<CriarVagasSRSParam> ObterVagaPorId(string token, int vagaId)
        {
            var baseUrl = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.BASE_URL_SRS_SERVIDOR_INTERNO);
            var obterVagaPorId = "/api/" + _configuration["Clients:SRS:ObterVagaPorId"];
            var url = $"{baseUrl}{obterVagaPorId}?vagaId={vagaId}";
            var headers = new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("Authorization", $"Bearer {token}") };

            var responseMessage = await _apiClient.GetAsync<ApiGenericResult<CriarVagasSRSParam>>(url, headers);

            if (!responseMessage.Sucesso)
            {
                ExceptionUtil.TratarHttpStatusException(
                    Enum.Parse<HttpStatusCode>(responseMessage.HttpStatus),
                    responseMessage.Mensagem,
                    responseMessage.Resposta?.Mensagem
                );
            }
            return responseMessage.Resposta.Retorno;
        }

        public async Task<bool> GetColaboradorAtivoOuInativoPorCPF(string colaboradorCpf, string token)
        {
            bool ret;
            try
            {
                var baseUrl = _baseURLColaboradorPath;
                var BuscaColaborador = "Colaborador/BuscarNomeColaborador?cpfColaborador=" + colaboradorCpf;
                var url = baseUrl + BuscaColaborador;
                var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Authorization", $"Bearer {token}")
                };

                var responseMessage = await _apiClient.GetAsync<NomeColaboradorResult>(url, headers);

                if (responseMessage.Sucesso && responseMessage.Resposta.Mensagem == "Colaborador não encontrado no sistema.")
                {
                    return false;
                }
                if (responseMessage.Sucesso)
                {
                    return true;
                }
                else
                {
                    throw new Exception("Ocorreu um erro ao na busca do colaborador.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro ao na busca do colaborador.", ex);
            }
        }

        public async Task<List<CandidateRelatorioBI>> GetRelatorioCandidatosRaw(string token)
        {
            bool ret;
            try
            {
                var baseUrl = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.BASE_URL_SRS_SERVIDOR_INTERNO);
                var url = baseUrl + "/api/SRS/GetRelatorioCandidatosRaw";
                var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Authorization", $"Bearer {token}")
                };

                var responseMessage = await _apiClient.GetAsync<List<CandidateRelatorioBI>>(url, headers);

                if (responseMessage.Sucesso)
                {
                    return responseMessage.Resposta;
                }
                else
                {
                    throw new Exception("Ocorreu um erro na api do relatorio GetRelatorioCandidatosRaw.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro generico na api do relatorio GetRelatorioCandidatosRaw.", ex);
            }
        }

        public async Task<ApiGenericResultInteger> CadastroCandidatoFourmakersLinkedin(string token, CadastroCandidatoLinkedinInput param)
        {
            try
            {
                var baseUrl = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.BASE_URL_SRS_SERVIDOR_INTERNO) + "/api/";
                var cadastrarCandidato = _configuration["Clients:SRS:CadastroCandidatoFourmakersLinkedin"];
                var url = baseUrl + cadastrarCandidato;
                var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Authorization", $"Bearer {token}")
                };

                var responseMessage = await _apiClient.PostAsync<ApiGenericResultInteger>(param, url, headers);

                if (responseMessage.Sucesso)
                {
                    return responseMessage.Resposta;
                }
                else
                {
                    throw new Exception("Erro ao criar o candidato no SRS: " + responseMessage.Mensagem);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}