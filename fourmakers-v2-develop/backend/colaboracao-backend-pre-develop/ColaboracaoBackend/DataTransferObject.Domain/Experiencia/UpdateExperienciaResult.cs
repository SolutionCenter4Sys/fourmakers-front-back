using DataTransferObject.Domain.Base;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Experiencia
{
    public class UpdateExperienciaResult : StatusResult
    {
        [JsonPropertyName("Experiencia")]
        public UpdateExperienciaDTO experiencia { get; set; }
    }
}