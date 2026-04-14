using DataTransferObject.Domain.Base;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Metodologia
{
    public class MetodologiaResult : StatusResult
    {
        [JsonPropertyName("metodologia")]
        public ItemPerfilDTO Metodologia { get; set; }
    }
}