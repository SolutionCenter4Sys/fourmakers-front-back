using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.DadosPerfil
{
    public class IdentidadeDeGeneroResult : StatusResult
    {
        [JsonPropertyName("identidadeDeGenero")]
        public List<IdentidadeDeGeneroDTO> IdentidadeDeGenero { get; set; }
    }
}