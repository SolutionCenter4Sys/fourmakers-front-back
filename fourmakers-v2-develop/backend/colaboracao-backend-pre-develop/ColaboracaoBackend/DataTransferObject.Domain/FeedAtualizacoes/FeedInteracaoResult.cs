using DataTransferObject.Domain.Base;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.FeedAtualizacoes
{
    public class FeedInteracaoResult : StatusResult
    {
        [JsonPropertyName("feed")]
        public FeedDTO Feed { get; set; }
    }
}