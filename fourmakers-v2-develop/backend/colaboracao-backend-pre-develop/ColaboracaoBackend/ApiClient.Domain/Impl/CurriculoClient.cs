using ApiClient.Domain;
using ApiClient.Domain.Interfaces;
using ApiClient.Infra.Interfaces;
using Colaboracao.Helper;
using DataTransferObject.Domain;
using DataTransferObject.Domain.Apontamento.FolhaPonto;
using DataTransferObject.Domain.Financeiro.Holerite;
using DataTransferObject.Domain.Linkedin;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using DataTransferObject.Domain.Arquivo;
using DataTransferObject.Domain.Financeiro.NotaFiscal;
using DataTransferObject.Domain.Financeiro.Rubrica.Rubrica;
using DataTransferObject.Domain.Financeiro.Rubrica.RubricaCarga.AnaliseDocumentosResult;
using DataTransferObject.Domain.Marketing.Comunicacao.IA;
using Newtonsoft.Json.Linq;

namespace ApiClient.Domain.Impl
{
    public class CurriculoClient : ICurriculoClient
    {
        private IApiClient _apiClient;
        private IConfiguration _configuration;
        private string _baseURL;

        public CurriculoClient(IApiClient apiClient, IConfiguration configuration)
        {
            _apiClient = apiClient;
            _configuration = configuration;
            _baseURL = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.CURRICULO_API_PATH);
        }

        public async Task<Root> GetPerfilLinkedin(string urlPerfil, string tokenAcesso)
        {
            var url = _baseURL + _configuration["Clients:Curriculo:Profile"] + "/" + urlPerfil;

            var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>
                    (
                        "Authorization",
                        String.Format("Bearer {0}", tokenAcesso)
                    )
                };

            var responseMessage = await _apiClient.GetAsync<Root>(url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<Root> GetPerfilLinkedinRapidAPI(string urlPerfil, string tokenAcesso)
        {
            var url = _baseURL + _configuration["Clients:Curriculo:ProfileExterno"] + "/" + urlPerfil;

            var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>
                    (
                        "Authorization",
                        String.Format("Bearer {0}", tokenAcesso)
                    )
                };

            var responseMessage = await _apiClient.GetAsync<Root>(url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<ListaSkillDTO> SkillClassify(List<string> skills, string tokenAcesso)
        {
            var url = _baseURL + _configuration["Clients:Curriculo:Classify"];

            var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>
                    (
                        "Authorization",
                        String.Format("Bearer {0}", tokenAcesso)
                    )
                };

            var body = new
            {
                skills = skills
            };

            var responseMessage = await _apiClient.PostAsync<ListaSkillDTO>(body, url, headers);            

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }
        
        public async Task<RootIA> GetProfileByDocumentContent(byte[] documento, string tokenAcesso)
        {
            var url = _baseURL + _configuration["Clients:Curriculo:Document"];

            var headers = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>
                (
                    "Authorization",
                    String.Format("Bearer {0}", tokenAcesso)
                )
            };

            var responseMessage = await _apiClient.PostAsync<RootIA>(new { content = Convert.ToBase64String(documento) }, url, headers, 200);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }
        
        public async Task<RootIA> AnalizarDocumentoIA(string documentTipe, string base64Image,  string tokenAcesso)
        {
            var url = _baseURL + _configuration["Clients:Curriculo:Document"];

            var headers = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>
                (
                    "Authorization",
                    String.Format("Bearer {0}", tokenAcesso)
                )
            };
            
            var fileName = Guid.NewGuid() + ".png";

