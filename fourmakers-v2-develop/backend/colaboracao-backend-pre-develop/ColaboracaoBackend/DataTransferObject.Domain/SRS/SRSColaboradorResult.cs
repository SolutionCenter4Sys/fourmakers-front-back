using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.SRS
{
    public class SRSColaboradorResult
    {
        [JsonPropertyName("colaborador")]
        public SRSColaboradorDTO Colaborador { get; set; }
    }
}