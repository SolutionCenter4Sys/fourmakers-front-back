using DataTransferObject.Domain.Base;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.LG.Holerite
{
    public class HoleriteResult : StatusResult
    {
        [JsonPropertyName("holerite")]
        public HoleriteDTO Holerite { get; set; }
    }
}