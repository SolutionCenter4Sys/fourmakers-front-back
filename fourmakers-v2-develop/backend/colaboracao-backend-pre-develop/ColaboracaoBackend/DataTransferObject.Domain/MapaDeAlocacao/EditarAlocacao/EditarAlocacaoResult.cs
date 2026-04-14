using DataTransferObject.Domain.Base;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.MapaDeAlocacao.EditarAlocacao
{
    public class EditarAlocacaoResult : StatusResult
    {
        [JsonPropertyName("AlocacaoEditada")]
        public AlocacaoColabETbdDTO AlocacaoEditada { get; set; }
    }
}