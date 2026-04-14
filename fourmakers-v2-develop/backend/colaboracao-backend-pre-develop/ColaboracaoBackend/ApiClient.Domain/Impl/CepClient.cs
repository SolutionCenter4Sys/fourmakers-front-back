using ApiClient.Domain.Interfaces;
using ApiClient.Infra.Interfaces;
using DataTransferObject.Domain.Cep;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiClient.Domain.Impl
{
    public class CepClient : ICepClient
    {
        private IApiClient _apiClient;

        public CepClient(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<ConsultaCepDTO> ConsultaCep(string cep)
        {
            var url = String.Format("https://viacep.com.br/ws/{0}/json/", cep);

            var ret = await _apiClient.GetAsync<ViaCepResponseDTO>(url, new List<KeyValuePair<string, string>> { });

            if (ret.Sucesso)
            {
                return new ConsultaCepDTO
                {
                    Bairro = ret.Resposta.Bairro,
                    Cidade = ret.Resposta.Localidade,
                    Endereco = ret.Resposta.Logradouro,
                    Uf = ret.Resposta.Uf,
                    Cep = cep,
                };
            }
            else
            {
                return null;
            }
        }
    }
}