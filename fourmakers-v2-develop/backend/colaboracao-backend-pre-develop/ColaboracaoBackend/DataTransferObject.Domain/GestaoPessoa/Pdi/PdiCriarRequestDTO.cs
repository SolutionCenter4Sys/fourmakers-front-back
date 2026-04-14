using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace DataTransferObject.Domain.GestaoPessoa.Pdi
{
    public class PdiCriarRequestDTO
    {
        public string Titulo { get; set; }
        public string Descricao { get; set; }

        /// <summary>Prazo global do PDI (opcional). Aceita "deadline" ou "deadLine" no JSON.</summary>
        [JsonProperty("deadline", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("deadline")]
        public DateTime? DeadLine { get; set; }

        public List<PdiSkillInputDTO> Skills { get; set; } = new List<PdiSkillInputDTO>();
        public List<PdiActionPlanInputDTO> ActionPlans { get; set; } = new List<PdiActionPlanInputDTO>();
    }
}
