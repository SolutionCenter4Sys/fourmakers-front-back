using DataTransferObject.Domain.Marketing.Comunicacao.Configuracao;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.Marketing.Comunicacao.Configuracao
{
    public interface IComunicacaoConfiguracaoRepository
    {
        Task<ConfiguracaoNotificacaoDTO> InserirOuAtualizarConfiguracaoAsync(string codigoInternoColaborador, int orgId, ConfiguracaoNotificacaoDTO request);
        Task<ConfiguracaoNotificacaoDTO> ObterConfiguracaoAsync(string codigoInternoColaborador, int orgId);
        /// <summary>
        /// Lista colaboradores da org que possuem notifica_plataforma = 1 em tb_mkt_colaborador_configuracao (opt-in).
        /// </summary>
        Task<List<ColaboradorNotificaPlataformaDTO>> ListarColaboradoresComNotificaPlataformaAsync(int orgId);

        /// <summary>
        /// Como <see cref="ListarColaboradoresComNotificaPlataformaAsync"/> restringindo a membros de pelo menos um dos grupos informados.
        /// </summary>
        Task<List<ColaboradorNotificaPlataformaDTO>> ListarColaboradoresComNotificaPlataformaPorGruposAsync(int orgId, IReadOnlyList<string> grupoIds);
        /// <summary>
        /// Lista colaboradores da org que possuem notifica_email = 1 e e-mail cadastrado (opt-in).
        /// </summary>
        Task<List<ColaboradorNotificaEmailDTO>> ListarColaboradoresComNotificaEmailAsync(int orgId);

        /// <summary>
        /// Como <see cref="ListarColaboradoresComNotificaEmailAsync"/> restringindo a membros de pelo menos um dos grupos informados.
        /// </summary>
        Task<List<ColaboradorNotificaEmailDTO>> ListarColaboradoresComNotificaEmailPorGruposAsync(int orgId, IReadOnlyList<string> grupoIds);

        /// <summary>
        /// Lê tb_mkt_org_config.enviar_email_publicacao_oficial_sempre (false se não houver linha ou for 0).
        /// </summary>
        Task<bool> ObterEnviarEmailPublicacaoOficialSempreAsync(int orgId);

        /// <summary>
        /// Colaboradores ativos da org com e-mail em tb_usuario (uso quando a org força e-mail de publicação oficial).
        /// </summary>
        Task<List<ColaboradorNotificaEmailDTO>> ListarColaboradoresComEmailCadastradoAtivosNaOrgAsync(int orgId);

        /// <summary>
        /// Como <see cref="ListarColaboradoresComEmailCadastradoAtivosNaOrgAsync"/> restringindo a membros de pelo menos um dos grupos informados.
        /// </summary>
        Task<List<ColaboradorNotificaEmailDTO>> ListarColaboradoresComEmailCadastradoAtivosNaOrgPorGruposAsync(int orgId, IReadOnlyList<string> grupoIds);

        /// <summary>
        /// Lê tb_mkt_org_config.enviar_notificacao_publicacao_oficial_sempre (false se não houver linha ou for 0).
        /// </summary>
        Task<bool> ObterEnviarNotificacaoPublicacaoOficialSempreAsync(int orgId);

        /// <summary>
        /// Colaboradores ativos da org em tb_usuario (uso quando a org força notificação de publicação oficial).
        /// </summary>
        Task<List<ColaboradorNotificaPlataformaDTO>> ListarColaboradoresAtivosNaOrgParaNotificacaoAsync(int orgId);

        /// <summary>
        /// Como <see cref="ListarColaboradoresAtivosNaOrgParaNotificacaoAsync"/> restringindo a membros de pelo menos um dos grupos informados.
        /// </summary>
        Task<List<ColaboradorNotificaPlataformaDTO>> ListarColaboradoresAtivosNaOrgParaNotificacaoPorGruposAsync(int orgId, IReadOnlyList<string> grupoIds);

        /// <summary>
        /// Opt-in plataforma: membros dos grupos ligados à comunidade ou em <c>tb_mkt_comunidade_usuario_participando</c>,
        /// excluindo <c>tb_mkt_comunidade_privada_usuario_nao_participando</c>.
        /// </summary>
        Task<List<ColaboradorNotificaPlataformaDTO>> ListarColaboradoresComNotificaPlataformaPorComunidadeAsync(int orgId, string comunidadeId);

        /// <summary>
        /// Opt-in e-mail: mesma regra de <see cref="ListarColaboradoresComNotificaPlataformaPorComunidadeAsync"/>.
        /// </summary>
        Task<List<ColaboradorNotificaEmailDTO>> ListarColaboradoresComNotificaEmailPorComunidadeAsync(int orgId, string comunidadeId);
    }
}
