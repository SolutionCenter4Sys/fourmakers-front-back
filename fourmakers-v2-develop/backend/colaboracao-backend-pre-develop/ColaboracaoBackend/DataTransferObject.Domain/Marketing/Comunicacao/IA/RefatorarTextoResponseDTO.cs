using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Marketing.Comunicacao.IA
{
    /// <summary>
    /// Resposta da API de análise documental (refatorar-texto).
    /// A curriculo-api retorna <c>texto_refatorado</c>; <c>texto</c> fica como fallback.
    /// </summary>
    public class RefatorarTextoResponseDTO
    {
        [JsonPropertyName("texto_refatorado")]
        public string TextoRefatorado { get; set; }

        [JsonPropertyName("texto")]
        public string TextoLegado { get; set; }

        /// <summary>Texto a ser exibido pelo assistente (prioriza a chave oficial da API).</summary>
        [JsonIgnore]
        public string Texto => TextoRefatorado ?? TextoLegado;
    }
}
