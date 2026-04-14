using DataTransferObject.Domain.Base;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.MapaDeAlocacao
{
    public class CadastroMapaAlocacaoResult : StatusResult
    {
        [JsonPropertyName("mapaAlocacao")]
        public CadastroMapaAlocacaoDTO cadastroMapaAlocacao;
    }
}