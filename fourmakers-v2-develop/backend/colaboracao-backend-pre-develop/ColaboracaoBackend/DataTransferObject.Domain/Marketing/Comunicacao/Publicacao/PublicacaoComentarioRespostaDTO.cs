using System;
using System.Collections.Generic;

namespace DataTransferObject.Domain.Marketing.Comunicacao.Publicacao
{
    public class PublicacaoComentarioRespostaDTO
    {
        public Guid ComentarioId { get; set; }
        public string Conteudo { get; set; }
        public DateTime? DataCriacao { get; set; }
        public ColaboradorResumoDTO Autor { get; set; }
        /// <summary>Indica se o usuário atual reagiu a este comentário (emoji usado, data, etc.).</summary>
        public ComentarioInteracaoDTO Interacao { get; set; }
        /// <summary>Interações do comentário agrupadas por emoji (string) com quantidade.</summary>
        public List<InteracaoComentarioEmojiCountDTO> InteracoesPorEmoji { get; set; } = new List<InteracaoComentarioEmojiCountDTO>();
    }
}
