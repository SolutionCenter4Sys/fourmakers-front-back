using System;

namespace DataTransferObject.Domain.Marketing.Comunicacao.Publicacao
{
    /// <summary>
    /// Request para remover a interação do usuário em um comentário.
    /// </summary>
    public class RemoverInteracaoComentarioRequestDTO
    {
        public Guid ComentarioId { get; set; }
    }
}
