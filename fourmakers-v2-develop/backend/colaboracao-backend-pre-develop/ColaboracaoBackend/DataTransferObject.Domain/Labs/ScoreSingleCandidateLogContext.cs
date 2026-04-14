namespace DataTransferObject.Domain.Labs
{
    /// <summary>
    /// Contexto opcional para gravar log ao chamar ScoreSingleCandidate.
    /// Quando informado, o MatchService grava o request/response em tb_labs_log_score_single_candidates.
    /// </summary>
    public class ScoreSingleCandidateLogContext
    {
        public int OrgId { get; set; }
        public string? VagaId { get; set; }
        public string? CodigoInternoColaborador { get; set; }
    }
}
