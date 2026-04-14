using ApiClient.Domain;
using ApiClient.Infra.Interfaces;
using Colaboracao.Helper;
using DataTransferObject.Domain.Labs.MatchSemantico;
using Labs.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Labs.Infra
{
    /// <summary>
    /// Implementação do cliente para a API Match Semântico (GCP) — best_candidates/hyde.
    /// </summary>
    public class MatchSemanticoClient : IMatchSemanticoClient
    {
        private readonly IApiClient _apiClient;
        private readonly string _baseUrl;

        public MatchSemanticoClient(IApiClient apiClient)
        {
            _apiClient = apiClient;
            _baseUrl = (VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.MATCH_SEMANTICO_API) ?? "")
                .TrimEnd('/');
        }

        public async Task<MatchSemanticoHydeResult> BuscarMelhoresCandidatosAsync(MatchSemanticoHydeRequest request)
        {
            if (string.IsNullOrWhiteSpace(_baseUrl))
            {
                return new MatchSemanticoHydeResult
                {
                    Sucesso = false,
                    HttpStatus = "500",
                    Mensagem = "MATCH_SEMANTICO_API não configurada."
                };
            }

            var body = new
            {
                vaga = request?.Vaga,
                cidade = request?.Cidade,
                estado = request?.Estado,
                categoria = request?.Categoria,
                origem = request?.Origem
            };

            var url = $"{_baseUrl}/best_candidates/hyde";
            var headers = new List<KeyValuePair<string, string>>();

            var response = await _apiClient.PostAsync<MatchSemanticoHydeResponseDto>(body, url, headers);

            return new MatchSemanticoHydeResult
            {
                Sucesso = response.Sucesso,
                HttpStatus = response.HttpStatus,
                Mensagem = response.Mensagem,
                Resposta = response.Resposta != null ? response.Resposta.Result : null
            };
        }
    }
}
