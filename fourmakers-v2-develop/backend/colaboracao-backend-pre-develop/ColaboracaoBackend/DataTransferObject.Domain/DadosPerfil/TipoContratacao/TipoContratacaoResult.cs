using DataTransferObject.Domain.Base;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.DadosPerfil.TipoContratacao
{
    public class TipoContratacaoResult : StatusResult
    {
        [JsonPropertyName("TipoContratacao")]
        public TipoContratacaoDTO TipoContratacao { get; set; }
    }
}