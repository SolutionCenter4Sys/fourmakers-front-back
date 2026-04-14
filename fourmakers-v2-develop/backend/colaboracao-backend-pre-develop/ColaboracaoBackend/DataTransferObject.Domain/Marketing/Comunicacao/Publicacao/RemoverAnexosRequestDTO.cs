using System.Collections.Generic;

namespace DataTransferObject.Domain.Marketing.Comunicacao.Publicacao
{
    /// <summary>
    /// Request para remover múltiplos anexos de uma publicação.
    /// </summary>
    public class RemoverAnexosRequestDTO
    {
        /// <summary>
        /// Lista de IDs dos anexos a remover.
        /// </summary>
        public List<string> AnexoIds { get; set; } = new List<string>();
    }
}
