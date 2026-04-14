using ApiClient.Domain.Interfaces;
using ApiClient.Infra.Interfaces;
using Colaboracao.Helper;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.SRS.Vagas;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain.VagasSRS;

namespace ApiClient.Domain.Impl
{
    public class SRSVagaClient : ISRSVagaClient
    {
        private readonly IApiClient _apiClient;
        private readonly string _baseURLPath;

        public SRSVagaClient(IApiClient apiClient, IConfiguration configuration)
        {
            _apiClient = apiClient;
            _baseURLPath = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.BASE_URL_SRS_SERVIDOR_INTERNO) + "/api";
        }

        public async Task<ApiGenericResult<List<DropDownItemDTO>>> ListarSolicitantes(string token)
        {
            try
            {
                var url = $"{_baseURLPath}/VagaSRS/ListarSolicitantes";
                var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Authorization", $"Bearer {token}")
                };

                var response = await _apiClient.GetAsync<ApiGenericResult<List<DropDownItemDTO>>>(url, headers);
                if (response.Sucesso)
                {
                    return response.Resposta;
                }
                throw new Exception("Erro ao listar solicitantes: " + response.Mensagem);
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro ao listar solicitantes.", ex);
            }
        }

        public async Task<ApiGenericResult<List<DropDownItemDTO>>> ListarAprovadores(string token)
        {
            try
            {
                var url = $"{_baseURLPath}/VagaSRS/ListarAprovadores";
                var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Authorization", $"Bearer {token}")
                };

                var response = await _apiClient.GetAsync<ApiGenericResult<List<DropDownItemDTO>>>(url, headers);
                if (response.Sucesso)
                {
                    return response.Resposta;
                }
                throw new Exception("Erro ao listar aprovadores: " + response.Mensagem);
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro ao listar aprovadores.", ex);
            }
        }

        public async Task<ApiGenericResult<List<DropDownItemDTO>>> ListarTermometroVagas(string token)
        {
            try
            {
                var url = $"{_baseURLPath}/VagaSRS/ListarTermometroVagas";
                var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Authorization", $"Bearer {token}")
                };

                var response = await _apiClient.GetAsync<ApiGenericResult<List<DropDownItemDTO>>>(url, headers);
                if (response.Sucesso)
                {
                    return response.Resposta;
                }
                throw new Exception("Erro ao listar termômetro de vagas: " + response.Mensagem);
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro ao listar termômetro de vagas.", ex);
            }
        }

        public async Task<ApiGenericResult<List<DropDownItemDTO>>> ListarStackPrincipal(string token)
        {
            try
            {
                var url = $"{_baseURLPath}/VagaSRS/ListarStackPrincipal";
                var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Authorization", $"Bearer {token}")
                };

                var response = await _apiClient.GetAsync<ApiGenericResult<List<DropDownItemDTO>>>(url, headers);
                if (response.Sucesso)
                {
                    return response.Resposta;
                }
                throw new Exception("Erro ao listar stack principal: " + response.Mensagem);
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro ao listar stack principal.", ex);
            }
        }

        public async Task<ApiGenericResult<List<DropDownItemDTO>>> ListarConfiguracaoMaquina(string token, string idContaCrm, int hardskillId)
        {
            try
            {
                var url = $"{_baseURLPath}/VagaSRS/ListarConfiguracaoMaquina?idContaCrm={idContaCrm}&hardskillId={hardskillId}";
                var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Authorization", $"Bearer {token}")
                };

                var response = await _apiClient.GetAsync<ApiGenericResult<List<DropDownItemDTO>>>(url, headers);
                if (response.Sucesso)
                {
                    return response.Resposta;
                }
                throw new Exception("Erro ao listar configuração de máquina: " + response.Mensagem);
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro ao listar configuração de máquina.", ex);
            }
        }

        public async Task<ApiGenericResult<List<DropDownItemDTO>>> ListarTipoVaga(string token)
        {
            try
            {
                var url = $"{_baseURLPath}/VagaSRS/ListarTipoVaga";
                var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Authorization", $"Bearer {token}")
                };

                var response = await _apiClient.GetAsync<ApiGenericResult<List<DropDownItemDTO>>>(url, headers);
                if (response.Sucesso)
                {
                    return response.Resposta;
                }
                throw new Exception("Erro ao listar tipos de vaga: " + response.Mensagem);
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro ao listar tipos de vaga.", ex);
            }
        }

        public async Task<ApiGenericResult<List<DropDownItemDTO>>> ListarDuracaoContrato(string token)
        {
            try
            {
                var url = $"{_baseURLPath}/VagaSRS/ListarDuracaoContrato";
                var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Authorization", $"Bearer {token}")
                };

                var response = await _apiClient.GetAsync<ApiGenericResult<List<DropDownItemDTO>>>(url, headers);
                if (response.Sucesso)
                {
                    return response.Resposta;
                }
                throw new Exception("Erro ao listar duração de contrato: " + response.Mensagem);
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro ao listar duração de contrato.", ex);
            }
        }

        public async Task<ApiGenericResult<List<CargoDropdownItemDTO>>> ListarCargos(string token)
        {
            try
            {
                var url = $"{_baseURLPath}/VagaSRS/ListarCargos";
                var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Authorization", $"Bearer {token}")
                };

                var response = await _apiClient.GetAsync<ApiGenericResult<List<CargoDropdownItemDTO>>>(url, headers);
                if (response.Sucesso)
                {
                    return response.Resposta;
                }
                throw new Exception("Erro ao listar cargos: " + response.Mensagem);
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro ao listar cargos.", ex);
            }
        }

        public async Task<ApiGenericResult<List<DropDownItemDTO>>> ListarTipoContratacao(string token)
        {
            try
            {
                var url = $"{_baseURLPath}/VagaSRS/ListarTipoContratacao";
                var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Authorization", $"Bearer {token}")
                };

                var response = await _apiClient.GetAsync<ApiGenericResult<List<DropDownItemDTO>>>(url, headers);
                if (response.Sucesso)
                {
                    return response.Resposta;
                }
                throw new Exception("Erro ao listar tipos de contratação: " + response.Mensagem);
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro ao listar tipos de contratação.", ex);
            }
        }

        public async Task<ApiGenericResult<List<DropDownItemDTO>>> ListarCargaHoraria(string token)
        {
            try
            {
                var url = $"{_baseURLPath}/VagaSRS/ListarCargaHoraria";
                var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Authorization", $"Bearer {token}")
                };

                var response = await _apiClient.GetAsync<ApiGenericResult<List<DropDownItemDTO>>>(url, headers);
                if (response.Sucesso)
                {
                    return response.Resposta;
                }
                throw new Exception("Erro ao listar carga horária: " + response.Mensagem);
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro ao listar carga horária.", ex);
            }
        }

        public async Task<ApiGenericResult<List<DropDownItemDTO>>> ListarLocalTrabalho(string token)
        {
            try
            {
                var url = $"{_baseURLPath}/VagaSRS/ListarLocalTrabalho";
                var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Authorization", $"Bearer {token}")
                };

                var response = await _apiClient.GetAsync<ApiGenericResult<List<DropDownItemDTO>>>(url, headers);
                if (response.Sucesso)
                {
                    return response.Resposta;
                }
                throw new Exception("Erro ao listar local de trabalho: " + response.Mensagem);
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro ao listar local de trabalho.", ex);
            }
        }

        public async Task<ApiGenericResult<List<JobOrderSemanticaDTO>>> ListarVagasParaSemanticaComSkill(string token, int cursor, int limite)
        {
            try
            {
                var url = $"{_baseURLPath}/VagaSRS/ListarVagasParaSemanticaComSkill?cursor={cursor}&limite={limite}";
                var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Authorization", $"Bearer {token}")
                };

                var response = await _apiClient.GetAsync<ApiGenericResult<List<JobOrderSemanticaDTO>>>(url, headers);
                if (response.Sucesso)
                {
                    return response.Resposta;
                }
                throw new Exception("Erro ao listar vagas para semantica do SRS: " + response.Mensagem);
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro ao listar vagas para semantica do SRS.", ex);
            }
        }
        
        public async Task<ApiGenericResult<List<DropDownItemDTO>>> ListarUnidadesSRS(string token)
        {
            try
            {
                var url = $"{_baseURLPath}/VagaSRS/ListarUnidadesSRS";
                var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Authorization", $"Bearer {token}")
                };

                var response = await _apiClient.GetAsync<ApiGenericResult<List<DropDownItemDTO>>>(url, headers);
                if (response.Sucesso)
                {
                    return response.Resposta;
                }
                throw new Exception("Erro ao listar unidades SRS: " + response.Mensagem);
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro ao listar unidades SRS.", ex);
            }
        }
    }
}