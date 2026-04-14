using DataTransferObject.Domain.Base;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Certificado
{
    public class CertificadoResult : StatusResult
    {
        [JsonPropertyName("certificado")]
        public CertificadoDTO Certificado { get; set; }
    }
}