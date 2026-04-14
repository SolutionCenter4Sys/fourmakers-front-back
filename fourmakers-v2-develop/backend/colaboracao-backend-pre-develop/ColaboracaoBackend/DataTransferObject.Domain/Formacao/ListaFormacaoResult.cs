using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Formacao
{
    public class ListaFormacaoResult : StatusResult
    {
        public ListaFormacaoResult()
        {
            Formacao = new List<FormacaoDTO>();
        }

        [JsonPropertyName("formacao")]
        public List<FormacaoDTO> Formacao { get; set; }
    }
}