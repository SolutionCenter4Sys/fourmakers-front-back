using System;
using System.Threading.Tasks;

namespace Core.Domain.Labs
{
    /// <summary>
    /// Repositório para feedback do usuário sobre qual match preferiu (RankCandidatesIds vs Match Semântico).
    /// </summary>
    public interface ILabsFeedbackMatchSemanticoRepository
    {
        Task InserirAsync(Guid id, Guid idLogRankCandidatesIds, Guid idLogMatchSemantico, bool matchSemanticoMelhor, int tbOrgId, string? codigoInternoColaborador);

        /// <summary>
        /// Contagens por preferência: antigo = RankCandidatesIds (match_semantico_melhor = 0), novo = Match Semântico (= 1).
        /// </summary>
        Task<(int PreferiuMetodoAntigo, int PreferiuMetodoNovo)> ObterContagensPreferenciaPorOrgAsync(int tbOrgId);
    }
}
