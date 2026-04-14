using DataTransferObject.Domain.SRS.Candidate;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.SRS
{
    public class SRSCandidateResult
    {
        public SRSCandidateResult()
        {
            SRSCandidateDTO = new SRSCandidateDTO();
        }

        [JsonPropertyName("data")]
        public SRSCandidateDTO SRSCandidateDTO { get; set; }
        [JsonPropertyName("status")]
        public int Status { get; set; }
        [JsonPropertyName("sucess")]
        public bool Sucess { get; set; }
        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}