using DataTransferObject.Domain.SRS;
using DataTransferObject.Domain.SRS.Candidate;
using DataTransferObject.Domain.SRS.Cargos;
using DataTransferObject.Domain.SRS.LocalDeTrabalho;
using DataTransferObject.Domain.SRS.Offboarding;
using DataTransferObject.Domain.SRS.Onboarding;
using DataTransferObject.Domain.SRS.Tecnica;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SRS.Domain.Interfaces.Service
{
    public interface ISRSCandidateService
    {
        InsertCandidateDTO GetCandidate(string cpf);
        void InsertCandidate(SRSInsertCandidateParam param);
        bool ValidaSeJaExisteEmail(List<string> emails);
        void InsertEntrevistaTecnica(EntrevistaParam param);
        void EntrevistaRHInsert(EntrevistaParam param);
        List<EntrevistaDTO> GetEntrevista(int candidateId);
        List<SoEntrevistaDTO> GetSoEntrevista(int candidateId);
        List<SoEntrevistaIdDTO> GetSoEntrevistaId(int entrevistaId, int candidateId);
        void EditarEntrevista(EntrevistaDTO param);
        void InsertEntrevistaClienteGestor(EntrevistaParam param);
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
        void HistoricoCandidatoEntrevista(HistoricoCandidateEntrevistaDTO historico);
        HistoricoCandidateEntrevistaDTO GetHistoricoCandidatoEntrevistaEspecifica(int id);
        void EditarOnboarding(OnboardingDTO param);
        void RemoveOnboarding(int onboardingId);
        public List<CargoSimplesDTO> GetCargoSimples();
        public CargoDTO GetCargo(int cargoId);
        public List<LocalDeTrabalhoDTO> GetLocalDeTrabalho();
        public int UnidadeFourmakersToSRS(int idUnidade);
        public Task<AlterarStatusCandidaturaDTO> AlterarStatusCandidatura(AlterarStatusCandidaturaParam status, string email);
        Task<int> InsertCandidateEntrevista(EntrevistaParam param);

        Task<CadastroCandidatoInputResponse> CadastroCandidatoLinkedin(CadastroCandidatoInput request);
        Task<int> CadastroCandidatoFourmakersLinkedin(CadastroCandidatoLinkedinInput request);
        Task<AtualizaCadastroCandidatoInputResponse> AtualizaCurriculoCandidate(AtualizaCadastroCandidatoInput candidateId);
        List<CandidateRelatorioBI> GetRelatorioCandidate(string token);
        Task<CadastroCandidatoInputResponse> CadastrarAtualizarPreAprovarCandidadoService(CadastrarAtualizarPreAprovarCandidadoLinkedinInput model);

        void EnviarBancoTalentoFourmakers(string urlLinkedin, string codVaga, string email, string telefone);
    }
}