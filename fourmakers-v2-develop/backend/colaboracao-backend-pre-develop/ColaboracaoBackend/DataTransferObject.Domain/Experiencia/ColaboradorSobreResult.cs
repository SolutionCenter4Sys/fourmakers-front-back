using DataTransferObject.Domain.Base;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Experiencia
{
    public class ColaboradorSobreResult : StatusResult
    {
        [DisplayName("ColaboradorSobre")]
        [JsonPropertyName("colaboradorSobre")]
        public ColaboradorSobreDTO ColaboradorSobre { get; set; }
    }
}