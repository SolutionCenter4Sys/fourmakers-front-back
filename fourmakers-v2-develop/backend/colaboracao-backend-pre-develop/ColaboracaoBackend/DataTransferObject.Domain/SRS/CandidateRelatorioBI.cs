using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.SRS
{
    public class CandidateRelatorioBI
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        [JsonPropertyName("nomeCandidato")]
        public string NomeCandidato { get; set; }
        [JsonPropertyName("dataEntrevistaTecnica")]
        public DateTime? DataEntrevistaTecnica { get; set; }
        [JsonPropertyName("parecerTecnico")]
        public string ParecerTecnico { get; set; }
        [JsonPropertyName("notasEntrevistaTecnica")]
        public string NotasEntrevistaTecnica { get; set; }
        [JsonPropertyName("dataEntrevistaRh")]
        public DateTime? DataEntrevistaRh { get; set; }
        [JsonPropertyName("parecerRh")]
        public string ParecerRh { get; set; }
        [JsonPropertyName("notasEntrevistaRh")]
        public string NotasEntrevistaRh { get; set; }
        [JsonPropertyName("dataEntrevistaGestor")]
        public DateTime? DataEntrevistaGestor { get; set; }
        [JsonPropertyName("parecerGestor")]
        public string ParecerGestor { get; set; }
        [JsonPropertyName("notasEntrevistaGestor")]
        public string NotasEntrevistaGestor { get; set; }
        [JsonPropertyName("nomeRecrutador")]
        public string NomeRecrutador { get; set; }
        [JsonPropertyName("desiredPay")]
        public double DesiredPay { get; set; }
        [JsonPropertyName("candModalidade")]
        public string CandModalidade { get; set; }
        [JsonPropertyName("skillLegado")]
        public string SkillLegado { get; set; }
        [JsonPropertyName("skills")]
        public string Skills { get; set; }
    }
}