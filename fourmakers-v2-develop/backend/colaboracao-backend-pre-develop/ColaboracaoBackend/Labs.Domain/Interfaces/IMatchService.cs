using DataTransferObject.Domain.Labs;
using DataTransferObject.Domain.Match;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Labs.Domain.Interfaces
{
    /// <summary>
    /// Serviço de domínio que expõe os métodos do Match (RankCandidates, ScoreSingleCandidate, RankCandidatesIds).
    /// </summary>
    public interface IMatchService
    {
        Task<List<CandidatosMatchResponse>> RankCandidates(CandidatosMatchRequest request);
        /// <summary>
        /// Quando <paramref name="logContext"/> é informado, grava request/response em tb_labs_log_score_single_candidates e retorna o id em <see cref="ScoreSingleCandidateResult.IdLogScoreSingleCandidates"/>.
        /// </summary>
        Task<ScoreSingleCandidateResult> ScoreSingleCandidate(ScoreSingleCandidateRequest request, ScoreSingleCandidateLogContext? logContext = null);
        /// <summary>
        /// Quando <paramref name="logContext"/> é informado, grava request/response em tb_labs_log_rank_candidates_ids e retorna o id em <see cref="RankCandidatesIdsResult.IdLogRankCandidatesIds"/>.
        /// </summary>
        Task<RankCandidatesIdsResult> RankCandidatesIds(CandidatosMatchRequestIds request, RankCandidatesIdsLogContext? logContext);
    }
}
