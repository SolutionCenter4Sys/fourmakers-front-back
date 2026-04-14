using DataTransferObject.Domain.Historico;
using DataTransferObject.Domain.SRS;
using DataTransferObject.Domain.SRS.Candidate;
using DataTransferObject.Domain.SRS.Cargos;
using DataTransferObject.Domain.SRS.LocalDeTrabalho;
using DataTransferObject.Domain.SRS.Offboarding;
using DataTransferObject.Domain.SRS.Onboarding;
using DataTransferObject.Domain.SRS.Tecnica;
using DataTransferObject.Domain.SRS.Vagas;
using DataTransferObject.Domain.Vaga;
using DataTransferObject.Domain.VagasSRS;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SRS.Infra.Interfaces
{
    public interface ISRSInfraClient
    {
        InsertCandidateDTO GetCandidate(string cpf);
        InsertCandidateDTO GetCandidate(int candidateId);
        int? GetCandidateId(string linkedin);
        Task<List<VagaDTO>> GetVagaInfra();

        void InsertCandidate(SRSInsertCandidateParam param, string cpf);

        bool ValidaSeJaExisteEmail(string email);

        void EntrevistaTecnicaInsert(EntrevistaParam param);

        void EntrevistaRHInsert(EntrevistaParam param);

        List<EntrevistaDTO> GetEntrevista(int candidateId);

        List<SoEntrevistaDTO> GetSoEntrevista(int candidateId);

        List<SoEntrevistaIdDTO> GetSoEntrevistaId(int entrevistaId, int candidateId);

        void EditarEntrevista(EntrevistaDTO param);

        void EntrevistaClienteGestorInsert(EntrevistaParam param);

        int insertOffboarding(OffboardingDTO param);

        void EditarOffboarding(OffboardingDTO param);

        List<OffboardingDTO> GetOffboarding(int candidateId);

        void InsertSkillCandidate(CandidateSkillParam param);
        void InsertCandidateSkillEntrevista(EntrevistaSkillParam param);

        void EditarSkillCandidate(CandidateSkillDTO param);

        void RemoveSkillCandidate(int skillId);

        List<CandidateSkillDTO> GetCandidateSkill(int candidate_id);

        List<CandidateSkillDTO> GetCandidateSkillEntrevista(int candidateId, int entrevistaId);

        void InsertOnboarding(OnboardingParam param);

        List<OnboardingDTO> GetOnboarding(int candidate_id);
        List<HistoricoCandidateEntrevistaDTO> GetHistoricoCandidatoEntrevistas(int candidateId);
        HistoricoCandidateEntrevistaDTO GetHistoricoCandidatoEntrevistaEspecifica(int id);
        void HistoricoCandidatoEntrevista(HistoricoCandidateEntrevistaDTO param);

        void EditarOnboarding(OnboardingDTO param);

        void RemoveOnboarding(int onboardingId);

        List<CargoSimplesDTO> GetCargoSimples();

        CargoDTO GetCargo(int cargoId);

        List<LocalDeTrabalhoDTO> GetLocalDeTrabalho();

        int UnidadeFourmakersToSRS(int idUnidade);

        Task<CriarVagasSRSParam> CriarOuAtualizarVagaFourmakersSRS(CriarVagasSRSParam param);

        int? BuscaUserId(string email);

        Task<CriarVagasParam> EditarVagasFourmakersSRS(CriarVagasParam param);

        Task<SkillVagaParam> EditarSkillVagaFourmakersSRS(SkillVagaParam param);

        Task<SkillVagaParam> InserirSkillVagaFourmakersSRS(SkillVagaParam param);

        Task<SkillVagaParam> RemoverSkillVagaFourmakersSRS(int skillId);

        Task<List<SkillVagaParam>> BuscaSkillsVagaFourmakersSRS(int vagaId);

        Task<AlterarStatusCandidaturaDTO> AlterarStatusCandidatura(AlterarStatusCandidaturaParam status, string nomeAnalista);

        string BuscaNomeAnalista(string email);
        Task<int> InsertCandidateEntrevistaAsync(EntrevistaParam param);

        void InsertHistoricoCandidato(HistoricoDTO param);

        Task<T> GenericPowerAutomateRequest<T>(HttpMethod method, string uri, FormUrlEncodedContent content, string token = null);
        Task<T> GenericPowerAutomateRequestWithBody<T>(HttpMethod method, string uri, object bodyRequest, string token = null);
        void InsertCandidateLinkedin(SRSInsertCandidateParam param);
        int GetCandidateIdByVanityName(string vanityName);
        List<CandidateRelatorioBI> GetRelatorioCandidatos();
        void InsertCandidateCurriculum(int candidateId, string headline, string summary, string geolationname, DateTime dataCreated, DateTime dateModified);
        void UpdateCandidateCurriculum(int candidateId, string headline, string summary, string geolationname, DateTime dateModified);
        void InsertCandidateCurriculumCertification(int candidateId, string certificateName, string authority, DateTime? dateStarted, DateTime? dateEnded, DateTime dataCreated, DateTime dateModified);
        List<string> GetCandidateCurriculumCertification(int candidateId);
        void InsertCandidateCurriculumCompany(int candidateId, string companyName, string description, string title, string locationName, DateTime? dateStarted, DateTime? dateEnded, bool empresaAtual, DateTime dataCreated, DateTime dateModified);
        List<string> GetCandidateCurriculumCompany(int candidateId);
        void InsertCandidateCurriculumDegree(int candidateId, string degreeName, string fieldStudy, string schoolName, DateTime? dateStarted, DateTime? dateEnded, DateTime dataCreated, DateTime dateModified);
        List<string> GetCandidateCurriculumDegree(int candidateId);
        Task<bool> AlterarCategoriaHabilidade(SRSAlterarCategoriaHabilidadeParam param);
        Task<Usuario> ValidarUsuarioRepository(int? userId, string userName);
        int SendToLogSqsQueueLog(SRSInsertCandidateParamSQS candidateParamSQS);
        Task<CriarVagasSRSParam> ObterVagaPorId(int vagaId);
        void UpdateLogStatus(int idLog, string status);
        int VerificaLogExistenteCandidateID(int candidateId);
        Task<List<JobOrderDTO>> BuscarVagasDoUltimoAno();
    }
}