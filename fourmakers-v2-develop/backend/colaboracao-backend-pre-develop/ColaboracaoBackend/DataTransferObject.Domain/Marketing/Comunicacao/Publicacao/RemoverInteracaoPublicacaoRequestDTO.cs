using System;

namespace DataTransferObject.Domain.Marketing.Comunicacao.Publicacao
{
    /// <summary>
    /// Request para remover a interação (emoji) do usuário em uma publicação.
    /// </summary>
    public class RemoverInteracaoPublicacaoRequestDTO
    {
        public Guid PublicacaoId { get; set; }
    }
}
