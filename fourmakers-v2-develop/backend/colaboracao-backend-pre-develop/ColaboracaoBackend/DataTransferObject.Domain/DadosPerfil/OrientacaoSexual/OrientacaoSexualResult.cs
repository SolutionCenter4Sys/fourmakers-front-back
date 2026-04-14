using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.DadosPerfil
{
    public class OrientacaoSexualResult : StatusResult
    {
        [JsonPropertyName("orientacaoSexual")]
        public List<OrientacaoSexualDTO> OrientacaoSexual { get; set; }
    }
}