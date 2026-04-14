using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Competencia;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Colaborador
{
    public class ItensSumarioResult : StatusResult
    {
        [JsonPropertyName("skills")]
        public List<SkillSumarioDTO> skills { get; set; }
        public List<SkillSumarioDTO> niveis { get; set; }
    }
}