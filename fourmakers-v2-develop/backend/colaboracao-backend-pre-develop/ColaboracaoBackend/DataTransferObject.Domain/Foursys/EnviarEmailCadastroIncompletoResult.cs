using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Foursys
{
    public class EnviarEmailCadastroIncompletoResult : StatusResult
    {
        [JsonPropertyName("emailsEnviados")]
        public List<string> EmailsEnviados { get; set; }
        [JsonPropertyName("emailsNaoEnviados")]
        public List<string> EmailsNaoEnviados { get; set; }
    }
}