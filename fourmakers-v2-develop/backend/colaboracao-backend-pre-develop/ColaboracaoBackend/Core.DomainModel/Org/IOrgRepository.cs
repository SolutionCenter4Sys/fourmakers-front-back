using DataTransferObject.Domain.Util;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.DomainModel.Org
{
    public interface IOrgRepository
    {
        OrgDTO BuscarOrg(int orgId);
        int? BuscarColaboradorOrgId(string codColaborador);
        List<OrgDTO> GetAllOrgsId(bool ignorarOrgFourmakers);
        Task<string> BuscaSubDominioOrg(int orgId);
    }
}