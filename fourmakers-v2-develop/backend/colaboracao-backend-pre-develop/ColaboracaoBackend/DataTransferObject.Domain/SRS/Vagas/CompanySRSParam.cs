using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.SRS.Vagas
{
    public class CompanySRSParam
    {
        [JsonPropertyName("codigoCRM")]
        public String CodigoCRM { get; set; }

        [JsonPropertyName("nome")]
        public String Nome { get; set; }
    }
}