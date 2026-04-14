using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Experiencia
{
    public class ExperienciaEmpresaDTO
    {
        [JsonPropertyName("empresa")]
        public string Empresa { get; set; }

        [JsonPropertyName("dataInicio")]
        public DateTime DataInicio { get; set; }

        [JsonPropertyName("dataSaida")]
        public DateTime? DataSaida { get; set; }
        [JsonPropertyName("atual")]
        public bool Atual { get; set; }

        [JsonPropertyName("experiencias")]
        public List<ItemExperienciaEmpresaDTO> Experiencias { get; set; }
        [JsonPropertyName("realizacoes")]
        public List<RealizacaoColaboradorDTO> Realizacoes { get; set; }
    }
}