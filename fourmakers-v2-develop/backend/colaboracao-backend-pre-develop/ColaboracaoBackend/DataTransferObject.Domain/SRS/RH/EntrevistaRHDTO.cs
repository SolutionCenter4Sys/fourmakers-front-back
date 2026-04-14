using DataTransferObject.Domain.SRS.Candidate;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.SRS.RH
{
    public class EntrevistaRHDTO
    {
        [JsonPropertyName("id")]
        public int id { get; set; }
        [JsonPropertyName("candidate_id")]
        public int candidate_id { get; set; }
        [JsonPropertyName("joborder_id")]
        public int joborder_id { get; set; }
        [JsonPropertyName("analista")]
        public int analista { get; set; }
        [JsonPropertyName("prcanalista")]
        public string prcanalista { get; set; }
        [JsonPropertyName("dataEntrevista")]
        public DateTime? dataEntrevista { get; set; }
        [JsonPropertyName("horaInicioEntrevista")]
        public string horaInicioEntrevista { get; set; }
        [JsonPropertyName("horaFinalEntrevista")]
        public string horaFinalEntrevista { get; set; }
        [JsonPropertyName("notesDPA")]
        public string notesDPA { get; set; }
        [JsonPropertyName("skills")]
        public List<CandidateSkillDTO> Skills { get; set; }
    }
}