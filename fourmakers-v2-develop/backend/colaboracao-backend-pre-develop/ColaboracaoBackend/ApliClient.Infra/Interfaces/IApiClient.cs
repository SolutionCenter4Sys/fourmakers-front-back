using ApiClient.Infra.Impl;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace ApiClient.Infra.Interfaces
{
    public interface IApiClient
    {
        Task<RespostaServico<T>> PostAsync<T>(object item, string url, List<KeyValuePair<string, string>> headers, long segundostimeout = 100);
        Task<RespostaServico<T>> GetAsync<T>(string url, List<KeyValuePair<string, string>> headers);
        Task<RespostaServico<T>> PutAsync<T>(object item, string url, List<KeyValuePair<string, string>> headers);
        Task<RespostaServico<T>> DeleteAsync<T>(object item, string url, List<KeyValuePair<string, string>> headers);
        Task<RespostaServico<T>> PostMultiFormAsync<T>(MultipartFormDataContent form, string url, List<KeyValuePair<string, string>> headers);
        Task<RespostaServico<T>> PostWwwFormAsync<T>(List<KeyValuePair<string, string>> form, string url, List<KeyValuePair<string, string>> headers);
    }
}