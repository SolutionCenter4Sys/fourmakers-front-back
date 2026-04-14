using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Base
{
    public class GetItemPerfilResult : StatusResult
    {
        [JsonPropertyName("itemPerfil")]
        public ItemPerfilDTO ItemPerfil { get; set; }
    }
}