using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.SRS
{
    public class VagasIndicadasResult : StatusResult
    {
        [JsonPropertyName("vagasIndicadas")]
        public List<VagaIndicadaDTO> vagasIndicadas { get; set; }
    }
}