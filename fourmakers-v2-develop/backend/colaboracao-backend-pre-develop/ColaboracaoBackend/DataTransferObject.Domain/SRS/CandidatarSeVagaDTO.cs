using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.SRS
{
    public class CandidatarSeVagaDTO
    {
        [JsonPropertyName("candidate_joborder_id")]
        public long? Candidate_Joborder_Id { get; set; }

        [JsonPropertyName("joborder_id")]
        public long Joborder_Id { get; set; }

        [JsonPropertyName("candidate_id")]
        public long Candidate_Id { get; set; }

        [JsonPropertyName("origem")]
        public string Origem { get; set; }

        [JsonPropertyName("user_id_action")]
        public long User_Id_Action { get; set; }

        [JsonPropertyName("reason_cancellation")]
        public long? Reason_Cancellation { get; set; }

        [JsonPropertyName("status")]
        public int Status { get; set; }

        [JsonPropertyName("date_created")]
        public DateTime Date_Created { get; set; }

        [JsonPropertyName("date_modified")]
        public DateTime Date_Modified { get; set; }
    }
}