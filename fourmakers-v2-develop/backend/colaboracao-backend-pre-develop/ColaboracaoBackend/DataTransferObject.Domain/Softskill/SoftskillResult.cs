using DataTransferObject.Domain.Base;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Softskill
{
    public class SoftskillResult : StatusResult
    {
        [JsonPropertyName("softskill")]
        public ItemPerfilDTO Softskill { get; set; }
    }
}