using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Filtro
{
    public class FiltroSoftSkillNivelDTO
    {
        [JsonIgnore]
        public long filtroId { get; set; }

        [JsonPropertyName("softskill_id")]
        public long SoftSkillId { get; set; }

        [JsonPropertyName("softskillDescricao")]
        public string SoftSkillDescricao { get; set; }

        [JsonPropertyName("nivel_id")]
        public long NivelId { get; set; }// = ((long)EnumNivel.QUALQUER);

        [JsonPropertyName("nivelDescricao")]
        public string NivelDescricao { get; set; }
    }
}