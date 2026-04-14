using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Marketing.Comunicacao.Profissionais;
using DataTransferObject.Domain.Marketing.Comunicacao.Publicacao;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Marketing.Domain.Interfaces.Comunicacao.Publicacao
{
    public interface IComunicacaoPublicacaoService
    {
        Task<ApiGenericResult<PublicacaoFeedResponseDTO>> ObterListaPublicacaoGeralAsync(string codigoInternoColaborador, int orgId, bool somenteLeituraObrigatoria, string tipo, List<string> labels = null, List<string> tags = null, List<string> status = null, List<string> statusAprovacao = null, string comunidadeId = null, bool somenteNaoOcultoNoFeed = false, bool somentePublicacaoOficial = false);
        /// <summary>
        /// Lista sugestões de nome de pasta (publicações ativas da org), para autocomplete no formulário.
        /// </summary>
        Task<ApiGenericResult<PublicacaoPastasSugestaoResponseDTO>> ObterSugestoesPastasAsync(string codigoInternoColaborador, int orgId);
        Task<ApiGenericResult> ConfirmarLeituraObrigatoriaAsync(string codigoInternoColaborador, int orgId, ConfirmarLeituraObrigatoriaRequestDTO request);
        /// <summary>
        /// Retorna a lista de pessoas que aceitaram/confirmaram leitura do informativo ou documento (mesma estrutura de profissionais).
        /// </summary>
        Task<ApiGenericResult<List<ProfissionalDTO>>> ObterColaboradoresQueConfirmaramLeituraAsync(string publicacaoId, string codigoInternoColaborador, int orgId);
        Task<ApiGenericResult<PublicacaoDetalheDTO>> InserirPublicacaoAsync(string codigoInternoColaborador, int orgId, InserirPublicacaoRequestDTO request);
        Task<ApiGenericResult<PublicacaoDetalheDTO>> InserirPublicacaoAsync(string codigoInternoColaborador, int orgId, InserirPublicacaoRequestDTO request, List<PublicacaoAnexoUploadDTO> anexosUpload);
        Task<ApiGenericResult<PublicacaoDetalheDTO>> ObterPublicacaoPorIdAsync(string publicacaoId, string codigoInternoColaborador, int orgId);
        Task<ApiGenericResult> AtualizarPublicacaoAsync(string codigoInternoColaborador, int orgId, AtualizarPublicacaoRequestDTO request);
        Task<ApiGenericResult> AdicionarAnexosAsync(string publicacaoId, string codigoInternoColaborador, int orgId, List<PublicacaoAnexoInputDTO> anexos, List<PublicacaoAnexoUploadDTO> anexosUpload);
        Task<ApiGenericResult> RemoverAnexoAsync(string publicacaoId, string anexoId, string codigoInternoColaborador, int orgId);
        Task<ApiGenericResult> RemoverAnexosAsync(string publicacaoId, List<string> anexoIds, string codigoInternoColaborador, int orgId);
        Task<ApiGenericResult> DeletarPublicacaoAsync(string publicacaoId, string codigoInternoColaborador, int orgId);
        Task<ApiGenericResult> ArquivarPublicacaoAsync(string publicacaoId, string codigoInternoColaborador, int orgId);
        /// <summary>
        /// Atualiza apenas o bit ocultar_no_feed da publicação.
        /// </summary>
        Task<ApiGenericResult> AtualizarOcultarNoFeedAsync(string codigoInternoColaborador, int orgId, AtualizarOcultarNoFeedRequestDTO request);
        Task<ApiGenericResult<Guid>> AdicionarComentarioAsync(string codigoInternoColaborador, int orgId, InserirComentarioRequestDTO request);
        Task<ApiGenericResult> AtualizarComentarioAsync(string codigoInternoColaborador, int orgId, AtualizarComentarioRequestDTO request);
        Task<ApiGenericResult> RemoverComentarioAsync(string publicacaoId, string comentarioId, string codigoInternoColaborador, int orgId);
        Task<ApiGenericResult> AdicionarInteracaoComentarioAsync(string codigoInternoColaborador, int orgId, AdicionarInteracaoComentarioRequestDTO request);
        Task<ApiGenericResult> RemoverInteracaoComentarioAsync(string codigoInternoColaborador, int orgId, RemoverInteracaoComentarioRequestDTO request);
        Task<ApiGenericResult> AdicionarInteracaoPublicacaoAsync(string codigoInternoColaborador, int orgId, AdicionarInteracaoPublicacaoRequestDTO request);
        Task<ApiGenericResult> RemoverInteracaoPublicacaoAsync(string codigoInternoColaborador, int orgId, RemoverInteracaoPublicacaoRequestDTO request);
    }
}
