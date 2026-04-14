using System;

namespace DataTransferObject.Domain.Social
{
    /// <summary>
    /// Linha indicando qual reação o usuário aplicou em qual feedback (mapeamento interno do repositório).
    /// Feedback360Id como Guid para compatibilidade com o driver MySQL (CHAR(36)).
    /// </summary>
    public class Feedback360MuralReacaoUsuarioRowDTO
    {
        public Guid Feedback360Id { get; set; }
        public int ReacaoId { get; set; }
    }
}
