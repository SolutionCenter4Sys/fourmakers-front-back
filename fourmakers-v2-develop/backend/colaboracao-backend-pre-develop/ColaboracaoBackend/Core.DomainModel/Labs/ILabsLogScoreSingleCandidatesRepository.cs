using System;
using System.Threading.Tasks;

namespace Core.Domain.Labs
{
    /// <summary>
    /// Repositório para log de chamadas ao ScoreSingleCandidate (Match).
    /// </summary>
    public interface ILabsLogScoreSingleCandidatesRepository
    {
        /// <summary>
        /// Insere o log e retorna o id (GUID) do registro (tb_labs_log_score_single_candidates.id).
        /// </summary>
        Task<Guid> InserirAsync(int tbOrgId, string? idVaga, string? codigoInternoColaborador, string? objetoRequest, string? objetoResponse);

        /// <summary>
        /// Verifica se existe registro com o id informado.
        /// </summary>
        Task<bool> ExisteAsync(Guid id);
    }
}
