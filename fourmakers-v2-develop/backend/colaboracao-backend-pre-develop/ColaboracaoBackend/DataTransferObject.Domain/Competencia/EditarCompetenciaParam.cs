using Competencia.Domain.Enums;
using System.Text.Json.Serialization;

namespace Competencia.API.DTOs
{
    public class EditarCompetenciaParam
    {
        [JsonPropertyName("idCompetencia")]
        public int Id { get; set; }
        [JsonPropertyName("descricao")]
        public string Descricao { get; set; }
        [JsonPropertyName("ativo")]
        public bool Ativo { get; set; }
        [JsonPropertyName("enumTipoCompetencia")]
        public TipoCompetenciaSRSEnum TipoCompetencia { get; set; }
    }
}