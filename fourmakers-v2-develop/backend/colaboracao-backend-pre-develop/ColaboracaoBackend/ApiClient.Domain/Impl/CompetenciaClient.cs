using ApiClient.Domain.Interfaces;
using ApiClient.Infra.Interfaces;
using Colaboracao.Helper;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Certificado;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.SRS.Candidate;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Competencia.API.DTOs;
using Competencia.Domain.Enums;
using DataTransferObject.Domain.Competencia.MapaCompetencia;

namespace ApiClient.Domain.Impl
{
    public class CompetenciaClient : ICompetenciaClient
    {
        private IApiClient _apiClient;
        private IConfiguration _configuration;
        private string _baseURL;
        private ILogger _log;

        public CompetenciaClient(IApiClient apiClient, IConfiguration configuration, ILogger<CompetenciaClient> log)
        {
            _apiClient = apiClient;
            _configuration = configuration;
            _baseURL = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.COLABORACAO_BASE) +
                VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.COMPETENCIA_API_PATH);
            _log = log;
        }

        public async Task<List<ItemPerfilResult>> AdicionarCompetenciaColaborador(List<AdicionarRemoverItemDTO> dtos, string token)
        {
            var baseUrl = _baseURL;
            var adicionarCompetencia = _configuration["Clients:Competencia:AdicionarCompetenciaColaborador"];
            var url = baseUrl + adicionarCompetencia;
            var objeto = new { dtos };

            var headers = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Authorization",
                    String.Format("Bearer {0}", token))
            };

            var responseMessage = await _apiClient.PostAsync<CompetenciaColaboradorResult>(dtos, url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta.Respostas;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<List<SkillValuePair>> GetHardSkillInfoByDescricao(List<string> skills, string token)
        {
            var baseUrl = _baseURL;
            var endpoint = _configuration["Clients:Competencia:GetHardSkillInfoByDescricao"];
            var url = baseUrl + endpoint;

            var headers = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Authorization",
                    String.Format("Bearer {0}", token))
            };

            var responseMessage = await _apiClient.PostAsync<List<SkillValuePair>>(new { Skills = skills }, url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task AlteraCertificadoPrincipalColaborador(string cpf, long id, string tokenUsuario)
        {
            var baseUrl = _baseURL;
            var alteraCertificadoPrincipal = _configuration["Clients:Competencia:AlteraCertificadoPrincipalColaborador"];
            var url = baseUrl + alteraCertificadoPrincipal + "?id=" + id + "&cpf=" + cpf;

            var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>
                    (
                        "Authorization",
                        String.Format("Bearer {0}", tokenUsuario)
                    )
                };

            var objeto = new Object() { };

            var responseMessage = await _apiClient.PostAsync<StatusResult>(objeto, url, headers);

            if (!responseMessage.Sucesso)
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }
        public async Task AlteraCertificadoColaborador(string cpf, long competenciaColaboradorId, byte[] file, TipoCertificadoEnum tipo, DateTime dataConclusao, string instituicao, string descricao, string tokenUsuario)
        {
            var baseUrl = _baseURL;
            var alteraCertificado = _configuration["Clients:Competencia:AlteraCertificadoColaborador"];
            var url = baseUrl + alteraCertificado + "?id=" + competenciaColaboradorId + "&cpf=" + cpf;

            var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>
                    (
                        "Authorization",
                        String.Format("Bearer {0}", tokenUsuario)
                    )
                };

            var objeto = new CertificadoDTO()
            {
                conclusao = dataConclusao,
                descricao = descricao,
                instituicao = instituicao,
            };

            var responseMessage = await _apiClient.PostAsync<StatusResult>(objeto, url, headers);

            if (!responseMessage.Sucesso)
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<ItemPerfilResult> AlterarCompetenciaColaborador(AdicionarRemoverItemDTO dtos, string tokenUsuario)
        {
            var baseUrl = _baseURL;
            var listarCompetencias = _configuration["Clients:Competencia:AlterarCompetenciaColaborador"];
            var url = baseUrl + listarCompetencias;

            var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>
                    (
                        "Authorization",
                        String.Format("Bearer {0}", tokenUsuario)
                    )
                };

            var responseMessage = await _apiClient.PostAsync<ItemPerfilResult>(dtos, url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<ItemPerfilDTO> GetCompetenciaById(long id, string tokenUsuario)
        {
            var baseUrl = _baseURL;
            var getCompetenciaById = _configuration["Clients:Competencia:GetCompetenciaById"];
            var url = baseUrl + getCompetenciaById + $"?id={id}";

            var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>
                    (
                        "Authorization",
                        String.Format("Bearer {0}", tokenUsuario)
                    )
                };

            var responseMessage = await _apiClient.GetAsync<CompetenciaResult>(url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta.Competencia;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<CertificadoDTO> InserirCertificadoCompetenciaColaborador(string cpfRequest, long competenciaColaboradorId, byte[] file, TipoCertificadoEnum tipo, string tokenUsuario, DateTime dataConclusao, string insituicao, string descricao, int cargaHoraria)
        {
            var baseUrl = _baseURL;
            var inserirCertificadoCompetencia = _configuration["Clients:Competencia:InserirCertificadoCompetenciaColaborador"];
            var url = baseUrl + inserirCertificadoCompetencia;

            var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>
                    (
                        "Authorization",
                        String.Format("Bearer {0}", tokenUsuario)
                    )
                };

            MultipartFormDataContent form = new MultipartFormDataContent();

            form.Add(new StringContent(cpfRequest), "cpfRequest");
            form.Add(new StringContent(competenciaColaboradorId.ToString()), "competenciaColaboradorId");
            form.Add(new StringContent(tipo.ToString()), "tipo");
            form.Add(new ByteArrayContent(file, 0, file.Length), "arquivo", "arquivo");
            form.Add(new StringContent(dataConclusao.ToString()), "dataConclusao");
            form.Add(new StringContent(insituicao), "insituicao");
            form.Add(new StringContent(descricao), "descricao");
            form.Add(new StringContent(cargaHoraria.ToString()), "cargaHoraria");
            var responseMessage = await _apiClient.PostMultiFormAsync<CertificadoResult>(form, url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta.Certificado;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<List<CompetenciaColaboradorDTO>> ListarCompetenciasColaborador(string cpf, string tokenUsuario)
        {
            var baseUrl = _baseURL;
            var listarCompetencias = _configuration["Clients:Competencia:ListarCompetenciaColaborador"];
            var url = baseUrl + listarCompetencias + "?cpfColaborador=" + cpf;

            var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>
                    (
                        "Authorization",
                        String.Format("Bearer {0}", tokenUsuario)
                    )
                };

            var responseMessage = await _apiClient.GetAsync<ListaCompetenciaColaboradorResult>(url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta.Competencia;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<List<long>> ListarIdsPorCompetenciaId(long id, string tokenUsuario)
        {
            var baseUrl = _baseURL;
            var listarCompetencias = _configuration["Clients:Competencia:ListarIdsPorCompetenciaId"];
            var url = baseUrl + listarCompetencias + "?id=" + id;

            var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>
                    (
                        "Authorization",
                        String.Format("Bearer {0}", tokenUsuario)
                    )
                };

            var responseMessage = await _apiClient.GetAsync<ListaIdsPorCompetenciaResult>(url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta.ListaDeIds;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<CertificadoColaboradorResult> ListarCertificadoColaboradorPorCodigoInterno(string codInternoColaborador, string tokenUsuario)
        {
            var baseUrl = _baseURL;
            var listarCertificados = _configuration["Clients:Competencia:ListarCertificadoColaboradorPorCodigoInterno"];
            var url = baseUrl + listarCertificados + "?codInternoColaborador=" + codInternoColaborador;

            var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>
                    (
                        "Authorization",
                        String.Format("Bearer {0}", tokenUsuario)
                    )
                };

            var responseMessage = await _apiClient.GetAsync<CertificadoColaboradorResult>(url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<StatusResult> MergeCompetenciaColaborador(MergeItemPerfilDTO dtos, string tokenUsuario)
        {
            var baseUrl = _baseURL;
            var listarCompetencias = _configuration["Clients:Competencia:MergeCompetenciaColaborador"];
            var url = baseUrl + listarCompetencias;

            var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>
                    (
                        "Authorization",
                        String.Format("Bearer {0}", tokenUsuario)
                    )
                };

            var responseMessage = await _apiClient.PostAsync<StatusResult>(dtos, url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task RemoveCertificadoCompetenciaColaborador(string cpf, long id, string tokenUsuario)
        {
            var baseUrl = _baseURL;
            var removerCertificadoCompetencia = _configuration["Clients:Competencia:RemoveCertificadoCompetenciaColaborador"];
            var url = baseUrl + removerCertificadoCompetencia + "?id=" + id + "&cpf=" + cpf;
            var objeto = new Object() { };

            var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>
                    (
                        "Authorization",
                        String.Format("Bearer {0}", tokenUsuario)
                    )
                };

            var responseMessage = await _apiClient.PostAsync<StatusResult>(objeto, url, headers);

            if (!responseMessage.Sucesso)
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<StatusResult> RemoverCompetenciaColaborador(string token, string cpf, long id)
        {
            var baseUrl = _baseURL;
            var removerCompetencia = _configuration["Clients:Competencia:RemoverCompetenciaColaborador"];
            var url = baseUrl + removerCompetencia;
            var objeto = new
            {
                CompetenciaId = id,
                Cpf = cpf
            };

            var headers = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Authorization",
                    String.Format("Bearer {0}", token))
            };

            var responseMessage = await _apiClient.PostAsync<StatusResult>(objeto, url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<CompetenciaResult> AdicionarCompetencia(string descricao, string token)
        {
            var baseUrl = _baseURL;
            var removerCompetencia = _configuration["Clients:Competencia:AdicionarCompetencia"];
            var url = baseUrl + removerCompetencia;
            var objeto = new
            {
                descricao = descricao,
            };

            var headers = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Authorization",
                    String.Format("Bearer {0}", token))
            };

            var responseMessage = await _apiClient.PostAsync<CompetenciaResult>(objeto, url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }
        
        public async Task<ApiGenericResult<AlterarNomeCompetenciaResultDTO>> AlterarNomeCompetencia(EditarNomeCompetenciaParam param, string token)
        {
            var baseUrl = _baseURL;
            var alterarCompetencia = _configuration["Clients:Competencia:AlterarNomeCompetencia"];
            var url = baseUrl + alterarCompetencia;
            var objeto = param;

            var headers = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Authorization",
                    String.Format("Bearer {0}", token))
            };

            var responseMessage = await _apiClient.PostAsync<ApiGenericResult<AlterarNomeCompetenciaResultDTO>>(objeto, url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }
        
        public async Task<List<CompetenciaNomeEIdDTO>> ListarNomeDeSkillsPorTipo(TipoCompetenciaSRSEnum tipo, string tokenUsuario)
        {
            var baseUrl = _baseURL;
            var listarCertificados = _configuration["Clients:Competencia:ListarNomeDeSkillsPorTipo"];
            var url = baseUrl + listarCertificados + "?tipo=" + (int)tipo;

            var headers = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>
                (
                    "Authorization",
                    String.Format("Bearer {0}", tokenUsuario)
                )
            };

            var responseMessage = await _apiClient.GetAsync<ApiGenericResult<List<CompetenciaNomeEIdDTO>>>(url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta.Retorno;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }
    }
}