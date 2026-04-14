using DataTransferObject.Domain.Nivel;
using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.SkillDesconhecida
{
    public class SkillDesconhecidaColaboradorDTO
    {
        public long Id { get; set; }

        [JsonIgnore]
        public int IdSkillDesconhecida { get; set; }

        [JsonIgnore]
        public long? IdNivel { get; set; }

        [JsonIgnore]
        public string ColaboradorCpf { get; set; }

        public SkillDesconhecidaDTO SkillDesconhecida { get; set; }
        public DateTime Data { get; set; }
        public NivelDTO Nivel { get; set; }
    }
}