using System;
using System.Collections.Generic;

namespace DataTransferObject.Domain.Marketing.Comunicacao.Publicacao
{
    /// <summary>
    /// DTO para publicações ativas ainda não processadas para envio de notificação/e-mail (oficial e comunidade).
    /// </summary>
    public class PublicacaoOficialEnvioDTO
    {
        public Guid Id { get; set; }
        /// <summary>Indica se a publicação é oficial (coluna <c>publicacao_oficial</c>).</summary>
        public bool PublicacaoOficial { get; set; }
        /// <summary>Comunidade em <c>tb_mkt_comunidade_publicacao</c>, quando existir (1:1 com a publicação).</summary>
        public string ComunidadeId { get; set; }
        /// <summary>Nome da comunidade, quando <see cref="ComunidadeId"/> estiver preenchido.</summary>
        public string ComunidadeNome { get; set; }
        public string Titulo { get; set; }
        /// <summary>Subtítulo ou headline da publicação (para exibição no e-mail/notificação).</summary>
        public string Subtitulo { get; set; }
        public int OrgId { get; set; }
        /// <summary>
        /// Ids dos grupos em <c>tb_mkt_publicaco_grupo</c>. Vazio = comunicado para toda a org (mesma regra do feed).
        /// </summary>
        public List<string> GrupoIds { get; set; } = new List<string>();
    }
}
