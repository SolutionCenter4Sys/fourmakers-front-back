using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Escolaridade
{
    public class AddEscolaridadeDTO
    {
        [JsonPropertyName("instituicao")]
        public string Instituicao { get; set; }

        [JsonPropertyName("formacaoId")]
        public int FormacaoId { get; set; }

        [JsonPropertyName("descricao")]
        public string Descricao { get; set; }

        [JsonPropertyName("dataInicio")]
        public DateTime DataInicio { get; set; }

        [JsonPropertyName("dataTermino")]
        public DateTime? DataTermino { get; set; }

        [JsonPropertyName("cpf")]
        public string ColaboradorCpf { get; set; }

        [JsonPropertyName("tipoDiploma")]
        public int TipoDiplomaId { get; set; }
    }
}