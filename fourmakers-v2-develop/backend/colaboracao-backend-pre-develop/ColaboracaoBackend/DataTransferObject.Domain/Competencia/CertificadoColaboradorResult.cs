using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Competencia
{
    public class CertificadoColaboradorResult : StatusResult
    {
        [JsonPropertyName("certificados")]
        public List<CertificadoColaboradorDTO> Certificados { get; set; }
    }
}