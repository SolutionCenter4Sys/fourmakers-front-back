using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Marketing.Comunicacao.Grupo
{
    /// <summary>
    /// Indica se o usuário logado está em algum grupo que concede cada permissão.
    /// Se não tiver nenhum grupo ou todos os grupos tiverem false para a permissão, retorna false.
    /// </summary>
    public class PermissoesGrupoUsuarioResponseDTO
    {
        public bool PermiteCriarPublicacaoOficial { get; set; }
        public bool PublicacaoOficialRequerAprovacao { get; set; }
        public bool AprovaPublicacaoOficial { get; set; }

        public bool PermiteCriarComunidade { get; set; }
        /// <summary>
        /// Indica se o usuário possui a funcionalidade de sistema GESTAO_COMUNICADOS (via grupo de acesso).
        /// </summary>
        public bool GestaoComunicados { get; set; }
        /// <summary>
        /// Indica se o usuário está em algum grupo que permite acessar Analytics (permite_acessar_analytics).
        /// </summary>
        public bool PermiteAcessarAnalytics { get; set; }

        /// <summary>
        /// Configuração da org (tb_mkt_org_config.somente_publicacao_oficial_no_feed), preenchida na montagem das permissões.
        /// O front usa para decidir enviar somentePublicacaoOficial em ObterListaPublicacaoGeral.
        /// </summary>
        [JsonPropertyName("somentePublicacaoOficialNoFeed")]
        public bool SomentePublicacaoOficialNoFeed { get; set; }

        /// <summary>
        /// Configuração da org (tb_mkt_org_config.ocultar_criador_comunidade), preenchida na montagem das permissões.
        /// </summary>
        [JsonPropertyName("ocultarCriadorComunidade")]
        public bool OcultarCriadorComunidade { get; set; }
    }
}
