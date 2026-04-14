using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Social
{
    /// <summary>
    /// Resposta da geração do modelo STAR do feedback 360 (IA Moxe): situação, tarefa, ação, resultado e prévia.
    /// Retornado como JSON para o frontend converter e exibir.
    /// </summary>
    public class GerarModeloStarMoxeResultadoDTO
    {
        [JsonPropertyName("situacao")]
        public string Situacao { get; set; }

        [JsonPropertyName("tarefa")]
        public string Tarefa { get; set; }

        [JsonPropertyName("acao")]
        public string Acao { get; set; }

        [JsonPropertyName("resultado")]
        public string Resultado { get; set; }

        [JsonPropertyName("previa")]
        public string Previa { get; set; }
    }
}
