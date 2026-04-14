using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Vaga
{
    public class SkillsCandidatosDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("candidatoId")]
        public int CandidatoId { get; set; }

        [JsonPropertyName("categoriaId")]
        public int? CategoriaId { get; set; }

        [JsonPropertyName("descricaoId")]
        public int? DescricaoId { get; set; }

        [JsonPropertyName("nivelId")]
        public int? NivelId { get; set; }

        [JsonPropertyName("dataCriacao")]
        public DateTime? DataCriacao { get; set; }

        [JsonPropertyName("dataAlteracao")]
        public DateTime? DataAlteracao { get; set; }
    }
}