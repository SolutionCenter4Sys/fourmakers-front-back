using System;
using DataTransferObject.Domain.VagasSRS;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Vaga
{
    public class VagaSkillAnonymousRecrutamentoDTO
    {
        [JsonIgnore]
        public long SkillId { get; set; }
        public string SkillDescription { get; set; }

        [JsonIgnore]
        public long SkillNivelId { get; set; }
        public string SkillNivelDescription { get; set; }

        [JsonIgnore]
        public int TipoSkillId { get; set; }
        public string TypeSkillsDescription { get; set; }
    }
} 