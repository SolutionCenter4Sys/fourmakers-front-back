using DataTransferObject.Domain.Base;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Hobby
{
    public class HobbyResult : StatusResult
    {
        [JsonPropertyName("hobbie")]
        public ItemPerfilDTO Hobbie { get; set; }
    }
}