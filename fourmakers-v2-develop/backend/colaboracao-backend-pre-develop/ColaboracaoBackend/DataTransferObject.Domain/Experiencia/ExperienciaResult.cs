using DataTransferObject.Domain.Base;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Experiencia
{
    public class ExperienciaResult : StatusResult
    {
        [JsonPropertyName("Experiencia")]
        public ExperienciaDTO experiencia { get; set; }
    }
}