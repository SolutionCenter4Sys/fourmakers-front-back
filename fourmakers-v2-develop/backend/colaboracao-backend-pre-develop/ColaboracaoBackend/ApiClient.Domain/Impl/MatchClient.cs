using ApiClient.Domain.Interfaces;
using ApiClient.Infra.Interfaces;
using Colaboracao.Helper;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Match;
using DataTransferObject.Domain.Usuario;
using DataTransferObject.Domain.Vaga;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace ApiClient.Domain.Impl
{
    public class MatchClient : IMatchClient
    {
        private IApiClient _apiClient;
        private IConfiguration _configuration;
        private ILogger _log;
        private string _baseURL;
        private string _baseExtractorURL;
        private string _token;

        public MatchClient(IApiClient apiClient, IConfiguration configuration, ILogger<UsuarioClient> logger)
        {
            _apiClient = apiClient;
            _configuration = configuration;
            _baseURL = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.MATCH_API);
            _baseExtractorURL = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.EXTRACTOR_API);
            _log = logger;
            _token = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.TOKEN_MATCH_API);
        }

        public async Task<List<CandidatosMatchResponse>> RankCandidates(CandidatosMatchRequest request)
        {
            var url = _baseURL + "/rank-candidates";
            var headers = new List<KeyValuePair<string, string>>();
            headers.Add(new KeyValuePair<string, string>("X-API-Key", _token));
            
            _log.LogInformation($"Match Rank Candidate");
            _log.LogInformation(JsonConvert.SerializeObject(request));
            var responseMessage = await _apiClient.PostAsync<List<CandidatosMatchResponse>>(request, url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta;
            }
            else
            {
                _log.LogError(JsonConvert.SerializeObject("Erro na requisicao do Match"));
                _log.LogError(JsonConvert.SerializeObject(responseMessage.Mensagem, Formatting.Indented));
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<List<CandidatosMatchResponse>> RankCandidatesIds(CandidatosMatchRequestIds request)
        {
            var url = _baseURL + "/rank-candidates-ids";
            var headers = new List<KeyValuePair<string, string>>();
            headers.Add(new KeyValuePair<string, string>("X-API-Key", _token));

            _log.LogInformation($"Match Rank Candidate");
            _log.LogInformation(JsonConvert.SerializeObject(request));
            var responseMessage = await _apiClient.PostAsync<List<CandidatosMatchResponse>>(request, url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta;
            }
            else
            {
                _log.LogError(JsonConvert.SerializeObject("Erro na requisicao do Match"));
                _log.LogError(JsonConvert.SerializeObject(responseMessage.Mensagem, Formatting.Indented));
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<CandidatosMatchResponse> ScoreSingleCandidate(ScoreSingleCandidateRequest request)
        {
            var url = _baseURL + "/score-single-candidate";
            var headers = new List<KeyValuePair<string, string>>();
            headers.Add(new KeyValuePair<string, string>("X-API-Key", _token));

            _log.LogInformation($"Match Single Candidate");
            _log.LogInformation(JsonConvert.SerializeObject(request));
            var responseMessage = await _apiClient.PostAsync<CandidatosMatchResponse>(request, url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta;
            }
            else
            {
                _log.LogError(JsonConvert.SerializeObject("Erro na requisicao do Match (score-single-candidate)"));
                _log.LogError(JsonConvert.SerializeObject(responseMessage.Mensagem, Formatting.Indented));
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<ExtrairPerfilDeUmPromptResponse> ExtrairPerfilDeUmPrompt(ExtrairPerfilDeUmPromptRequest request)
        {
            var url = _baseExtractorURL + "/extract-vaga";
            var headers = new List<KeyValuePair<string, string>>();
            headers.Add(new KeyValuePair<string, string>("X-API-Key", _token));

            var externalApiRequest = new
            {
                texto_vaga = request.texto_vaga,
                llm_provider = "openai",
                model_name = "gpt-4o-mini",
                api_key = "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" // TODO
            };

            var jsonContent = JsonConvert.SerializeObject(externalApiRequest);
            var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            _log.LogInformation($"ExtrairPerfilDeUmPrompt - Chamando API");
            _log.LogInformation(JsonConvert.SerializeObject(externalApiRequest));

            var responseMessage = await _apiClient.PostAsync<ExtrairPerfilDeUmPromptResponse>(externalApiRequest, url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta;
            }
            else
            {
                _log.LogError(JsonConvert.SerializeObject("Erro na requisicao do Match Extrair Perfil Prompt"));
                _log.LogError(JsonConvert.SerializeObject(responseMessage.Mensagem, Formatting.Indented));
                throw new Exception(responseMessage.Mensagem);
            }
        }
    }
}