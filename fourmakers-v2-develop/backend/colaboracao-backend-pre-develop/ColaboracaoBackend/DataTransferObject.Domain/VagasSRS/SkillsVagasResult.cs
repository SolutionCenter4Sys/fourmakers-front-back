using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.VagasSRS
{
    public class SkillsVagasResult : StatusResult
    {
        public SkillsVagasResult()
        {
            Skills = new List<SkillsVagasDTO>();
        }

        [JsonPropertyName("skills")]
        public List<SkillsVagasDTO> Skills { get; set; }
    }
}