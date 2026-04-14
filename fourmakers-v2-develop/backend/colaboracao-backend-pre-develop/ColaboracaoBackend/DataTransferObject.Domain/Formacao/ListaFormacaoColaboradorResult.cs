using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Formacao
{
    public class ListaFormacaoColaboradorResult : StatusResult
    {
        public ListaFormacaoColaboradorResult()
        {
            Formacao = new List<FormacaoColaboradorDTO>();
        }

        [JsonPropertyName("formacao")]
        public List<FormacaoColaboradorDTO> Formacao { get; set; }
    }
}