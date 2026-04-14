using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.SRS
{
    public class VagaIndicadaDTO
    {
        [JsonPropertyName("vagaId")]
        public long vagaId { get; set; }
        [JsonPropertyName("vagaNome")]
        public string vagaNome { get; set; }
        [JsonPropertyName("candidatoNome")]
        public string candidatoNome { get; set; }
        [JsonPropertyName("vagaLink")]
        public string vagaLink { get; set; }
        [JsonPropertyName("vagaStatus")]
        public string vagaStatus { get; set; }

        [JsonPropertyName("tipoVaga")]
        public int? tipoVaga { get; set; }
    }
}