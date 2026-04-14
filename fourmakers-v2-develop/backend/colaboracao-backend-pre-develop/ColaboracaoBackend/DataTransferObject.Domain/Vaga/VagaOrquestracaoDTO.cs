using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Vaga
{
    public class VagaOrquestracaoDTO
    {
        [JsonPropertyName("idVaga")]
        public int? IdVaga { get; set; }

        [JsonPropertyName("titulo")]
        public string Titulo { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("dataAprovacao")]
        public DateTime? DataAprovacao { get; set; }

        [JsonPropertyName("candidaturas")]
        public string Candidaturas { get; set; }

        [JsonPropertyName("hardSkill")]
        public string HardSkill { get; set; }
    }
}