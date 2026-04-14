using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.DadosPerfil
{
    public class EscolaridadeResult : StatusResult
    {
        [JsonPropertyName("escolaridade")]
        public List<EscolaridadeDTO> Escolaridade { get; set; }
    }
}