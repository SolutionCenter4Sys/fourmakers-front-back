using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.MapaDeAlocacao
{
    public class ProjetosCchResult : StatusResult
    {
        [JsonPropertyName("ProjetosCchResult")]
        public List<ProjetosCchDTO> projetosCch { get; set; }
    }
}