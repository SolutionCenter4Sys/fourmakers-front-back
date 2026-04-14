using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Colaborador
{
    public class SimpleColaboradorDTO
    {
        [JsonPropertyName("cpf")]
        public string Cpf { get; set; }

        [JsonPropertyName("nomeCompleto")]
        public string NomeCompleto { get; set; }
    }
}