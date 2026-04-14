using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.MapaDeAlocacao
{
    public class MesesDTO
    {
        [JsonPropertyName("ociosidadeMes")]
        public string OciosidadeMes { get; set; }
        [JsonPropertyName("forca")]
        public double Forca { get; set; }
        [JsonPropertyName("nomeMesAtual")]
        public string NomeMesAtual { get; set; }
    }
}