using System.Collections.Generic;
using DataTransferObject.Domain;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados;
using DataTransferObject.Domain.Social;
using DataTransferObject.Domain.Usuario;

namespace Social.Domain.Interfaces
{
    public interface IJornadaComercialAppService
    {
        Task<KanbanEncontrosAcoesComerciais> BuscarKanbanEncontrosAcoesComerciais(string busca, DateTime? dataInicio, DateTime? dataFim, int orgIdUsuarioLogado);
        Task<EncontroAiPassos> AtualizarStatusAcoes(AtualizarIteracoesAcoesParam p);
        Task<InteracaoAcoesResponseDTO> ListarInteracaoAcao(int interacaoAcaoId);
        Task<ApiGenericResult<ComentariosAcoesResponseDTO>> InsercaoComentariosAcoes(ComentariosAcoesParamDTO param, UsuarioLogadoDTO user);
        Task<ApiGenericResult<ComentariosAcoesResponseDTO>> AtualizarComentario(ComentariosAcoesResponseDTO param, UsuarioLogadoDTO user);
        Task<ApiGenericResult<bool>> DeletarComentario(long comentarioId);
        Task<ApiGenericResult<List<ComentariosAcoesResponseDTO>>> ListarComentariosPorInteracao(int interacaoAcoesId);
        Task<InteracaiAIResponse> InserirInteracaoIA(EncontroAiParamInclusao p);
        Task<InteracaiAIResponse> AtualizarInteracaoIA(EncontroAiParamAtualizacao p);
        Task<bool> DeletarInteracaoIA(int interacaoIA);
        Task<IEnumerable<CategoriaAssuntoDTO>> ListarCategorias();
        Task<IEnumerable<SubcategoriaAssuntoDTO>> ListarSubcategoriasPorCategoria(int categoriaId);
        Task<IEnumerable<CategoriaAssuntoComSubDTO>> ListarCategoriasComSub();
        Task<ApiGenericResult<bool>> DeletarInteracaoCategoria(long interacaoCategoriaId);
        Task<ApiGenericResult<InteracaoCategoriaSubResponseDTO>> AtualizarInteracaoCategoria(CategoriaAtualizacaoParamDTO param);
        Task<ApiGenericResult<InteracaoCategoriaSubResponseDTO>> InsercaoInteracaoCategoria(CategoriaInsercaoParamDTO param);

        Task<ApiGenericResult<PaginadoDTO<AgendaHierarquicaDTO>>> FiltroAgendasInteracoesCategoriaSub(
                                                string nomeCliente, DateTime? dataAgendadaInicio, DateTime? dataAgendadaFim,
                                                int? categoriaId, int? subCategoriaId, int pagina, int limite);

        Task<ApiGenericResult<IntegracaoMoxeResponseDTO>> BuscarProximosPassosIntegracaoMoxe(IntegracaoMoxeRequestDTO request);

        Task<ApiGenericResult<AgendaSolicitanteDTO>> SolicitarParticiparAgenda(int agendaId, string codigoColaboradorSolicitante, string codigoColaboradorCriador);
        Task<ApiGenericResult<AgendaSolicitanteDTO>> ObterAgendaSolicitantePorId(int id);
        Task<ApiGenericResult<List<AgendaSolicitanteDTO>>> ListarAgendaSolicitantesPorAgendaComercial(int tbAgendasComerciaisId);
        Task<ApiGenericResult<AgendaSolicitanteDTO>> AceitarRecusarSolicitanteNaAgenda(AgendaSolicitanteAtualizacaoDTO param);
        Task<ApiGenericResult<bool>> ExcluirAgendaSolicitante(int tbAgendasComerciaisId, string codigoColaboradorExterno);

        Task<ApiGenericResult<List<AgendaConvidadoDTO>>> ConvidarParaAgenda(ConvidarParaAgendaRequestDTO param);
        Task<ApiGenericResult<AgendaConvidadoDTO>> ObterAgendaConvidadoPorId(int id);
        Task<ApiGenericResult<List<AgendaConvidadoDTO>>> ListarAgendaConvidadosPorAgendaComercial(int tbAgendasComerciaisId);
        Task<ApiGenericResult<AgendaConvidadoDTO>> AceitarRecusarConvidadoNaAgenda(AgendaConvidadoAtualizacaoDTO param);
        Task<ApiGenericResult<bool>> ExcluirAgendaConvidado(int tbAgendasComerciaisId, string codigoColaboradorExterno);
    }
}