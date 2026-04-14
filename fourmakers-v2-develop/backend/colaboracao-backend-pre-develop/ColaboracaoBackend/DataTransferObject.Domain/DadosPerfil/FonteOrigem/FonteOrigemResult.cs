using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.DadosPerfil
{
    public class FonteOrigemResult : StatusResult
    {
        [JsonPropertyName("FonteOrigem")]
        public List<FonteOrigemDTO> FonteOrigem { get; set; }
    }
}