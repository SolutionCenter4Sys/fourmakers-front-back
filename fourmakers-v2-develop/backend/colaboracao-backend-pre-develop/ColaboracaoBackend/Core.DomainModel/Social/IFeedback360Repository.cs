using DataTransferObject.Domain.Social;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.Social
{
    public interface IFeedback360Repository
    {
        Task<Feedback360DTO> GetByIdAsync(Guid id);
        Task<IEnumerable<Feedback360DTO>> ListarRecebidosPorColaboradorFiltradoAsync(Guid codigoInternoColaborador, DateTime dataInicio, DateTime dataFim, int? avaliacaoId);
        Task<IEnumerable<Feedback360DTO>> ListarEnviadosPorColaboradorAsync(Guid codigoInternoColaborador, FiltroFeedback360DTO filtro);
        Task<IEnumerable<Feedback360DTO>> ListarRecebidosPorColaboradorAsync(Guid codigoInternoColaborador, FiltroFeedback360DTO filtro);
        Task<IEnumerable<Feedback360RelacionamentoDTO>> ListarRelacionamentosAsync();
        Task<IEnumerable<Feedback360AvaliacaoDTO>> ListarAvaliacoesAsync();
        Task AddAsync(Feedback360DTO feedback, IReadOnlyList<Guid> codigosInternosDestinatarios);
        Task UpdateAsync(Feedback360DTO feedback);
        Task InserirLogEdicaoAsync(Guid id, Guid feedback360Id, Guid codigoColaboradorAlterador, string acao, string objetoJson, string alteracaoJson);
        /// <summary>Verifica se o colaborador está ativo na org (tb_colaborador_org.ativo = 1).</summary>
        Task<bool> ColaboradorAtivoNaOrgAsync(Guid codigoInternoColaborador, int orgId);
        /// <summary>Retorna todos os colaboradores subordinados ao gestor (recursivo via tb_colaborador_hierarquia).</summary>
        Task<IEnumerable<ColaboradorGestorFeedbackDTO>> ListarSubordinadosGestorAsync(Guid codigoInternoGestor, int orgId);
        /// <summary>Gera modelo STAR do feedback 360 via API Moxe (IA). Recebe situação, data (formatada) e contexto; retorna STAR + prévia em JSON.</summary>
        Task<GerarModeloStarMoxeResultadoDTO> GerarModeloStarMoxeAsync(string situacao, string dataInteracao, string contexto);
        /// <summary>Lista todos os feedbacks da organização para o mural de reconhecimento (por orgId). Suporta filtros e paginação (limit/cursor).</summary>
        Task<IEnumerable<Feedback360DTO>> ListarMuralReconhecimentoPorOrgAsync(int orgId, FiltroFeedback360DTO filtro);

        /// <summary>Busca o mural de reconhecimento com reações (emoji + contagem por card e se o usuário logado reagiu).</summary>
        Task<IEnumerable<Feedback360MuralReconhecimentoDTO>> BuscarMuralReconhecimentoAsync(int orgId, FiltroFeedback360DTO filtro, Guid codigoInternoColaboradorLogado);

        /// <summary>Lista as opções de reação (emojis) ativas para o mural.</summary>
        Task<IEnumerable<Feedback360MuralReacaoDTO>> ListarReacoesMuralAsync();

        /// <summary>Define a reação do colaborador no card (estado final). ReacaoId = 0 remove; ReacaoId &gt; 0 define/substitui. Retorna o reacaoId atual após a operação (0 = nenhuma). Operação atômica sem leitura prévia.</summary>
        Task<int> DefinirReacaoMuralAsync(Guid feedback360Id, int reacaoId, Guid codigoInternoColaborador);
    }
}
