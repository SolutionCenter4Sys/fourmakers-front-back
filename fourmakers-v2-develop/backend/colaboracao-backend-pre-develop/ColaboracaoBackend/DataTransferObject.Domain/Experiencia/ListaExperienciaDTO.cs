using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Experiencia
{
    public class ListaExperienciaDTO
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        [JsonPropertyName("Funcao")]
        [Display(Name = "Funcao")]
        public string Funcao { get; set; }
        [JsonPropertyName("empresa")]
        [Display(Name = "Empresa")]
        public string Empresa { get; set; }
        [JsonPropertyName("Projetos")]
        [Display(Name = "Projetos")]
        public List<string> Projetos { get; set; }
        [JsonPropertyName("Atividades")]
        [Display(Name = "Atividades")]
        public string Atividades { get; set; }
        [Display(Name = "Data de inicio")]
        [DataType(DataType.DateTime)]
        [JsonPropertyName("dataInicio")]
        public DateTime DataInicio { get; set; }
        [Display(Name = "Data de saida")]
        [DataType(DataType.DateTime)]
        [JsonPropertyName("dataSaida")]
        public DateTime? DataSaida { get; set; }
        [JsonPropertyName("atual")]
        public bool Atual { get; set; }
        //[JsonIgnore]
        [JsonPropertyName("cpf")]
        //[Display(Name = "CPF")]
        [JsonIgnore]
        public string ColaboradorCpf { get; set; }
    }
}