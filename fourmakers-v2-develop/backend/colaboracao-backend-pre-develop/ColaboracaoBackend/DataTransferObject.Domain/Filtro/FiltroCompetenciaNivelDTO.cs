using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Filtro
{
    public class FiltroCompetenciaNivelDTO
    {
        [JsonIgnore]
        public long filtroId { get; set; }

        [JsonPropertyName("competencia_id")]
        public long CompetencaId { get; set; }

        [JsonPropertyName("competenciaDescricao")]
        public string CompetenciaDescricao { get; set; }

        [JsonPropertyName("nivel_id")]
        public long NivelId { get; set; }// = ((long)EnumNivel.QUALQUER);

        [JsonPropertyName("nivelDescricao")]
        public string NivelDescricao { get; set; }
    }
}