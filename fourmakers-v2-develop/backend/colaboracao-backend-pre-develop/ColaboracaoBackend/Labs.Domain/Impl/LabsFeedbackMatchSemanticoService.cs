using Core.Domain.Labs;
using DataTransferObject.Domain.Labs;
using Labs.Domain.Interfaces;
using Logs.Infra.Attributes;
using System;
using System.Threading.Tasks;

namespace Labs.Domain.Impl
{
    /// <summary>
    /// Serviço para registrar feedback do usuário sobre qual resultado de match preferiu.
    /// </summary>
    [LogDomainClass]
    public class LabsFeedbackMatchSemanticoService : ILabsFeedbackMatchSemanticoService
    {
        private readonly ILabsFeedbackMatchSemanticoRepository _repository;
        private readonly ILabsLogRankCandidatesIdsRepository _logRankCandidatesIdsRepository;
        private readonly ILabsLogMatchSemanticoRepository _logMatchSemanticoRepository;

        public LabsFeedbackMatchSemanticoService(
            ILabsFeedbackMatchSemanticoRepository repository,
            ILabsLogRankCandidatesIdsRepository logRankCandidatesIdsRepository,
            ILabsLogMatchSemanticoRepository logMatchSemanticoRepository)
        {
            _repository = repository;
            _logRankCandidatesIdsRepository = logRankCandidatesIdsRepository;
            _logMatchSemanticoRepository = logMatchSemanticoRepository;
        }

        public async Task RegistrarFeedbackAsync(FeedbackMatchSemanticoRequest request, int orgId, string? codigoInternoColaborador)
        {
            if (request.IdLogRankCandidatesIds == Guid.Empty)
                throw new ApplicationException("IdLogRankCandidatesIds é obrigatório.");
            if (request.IdLogMatchSemantico == Guid.Empty)
                throw new ApplicationException("IdLogMatchSemantico é obrigatório.");

            var existeRank = await _logRankCandidatesIdsRepository.ExisteAsync(request.IdLogRankCandidatesIds);
            if (!existeRank)
                throw new ApplicationException($"Log RankCandidatesIds não encontrado: {request.IdLogRankCandidatesIds}.");

            var existeMatchSemantico = await _logMatchSemanticoRepository.ExisteAsync(request.IdLogMatchSemantico);
            if (!existeMatchSemantico)
                throw new ApplicationException($"Log Match Semântico não encontrado: {request.IdLogMatchSemantico}.");

            var id = Guid.NewGuid();
            await _repository.InserirAsync(
                id,
                request.IdLogRankCandidatesIds,
                request.IdLogMatchSemantico,
                request.MatchSemanticoMelhor,
                orgId,
                codigoInternoColaborador);
        }

        public async Task<MatchSemanticoMetricasDto> ObterMetricasAsync(int orgId)
        {
            var quantidadeMatchsGerados = await _logMatchSemanticoRepository.ContarPorOrgAsync(orgId);
            var (qtdAntigo, qtdNovo) = await _repository.ObterContagensPreferenciaPorOrgAsync(orgId);
            var totalFeedback = qtdAntigo + qtdNovo;

            decimal pctAntigo = 0;
            decimal pctNovo = 0;
            if (totalFeedback > 0)
            {
                pctAntigo = Math.Round((decimal)qtdAntigo * 100m / totalFeedback, 2, MidpointRounding.AwayFromZero);
                pctNovo = Math.Round((decimal)qtdNovo * 100m / totalFeedback, 2, MidpointRounding.AwayFromZero);
            }

            return new MatchSemanticoMetricasDto
            {
                QuantidadeMatchsGerados = quantidadeMatchsGerados,
                PorcentagemPreferenciaMetodoAntigo = pctAntigo,
                PorcentagemPreferenciaMetodoNovo = pctNovo,
                QuantidadeFeedbacksMetodoAntigo = qtdAntigo,
                QuantidadeFeedbacksMetodoNovo = qtdNovo
            };
        }
    }
}
