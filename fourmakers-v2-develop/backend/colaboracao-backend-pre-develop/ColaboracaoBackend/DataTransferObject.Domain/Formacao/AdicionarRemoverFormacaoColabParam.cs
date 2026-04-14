using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Formacao
{
    public class AdicionarRemoverFormacaoColabParam
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("nivelId")]
        public long? NivelId { get; set; }
        //public long NivelId { get; set; }

        [JsonPropertyName("cpf")]
        public string Cpf { get; set; }

        [JsonPropertyName("emissor")]
        public string Emissor { get; set; }

        [JsonPropertyName("data_conclusao")]
        public DateTime DataConclusao { get; set; }
    }
}