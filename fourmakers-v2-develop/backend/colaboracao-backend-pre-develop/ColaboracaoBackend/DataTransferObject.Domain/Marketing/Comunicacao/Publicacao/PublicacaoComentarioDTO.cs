using System;
using System.Collections.Generic;

namespace DataTransferObject.Domain.Marketing.Comunicacao.Publicacao
{
    public class PublicacaoComentarioDTO
    {
        public Guid ComentarioId { get; set; }
        public string Conteudo { get; set; }
        public DateTime? DataCriacao { get; set; }
        public ColaboradorResumoDTO Autor { get; set; }
        public ComentarioAnaliticoDTO Analitico { get; set; }
        public List<PublicacaoComentarioRespostaDTO> RespostaComentarios { get; set; } = new List<PublicacaoComentarioRespostaDTO>();
        public ComentarioInteracaoDTO Interacao { get; set; }
        /// <summary>Interações do comentário agrupadas por emoji (string) com quantidade.</summary>
        public List<InteracaoComentarioEmojiCountDTO> InteracoesPorEmoji { get; set; } = new List<InteracaoComentarioEmojiCountDTO>();
    }
}
