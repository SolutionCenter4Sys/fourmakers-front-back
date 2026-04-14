using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.SRS.Candidate
{
    public class CandidateSkillDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("candidate_id")]
        public int Candidate_id { get; set; }
        [JsonPropertyName("category_id")]
        public int Category_id { get; set; }
        [JsonPropertyName("description_id")]
        public int Description_id { get; set; }
        [JsonPropertyName("nivel_id")]
        public int Nivel_id { get; set; }

        [JsonPropertyName("entrevista_id")]
        public int Entrevista_id { get; set; }

        [JsonPropertyName("date_created")]
        public DateTime Date_Created { get; set; }

        [JsonPropertyName("date_modified")]
        public DateTime Date_Modified { get; set; }
    }
}