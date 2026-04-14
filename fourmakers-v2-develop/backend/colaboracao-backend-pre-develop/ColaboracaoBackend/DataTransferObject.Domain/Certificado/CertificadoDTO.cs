using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Certificado
{
    public class CertificadoDTO
    {
        [JsonPropertyName("path")]
        public string Path { get; set; }

        [JsonPropertyName("thumb")]
        public string Thumb { get; set; }

        [JsonPropertyName("idCertificado")]
        public long? IdCertificado { get; set; }

        [JsonPropertyName("principal")]
        public bool Principal { get; set; }
        [JsonPropertyName("ativo")]
        public bool ativo { get; set; }

        [JsonPropertyName("conclusao")]
        public DateTime? conclusao { get; set; }

        [JsonPropertyName("descricao")]
        public string descricao { get; set; }

        [JsonPropertyName("instituicao")]
        public string instituicao { get; set; }

        [JsonPropertyName("carga_horaria")]
        public int cargaHoraria { get; set; }

        [JsonIgnore]
        public string CodigoInternoColaborador { get; set; }
    }
}