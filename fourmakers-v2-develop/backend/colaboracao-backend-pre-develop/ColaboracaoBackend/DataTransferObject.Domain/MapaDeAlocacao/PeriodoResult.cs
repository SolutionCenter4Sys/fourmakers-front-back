using DataTransferObject.Domain.Base;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.MapaDeAlocacao
{
    public class PeriodoResult : StatusResult
    {
        [JsonPropertyName("periodo")]
        public PeriodoDTO Periodo { get; set; }
    }
}