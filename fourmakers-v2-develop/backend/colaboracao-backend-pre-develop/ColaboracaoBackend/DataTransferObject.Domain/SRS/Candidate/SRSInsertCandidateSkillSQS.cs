using Newtonsoft.Json;

namespace DataTransferObject.Domain.SRS.Candidate
{
    public class SRSInsertCandidateSkillSQS
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("nivel")]
        public string Nivel { get; set; }
    }
}