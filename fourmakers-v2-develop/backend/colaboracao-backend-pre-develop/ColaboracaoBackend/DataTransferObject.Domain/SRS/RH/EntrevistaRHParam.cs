using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.SRS.RH
{
    public class EntrevistaRHParam
    {
        [JsonPropertyName("candidato_id")]
        public int Candidate_id { get; set; }
        [JsonPropertyName("joborder_id")]
        public int Joborder_id { get; set; }
        [JsonPropertyName("analista")]
        public int Analista { get; set; }
        [JsonPropertyName("prc_analista")]
        public string PrcAnalista { get; set; }
        [JsonPropertyName("data_entrevista")]
        public DateTime? DataEntrevista { get; set; }
        [JsonPropertyName("hora_inicio_entrevista")]
        public string HoraInicioEntrevista { get; set; }
        [JsonPropertyName("hora_final_entrevista")]
        public string HoraFinalEntrevista { get; set; }
        [JsonPropertyName("notes_dpa")]
        public string NotesDPA { get; set; }
    }
}