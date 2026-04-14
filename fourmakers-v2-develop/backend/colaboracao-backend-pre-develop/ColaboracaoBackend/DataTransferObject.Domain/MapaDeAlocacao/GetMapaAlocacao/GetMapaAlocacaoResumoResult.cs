using DataTransferObject.Domain.Base;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.MapaDeAlocacao
{
    public class GetMapaAlocacaoResumoResult : StatusResult
    {
        [JsonPropertyName("mapaAlocacao")]
        public GetMapaAlocacaoResumoOutputDTO MapaAlocacao { get; set; }
    }
}