using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Experiencia
{
    public class RemoverExperienciaProfissionalDTO
    {
        [JsonPropertyName("experienciaId")]
        public long ExperienciaId { get; set; }
        //[JsonIgnore]
        [JsonPropertyName("cpf")]
        public string Cpf { get; set; }
    }
}