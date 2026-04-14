using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.FeedAtualizacoes
{
    public class FeedResult : StatusResult
    {
        [JsonPropertyName("feed")]
        public List<FeedDTO> Feed { get; set; }
    }
}