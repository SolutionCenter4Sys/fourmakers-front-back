using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.FeedAtualizacoes
{
    public class FeedV2Result : StatusResult
    {
        [JsonPropertyName("feed")]
        public List<FeedV2DTO> Feed { get; set; }
    }
}