            var responseMessage = await _apiClient.PostAsync<RootIA>(new { fileName = fileName, contentType = "image/png", imageBase64 = base64Image }, url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<SumarioFolhaPontoDTO> SumarioFolhaPonto(string documentType, string base64Pdf,  string tokenAcesso)
        {
            var url = _baseURL + _configuration["Clients:Curriculo:FolhaSumario"];

            var headers = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>
                (
                    "Authorization",
                    String.Format("Bearer {0}", tokenAcesso)
                )
            };
            
            var fileName = Guid.NewGuid() + ".png";

            var responseMessage = await _apiClient.PostAsync<SumarioFolhaPontoDTO>(new { fileName = fileName, contentType = documentType, pdfBase64 = base64Pdf }, url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }
        public async Task<RelatorioPontoRootDTO> AnaliseColaboradorFolhaPonto(string documentType, string base64Pdf,  string tokenAcesso)
        {
            var url = _baseURL + _configuration["Clients:Curriculo:FolhaAnaliseColaborador"];

            var headers = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>
                (
                    "Authorization",
                    String.Format("Bearer {0}", tokenAcesso)
                )
            };
            
            var fileName = Guid.NewGuid() + ".pdf";

            var responseMessage = await _apiClient.PostAsync<RelatorioPontoRootDTO>(new { fileName = fileName, contentType = documentType, pdfBase64 = base64Pdf }, url, headers, 500);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<SumarioHoleriteIaResult> SumarioHolerite(string documentType, string base64Pdf,  string tokenAcesso)
        {
            var url = _baseURL + _configuration["Clients:Curriculo:HoleriteSumario"];

            var headers = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>
                (
                    "Authorization",
                    String.Format("Bearer {0}", tokenAcesso)
                )
            };
            
            var fileName = Guid.NewGuid() + ".png";

            var responseMessage = await _apiClient.PostAsync<SumarioHoleriteIaResult>(new { fileName = fileName, contentType = documentType, pdfBase64 = base64Pdf }, url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }
        
        
        public async Task<RubricaCargaResult> InserirRubricaCarga(string rubricaUrl, string rubricaPath, string rubricaId, string codigoRubricaFrequencia, int mesInicial, int anoInicial, string cpfRequest, string codDiretoria, string tokenAcesso, int orgId)
        {
            var url = _baseURL + _configuration["Clients:Curriculo:RubricaCarga"];

            var headers = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>
                (
                    "Authorization",
                    String.Format("Bearer {0}", tokenAcesso)
                )
            };

            var body = new
            {
                arquivo_url = rubricaUrl,
                rubrica_path = rubricaPath,
                tb_rubrica_id = rubricaId,
                codigo_rubrica_frequencia = codigoRubricaFrequencia,
                mes_inicial = mesInicial,
                ano_inicial = anoInicial,
                codigo_interno_colaborador_alteracao = cpfRequest,
                codigo_interno_colaborador_criacao = cpfRequest,
                cod_diretoria = codDiretoria,
                tb_org_id = orgId
            };

            var responseMessage = await _apiClient.PostAsync<RubricaCargaResult>(body, url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<AnaliseRubricaCargaResult<AnaliseUnimedResultWrapper>> AnaliseUnimed(string base64Pdf,  string tokenAcesso)
        {
            var url = _baseURL + _configuration["Clients:Curriculo:RubricaCargaUnimed"];

            var headers = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>
                (
                    "Authorization",
                    String.Format("Bearer {0}", tokenAcesso)
                )
            };

            var body = new
            {
                pdf_base64 = base64Pdf,
            };

            var responseMessage = await _apiClient.PostAsync<AnaliseRubricaCargaResult<AnaliseUnimedResultWrapper>>(body, url, headers, 300);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }
        
        public async Task<AnaliseRubricaCargaResult<AnalizeAmilResultWrapper>> AnaliseAmil(string base64Pdf,  string tokenAcesso)
        {
            var url = _baseURL + _configuration["Clients:Curriculo:RubricaCargaAmil"];

            var headers = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>
                (
                    "Authorization",
                    String.Format("Bearer {0}", tokenAcesso)
                )
            };

            var body = new
            {
                pdf_base64 = base64Pdf,
            };

            var responseMessage = await _apiClient.PostAsync<dynamic>(body, url, headers, 300);
            

            if (responseMessage.Sucesso)
            {
                Console.WriteLine(responseMessage.Resposta);
                var procesamento = ProcessarRetornoAmil(responseMessage.Resposta);
                return procesamento;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }
        
        private AnaliseRubricaCargaResult<AnalizeAmilResultWrapper> ProcessarRetornoAmil(object resposta)
        {
            JToken root;

            switch (resposta)
            {
                case JToken token:
                    root = token;
                    break;
                case string s:
                    root = JToken.Parse(s);
                    break;
                case JsonElement jsonElement: // <<< trata JsonElement
                    root = JToken.Parse(jsonElement.GetRawText());
                    break;
                default:
                    root = JToken.FromObject(resposta);
                    break;
            }

            bool success =
                root["success"]?.Value<bool?>()
                ?? root["Success"]?.Value<bool?>()
                ?? root["sucesso"]?.Value<bool?>()
                ?? root["Sucesso"]?.Value<bool?>()
                ?? false;

            var result = new AnaliseRubricaCargaResult<AnalizeAmilResultWrapper>
            {
                Success = success,
                Dados = new AnalizeAmilResultWrapper { Data = new List<AnaliseAmilResult>() }
            };

            // Tenta pegar o nó "dados" ou "data"
            var dadosNode = root["dados"] ?? root["Dados"];
            var dataNode  = dadosNode?["data"] ?? dadosNode?["Data"];

            // Se não houver "data", usa o próprio "dados"
            var payload = dataNode ?? dadosNode ?? root["data"] ?? root["Data"];

            if (payload == null || payload.Type == JTokenType.Null)
                return result;

            if (payload.Type == JTokenType.Array)
            {
                result.Dados.Data = payload.ToObject<List<AnaliseAmilResult>>() ?? new List<AnaliseAmilResult>();
            }
            else if (payload.Type == JTokenType.Object)
            {
                var single = payload.ToObject<AnaliseAmilResult>();
                if (single != null) result.Dados.Data.Add(single);
            }

            return result;
        }
        
        public async Task<AnaliseRubricaCargaResult<AnalisePortoSeguroOdontoResultWrapper>> AnalisePortoSeguroOdonto(string base64Pdf,  string tokenAcesso)
        {
            var url = _baseURL + _configuration["Clients:Curriculo:RubricaCargaPortoSeguroOdonto"];

            var headers = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>
                (
                    "Authorization",
                    String.Format("Bearer {0}", tokenAcesso)
                )
            };

            var body = new
            {
                pdf_base64 = base64Pdf,
            };

            var responseMessage = await _apiClient.PostAsync<AnaliseRubricaCargaResult<AnalisePortoSeguroOdontoResultWrapper>>(body, url, headers, 300);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }
        
