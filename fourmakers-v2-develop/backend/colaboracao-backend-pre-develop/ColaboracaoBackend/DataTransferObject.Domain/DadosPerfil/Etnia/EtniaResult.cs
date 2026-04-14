using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.DadosPerfil
{
    public class EtniaResult : StatusResult
    {
        [JsonPropertyName("etnia")]
        public List<EtniaDTO> Etnia { get; set; }
    }
}