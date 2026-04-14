using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Base
{
    public class ItemPerfilResult : StatusResult
    {
        [JsonPropertyName("itemId")]
        public long ItemId { get; set; }
    }
}