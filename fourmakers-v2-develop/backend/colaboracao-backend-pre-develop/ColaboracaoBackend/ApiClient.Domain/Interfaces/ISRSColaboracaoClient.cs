using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.SRS;
using DataTransferObject.Domain.SRS.Candidate;
using DataTransferObject.Domain.SRS.Vagas;
using DataTransferObject.Domain.Vaga;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiClient.Domain.Interfaces
{
    public interface ISRSColaboracaoClient
    {
        Task<List<VagaDTO>> BuscarVagaSRS(string token);
        Task<CriarVagasSRSParam> EditarCriarVaga(string token, CriarVagasSRSParam param);
        Task<CriarVagasSRSParam> ObterVagaPorId(string token, int vagaId);
        Task<bool> GetColaboradorAtivoOuInativoPorCPF(string colaboradorCpf, string token);
        Task<List<CandidateRelatorioBI>> GetRelatorioCandidatosRaw(string token);
        Task<ApiGenericResultInteger> CadastroCandidatoFourmakersLinkedin(string token, CadastroCandidatoLinkedinInput param);
    }
}