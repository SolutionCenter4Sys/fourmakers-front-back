using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.SRS;
using DataTransferObject.Domain.SRS.Candidate;
using DataTransferObject.Domain.VagasSRS;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiClient.Domain.Interfaces
{
    public interface ISRSClient
    {
        public Task<TokenSRS> Autenticacao();

        public Task<SRSColaboradorResult> GetColaboradorSRS(string email, string tokenUsuario);

        public Task<List<JobOrderDTO>> Requisicao(string tokenUsuario);

        public Task<List<MotivosDTO>> RequisicaoRazao(string tokenUsuario);

        public Task<List<VagasInscritoDTO>> CandidatoInscritoNasVagas(string tokenUsuario, int candidate_id);

        public Task<CandidatarResult> CandidatarSeAUmaVaga(string tokenUsuario, long joborder_id, int candidate_id, string origem);

        //public Task<DescandidatarResult> DescandidatarSe(string tokenUsuario, long joborder_id, string cpf, string origem, int status_reason_cancellation, int reason_cancellation);

        public Task<SRSCandidateResult> GetCandidate(string tokenUsuario, string cpf);

        public Task<SRSCandidatePostDTO> PostCandidate(string tokenUsuario, string cpf, SRSCandidateDTO colabInfoCandidate, List<SRSCandidateContatosEmergenciaDTO> contatos_emergencia);

        Task<DescandidatarResult> DescandidatarSeDeUmaVaga(string tokenUsuario, long joborder_id, int candidate_id, string origem, int status_reason_cancellation, int reason_cancellation);

        Task<ApiGenericResult<bool>> AlterarCategoriaHabilidade(SRSAlterarCategoriaHabilidadeParam param, string tokenUsuario);
    }
}