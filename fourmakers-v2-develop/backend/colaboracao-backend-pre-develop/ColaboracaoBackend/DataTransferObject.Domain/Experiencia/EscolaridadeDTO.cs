using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Escolaridade
{
    public class EscolaridadeDTO
    {
        //public EscolaridadeDTO()
        //{
        //    Diploma = new DiplomaDTO();
        //}

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("instituicao")]
        public string Instituicao { get; set; }

        [JsonPropertyName("formacaoId")]
        public long? FormacaoId { get; set; }
        [JsonPropertyName("formacaoDescricao")]
        public string FormacaoDescricao { get; set; }

        [JsonPropertyName("dataInicio")]
        public DateTime DataInicio { get; set; }

        [JsonPropertyName("dataTermino")]
        public DateTime? DataTermino { get; set; }

        [JsonPropertyName("descricao")]
        public string Descricao { get; set; }

        [JsonIgnore]
        [JsonPropertyName("ativo")]
        public bool Ativo { get; set; }

        [JsonIgnore]
        [JsonPropertyName("cpfColaborador")]
        public string ColaboradorCpf { get; set; }

        [JsonIgnore]
        [JsonPropertyName("DataCriacao")]
        public DateTime DataCriacao { get; set; }

        [JsonIgnore]
        [JsonPropertyName("DataAlteracao")]
        public DateTime DataAlteracao { get; set; }

        [JsonPropertyName("TipoDiplomaId")]
        public int? TipoDiplomaId { get; set; }

        [JsonIgnore]
        public string FilePathInternal { get; set; }

        [JsonPropertyName("path")]
        public string FilePath { get; set; }

        //[JsonPropertyName("diploma")]
        //[Display(Name = "Diploma")]
        //public DiplomaDTO Diploma { get; set; }
    }
}