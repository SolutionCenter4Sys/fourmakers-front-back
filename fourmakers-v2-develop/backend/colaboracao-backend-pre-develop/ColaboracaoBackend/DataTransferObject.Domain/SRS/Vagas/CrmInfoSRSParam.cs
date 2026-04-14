using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.SRS.Vagas
{
    public class CrmInfoSRSParam
    {
        [JsonPropertyName("PropostaOportunidadeCCRM")]
        public String PropostaOportunidadeCCRM { get; set; }

        [JsonPropertyName("nomeContato")]
        public String NomeContato { get; set; }

        [JsonPropertyName("emailContato")]
        public String EmailContato { get; set; }

        [JsonPropertyName("telefoneContato")]
        public String TelefoneContato { get; set; }

        [JsonPropertyName("copiaEmailContato")]
        public String CopiaEmailContato { get; set; }
    }
}