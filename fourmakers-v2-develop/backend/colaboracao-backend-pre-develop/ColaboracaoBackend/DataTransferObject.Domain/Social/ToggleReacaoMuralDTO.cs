using System;

namespace DataTransferObject.Domain.Social
{
    /// <summary>
    /// Payload para definir a reação do colaborador no card do mural (estado final desejado).
    /// Regras: ReacaoId = 0 remove a reação; ReacaoId &gt; 0 define ou substitui pela reação (uma por feedback por colaborador).
    /// </summary>
    public class ToggleReacaoMuralDTO
    {
        /// <summary>Id do feedback (card) no mural (<c>tb_feedback360.id</c>).</summary>
        public Guid Feedback360Id { get; set; }
        /// <summary>Id do emoji em tb_feedback360_mural_emojis_reacao. Use 0 para remover a reação.</summary>
        public int ReacaoId { get; set; }
    }
}
