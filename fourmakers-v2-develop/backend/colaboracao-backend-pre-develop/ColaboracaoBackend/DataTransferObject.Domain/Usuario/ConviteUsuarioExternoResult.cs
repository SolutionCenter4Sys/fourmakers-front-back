using DataTransferObject.Domain.Base;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Usuario
{
    public class ConviteUsuarioExternoResult : StatusResult
    {
        [JsonPropertyName("convite")]
        public ConviteUsuarioExternoDTO Convite { get; set; }
    }
}