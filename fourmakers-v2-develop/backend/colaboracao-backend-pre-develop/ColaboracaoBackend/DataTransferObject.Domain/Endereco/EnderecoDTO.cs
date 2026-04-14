using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Endereco
{
    public class EnderecoDTO
    {
        [JsonPropertyName("cep")]
        public string Cep { get; set; }

        [JsonPropertyName("endereco")]
        public string Endereco { get; set; }

        [JsonPropertyName("complemento")]
        public string Complemento { get; set; }

        [JsonPropertyName("numero")]
        public int? Numero { get; set; }

        [JsonPropertyName("bairro")]
        public string Bairro { get; set; }

        [JsonPropertyName("cidade")]
        public string Cidade { get; set; }

        [JsonPropertyName("estado")]
        public string Estado { get; set; }

        [JsonPropertyName("com_quem_mora")]
        public string ComQuemMora { get; set; }

        [JsonPropertyName("internacional_linha_um")]
        public string InternacionalLinhaUm { get; set; }

        [JsonPropertyName("internacional_linha_dois")]
        public string InternacionalLinhaDois { get; set; }

        [JsonPropertyName("id")]
        public long? Id { get; set; }
    }
}