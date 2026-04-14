using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.MapaDeAlocacao.CargaMapaAlocacao
{
    public class BuscarCargaMapaAlocacaoDTO
    {
        [JsonPropertyName("collaborator")]
        public string Collaborator { get; set; }

        [JsonPropertyName("daily-hours")]
        public double DailyHours { get; set; }

        [JsonPropertyName("date-final")]
        public DateTime DateFinal { get; set; }

        [JsonPropertyName("date-start")]
        public DateTime DateStart { get; set; }

        [JsonPropertyName("manager-name")]
        public string ManagerName { get; set; }

        [JsonPropertyName("projectname")]
        public string Projectname { get; set; }

        [JsonPropertyName("manager-email")]
        public string ManagerEmail { get; set; }

        [JsonPropertyName("cdColaboradorCCH")]
        public long? CdColaboradorCCH { get; set; }

        [JsonPropertyName("cdProjeto")]
        public long CdProjeto { get; set; }
        
        [JsonPropertyName("ehTBD")]
        public bool EhTbd { get; set; }
        
        [JsonPropertyName("custoPerfil")]
        public decimal? CustoPerfil { get; set; }
        
        [JsonPropertyName("rateCard")]
        public decimal? RateCard { get; set; }
        
    }
}