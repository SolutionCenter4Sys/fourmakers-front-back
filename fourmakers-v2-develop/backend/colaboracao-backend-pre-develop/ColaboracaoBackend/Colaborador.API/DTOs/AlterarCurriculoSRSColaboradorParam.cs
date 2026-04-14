using System.Text.Json.Serialization;

namespace Colaborador.API.DTOs
{
    public class AlterarCurriculoSRSColaboradorParam
    {
        public byte[] file { get; set; }

        [JsonPropertyName("qualifications_abstract")]
        public string qualifications_abstract { get; set; }

        [JsonPropertyName("technologies")]
        public string technologies { get; set; }

        [JsonPropertyName("degree")]
        public string degree { get; set; }

        [JsonPropertyName("languages")]
        public string languages { get; set; }

        [JsonPropertyName("courses")]
        public string courses { get; set; }

        [JsonPropertyName("certifications")]
        public string certifications { get; set; }
    }
}