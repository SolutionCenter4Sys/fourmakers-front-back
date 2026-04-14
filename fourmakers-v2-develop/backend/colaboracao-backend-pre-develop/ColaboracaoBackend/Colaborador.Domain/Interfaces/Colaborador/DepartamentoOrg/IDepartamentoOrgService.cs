using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Colaborador.DepartamentoOrg;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Colaborador.Domain.Interfaces.Colaborador.DepartamentoOrg
{
    public interface IDepartamentoOrgService
    {
        Task<ApiGenericResult<IEnumerable<DepartamentoOrgResult>>> ListarDepartamentoOrgs(string cpfRequest, int orgId);
        Task<ApiGenericResult<DepartamentoOrgResult>> ObterDepartamentoOrgPorId(Guid id, string CpfRequest, int orgId);
        Task<ApiGenericResult<DepartamentoOrgResult>> InserirDepartamentoOrg(DepartamentoOrgInput departamentoOrgInput, string cpfRequest, int orgId);
        Task<ApiGenericResult<DepartamentoOrgResult>> AtualizarDepartamentoOrg(DepartamentoOrgInput departamentoOrgInput, Guid id, string cpfRequest, int orgId);
        Task<ApiGenericResult> DeletarDepartamentoOrg(Guid id, string cpfRequest, int orgId);
    }
}