        public async Task<T> AnaliseXlsxGenerico<T>(string base64Xlsx, dynamic fields, List<string> requiredFields, string tokenAcesso, int headerRow = 0)
        {
            var url = _baseURL + _configuration["Clients:Curriculo:RubricaCargaXlsxGenerico"];

            var headers = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>
                (
                    "Authorization",
                    String.Format("Bearer {0}", tokenAcesso)
                )
            };

            var body = new
            {
                xlsx_base64 = base64Xlsx,
                field_mappings = fields,
                required_fields = requiredFields,
                header_row = headerRow
            };

            var responseMessage = await _apiClient.PostAsync<AnaliseRubricaCargaResult<T>>(body, url, headers, 300);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta.Dados;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }
        
        public async Task<AnaliseRubricaCargaResult<AnalizarGenericoProfarmaWrapper>> AnaliseProfarma(string base64Pdf,  string tokenAcesso)
        {
            var url = _baseURL + _configuration["Clients:Curriculo:RubricaCargaProfarma"];

            var headers = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>
                (
                    "Authorization",
                    String.Format("Bearer {0}", tokenAcesso)
                )
            };

            var body = new
            {
                pdf_base64 = base64Pdf,
            };

            var responseMessage = await _apiClient.PostAsync<AnaliseRubricaCargaResult<AnalizarGenericoProfarmaWrapper>>(body, url, headers, 300);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }
        
        public async Task<AnaliseRubricaCargaResult<AnaliseGenericoResultWrapper>> AnaliseGenerico(string base64Pdf,  string tokenAcesso, string campos, string informacoesAdicionais)
        {
            var url = _baseURL + _configuration["Clients:Curriculo:RubricaCargaGenerico"];

            var headers = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>
                (
                    "Authorization",
                    String.Format("Bearer {0}", tokenAcesso)
                )
            };

            var body = new
            {
                pdf_base64 = base64Pdf, 
                mapeamento_campos = campos,
                informacoes_adicionais = informacoesAdicionais
            };

            var responseMessage = await _apiClient.PostAsync<AnaliseRubricaCargaResult<AnaliseGenericoResultWrapper>>(body, url, headers, 300);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }
        
        public async Task<HoleriteAnaliseResultDTO> AnaliseHoleriteColaborador(string documentType, string base64Pdf,  string tokenAcesso)
        {
            var url = _baseURL + _configuration["Clients:Curriculo:HoleriteAnaliseColaborador"];

            var headers = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>
                (
                    "Authorization",
                    String.Format("Bearer {0}", tokenAcesso)
                )
            };
            
            var fileName = Guid.NewGuid() + ".pdf";

            var responseMessage = await _apiClient.PostAsync<HoleriteAnaliseResultDTO>(new { fileName = fileName, contentType = documentType, pdfBase64 = base64Pdf }, url, headers, 600);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }
        
        public async Task<RubricaCargaConsultaResult> BuscarStatusCargaPorId(string cargaId,string tokenAcesso)
        {
            var url = _baseURL + _configuration["Clients:Curriculo:RubricaCarga"];
            var finalPath = url + $"/{cargaId}";

            var headers = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>
                (
                    "Authorization",
                    String.Format("Bearer {0}", tokenAcesso)
                )
            };
            

            var responseMessage = await _apiClient.GetAsync<RubricaCargaConsultaResult>(finalPath, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }
        
        public async Task<AnaliseIANotaFiscalValorDTO> AnalisarValorNotaFiscal(Base64DTO base64Dto,string tokenAcesso)
        {
            var url = _baseURL + _configuration["Clients:Curriculo:NotaFiscalValor"];

            var headers = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>
                (
                    "Authorization",
                    String.Format("Bearer {0}", tokenAcesso)
                )
            };

            var contentType = "";

            if (base64Dto.Tipo == ArquivoTipoEnum.pdf)
            {
                contentType = "application/pdf";
            }
            else
            {
                contentType = $"image/{base64Dto.Tipo.ToString().ToLower()}";
            }
            
            var responseMessage = await _apiClient.PostAsync<AnaliseIANotaFiscalValorDTO>(new { fileBase64 = base64Dto.Base64, contentType }, url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<string> RefatorarTextoAsync(string texto, ModoRefatoracaoTextoEnum modo, bool negrito, string tokenAcesso)
        {
            var url = _baseURL + _configuration["Clients:Curriculo:RefatorarTexto"];

            var headers = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Authorization", String.Format("Bearer {0}", tokenAcesso))
            };

            var body = new
            {
                texto = texto ?? string.Empty,
                modo = modo.ToString().ToLowerInvariant(),
                negrito = negrito
            };

            var responseMessage = await _apiClient.PostAsync<RefatorarTextoResponseDTO>(body, url, headers);

            if (responseMessage.Sucesso && responseMessage.Resposta != null)
            {
                return responseMessage.Resposta.Texto ?? string.Empty;
            }

            throw new Exception(responseMessage?.Mensagem ?? "Erro ao refatorar texto.");
        }
    }
}