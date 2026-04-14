using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Cep
{
    public class ConsultaCepDTO
    {
        [JsonPropertyName("endereco")]
        public string Endereco { get; set; }

        [JsonPropertyName("cidade")]
        public string Cidade { get; set; }

        [JsonPropertyName("uf")]
        public string Uf { get; set; }

        [JsonPropertyName("bairro")]
        public string Bairro { get; set; }

        [JsonPropertyName("cep")]
        public string Cep { get; set; }
    }
}