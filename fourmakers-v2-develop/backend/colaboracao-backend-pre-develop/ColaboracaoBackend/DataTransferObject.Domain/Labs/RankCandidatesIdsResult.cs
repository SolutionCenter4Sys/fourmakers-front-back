using DataTransferObject.Domain.Match;
using System;
using System.Collections.Generic;

namespace DataTransferObject.Domain.Labs
{
    /// <summary>
    /// Resultado de RankCandidatesIds. Quando log é gravado, <see cref="IdLogRankCandidatesIds"/> contém o id (GUID) do registro em tb_labs_log_rank_candidates_ids.
    /// </summary>
    public class RankCandidatesIdsResult
    {
        public List<CandidatosMatchResponse> Candidates { get; set; } = new List<CandidatosMatchResponse>();
        /// <summary>
        /// Id (GUID) do log gravado em tb_labs_log_rank_candidates_ids (quando contexto de log foi informado).
        /// </summary>
        public Guid? IdLogRankCandidatesIds { get; set; }
    }
}
