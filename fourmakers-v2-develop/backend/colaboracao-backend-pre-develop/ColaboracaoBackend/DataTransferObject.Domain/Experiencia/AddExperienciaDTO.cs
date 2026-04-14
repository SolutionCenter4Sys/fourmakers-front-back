using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Experiencia
{
    public class AddExperienciaDTO
    {
        [JsonPropertyName("funcao")]
        [Display(Name = "Funcao")]
        public string Funcao { get; set; }
        [JsonPropertyName("empresa")]
        [Display(Name = "Empresa")]
        public string Empresa { get; set; }
        [JsonPropertyName("Projetos")]
        [Display(Name = "Projetos")]
        public List<string> Projetos { get; set; }
        [JsonPropertyName("atividades")]
        [Display(Name = "Atividades")]
        public string Atividades { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Data de inicio")]
        [DataType(DataType.DateTime)]
        [JsonPropertyName("dataInicio")]
        public DateTime DataInicio { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Data de saida")]
        [DataType(DataType.DateTime)]
        [JsonPropertyName("dataSaida")]
        public DateTime? DataSaida { get; set; }
    }
}