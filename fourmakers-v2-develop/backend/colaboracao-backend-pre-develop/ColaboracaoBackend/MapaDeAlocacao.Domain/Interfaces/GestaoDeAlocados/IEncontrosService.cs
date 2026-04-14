using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.SolicitacaoParticipacao;
using DataTransferObject.Domain.Usuario;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MapaDeAlocacao.Domain.Interfaces.GestaoDeAlocados
{
    public interface IEncontrosService
    {
        Task<ApiGenericResult<ArquivoEncontroDto>> DeletarArquivo(int id);
        Task<ApiGenericResult<ArquivoEncontroDto>> InserirArquivo(ArquivoEncontroParam param);
        Task<ApiGenericResult<List<InteracoesCategoriaSubResponseDTO>>> ListarCategoriasSubPorInteracaoId(int interacaoId);
        Task<ApiGenericResult<InteracoesDTO>> criarEncontro(EncontrosDTO param, UsuarioLogadoDTO user);
        Task<ApiGenericResult<InteracoesDTO>> atualizarEncontro(string encontroId, EncontrosParam param);
        Task<ApiGenericResult<EncontrosResponse>> buscarEncontroPorId(string encontroId);
        Task<ApiGenericResult<EncontrosResponseDetalhado>> BuscarEncontroPorIdComComentarios(string id);
        Task<ApiGenericResult<bool>> deletarEncontro(string encontroId);
        Task<ApiGenericResult<EncontrosResponse>> buscarEncontroPorColaboradorId(string colaboradorId);
        Task<ApiGenericResult<List<EncontrosResponse>>> buscarTodosEncontros();
        
        Task<ApiGenericResult<AgendaEncontroResult>> carregarAgenda(int id, string cpfUsuarioLogado);
        Task<ApiGenericResult<AgendaEncontroResult>> criarAgenda(AgendaEncontroParam param, UsuarioLogadoDTO user);
        Task<ApiGenericResult<AgendaEncontroResult>> atualizarAgenda(int agendaId, AgendaEncontroParam param, UsuarioLogadoDTO user);
        Task<ApiGenericResult<bool>> deletarAgenda(string agendaId);
        Task<ApiGenericResult<List<AgendaEncontroResult>>> buscarAgendaEncontros(string cpfUsuarioLogado, string dataInicio, string dataFim) ;
        Task<ApiGenericResult<List<AgendaEncontroResult>>> buscarAgendaEncontrosPorColaboradorId(string colaboradorId);
        Task<ApiGenericResult<List<AgendaEncontroResult>>> buscarAgendaEncontrosPorData(string data);
        Task<ApiGenericResult<List<EncontrosResponse>>> buscarEncontrosPorAgendaId(string agendaId);
        Task<ApiGenericResult<EncontroAi>> CriarEncontroAi(EncontroAi ai);
        Task<ApiGenericResult<EncontroAi>> AtualizarEncontroAi(EncontroAi ai);
        Task<ApiGenericResult<bool>> DeletarEncontroAi(int id);
        Task<ApiGenericResult<EncontroAi>> BuscarEncontroAiPorEncontroId(int encontroId);
        Task<ApiGenericResult<List<SolicitacaoResponse>>> BuscarSolicitacoesAgendas(string cpfUsuarioLogado, int cursor, int limite);
        Task<ApiGenericResult<List<SolicitacaoResponse>>> BuscarSolicitacoesMinhasAgendas(string cpfUsuarioLogado, int cursor, int limite);
        Task<ApiGenericResult<List<SolicitacaoResponse>>> BuscarSolicitacoesPorAgenda(int agendaId, int cursor, int limite);
        Task<ApiGenericResult<bool>> AprovarReprovarSolicitacaoAgenda(StatusSolicitacaoParticipante decisaoStatus, int agendaId, string codigoColaboradorSolicitante, string cpfUsuarioLogado);
        Task<ApiGenericResult<bool>> AceitarRecusarConviteAgenda(StatusSolicitacaoParticipante decisaoStatus, int agendaId, string codigoColaborador);
        Task<ApiGenericResult<bool>> SolicitarParticipacaoAgenda(int agendaId, string codigoColaboradorSolicitante, string codigoColaboradorCriador);
        Task<ApiGenericResult<ConvidarParticipantesAgendaParam>> ConvidarParticipanteAgenda(ConvidarParticipantesAgendaParam param);
        Task<ApiGenericResult<List<AgendaEncontroResult>>> BuscarAgendasFilhosCompletas(string agendaId, string cpfUsuarioLogado);
        Task<ApiGenericResult<List<int>>> BuscarIdAgendasFilhos(string agendaId);
        Task<ApiGenericResult<List<AgendaObjetivoDTO>>> ListarTodosObjetivos();
    }
};