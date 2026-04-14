using DataTransferObject.Domain.Marketing.Comunicacao.Publicacao;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.Marketing.Comunicacao.Publicacao
{
    public interface IComunicacaoPublicacaoRepository
    {
        Task CriarOuAtualizarInteracoesParaFeedGeralAsync(string codigoInternoColaborador, int orgId, bool somenteLeituraObrigatoria, string tipo, List<string> labels, List<string> tags, List<string> status, List<string> statusAprovacao, string comunidadeId, bool somenteNaoOcultoNoFeed = false, bool somentePublicacaoOficial = false);
        Task<PublicacaoFeedResponseDTO> ObterListaPublicacaoGeralAsync(string codigoInternoColaborador, int orgId, bool somenteLeituraObrigatoria, string tipo, List<string> labels, List<string> tags, List<string> status, List<string> statusAprovacao, string comunidadeId, bool somenteNaoOcultoNoFeed = false, bool somentePublicacaoOficial = false);
        Task<bool> ConfirmarLeituraObrigatoriaAsync(Guid publicacaoId, string codigoInternoColaborador, int orgId);
        Task<Guid> InserirPublicacaoAsync(string codigoInternoColaboradorCriacao, int orgId, InserirPublicacaoRequestDTO request, string publicacaoStatus, string aprovacaoStatus, DateTime? dataPublicacao);

        /// <summary>
        /// Reconstrói as sugestões de pasta da org a partir de todas as publicações com status ativa (pasta não vazia).
        /// </summary>
        Task SincronizarSugestoesPastasPorPublicacoesAtivasAsync(int orgId);

        /// <summary>
        /// Lista nomes de pasta sugeridos para a org (tabela sincronizada com publicações ativas).
        /// </summary>
        Task<List<string>> ObterNomesPastasSugestaoAsync(int orgId);
        Task<PublicacaoDetalheDTO> ObterPublicacaoPorIdAsync(Guid publicacaoId, string codigoInternoColaborador, int orgId);
        /// <summary>
        /// Retorna os códigos dos colaboradores que confirmaram leitura (aceitaram) o informativo/documento, ordenados por data_confirmou_leitura.
        /// </summary>
        Task<List<string>> ObterCodigosColaboradoresQueConfirmaramLeituraAsync(Guid publicacaoId, int orgId);
        Task<bool> AtualizarPublicacaoAsync(string codigoInternoColaboradorAlteracao, int orgId, AtualizarPublicacaoRequestDTO request);
        Task<bool> DeletarPublicacaoAsync(string publicacaoId, string codigoInternoColaboradorAlteracao, int orgId);
        Task<bool> ArquivarPublicacaoAsync(string publicacaoId, string codigoInternoColaboradorAlteracao, int orgId);
        /// <summary>
        /// Atualiza apenas o bit ocultar_no_feed da publicação.
        /// </summary>
        Task<bool> AtualizarOcultarNoFeedAsync(Guid publicacaoId, string codigoInternoColaboradorAlteracao, int orgId, bool ocultarNoFeed);
        //Task<PublicacaoGerencialResponseDTO> ObterListaPublicacaoAgendadoEAprovacaoAsync(int orgId, string codigoInternoColaborador);
        Task<bool> PublicarAgoraAsync(string publicacaoId, string codigoInternoColaborador, int orgId);
        Task<bool> AprovarComunicacaoAsync(string publicacaoId, string codigoInternoColaborador, int orgId);
        Task<bool> RejeitarComunicacaoAsync(string publicacaoId, string codigoInternoColaborador, int orgId, string motivoRejeicao);
        Task<List<string>> ObterGruposExistentesAsync(List<string> grupoIds, int orgId);
        /// <summary>
        /// Verifica se o usuário logado está em algum grupo que permite publicar sem aprovação (requer aprovacao = false).
        /// </summary>
        Task<bool> UsuarioEstaEmAlgumGrupoQuePermitePublicarSemAprovacaoAsync(string codigoInternoColaborador, int orgId);
        Task<bool> InserirAnexosPublicacaoAsync(Guid publicacaoId, List<PublicacaoAnexoInputDTO> anexos);
        Task<PublicacaoAnexoDTO> ObterAnexoPublicacaoAsync(Guid publicacaoId, Guid anexoId);
        Task<bool> RemoverAnexoPublicacaoAsync(Guid publicacaoId, Guid anexoId);
        Task<Guid?> InserirComentarioAsync(Guid publicacaoId, string codigoInternoColaborador, string conteudo, Guid? comentarioPaiId);
        Task<(string CodigoAutor, DateTime DataCriacao)?> ObterComentarioParaValidacaoAsync(Guid comentarioId);
        Task<bool> AtualizarComentarioAsync(Guid comentarioId, string codigoInternoColaborador, string conteudo);
        Task<bool> RemoverComentarioAsync(Guid comentarioId, string codigoInternoColaborador);
        Task<bool> AdicionarInteracaoComentarioAsync(Guid comentarioId, string codigoInternoColaborador, string emoji);
        Task<bool> RemoverInteracaoComentarioAsync(Guid comentarioId, string codigoInternoColaborador);
        Task<bool> AdicionarInteracaoPublicacaoAsync(Guid publicacaoId, string codigoInternoColaborador, string emoji);
        Task<bool> RemoverInteracaoPublicacaoAsync(Guid publicacaoId, string codigoInternoColaborador);

        /// <summary>
        /// Lista IDs de publicações agendadas que já passaram da data de postar e estão aprovadas ou não requerem aprovação.
        /// </summary>
        Task<List<Guid>> ListarIdsAgendadasParaAtivarAsync(DateTime dataReferencia);

        /// <summary>
        /// Lista IDs de publicações ativas que já passaram da data de validade.
        /// </summary>
        Task<List<Guid>> ListarIdsAtivasParaExpirarAsync(DateTime dataReferencia);

        /// <summary>
        /// Atualiza status e opcionalmente data_publicacao de várias publicações em lote.
        /// </summary>
        Task AtualizarStatusPublicacaoEmLoteAsync(IEnumerable<Guid> ids, string publicacaoStatus, DateTime? dataPublicacao = null);

        /// <summary>
        /// Lista publicações oficiais ativas (aprovadas ou nao_requer) ainda não processadas para envio de notificação/e-mail.
        /// </summary>
        Task<List<PublicacaoOficialEnvioDTO>> ListarPublicacoesAtivasNaoProcessadasEnvioAsync();

        /// <summary>
        /// Marca as publicações indicadas como processadas para envio de notificação e e-mail.
        /// </summary>
        Task AtualizarProcessadoEnvioNotificacaoEmailAsync(IEnumerable<Guid> publicacaoIds);

        /// <summary>
        /// Insere registro no log de envio (notificacao ou email) para publicacao oficial.
        /// tipo: "notificacao" ou "email".
        /// </summary>
        Task InserirLogEnvioPublicacaoOficialAsync(Guid publicacaoId, string tipo, string codigoInternoColaborador, int orgId);
    }
}
