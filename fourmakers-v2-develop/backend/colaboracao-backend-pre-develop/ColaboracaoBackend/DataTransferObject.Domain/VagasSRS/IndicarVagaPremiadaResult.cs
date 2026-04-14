using DataTransferObject.Domain.Base;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.VagasSRS
{
    public class IndicarVagaPremiadaResult : StatusResult
    {
        [JsonPropertyName("Convite")]
        public string Convite { get; set; }
    }
}