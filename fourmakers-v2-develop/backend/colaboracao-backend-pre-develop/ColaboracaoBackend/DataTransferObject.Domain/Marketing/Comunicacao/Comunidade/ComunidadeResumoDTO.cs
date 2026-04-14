using System;
using System.Collections.Generic;
using DataTransferObject.Domain.Marketing.Comunicacao;

namespace DataTransferObject.Domain.Marketing.Comunicacao.Comunidade
{
    public class ComunidadeResumoDTO
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public string? CapaUrl { get; set; }
        public string Tipo { get; set; }
        public bool PermitePostagemMembro { get; set; }
        public bool PermiteSair { get; set; }
        public string PublicacaoConfiguracaoPolitica { get; set; }
        public bool PublicacaoPermiteComentario { get; set; }
        public bool PublicacaoPermiteLikeHabilitado { get; set; }
        public int TotalPublicacoes { get; set; }
        public int TotalMembros { get; set; }
        /// <summary>Código interno do colaborador que criou a comunidade (tb_mkt_comunidade.codigo_interno_colaborador_criacao).</summary>
        public string CodigoInternoColaboradorCriacao { get; set; }
        public DateTime? DataCriacao { get; set; }
        /// <summary>Dados do criador (mesmo padrão de enriquecimento do autor em publicações).</summary>
        public ColaboradorResumoDTO Criador { get; set; }
        public string CodigoInternoColaboradorUltimaAlteracao { get; set; }
        public DateTime? DataUltimaAlteracao { get; set; }
        /// <summary>Dados de quem alterou por último.</summary>
        public ColaboradorResumoDTO UltimoAlterador { get; set; }
        /// <summary>
        /// Indica se o usuário logado participa ativamente da comunidade.
        /// Pública: registro em tb_mkt_comunidade_usuario_participando.
        /// Privada: membro de algum grupo vinculado ou em usuario_participando; se permite_sair, exclui quem está em tb_mkt_comunidade_privada_usuario_nao_participando.
        /// </summary>
        public bool Participando { get; set; }

        /// <summary>
        /// Lista de moderadores da comunidade. Usado na listagem para permitir exibir/ocultar ações de editar e arquivar no card.
        /// </summary>
        public List<ComunidadeMembroDTO> Moderadores { get; set; } = new List<ComunidadeMembroDTO>();
    }
}
