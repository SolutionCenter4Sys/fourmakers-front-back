using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Vaga
{
    public class SkillsVagasSrsDTO
    {
        [JsonPropertyName("vagaId")]
        public int VagaId { get; set; }

        [JsonPropertyName("categoriaId")]
        public int? CategoriaId { get; set; }
        [JsonPropertyName("categoriaIdFourmakers")]
        public int? CategoriaIdFourmakers { get; set; }

        [JsonPropertyName("descricaoId")]
        public int? DescricaoId { get; set; }

        [JsonPropertyName("nivelId")]
        public int? NivelId { get; set; }

        [JsonPropertyName("dataCriacao")]
        public DateTime? DataCriacao { get; set; }

        [JsonPropertyName("dataAlteracao")]
        public DateTime? DataAlteracao { get; set; }

        [JsonIgnore]
        [JsonPropertyName("id")]
        public int Id { get; set; }
    }
}