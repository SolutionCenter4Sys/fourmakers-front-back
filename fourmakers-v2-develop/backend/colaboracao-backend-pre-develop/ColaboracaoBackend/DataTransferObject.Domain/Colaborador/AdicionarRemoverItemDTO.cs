using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Colaborador
{
    public class AdicionarRemoverItemDTO
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("nivelId")]
        public long? NivelId { get; set; }

        [JsonPropertyName("cpf")]
        public string Cpf { get; set; }

        [JsonPropertyName("interesseAtivo")]
        public bool InteresseAtivo { get; set; } = false;
    }
}