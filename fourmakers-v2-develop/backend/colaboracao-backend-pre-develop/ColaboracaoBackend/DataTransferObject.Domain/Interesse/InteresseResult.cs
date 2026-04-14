using DataTransferObject.Domain.Base;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Interesse
{
    public class InteresseResult : StatusResult
    {
        [JsonPropertyName("interesse")]
        public ItemPerfilDTO Interesse { get; set; }
    }
}