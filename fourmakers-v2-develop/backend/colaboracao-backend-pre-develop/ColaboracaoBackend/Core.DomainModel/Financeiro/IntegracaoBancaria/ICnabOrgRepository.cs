using DataTransferObject.Domain.Financeiro.IntegracaoBancaria.CnabOrg;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain.Financeiro.IntegracaoBancaria.RemessaBancaria;

namespace Core.Domain.Financeiro.IntegracaoBancaria
{
    public interface ICnabOrgRepository
    {
        Task<IEnumerable<CnabOrgResult>> ListarCnabOrgsAsync(int orgid);
        Task<CnabOrgResult> ObterCnabOrgPorIdAsync(Guid id);
        Task<CnabOrgResult> ObterCnabOrgPorCodigoAsync(string codigo, int orgId);
        Task<CnabOrgResult> ObterCnabOrgPorChaveUnicaAsync(int orgId, string diretoria, string formaPagamento);
        Task<CnabOrgResult> InserirCnabOrgAsync(CnabOrgInput parametroRepositoryInput);
        Task<CnabOrgResult> AtualizarCnabOrgAsync(CnabOrgInput parametroRepositoryInput);
        Task<bool> DeletarCnabOrgAsync(Guid id);
        Task<IEnumerable<DiretoriaResultDTO>> ListarDiretoriasAsync(int orgId);
    }

}
