using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Experiencia
{
    public class ItemExperienciaEmpresaDTO
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        [JsonPropertyName("funcao")]
        public string Funcao { get; set; }
        [JsonPropertyName("projetos")]
        public List<string> Projetos { get; set; }
        [JsonPropertyName("atividades")]
        public string Atividades { get; set; }
        [JsonPropertyName("dataInicio")]
        public DateTime DataInicio { get; set; }

        [JsonPropertyName("dataSaida")]
        public DateTime? DataSaida { get; set; }
        [JsonPropertyName("atual")]
        public bool Atual { get; set; }

        [JsonPropertyName("cpf")]
        public string ColaboradorCpf { get; set; }
    }
}