using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Apontamento
{
    public class ListarVigenciaResult
    {
        public VigenciaSimplesDTO mesVigente { get; set; }

        public List<VigenciaSimplesDTO> meses { get; set; }
    }

    public class VigenciaSimplesDTO
    {
        [JsonPropertyName("mes")]
        public int Mes { get; set; }

        [JsonPropertyName("ano")]
        public int Ano { get; set; }

        [JsonPropertyName("label")]
        public string Label { get; set; }
    }
}