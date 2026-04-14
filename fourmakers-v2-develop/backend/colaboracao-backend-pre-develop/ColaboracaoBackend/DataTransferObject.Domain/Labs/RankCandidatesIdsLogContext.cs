namespace DataTransferObject.Domain.Labs
{
    /// <summary>
    /// Contexto opcional para gravar log ao chamar RankCandidatesIds.
    /// Quando informado, o MatchService grava o request/response em tb_labs_log_rank_candidates_ids.
    /// </summary>
    public class RankCandidatesIdsLogContext
    {
        public int OrgId { get; set; }
        public string? VagaId { get; set; }
        public string? CodigoInternoColaborador { get; set; }
    }
}
