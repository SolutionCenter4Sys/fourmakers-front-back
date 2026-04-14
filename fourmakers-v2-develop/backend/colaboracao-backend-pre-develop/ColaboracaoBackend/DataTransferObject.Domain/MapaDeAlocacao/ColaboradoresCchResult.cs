using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.MapaDeAlocacao
{
    public class ColaboradoresCchResult : StatusResult
    {
        [JsonPropertyName("ColaboradoresCchResult")]
        public List<ColaboradorCchDTO> ColaboradoresCch { get; set; }
    }
}