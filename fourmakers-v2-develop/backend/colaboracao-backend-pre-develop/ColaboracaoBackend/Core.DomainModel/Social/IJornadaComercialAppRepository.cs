using DataTransferObject.Domain;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados;
using DataTransferObject.Domain.Social;
using DataTransferObject.Domain.Usuario;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.Social
{
    public interface IJornadaComercialAppRepository
    {
        Task<KanbanEncontrosAcoesComerciais> BuscarKanbanEncontrosAcoesComerciais(string busca, DateTime? dataInicio, DateTime? dataFim, int orgIdUsuarioLogado);
        Task<EncontroAiPassos> AtualizarStatusAcoes(AtualizarIteracoesAcoesParam p);
        Task<InteracaoAcoesResponseDTO> ListarInteracaoAcao(int interacaoAcaoId);
        Task<ComentariosAcoesResponseDTO> InsercaoComentariosAcoes(ComentariosAcoesParamDTO param, UsuarioLogadoDTO user);
        Task<ComentariosAcoesResponseDTO> AtualizarComentario(ComentariosAcoesResponseDTO param, UsuarioLogadoDTO user);
        Task<bool> DeletarComentario(long comentarioId);
        Task<List<ComentariosAcoesResponseDTO>> ListarComentariosPorInteracao(int interacaoAcoesId);
        Task<InteracaiAIResponse> InserirInteracaoIA(EncontroAiParamInclusao p);
        Task<InteracaiAIResponse> AtualizarInteracaoIA(EncontroAiParamAtualizacao p);
        Task<bool> DeletarInteracaoIA(int interacaoIA);
        Task<IEnumerable<CategoriaAssuntoDTO>> ListarCategorias();
        Task<IEnumerable<SubcategoriaAssuntoDTO>> ListarSubcategoriasPorCategoria(int categoriaId);
        Task<IEnumerable<CategoriaAssuntoComSubDTO>> ListarCategoriasComSub();
        Task<bool> DeletarInteracaoCategoria(long interacaoCategoriaId);
        Task<InteracaoCategoriaSubResponseDTO> AtualizarInteracaoCategoria(CategoriaAtualizacaoParamDTO param);
        Task<InteracaoCategoriaSubResponseDTO> InsercaoInteracaoCategoria(CategoriaInsercaoParamDTO param);

        Task<(List<AgendaHierarquicaDTO> Data, int TotalRegistros)> FiltroAgendasInteracoesCategoriaSub(
           string nomeCliente = null,
           DateTime? dataAgendadaInicio = null,
           DateTime? dataAgendadaFim = null,
           int? categoriaId = null,
           int? subCategoriaId = null,
           int pagina = 1,
           int limite = 10);

        Task<IntegracaoMoxeResponseDTO> BuscarProximosPassosIntegracaoMoxe(IntegracaoMoxeRequestDTO request);

        Task<AgendaSolicitanteDTO> SolicitarParticiparAgenda(int tbAgendasComerciaisId, string codigoColaboradorExterno, string codColaboradorInternoCriador);
        Task<AgendaSolicitanteDTO> ObterAgendaSolicitantePorId(int id);
        Task<IReadOnlyList<AgendaSolicitanteDTO>> ListarAgendaSolicitantesPorAgendaComercial(int tbAgendasComerciaisId);
        Task<AgendaSolicitanteDTO> AceitarRecusarSolicitanteNaAgenda(AgendaSolicitanteAtualizacaoDTO param);
        Task<bool> ExcluirAgendaSolicitante(int tbAgendasComerciaisId, string codigoColaboradorExterno);

        Task<IReadOnlyList<AgendaConvidadoDTO>> ConvidarParaAgenda(int tbAgendasComerciaisId, IReadOnlyList<string> codigosColaboradorExterno);
        Task<AgendaConvidadoDTO> ObterAgendaConvidadoPorId(int id);
        Task<IReadOnlyList<AgendaConvidadoDTO>> ListarAgendaConvidadosPorAgendaComercial(int tbAgendasComerciaisId);
        Task<AgendaConvidadoDTO> AceitarRecusarConvidadoNaAgenda(AgendaConvidadoAtualizacaoDTO param);
        Task<bool> ExcluirAgendaConvidado(int tbAgendasComerciaisId, string codigoColaboradorExterno);
    }
}
