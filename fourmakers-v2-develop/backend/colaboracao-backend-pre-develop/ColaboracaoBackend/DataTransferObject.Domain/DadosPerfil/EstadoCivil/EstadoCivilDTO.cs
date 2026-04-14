using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.DadosPerfil
{
    public class EstadoCivilDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("descricao")]
        public string Descricao { get; set; }
    }
}