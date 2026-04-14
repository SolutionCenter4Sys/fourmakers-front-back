using System.Text.Json.Serialization;

namespace MapaDeAlocacao.API.DTOs
{
    public class AlteraPerfilAlocacaoParam
    {
        [JsonPropertyName("periodoAlocacaoId")]
        public long PeriodoAlocacaoId { get; set; }
        [JsonPropertyName("perfilId")]
        public string PerfilId { get; set; }
    }
}