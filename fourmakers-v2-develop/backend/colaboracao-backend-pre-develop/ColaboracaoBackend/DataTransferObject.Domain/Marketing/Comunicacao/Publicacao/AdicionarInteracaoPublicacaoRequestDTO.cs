using System;

namespace DataTransferObject.Domain.Marketing.Comunicacao.Publicacao
{
    /// <summary>
    /// Request para adicionar interação (emoji) em uma publicação.
    /// </summary>
    public class AdicionarInteracaoPublicacaoRequestDTO
    {
        public Guid PublicacaoId { get; set; }
        /// <summary>Emoji da interação (opcional). Máximo 50 caracteres.</summary>
        public string Emoji { get; set; }
    }
}
