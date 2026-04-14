using System;

namespace DataTransferObject.Domain.Social
{
    /// <summary>
    /// Linha de contagem de reações por feedback e emoji (mapeamento interno do repositório).
    /// Feedback360Id como Guid para compatibilidade com o driver MySQL (CHAR(36)).
    /// </summary>
    public class Feedback360MuralReacaoContagemRowDTO
    {
        public Guid Feedback360Id { get; set; }
        public int ReacaoId { get; set; }
        public string Emoji { get; set; }
        public int Quantidade { get; set; }
    }
}
