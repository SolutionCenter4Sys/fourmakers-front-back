using Competencia.Domain.Enums;
using System.Text.Json.Serialization;

namespace Competencia.API.DTOs
{
    public class AdicionarCompParam
    {
        [JsonPropertyName("descricao")]
        public string Descricao { get; set; }

        [JsonPropertyName("enumTipoCompetencia")]
        public TipoCompetenciaSRSEnum TipoCompetenciaEnum { get; set; }
    }
}