using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Colaborador
{
    public class ColaboradorQualificadoDTO
    {
        [JsonPropertyName("codigoInternoColaborador")]
        public string CodigoInternoColaborador { get; set; }

        [JsonPropertyName("nomeCompleto")]
        public string NomeCompleto { get; set; }

        [JsonPropertyName("emailAlternativo")]
        public string EmailAlternativo { get; set; }

        [JsonPropertyName("dataQualificacao")]
        public DateTime DataQualificacao { get; set; }
    }
} 