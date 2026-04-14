using DataTransferObject.Domain.Competencia;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain
{
    public class RealizacaoColaboradorDTO
    {
        [JsonPropertyName("cliente")]
        public string Cliente { get; set; }
        [JsonPropertyName("projeto")]
        public string Projeto { get; set; }
        [JsonPropertyName("horas")]
        public double Horas { get; set; }
        [JsonPropertyName("dataInicio")]
        public DateTime DataInicio { get; set; }
        [JsonPropertyName("dataFim")]
        public DateTime DataFim { get; set; }
        [JsonPropertyName("atual")]
        public bool Atual { get; set; }
        [JsonPropertyName("perfil")]
        public string Perfil { get; set; }
        [JsonPropertyName("skills")]
        public List<SkillNivelDTO> Skills { get; set; }
    }
}