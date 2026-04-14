using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.MapaDeAlocacao
{
    public class HorasTotaisDTO
    {
        [JsonPropertyName("ociosidadeResumo")]
        public double OciosidadeResumo { get; set; }

        [JsonPropertyName("meses")]
        public List<MesesDTO> Meses { get; set; }
    }
}