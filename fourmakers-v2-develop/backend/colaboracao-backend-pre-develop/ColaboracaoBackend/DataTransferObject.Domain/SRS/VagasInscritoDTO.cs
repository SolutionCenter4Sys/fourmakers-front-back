using DataTransferObject.Domain.Base;
using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.SRS
{
    public class VagasInscritoDTO : StatusResult
    {
        [JsonPropertyName("candidatejoborder_prerelease_id")]
        public long Candidatejoborder_Prerelease_Id { get; set; }

        [JsonPropertyName("joborder_id")]
        public long Joborder_Id { get; set; }
        [JsonPropertyName("jo_stVaga")]
        public string Jo_Stvaga { get; set; }

        [JsonPropertyName("joborder_title")]
        public string Joborder_Title { get; set; }

        [JsonPropertyName("candidate_id")]
        public long Candidate_Id { get; set; }

        [JsonPropertyName("site_id")]
        public long Site_Id { get; set; }

        [JsonPropertyName("origem")]
        public string Origem { get; set; }

        [JsonPropertyName("status")]
        public int Status { get; set; }

        [JsonPropertyName("status_description")]
        public string Status_Description { get; set; }

        [JsonPropertyName("user_id_action")]
        public long User_Id_Action { get; set; }

        [JsonPropertyName("status_reason_cancellation")]
        public int? Status_Reason_Cancellation { get; set; }

        [JsonPropertyName("reason_cancellation")]
        public string Reason_Cancellation { get; set; }

        [JsonPropertyName("date_created")]
        public DateTime Date_Created { get; set; }

        [JsonPropertyName("date_modified")]
        public DateTime Date_Modified { get; set; }

        [JsonPropertyName("status_candidate_joborder")]
        public int? Status_Candidate_Joborder { get; set; }

        [JsonPropertyName("description_candidate_joborder")]
        public string Description_Candidate_Joborder { get; set; }
        public bool NovaEstruturaRecrutamento { get; set; }
        public long CodigoVagaRecrutamento { get; set; }
    }
}