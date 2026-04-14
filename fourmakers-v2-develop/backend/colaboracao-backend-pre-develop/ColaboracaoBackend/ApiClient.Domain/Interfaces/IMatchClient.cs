using DataTransferObject.Domain.Match;
using DataTransferObject.Domain.Usuario;
using DataTransferObject.Domain.Vaga;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiClient.Domain.Interfaces
{
    public interface IMatchClient
    {
        Task<List<CandidatosMatchResponse>> RankCandidates(CandidatosMatchRequest request);
        Task<CandidatosMatchResponse> ScoreSingleCandidate(ScoreSingleCandidateRequest request);
        Task<ExtrairPerfilDeUmPromptResponse> ExtrairPerfilDeUmPrompt(ExtrairPerfilDeUmPromptRequest request);
        Task<List<CandidatosMatchResponse>> RankCandidatesIds(CandidatosMatchRequestIds request);
    }
}