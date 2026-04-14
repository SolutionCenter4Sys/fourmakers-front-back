using Competencia.Domain.Enums;
using System.Text.Json.Serialization;

namespace Competencia.API.DTOs
{
    public class ReprovarCompetenciaParam
    {
        [JsonPropertyName("idCompetencia")]
        public int Id { get; set; }
        [JsonPropertyName("enumTipoCompetencia")]
        public TipoCompetenciaSRSEnum TipoCompetencia { get; set; }
    }
}