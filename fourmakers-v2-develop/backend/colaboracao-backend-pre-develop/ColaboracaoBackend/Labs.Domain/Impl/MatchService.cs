using ApiClient.Domain.Interfaces;
using Colaboracao.Core;
using Core.Domain.Labs;
using DataTransferObject.Domain.Labs;
using DataTransferObject.Domain.Log;
using DataTransferObject.Domain.Match;
using Labs.Domain.Interfaces;
using Logs.Infra;
using Logs.Infra.Attributes;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace Labs.Domain.Impl
{
    /// <summary>
    /// Serviço de domínio que delega ao MatchClient (RankCandidates, ScoreSingleCandidate, RankCandidatesIds).
    /// </summary>
    [LogDomainClass]
    public class MatchService : IMatchService
    {
        private readonly IMatchClient _matchClient;
        private readonly ILabsLogRankCandidatesIdsRepository _logRankCandidatesIdsRepository;
        private readonly ILabsLogScoreSingleCandidatesRepository _logScoreSingleCandidatesRepository;
        private readonly ILogCore _log;

        public MatchService(IMatchClient matchClient, ILabsLogRankCandidatesIdsRepository logRankCandidatesIdsRepository, ILabsLogScoreSingleCandidatesRepository logScoreSingleCandidatesRepository, ILogCore log)
        {
            _matchClient = matchClient;
            _logRankCandidatesIdsRepository = logRankCandidatesIdsRepository;
            _logScoreSingleCandidatesRepository = logScoreSingleCandidatesRepository;
            _log = log;
        }

        public Task<List<CandidatosMatchResponse>> RankCandidates(CandidatosMatchRequest request)
            => _matchClient.RankCandidates(request);

        public async Task<ScoreSingleCandidateResult> ScoreSingleCandidate(ScoreSingleCandidateRequest request, ScoreSingleCandidateLogContext? logContext = null)
        {
            var response = await _matchClient.ScoreSingleCandidate(request);
            Guid? idLog = null;

            if (logContext != null)
            {
                try
                {
                    idLog = await _logScoreSingleCandidatesRepository.InserirAsync(
                        logContext.OrgId,
                        logContext.VagaId,
                        logContext.CodigoInternoColaborador,
                        JsonSerializer.Serialize(request),
                        JsonSerializer.Serialize(response));
                }
                catch (Exception ex)
                {
                    _log.Log("Erro ao gravar log Match ScoreSingleCandidate", LevelsEnum.Warning);
                    _log.Log(ex.Message, LevelsEnum.Warning);
                }
            }

            return new ScoreSingleCandidateResult { Response = response, IdLogScoreSingleCandidates = idLog };
        }

        public async Task<RankCandidatesIdsResult> RankCandidatesIds(CandidatosMatchRequestIds request, RankCandidatesIdsLogContext? logContext)
        {
            var response = await _matchClient.RankCandidatesIds(request);
            Guid? idLog = null;

            if (logContext != null)
            {
                try
                {
                    idLog = await _logRankCandidatesIdsRepository.InserirAsync(
                        logContext.OrgId,
                        logContext.VagaId,
                        logContext.CodigoInternoColaborador,
                        JsonSerializer.Serialize(request),
                        JsonSerializer.Serialize(response));
                }
                catch (System.Exception ex)
                {
                    _log.Log("Erro ao gravar log Match RankCandidatesIds", LevelsEnum.Warning);
                    _log.Log(ex.Message, LevelsEnum.Warning);
                }
            }

            return new RankCandidatesIdsResult { Candidates = response, IdLogRankCandidatesIds = idLog };
        }
    }
}
