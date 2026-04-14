using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.VagasSRS
{
    public class JobOrdersResponse
    {
        public JobOrdersResponse()
        {
            Data = new List<JobOrderDTO>();
        }
        [JsonPropertyName("status")]
        public long Status { get; set; }

        [JsonPropertyName("sucess")]
        public bool Sucess { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("data")]
        public List<JobOrderDTO> Data { get; set; }
    }
}