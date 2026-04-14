using ApiClient.Domain.Interfaces;
using ApiClient.Infra.Interfaces;
using Colaboracao.Helper;
using DataTransferObject.Domain.BI;
using DataTransferObject.Domain.Filtro;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiClient.Domain.Impl
{
    public class BIClient : IBIClient
    {
        private IApiClient _apiClient;
        private IConfiguration _configuration;
        private string _baseURL;

        public BIClient(IApiClient apiClient, IConfiguration configuration)
        {
            _apiClient = apiClient;
            _configuration = configuration;
            _baseURL = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.COLABORACAO_BASE) + VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.BI_API_PATH);
        }
        
        

        public async Task<bool> ArmazenaTrending(List<FiltroCompetenciaNivelDTO> paramCompetenciaNivel, List<FiltroFormacaoNivelDTO> paramFormacaoNivel, List<FiltroMetodologiaNivelDTO> paramMetodologiaNivel, List<FiltroDominioNivelDTO> paramDominioNivel, List<FiltroModeloReferenciaNivelDTO> paramModeloNivel, List<FiltroInteresseDTO> paramInteresse, List<FiltroHobbyDTO> paramHobby, string tokenUsuario)
        {
            var baseUrl = _baseURL;
            var listarIdBusca = _configuration["Clients:BI:ArmazenaTrending"];
            var url = baseUrl + listarIdBusca;

            var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>
                    (
                        "Authorization",
                        String.Format("Bearer {0}", tokenUsuario)
                    )
                };

            var objeto = new FiltroDTO();
            objeto.CompetenciaNivel = paramCompetenciaNivel;
            objeto.FormacaoNivel = paramFormacaoNivel;
            objeto.MetodologiaNivel = paramMetodologiaNivel;
            objeto.ModeloreferenciaNivel = paramModeloNivel;
            objeto.DominioNivel = paramDominioNivel;
            objeto.Hobby = paramHobby;
            objeto.Interesse = paramInteresse;

            var responseMessage = await _apiClient.PostAsync<ListaGraficoTrendingResult>(objeto, url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta.Sucesso;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }
    }
}