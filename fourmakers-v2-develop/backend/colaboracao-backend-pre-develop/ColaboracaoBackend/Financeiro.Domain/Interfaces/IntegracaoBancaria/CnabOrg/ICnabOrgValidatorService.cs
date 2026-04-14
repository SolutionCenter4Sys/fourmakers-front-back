using Colaboracao.Helper.Enum;
using DataTransferObject.Domain.Financeiro.IntegracaoBancaria.CnabOrg;
using System.Threading.Tasks;

namespace Financeiro.Domain.Interfaces.IntegracaoBancaria.CnabOrg
{
    public interface ICnabOrgValidatorService
    {
        Task ValidaCnabOrg(CnabOrgInput cnabOrgInput, CRUDEnum create);
    }
}
