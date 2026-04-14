using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.SRS.ClienteGestor
{
    public class GestorClienteEntrevistaDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
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
    }
}