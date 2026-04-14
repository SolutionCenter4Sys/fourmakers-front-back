using System;
using System.Collections.Generic;

namespace DataTransferObject.Domain.Marketing.Comunicacao.Publicacao
{
    public class PublicacaoDetalheDTO
    {
        public Guid PublicacaoId { get; set; }
        public string Tipo { get; set; }
        public string Titulo { get; set; }
        /// <summary>Subtítulo ou headline da publicação (segundo título).</summary>
        public string Subtitulo { get; set; }
        public string Conteudo { get; set; }
        public bool RequerConfirmacaoLeitura { get; set; }
        public bool PermiteComentarios { get; set; }
        public bool PermiteCurtidas { get; set; }
        public bool Fixada { get; set; }
        public bool PermiteDownload { get; set; }
        /// <summary>Quando true, a publicação não aparece no feed.</summary>
        public bool OcultarNoFeed { get; set; }
        public DateTime? DataAgendamentoPublicacao { get; set; }
        public DateTime? DataPublicacao { get; set; }
        public DateTime? DataValidadePublicacao { get; set; }
        public string PublicacaoStatus { get; set; }
        public string AprovacaoStatus { get; set; }
        /// <summary>Preenchido quando <see cref="AprovacaoStatus"/> é rejeitado.</summary>
        public string MotivoRejeicao { get; set; }
        /// <summary>Pasta lógica para agrupamento no feed (opcional).</summary>
        public string Pasta { get; set; }
        public List<PublicacaoLabelItemDTO> Labels { get; set; } = new List<PublicacaoLabelItemDTO>();
        /// <summary>Preenchido quando <see cref="Tipo"/> é <c>documento</c>.</summary>
        public List<PublicacaoTagItemDTO> Tags { get; set; } = new List<PublicacaoTagItemDTO>();
        public List<string> Grupos { get; set; } = new List<string>();
        /// <summary>Id da comunidade à qual a publicação está vinculada (null se não houver).</summary>
        public string ComunidadeId { get; set; }
        /// <summary>Nome da comunidade à qual a publicação está vinculada (null se não houver).</summary>
        public string ComunidadeNome { get; set; }
        public ColaboradorResumoDTO Autor { get; set; }
        public ColaboradorResumoDTO Aprovador { get; set; }
        public List<PublicacaoAnexoDTO> Anexos { get; set; } = new List<PublicacaoAnexoDTO>();
        public PublicacaoInteracaoDTO Interacao { get; set; }
        /// <summary>Quantidade de emojis selecionados na publicação, agrupados por emoji.</summary>
        public List<InteracaoComentarioEmojiCountDTO> InteracoesPorEmoji { get; set; } = new List<InteracaoComentarioEmojiCountDTO>();
        public List<PublicacaoComentarioDTO> Comentarios { get; set; } = new List<PublicacaoComentarioDTO>();
        public PublicacaoAnaliticoDTO Analitico { get; set; }
        public PublicacaoGerencialDTO Gerencial { get; set; }
    }
}
