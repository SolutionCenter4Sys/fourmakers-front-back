using System;
using System.ComponentModel.DataAnnotations;

namespace DataTransferObject.Domain.Labs
{
    /// <summary>
    /// Request para registrar feedback do usuário: qual resultado de match preferiu.
    /// </summary>
    public class FeedbackMatchSemanticoRequest
    {
        /// <summary>Id do log em tb_labs_log_rank_candidates_ids (RankCandidatesIds). Obrigatório.</summary>
        [Required(ErrorMessage = "IdLogRankCandidatesIds é obrigatório.")]
        public Guid IdLogRankCandidatesIds { get; set; }

        /// <summary>Id do log em tb_labs_log_match_semantico (Match Semântico / Hyde). Obrigatório.</summary>
        [Required(ErrorMessage = "IdLogMatchSemantico é obrigatório.")]
        public Guid IdLogMatchSemantico { get; set; }

        /// <summary>True = usuário preferiu o Match Semântico; False = preferiu o RankCandidatesIds.</summary>
        public bool MatchSemanticoMelhor { get; set; }
    }
}
