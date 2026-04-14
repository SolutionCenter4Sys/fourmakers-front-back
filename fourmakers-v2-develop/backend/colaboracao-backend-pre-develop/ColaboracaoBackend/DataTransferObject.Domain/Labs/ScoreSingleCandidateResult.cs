using DataTransferObject.Domain.Match;
using System;

namespace DataTransferObject.Domain.Labs
{
    /// <summary>
    /// Resultado de ScoreSingleCandidate. Quando log é gravado, <see cref="IdLogScoreSingleCandidates"/> contém o id (GUID) do registro em tb_labs_log_score_single_candidates.
    /// </summary>
    public class ScoreSingleCandidateResult
    {
        public CandidatosMatchResponse Response { get; set; } = new CandidatosMatchResponse();
        /// <summary>
        /// Id (GUID) do log gravado em tb_labs_log_score_single_candidates (quando contexto de log foi informado).
        /// </summary>
        public Guid? IdLogScoreSingleCandidates { get; set; }
    }
}
