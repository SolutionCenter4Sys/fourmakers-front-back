using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.DadosPerfil.TipoContratacao
{
    public class ListarTipoContratacaoDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("descricao")]
        public string Descricao { get; set; }
    }
}