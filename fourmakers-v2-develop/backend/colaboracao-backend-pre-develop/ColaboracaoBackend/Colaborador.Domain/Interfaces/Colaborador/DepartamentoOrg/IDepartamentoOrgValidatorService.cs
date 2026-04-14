using Colaboracao.Helper.Enum;
using DataTransferObject.Domain.Colaborador.DepartamentoOrg;
using System.Threading.Tasks;

namespace Colaborador.Domain.Interfaces.Colaborador.DepartamentoOrg
{
    public interface IDepartamentoOrgValidatorService
    {
        Task ValidaDepartamentoOrg(DepartamentoOrgInput departamentoOrgInput, CRUDEnum create);
    }
}