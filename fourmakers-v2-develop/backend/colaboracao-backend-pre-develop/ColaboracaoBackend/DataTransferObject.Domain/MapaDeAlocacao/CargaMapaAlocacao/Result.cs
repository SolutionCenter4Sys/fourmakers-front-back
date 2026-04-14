using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.MapaDeAlocacao.CargaMapaAlocacao
{
    public class Result
    {
        [JsonPropertyName("aguarde")]
        public bool aguarde { get; set; }

        [JsonPropertyName("collaborator")]
        public string collaborator { get; set; }

        [JsonPropertyName("daily-hours")]
        public double dailyhours { get; set; }

        [JsonPropertyName("date-start-txt")]
        public string datestarttxt { get; set; }

        [JsonPropertyName("date-final")]
        public DateTime datefinal { get; set; }

        [JsonPropertyName("date-final-txt")]
        public string datefinaltxt { get; set; }

        [JsonPropertyName("date-start")]
        public DateTime datestart { get; set; }

        [JsonPropertyName("hours")]
        public bool hours { get; set; }

        [JsonPropertyName("manager-name")]
        public string managername { get; set; }

        [JsonPropertyName("periodo")]
        public List<DateTime> periodo { get; set; }

        [JsonPropertyName("projectname")]
        public string projectname { get; set; }

        [JsonPropertyName("Created Date")]
        public DateTime CreatedDate { get; set; }

        [JsonPropertyName("Created By")]
        public string CreatedBy { get; set; }

        [JsonPropertyName("Modified Date")]
        public DateTime ModifiedDate { get; set; }

        [JsonPropertyName("manager-email")]
        public string manageremail { get; set; }

        [JsonPropertyName("atual")]
        public bool atual { get; set; }

        [JsonPropertyName("cdColaboradorCCH")]
        public int cdColaboradorCCH { get; set; }

        [JsonPropertyName("cdProjeto")]
        public int cdProjeto { get; set; }

        [JsonPropertyName("_id")]
        public string _id { get; set; }
    }
}