using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Projeto
{
    public class RemoveColaboradorProjetoDTO
    {
        [JsonPropertyName("id")]
        public int ColaboradorProjetoId { get; set; }

        [JsonPropertyName("cpf")]
        public string Cpf { get; set; }
    }
}