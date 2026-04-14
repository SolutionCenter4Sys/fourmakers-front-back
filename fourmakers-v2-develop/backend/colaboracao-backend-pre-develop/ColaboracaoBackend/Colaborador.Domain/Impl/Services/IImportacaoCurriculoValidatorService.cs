using Colaboracao.Helper.Enum;
using DataTransferObject.Domain.SRS.Candidate;
using DataTransferObject.Domain.Vaga;
using System.Threading.Tasks;

namespace Colaborador.Domain.Interfaces.Validadores
{
    public interface IImportacaoCurriculoValidatorServiceIImportacaoCurriculoValidatorServiceIImportacaoCurriculoValidatorService
    {
        Task ValidaVaga(int vagaId, VagaFourmakersDTO vaga, int orgId, CRUDEnum cRUDEnum);
    }
}