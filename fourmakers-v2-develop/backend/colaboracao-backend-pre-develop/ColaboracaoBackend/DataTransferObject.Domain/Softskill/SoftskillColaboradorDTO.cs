using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Nivel;
using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Softskill
{
    public class SoftskillColaboradorDTO
    {
        public SoftskillColaboradorDTO()
        {
            SoftSkill = new ItemPerfilDTO();
            Nivel = new NivelDTO();
        }

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonIgnore]
        public long IdSoftSkill { get; set; }

        [JsonIgnore]
        public long? IdNivel { get; set; }

        [JsonIgnore]
        public string ColaboradorCpf { get; set; }

        [JsonPropertyName("softskill")]
        public ItemPerfilDTO SoftSkill { get; set; }

        [JsonPropertyName("data")]
        public DateTime Data { get; set; }

        [JsonPropertyName("nivel")]
        public NivelDTO Nivel { get; set; }
    }
}