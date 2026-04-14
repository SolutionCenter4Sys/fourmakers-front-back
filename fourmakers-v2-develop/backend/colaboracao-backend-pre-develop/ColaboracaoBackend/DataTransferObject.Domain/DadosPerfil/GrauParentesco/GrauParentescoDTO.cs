using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.DadosPerfil.GrauParentesco
{
    public class GrauParentescoDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("descricao")]
        public string Descricao { get; set; }
    }
}