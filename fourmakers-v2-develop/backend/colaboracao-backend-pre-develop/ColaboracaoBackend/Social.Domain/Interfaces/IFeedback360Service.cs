using DataTransferObject.Domain.Social;
using DataTransferObject.Domain.Usuario;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Social.Domain.Interfaces
{
    public interface IFeedback360Service
    {
        Task<Feedback360DTO> EnviarFeedbackAsync(EnviarFeedback360DTO dto, UsuarioLogadoDTO usuario);
        Task<Feedback360DTO> AtualizarFeedbackAsync(Guid id, AtualizarFeedback360DTO dto, UsuarioLogadoDTO usuario);
        Task<Feedback360DTO> GetByIdAsync(Guid id);
        Task<FeedbackGestorResultadoDTO> ListarRecebidosGestorAsync(FiltroFeedbackGestorDTO filtro);
        Task<ListagemColaboradoresGestorFeedbackDTO> ListarColaboradoresGestorAsync(Guid codigoInternoGestor, int orgId);
        Task<IEnumerable<Feedback360DTO>> ListarEnviadosAsync(UsuarioLogadoDTO usuario, FiltroFeedback360DTO filtro);
        Task<Feedback360RecebidosResultadoDTO> ListarRecebidosAsync(UsuarioLogadoDTO usuario, FiltroFeedback360DTO filtro);
        Task<IEnumerable<Feedback360RelacionamentoDTO>> ListarRelacionamentosAsync();
        Task<IEnumerable<Feedback360AvaliacaoDTO>> ListarAvaliacoesAsync();
        /// <summary>Gera modelo STAR do feedback 360 com IA (Moxe) a partir de situação, data e contexto. Retorna STAR (situacao, tarefa, acao, resultado) e previa em JSON.</summary>
        Task<GerarModeloStarMoxeResultadoDTO> GerarModeloStarMoxeAsync(GerarModeloStarMoxeDTO dto, UsuarioLogadoDTO usuario);
        /// <summary>Busca todos os feedbacks da organização do usuário logado para o mural de reconhecimento (cards) com reações (emoji + contagem). Filtros opcionais: dataInicio, dataFim, sentimentoId, relacionamentoId, busca, limit, cursor.</summary>
        Task<IEnumerable<Feedback360MuralReconhecimentoDTO>> BuscarMuralReconhecimentoAsync(UsuarioLogadoDTO usuario, FiltroFeedback360DTO filtro);

        /// <summary>Lista as opções de reação (emojis) ativas para o mural.</summary>
        Task<IEnumerable<Feedback360MuralReacaoDTO>> ListarReacoesMuralAsync();

        /// <summary>Define a reação do usuário no card (estado final desejado). ReacaoId = 0 remove; ReacaoId &gt; 0 define ou substitui. Retorna o reacaoId atual após a operação (0 = nenhuma).</summary>
        Task<int> DefinirReacaoMuralAsync(Guid feedback360Id, int reacaoId, UsuarioLogadoDTO usuario);
    }
}
