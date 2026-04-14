using Colaboracao.Helper.Enum;
using DataTransferObject.Domain.SRS.Candidate;
using DataTransferObject.Domain.Vaga;
using System.Threading.Tasks;

namespace Colaborador.Domain.Interfaces.Validadores
{
    public interface IImportacaoCurriculoValidatorService
    {
        Task ValidaImportarColaboradorLinkedin(string perfilIN, int orgId, string codigoInternoColaboradorCadastrante);
    }
}