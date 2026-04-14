using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.DadosPerfil
{
    public class ModalidadeContratacaoResult : StatusResult
    {
        [JsonPropertyName("ModalidadeContratacao")]
        public List<ModalidadeContratacaoDTO> ModalidadeContratacao { get; set; }
    }
}