using DataTransferObject.Domain.Certificado;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Competencia
{
    public class CertificadoColaboradorDTO
    {
        [JsonPropertyName("competencia")]
        public CompetenciaDTO Competencia { get; set; }
        [JsonPropertyName("certificado")]
        public CertificadoDTO Certificado { get; set; }
    }
}