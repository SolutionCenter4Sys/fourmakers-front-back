using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.SRS.Cargos
{
    public class CargoDTO
    {
        [JsonPropertyName("cargo_kenoby_id")]
        public string cargo_kenoby_id { get; set; }
        [JsonPropertyName("papel")]
        public string papel { get; set; }
        [JsonPropertyName("responsabilidade")]
        public string responsabilidade { get; set; }
        [JsonPropertyName("conhecimento_tecnico")]
        public string conhecimento_tecnico { get; set; }
        [JsonPropertyName("habilidades_comportamentais")]
        public string habilidades_comportamentais { get; set; }
        [JsonPropertyName("conhecimentos_metodologia")]
        public string conhecimentos_metodologia { get; set; }
        [JsonPropertyName("formacao_academica")]
        public string formacao_academica { get; set; }
        [JsonPropertyName("certificacoes")]
        public string certificacoes { get; set; }
        [JsonPropertyName("experiencia")]
        public string experiencia { get; set; }
        [JsonPropertyName("equipamentos")]
        public string equipamentos { get; set; }
    }
}