using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.DadosPerfil
{
    public class EtniaDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("descricao")]
        public string Descricao { get; set; }
    }
}