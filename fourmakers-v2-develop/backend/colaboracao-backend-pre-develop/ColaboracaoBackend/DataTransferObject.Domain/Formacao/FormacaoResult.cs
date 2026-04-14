using DataTransferObject.Domain.Base;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Formacao
{
    public class FormacaoResult : StatusResult
    {
        [JsonPropertyName("formacao")]
        public FormacaoDTO Formacao { get; set; }
    }
}