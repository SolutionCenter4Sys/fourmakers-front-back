using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Financeiro.IntegracaoBancaria.CnabOrg;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Financeiro.Domain.Interfaces.IntegracaoBancaria.CnabOrg
{
    public interface ICnabOrgService
    {
        Task<ApiGenericResult<IEnumerable<CnabOrgResult>>> ListarCnabOrgs(string cpfRequest, int orgId);
        Task<ApiGenericResult<CnabOrgResult>> ObterCnabOrgPorId(Guid id, string CpfRequest, int orgId);
        Task<ApiGenericResult<CnabOrgResult>> InserirCnabOrg(CnabOrgInput cnabOrgInput, string cpfRequest, int orgId);
        Task<ApiGenericResult<CnabOrgResult>> AtualizarCnabOrg(CnabOrgInput cnabOrgInput, Guid id, string cpfRequest, int orgId);
        Task<ApiGenericResult> DeletarCnabOrg(Guid id, string cpfRequest, int orgId);
        Task<ApiGenericResult<IEnumerable<DiretoriaResultDTO>>> ListarDiretoriasDisponiveis(string cpfRequest, int orgId);
    }
}
