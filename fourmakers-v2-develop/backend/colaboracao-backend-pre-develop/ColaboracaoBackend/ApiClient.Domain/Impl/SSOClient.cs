using ApiClient.Domain.Interfaces;
using ApiClient.Infra.Interfaces;
using DataTransferObject.Domain.SSO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiClient.Domain.Impl
{
    public class SSOClient : ISSOClient
    {
        private IApiClient _apiClient;

        public SSOClient(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<TokenResult> GetToken(SSOCredentialsDTO credentials, string authCode)
        {
            var objeto = new List<KeyValuePair<string, string>> {
                new KeyValuePair<string, string>("client_id", credentials.ClientId),
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("scope", credentials.Scope ?? "openid"),
                new KeyValuePair<string, string>("code", authCode),
                new KeyValuePair<string, string>("redirect_uri", credentials.RedirectUrl),
                new KeyValuePair<string, string>("client_secret", credentials.ClientSecretValue),
                new KeyValuePair<string, string>("code_verifier", credentials.CodeVerifierPlain),
            };
            return await GetTokenBase(credentials, objeto);
        }

        public async Task<TokenResult> GetTokenPublic(SSOCredentialsDTO credentials, string authCode)
        {
            var objeto = new List<KeyValuePair<string, string>> {
                new KeyValuePair<string, string>("client_id", credentials.ClientId),
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("scope", credentials.Scope ?? "openid"),
                new KeyValuePair<string, string>("code", authCode),
                new KeyValuePair<string, string>("redirect_uri", credentials.RedirectUrl),
                new KeyValuePair<string, string>("code_verifier", credentials.CodeVerifierPlain),
            };
            return await GetTokenBase(credentials, objeto);
        }

        private async Task<TokenResult> GetTokenBase(SSOCredentialsDTO credentials, List<KeyValuePair<string, string>> form)
        {
            var url = credentials.BaseUrl + credentials.Tenant + "/" + credentials.TokenPath;

            var responseMessage = await _apiClient.PostWwwFormAsync<TokenResult>(form, url, new List<KeyValuePair<string, string>> { });

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<SSOUserDetailResult> GetUserDetail(SSOCredentialsDTO credentials, string token)
        {
            var baseUrl = credentials.GraphPath;
            var url = baseUrl + "me";
            var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>
                    (
                        "Authorization",
                        String.Format("Bearer {0}", token)
                    )
                };
            var responseMessage = await _apiClient.GetAsync<SSOUserDetailResult>(url, headers);
            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<TokenResult> ValidateRefreshToken(SSOCredentialsDTO credentials, string refreshToken)
        {
            var objeto = new List<KeyValuePair<string, string>> {
                new KeyValuePair<string, string>("client_id", credentials.ClientId),
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("scope", credentials.Scope ?? "openid"),
                new KeyValuePair<string, string>("refresh_token", refreshToken),
                new KeyValuePair<string, string>("client_secret", credentials.ClientSecretValue),
                new KeyValuePair<string, string>("code_verifier", credentials.CodeVerifierPlain),
            };

            return await ValidateRefreshTokenBase(credentials, objeto);
        }

        public async Task<TokenResult> ValidateRefreshTokenPublic(SSOCredentialsDTO credentials, string refreshToken)
        {
            var objeto = new List<KeyValuePair<string, string>> {
                new KeyValuePair<string, string>("client_id", credentials.ClientId),
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("scope", credentials.Scope ?? "openid"),
                new KeyValuePair<string, string>("refresh_token", refreshToken),
                new KeyValuePair<string, string>("code_verifier", credentials.CodeVerifierPlain),
            };

            return await ValidateRefreshTokenBase(credentials, objeto);
        }

        private async Task<TokenResult> ValidateRefreshTokenBase(SSOCredentialsDTO credentials, List<KeyValuePair<string, string>> form)
        {
            var url = credentials.BaseUrl + credentials.Tenant + "/" + credentials.TokenPath;

            var responseMessage = await _apiClient.PostWwwFormAsync<TokenResult>(form, url, new List<KeyValuePair<string, string>> { });

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