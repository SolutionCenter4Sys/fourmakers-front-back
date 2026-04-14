using Competencia.Domain.Enums;
using System.Text.Json.Serialization;

namespace Competencia.API
{
    public class UnificarCompetenciaParam
    {
        [JsonPropertyName("idCompetenciaSugerida")]
        public int IdCompetencia { get; set; }
        [JsonPropertyName("idCompetenciaConsolidada")]
        public int IdCompetenciaConsolidada { get; set; }
        [JsonPropertyName("enumTipoCompetencia")]
        public TipoCompetenciaSRSEnum TipoCompetenciaEnum { get; set; }
    }
}