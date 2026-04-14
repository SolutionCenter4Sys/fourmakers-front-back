using DataTransferObject.Domain.Colaborador.DepartamentoOrg;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.Colaborador.Colaborador
{
    public interface IDepartamentoOrgRepository
    {
        Task<IEnumerable<DepartamentoOrgResult>> ListarDepartamentoOrgsAsync(int orgid);
        Task<DepartamentoOrgResult> ObterDepartamentoOrgPorIdAsync(Guid id);
        Task<DepartamentoOrgResult> ObterDepartamentoOrgPorCodigoAsync(string codigo, int orgId);
        Task<DepartamentoOrgResult> InserirDepartamentoOrgAsync(DepartamentoOrgInput parametroRepositoryInput);
        Task<DepartamentoOrgResult> AtualizarDepartamentoOrgAsync(DepartamentoOrgInput parametroRepositoryInput);
        Task<bool> DeletarDepartamentoOrgAsync(Guid id);
    }
}