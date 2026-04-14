using ApiClient.Domain;
using ApiClient.Domain.Interfaces;
using Colaboracao.Helper;
using Colaboracao.Helper.Enum;
using Colaboracao.Helper.Util;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Marketing.Comunicacao.IA;
using Marketing.Domain.Interfaces.Comunicacao.IA;
using System;
using System.Threading.Tasks;

namespace Marketing.Domain.Impl.Comunicacao.IA
{
    public class ComunicacaoIAService : IComunicacaoIAService
    {
        private const string DESCRICAO_ENTIDADE = "Marketing - Comunicacao";
        private readonly ICurriculoClient _curriculoClient;

        public ComunicacaoIAService(ICurriculoClient curriculoClient)
        {
            _curriculoClient = curriculoClient;
        }

        public async Task<ApiGenericResult<string>> AssistenteAsync(AssistenteRequestDTO request)
        {
            var result = new ApiGenericResult<string>();
            try
            {
                var texto = request?.Texto?.Trim() ?? string.Empty;
                var tokenAcesso = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.CURRICULO_API_TOKEN);
                var textoRefatorado = await _curriculoClient.RefatorarTextoAsync(
                    texto,
                    request.Modo,
                    request.Negrito,
                    tokenAcesso);

                result.Retorno = textoRefatorado;
                result.Mensagem = textoRefatorado;
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, DESCRICAO_ENTIDADE);
            }
            return result;
        }
    }
}
