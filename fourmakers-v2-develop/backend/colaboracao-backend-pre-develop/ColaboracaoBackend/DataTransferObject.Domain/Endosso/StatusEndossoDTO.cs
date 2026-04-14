using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Endosso
{
    public class StatusEndossoDTO
    {
        [JsonPropertyName("descricaoStatus")]
        public string DescricaoStatus { get; set; }

        [JsonPropertyName("endossado")]
        public bool Endossado { get; set; }

        [JsonPropertyName("nivelEndosso")]
        public double NivelEndosso { get; set; }

        [JsonPropertyName("quantidadeEndosso")]
        public int QuantidadeEndosso { get; set; }

        [JsonPropertyName("quantidadeSolicitacaoEndosso")]
        public int QuantidadeSolicitacaoEndosso { get; set; }
    }
}