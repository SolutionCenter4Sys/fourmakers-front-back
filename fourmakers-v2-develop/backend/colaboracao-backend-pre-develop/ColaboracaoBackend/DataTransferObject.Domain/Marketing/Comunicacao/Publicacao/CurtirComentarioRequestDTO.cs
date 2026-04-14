using System;

namespace DataTransferObject.Domain.Marketing.Comunicacao.Publicacao
{
    /// <summary>
    /// Request para adicionar interação (emoji) em um comentário.
    /// </summary>
    public class AdicionarInteracaoComentarioRequestDTO
    {
        public Guid ComentarioId { get; set; }
        /// <summary>Emoji da interação (opcional). Máximo 50 caracteres.</summary>
        public string Emoji { get; set; }
    }
}
