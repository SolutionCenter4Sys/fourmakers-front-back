using DataTransferObject.Domain.Marketing.Comunicacao.Grupo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.Marketing.Comunicacao.Grupo
{
    public interface IComunicacaoGrupoRepository
    {
        /// <summary>Modelos de contratação distintos entre colaboradores elegíveis para grupo (referência para filtro em ColaboradoresDisponiveis).</summary>
        Task<List<ColaboradorDisponivelModeloContratacaoResumoDTO>> ObterSugestoesModeloContratacaoColaboradoresDisponiveisAsync(int orgId);

        /// <summary>Diretorias distintas entre colaboradores elegíveis para grupo (referência para filtro em ColaboradoresDisponiveis).</summary>
        Task<List<ColaboradorDisponivelDiretoriaResumoDTO>> ObterSugestoesDiretoriaColaboradoresDisponiveisAsync(int orgId);

        Task<ColaboradoresResponseDTO> ListarColaboradoresDisponiveisParaAdicionarAsync(string filtro, int orgId, List<string> codigosModeloContratacao = null, List<string> codigosDiretoria = null);
        Task<List<GrupoResumoDTO>> ListarGrupoResumoAsync(int orgId);
        Task<GrupoDetalheDTO> ObterGrupoAsync(string grupoId, int orgId);
        Task<string> InserirGrupoAsync(int orgId, InserirGrupoRequestDTO request);
        Task<bool> AtualizarGrupoAsync(string grupoId, int orgId, AtualizarGrupoRequestDTO request);
        Task<bool> DeletarGrupoAsync(string grupoId, int orgId);
        Task<bool> UsuarioPossuiPermissaoCriarComunidadeAsync(string codigoInternoColaborador, int orgId);

        /// <summary>
        /// Verifica se o usuário está em algum grupo que permite criar publicação oficial (permite_criar_publicacao_oficial = true).
        /// </summary>
        Task<bool> UsuarioPodeCriarPublicacaoOficialAsync(string codigoInternoColaborador, int orgId);

        /// <summary>
        /// Verifica se o usuário está em algum grupo que pode aprovar publicação oficial (aprova_publicacao_oficial = true).
        /// </summary>
        Task<bool> UsuarioPodeAprovarPublicacaoOficialAsync(string codigoInternoColaborador, int orgId);

        /// <summary>
        /// Retorna as permissões do usuário com base nos grupos em que está.
        /// Para cada permissão: true se estiver em algum grupo que permite; false se não tiver grupos ou nenhum grupo permitir.
        /// <paramref name="somentePublicacaoOficialNoFeed"/> e <paramref name="ocultarCriadorComunidade"/> vêm da configuração da org (tb_mkt_org_config), obtidos fora deste método.
        /// </summary>
        Task<PermissoesGrupoUsuarioResponseDTO> ObterPermissoesGruposUsuarioAsync(string codigoInternoColaborador, int orgId, bool somentePublicacaoOficialNoFeed, bool ocultarCriadorComunidade);

        /// <summary>
        /// Lê tb_mkt_org_config.somente_publicacao_oficial_no_feed para a org (false se não houver linha).
        /// </summary>
        Task<bool> ObterSomentePublicacaoOficialNoFeedOrgAsync(int orgId);

        /// <summary>
        /// Lê tb_mkt_org_config.ocultar_criador_comunidade para a org (false se não houver linha).
        /// </summary>
        Task<bool> ObterOcultarCriadorComunidadeOrgAsync(int orgId);
    }
}
