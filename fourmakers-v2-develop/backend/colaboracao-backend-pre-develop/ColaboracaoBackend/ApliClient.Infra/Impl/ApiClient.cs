using ApiClient.Infra.Interfaces;
using Colaboracao.Helper;
using Colaboracao.Helper.Util;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ApiClient.Infra.Impl
{
    public class ApiClient : IApiClient
    {
        private ILogger _log;

        public ApiClient(ILogger<ApiClient> log)
        {
            _log = log;
        }

        /// <summary>
        /// Metodo para requisicoes via Post
        /// </summary>
        /// <typeparam name="T">Tipo generico que deve ser passado para que esta classe construa um objeto deste vindo da API</typeparam>
        /// <param name="item">Objeto a ser enviado para a API</param>
        /// <param name="url">Endereco completo da API</param>
        /// <param name="headers">Cabeçalhos a serem utilizados no formato "string" "string"</param>
        /// <returns>Objeto do tipo RespostaServico contendo uma propriedade chamada "resposta" do tipo T passado na chamada do metodo</returns>
        public async Task<RespostaServico<T>> PostAsync<T>(object item, string url, List<KeyValuePair<string, string>> headers, long segundostimeout = 100)
        {
            try
            {
                var json = JsonSerializer.Serialize(item);
                var conteudo = new StringContent(json, Encoding.UTF8, "application/json");

                var _handler = new HttpClientHandler();
                _handler.ServerCertificateCustomValidationCallback =
                    (message, certificate, chain, sslPolicyErrors) => true;

                using (var client = new HttpClient(_handler))
                {
                    foreach (var header in headers)
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                    client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Fourmakers", "1.0.0"));
                    client.Timeout = TimeSpan.FromSeconds(segundostimeout);

                    using (var response = await client.PostAsync(url, conteudo))
                    {
                        return await ProcessResponseAsync<T>(response);
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                return new RespostaServico<T>
                {
                    HttpStatus = ex.StatusCode.ToString(),
                    Sucesso = false,
                    Mensagem = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new RespostaServico<T>
                {
                    HttpStatus = "400",
                    Sucesso = false,
                    Mensagem = ex.Message
                };
            }
        }

        /// <summary>
        /// Metodo para requisicoes via Get
        /// </summary>
        /// <typeparam name="T">Tipo generico que deve ser passado para que esta classe construa um objeto deste vindo da API</typeparam>
        /// <param name="url">Endereco completo da API</param>
        /// <param name="headers">Cabeçalhos a serem utilizados no formato "string" "string"</param>
        /// <returns>Objeto do tipo RespostaServico contendo uma propriedade chamada "resposta" do tipo T passado na chamada do metodo</returns>
        public async Task<RespostaServico<T>> GetAsync<T>(string url, List<KeyValuePair<string, string>> headers)
        {
            try
            {
                var _handler = new HttpClientHandler();
                _handler.ServerCertificateCustomValidationCallback =
                    (message, certificate, chain, sslPolicyErrors) => true;

                using (var client = new HttpClient(_handler))
                {
                    foreach (var header in headers)
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                    client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Fourmakers", "1.0.0"));

                    using (var response = await client.GetAsync(url))
                    {
                        return await ProcessResponseAsync<T>(response);
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                return new RespostaServico<T>
                {
                    HttpStatus = ex.StatusCode.ToString(),
                    Sucesso = false,
                    Mensagem = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new RespostaServico<T>
                {
                    HttpStatus = "400",
                    Sucesso = false,
                    Mensagem = ex.Message
                };
            }
        }

        private async Task<RespostaServico<T>> ProcessResponseAsync<T>(HttpResponseMessage response)
        {
            string responseDetail = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var produtoJsonString = responseDetail;
                var objeto = JsonSerializer.Deserialize<T>(produtoJsonString, new JsonSerializerOptions
                {
                    Converters = { new JsonStringEnumConverter() },
                    PropertyNameCaseInsensitive = true
                });

                return new RespostaServico<T>
                {
                    Resposta = objeto,
                    HttpStatus = response.StatusCode.ToString(),
                    Sucesso = true,
                    Mensagem = ""
                };
            }
            else
            {
                var resposta = JsonUtil.TryDeserializeJsonNewtonsoft<T>(responseDetail);
                var mensagem = response.ReasonPhrase.ToStringOuVazio();

                if (resposta == null && responseDetail.ToStringOuVazio() != string.Empty)
                {
                    mensagem = responseDetail;
                }

                if (resposta is DataTransferObject.Domain.Base.StatusResult result)
                {
                    mensagem = result.Mensagem;
                }

                return new RespostaServico<T>
                {
                    HttpStatus = response.StatusCode.ToString(),
                    Resposta = resposta,
                    Sucesso = false,
                    Mensagem = "ApiClient: " + mensagem
                };
            }
        }


        /// <summary>
        /// Metodo para requisicoes via Put
        /// </summary>
        /// <typeparam name="T">Tipo generico que deve ser passado para que esta classe construa um objeto deste vindo da API</typeparam>
        /// <param name="item">Objeto a ser enviado para a API</param>
        /// <param name="url">Endereco completo da API</param>
        /// <param name="headers">Cabeçalhos a serem utilizados no formato "string" "string"</param>
        /// <returns>Objeto do tipo RespostaServico contendo uma propriedade chamada "resposta" do tipo T passado na chamada do metodo</returns>
        public async Task<RespostaServico<T>> PutAsync<T>(object item, string url, List<KeyValuePair<string, string>> headers)
        {
            try
            {
                var json = JsonSerializer.Serialize(item);
                var conteudo = new StringContent(json, Encoding.UTF8, "application/json");

                var _handler = new HttpClientHandler();
                _handler.ServerCertificateCustomValidationCallback =
                    (message, certificate, chain, sslPolicyErrors) => true;

                using (var client = new HttpClient(_handler))
                {
                    foreach (var header in headers)
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                    client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Fourmakers", "1.0.0"));

                    using (var response = await client.PutAsync(url, conteudo))
                    {
                        string responseDetail = await response.Content.ReadAsStringAsync();

                        if (response.IsSuccessStatusCode)
                        {
                            var ProdutoJsonString = await response.Content.ReadAsStringAsync();
                            var objeto = JsonSerializer.Deserialize<T>(ProdutoJsonString);
                            return new RespostaServico<T>
                            {
                                Resposta = objeto,
                                HttpStatus = response.StatusCode.ToString(),
                                Sucesso = true,
                                Mensagem = ""
                            };
                        }
                        else
                        {
                            return new RespostaServico<T>
                            {
                                HttpStatus = response.StatusCode.ToString(),
                                Sucesso = false,
                                Mensagem = response.ReasonPhrase
                            };
                        }
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                return new RespostaServico<T>
                {
                    HttpStatus = ex.StatusCode.ToString(),
                    Sucesso = false,
                    Mensagem = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new RespostaServico<T>
                {
                    HttpStatus = "400",
                    Sucesso = false,
                    Mensagem = ex.Message
                };
            }
        }

        /// <summary>
        /// Metodo para requisicoes via Delete
        /// </summary>
        /// <typeparam name="T">Tipo generico que deve ser passado para que esta classe construa um objeto deste vindo da API</typeparam>
        /// <param name="item">Objeto a ser enviado para a API</param>
        /// <param name="url">Endereco completo da API</param>
        /// <param name="headers">Cabeçalhos a serem utilizados no formato "string" "string"</param>
        /// <returns>Objeto do tipo RespostaServico contendo uma propriedade chamada "resposta" do tipo T passado na chamada do metodo</returns>
        public async Task<RespostaServico<T>> DeleteAsync<T>(object item, string url, List<KeyValuePair<string, string>> headers)
        {
            try
            {
                var json = JsonSerializer.Serialize(item);
                var conteudo = new StringContent(json, Encoding.UTF8, "application/json");

                var _handler = new HttpClientHandler();
                _handler.ServerCertificateCustomValidationCallback =
                    (message, certificate, chain, sslPolicyErrors) => true;

                using (var client = new HttpClient(_handler))
                {
                    foreach (var header in headers)
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                    client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Fourmakers", "1.0.0"));

                    var request = new HttpRequestMessage
                    {
                        Content = conteudo,
                        Method = HttpMethod.Delete,
                        RequestUri = new Uri(url)
                    };

                    using (var response = await client.SendAsync(request))
                    {
                        string responseDetail = await response.Content.ReadAsStringAsync();

                        if (response.IsSuccessStatusCode)
                        {
                            var ProdutoJsonString = await response.Content.ReadAsStringAsync();
                            var objeto = JsonSerializer.Deserialize<T>(ProdutoJsonString);
                            return new RespostaServico<T>
                            {
                                Resposta = objeto,
                                HttpStatus = response.StatusCode.ToString(),
                                Sucesso = true,
                                Mensagem = ""
                            };
                        }
                        else
                        {
                            return new RespostaServico<T>
                            {
                                HttpStatus = response.StatusCode.ToString(),
                                Sucesso = false,
                                Mensagem = response.ReasonPhrase
                            };
                        }
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                return new RespostaServico<T>
                {
                    HttpStatus = ex.StatusCode.ToString(),
                    Sucesso = false,
                    Mensagem = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new RespostaServico<T>
                {
                    HttpStatus = "400",
                    Sucesso = false,
                    Mensagem = ex.Message
                };
            }
        }

        /// <summary>
        /// Metodo para requisicoes via Post
        /// </summary>
        /// <typeparam name="T">Tipo generico que deve ser passado para que esta classe construa um objeto deste vindo da API</typeparam>
        /// <param name="item">Objeto a ser enviado para a API</param>
        /// <param name="url">Endereco completo da API</param>
        /// <param name="headers">Cabeçalhos a serem utilizados no formato "string" "string"</param>
        /// <returns>Objeto do tipo RespostaServico contendo uma propriedade chamada "resposta" do tipo T passado na chamada do metodo</returns>
        public async Task<RespostaServico<T>> PostMultiFormAsync<T>(MultipartFormDataContent form, string url, List<KeyValuePair<string, string>> headers)
        {
            try
            {
                var _handler = new HttpClientHandler();
                _handler.ServerCertificateCustomValidationCallback =
                    (message, certificate, chain, sslPolicyErrors) => true;

                using (var client = new HttpClient(_handler))
                {
                    foreach (var header in headers)
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                    client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Fourmakers", "1.0.0"));

                    using (var response = await client.PostAsync(url, form))
                    {
                        string responseDetail = await response.Content.ReadAsStringAsync();

                        if (response.IsSuccessStatusCode)
                        {
                            var ProdutoJsonString = await response.Content.ReadAsStringAsync();
                            var objeto = JsonSerializer.Deserialize<T>(ProdutoJsonString);
                            return new RespostaServico<T>
                            {
                                Resposta = objeto,
                                HttpStatus = response.StatusCode.ToString(),
                                Sucesso = true,
                                Mensagem = ""
                            };
                        }
                        else
                        {
                            return new RespostaServico<T>
                            {
                                HttpStatus = response.StatusCode.ToString(),
                                Sucesso = false,
                                Mensagem = response.ReasonPhrase
                            };
                        }
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                return new RespostaServico<T>
                {
                    HttpStatus = ex.StatusCode.ToString(),
                    Sucesso = false,
                    Mensagem = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new RespostaServico<T>
                {
                    HttpStatus = "400",
                    Sucesso = false,
                    Mensagem = ex.Message
                };
            }
        }

        /// <summary>
        /// Metodo para requisicoes via Post para content-type application/x-www-form-urlencoded
        /// </summary>
        /// <typeparam name="T">Tipo generico que deve ser passado para que esta classe construa um objeto deste vindo da API</typeparam>
        /// <param name="item">Objeto a ser enviado para a API</param>
        /// <param name="url">Endereco completo da API</param>
        /// <param name="headers">Cabeçalhos a serem utilizados no formato "string" "string"</param>
        /// <returns>Objeto do tipo RespostaServico contendo uma propriedade chamada "resposta" do tipo T passado na chamada do metodo</returns>
        public async Task<RespostaServico<T>> PostWwwFormAsync<T>(List<KeyValuePair<string, string>> form, string url, List<KeyValuePair<string, string>> headers)
        {
            try
            {
                var _handler = new HttpClientHandler();
                _handler.ServerCertificateCustomValidationCallback =
                    (message, certificate, chain, sslPolicyErrors) => true;

                using (var client = new HttpClient(_handler))
                {
                    foreach (var header in headers)
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                    client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Fourmakers", "1.0.0"));
                    HttpContent content = new FormUrlEncodedContent(form);
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                    content.Headers.ContentType.CharSet = "UTF-8";
                    using (var response = await client.PostAsync(url, content))
                    {
                        string responseDetail = await response.Content.ReadAsStringAsync();

                        if (response.IsSuccessStatusCode)
                        {
                            var ProdutoJsonString = await response.Content.ReadAsStringAsync();
                            var objeto = JsonSerializer.Deserialize<T>(ProdutoJsonString);
                            return new RespostaServico<T>
                            {
                                Resposta = objeto,
                                HttpStatus = response.StatusCode.ToString(),
                                Sucesso = true,
                                Mensagem = ""
                            };
                        }
                        else
                        {
                            return new RespostaServico<T>
                            {
                                HttpStatus = response.StatusCode.ToString(),
                                Sucesso = false,
                                Mensagem = response.ReasonPhrase
                            };
                        }
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                return new RespostaServico<T>
                {
                    HttpStatus = ex.StatusCode.ToString(),
                    Sucesso = false,
                    Mensagem = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new RespostaServico<T>
                {
                    HttpStatus = "400",
                    Sucesso = false,
                    Mensagem = ex.Message
                };
            }
        }
    }
}