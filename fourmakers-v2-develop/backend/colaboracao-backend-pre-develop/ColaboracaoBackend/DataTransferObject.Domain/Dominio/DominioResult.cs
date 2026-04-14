using DataTransferObject.Domain.Base;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Dominio
{
    public class DominioResult : StatusResult
    {
        [JsonPropertyName("dominio")]
        public ItemPerfilDTO Dominio { get; set; }
    }
}