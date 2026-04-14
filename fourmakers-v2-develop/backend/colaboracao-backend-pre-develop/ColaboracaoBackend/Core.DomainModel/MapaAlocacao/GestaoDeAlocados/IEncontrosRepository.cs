using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados;
using DataTransferObject.Domain.Projeto.GestorExterno;
using DataTransferObject.Domain.Usuario;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.SolicitacaoParticipacao;

namespace MapaDeAlocacao.Domain.Impl.GestaoDeAlocados
{

    public interface IEncontrosRepository
    {
        Task<ArquivoEncontroDto> BuscarArquivoPorId(int id);
        Task<bool> DeletarArquivo(int id);
        
        Task<ArquivoEncontroDto> InserirArquivo(ArquivoEncontroParam param, string url);
        
        Task<InteracoesDTO> criarEncontro(EncontrosDTO param, UsuarioLogadoDTO user);
        Task<InteracoesDTO> atualizarEncontro(string encontroId, EncontrosParam param);
        Task<EncontrosResponse> buscarEncontroPorId(string encontroId);
        Task<EncontrosResponseDetalhado> BuscarEncontroPorIdComComentarios(string id);
        Task<IEnumerable<InteracoesCategoriaSubResponseDTO>> ListarCategoriasSubPorInteracaoId(int interacaoId);
        Task<bool> deletarEncontro(string encontroId);
        Task<EncontrosResponse> buscarEncontroPorColaboradorId(string colaboradorId);
        Task<List<EncontrosResponse>> buscarTodosEncontros();

        Task<AgendaEncontroResult> carregarAgenda(int id, string cpfUsuarioLogado);
        
        Task<AgendaEncontroResult> criarAgenda(AgendaEncontroParam param, UsuarioLogadoDTO user);

        Task<AgendaEncontroResult> atualizarAgenda(int agendaId, AgendaEncontroParam param, UsuarioLogadoDTO user);
        
        Task<bool> deletarAgenda(string agendaId);
        
        Task<List<AgendaEncontroResult>> buscarAgendaEncontros(string cpfUsuarioLogado, string dataInicio, string dataFim);
        Task<List<AgendaEncontroResult>> buscarAgendaEncontrosPorColaboradorId(string colaboradorId);
        Task<List<AgendaEncontroResult>> buscarAgendaEncontrosPorData(string data);
        
        Task<List<EncontrosResponse>> buscarEncontrosPorAgendaId(string agendaId);
        Task<EncontroAi> CriarEncontroAi(EncontroAi ai);
        Task<EncontroAi> AtualizarEncontroAi(EncontroAi ai);
        Task<bool> DeletarEncontroAi(int id);
        Task<EncontroAi> BuscarEncontroAiPorEncontroId(int encontroId);

        Task<UsuarioColaboradorDTO> BuscarDadosColaboradorEmail(string cpf);
        Task<GestorExternoResult> BuscarDadosGestorExternoEmail(string codigoGestor);
        Task<string> BuscarNomeClienteEmailAgenda(string codCliente);
        Task<List<string>> buscarDeviceTokensAppEmLote(List<string> codigosInternoColaborador);
        Task<string> buscarDeviceTokenApp(string codigoInternoColaborador);
        Task<string> BuscarNomeColaborador(string codigoColaborador);
        Task<List<SolicitacaoResponse>> BuscarSolicitacoesAgendas(string cpfUsuarioLogado, int cursor, int limite);
        Task<List<SolicitacaoResponse>> BuscarSolicitacoesMinhasAgendas(string cpfUsuarioLogado, int cursor, int limite);
        Task<List<SolicitacaoResponse>> BuscarSolicitacoesPorAgenda(int agendaId, int cursor, int limite);
        Task<bool> AprovarReprovarSolicitacaoAgenda(StatusSolicitacaoParticipante decisaoStatus, int agendaId, string codigoColaboradorSolicitante, string cpfUsuarioLogado);
        Task<bool> AceitarRecusarConviteAgenda(StatusSolicitacaoParticipante decisaoStatus, int agendaId, string codigoColaborador);
        Task<bool> SolicitarParticipacaoAgenda(int agendaId, string codigoColaboradorSolicitante, string codigoColaboradorCriador);
        Task<AgendaEncontroResult> BuscarAgendaPorId(int agendaId);
        Task<bool> ConvidarGestorExternoAgenda(string codigoGestorExterno, int agendaId);
        Task<bool> ConvidarColaboradorInternoAgenda(string codigoColaboradorInterno, int agendaId);
        Task<List<AgendaEncontroResult>> BuscarAgendasFilhosCompletas(string agendaId, string cpfUsuarioLogado);
        Task<List<int>> BuscarIdAgendasFilhos(string agendaId);
        Task<List<AgendaObjetivoDTO>> ListarTodosObjetivos();
    }
};