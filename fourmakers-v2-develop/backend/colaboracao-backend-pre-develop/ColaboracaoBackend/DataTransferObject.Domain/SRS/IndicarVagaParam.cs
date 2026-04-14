using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.SRS
{
    public class IndicarVagaParam
    {
        [JsonPropertyName("idVaga")]
        public long idVaga { get; set; }

        [JsonPropertyName("nome")]
        public string nome { get; set; }

        [JsonPropertyName("email")]
        public string email { get; set; }

        [JsonPropertyName("telefone")]
        public string telefone
        {
            get; set;
        }
        [JsonPropertyName("deOndeConhece")]
        public string deOndeConhece { get; set; }

        [JsonPropertyName("autorizou")]
        public bool autorizou { get; set; }

        [JsonPropertyName("estaDisponivel")]
        public bool estaDisponivel { get; set; }

        [JsonPropertyName("linkedin")]
        public string linkedin { get; set; }
    }
}