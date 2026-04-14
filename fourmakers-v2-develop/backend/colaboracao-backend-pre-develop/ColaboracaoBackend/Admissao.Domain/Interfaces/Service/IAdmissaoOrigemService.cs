using System;
using System.Threading.Tasks;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.SRS.AdmissaoOrigem;

namespace Admissao.Domain.Interfaces.Service;

public interface IAdmissaoOrigemService
{
    /// <summary>
    /// Vincula (ou substitui o vínculo de) um processo de admissão a uma vaga ou candidatura.
    /// Chamado pelo módulo de recrutamento após aprovar uma candidatura ou iniciar admissão por vaga.
    /// </summary>
    Task<ApiGenericResult<AdmissaoOrigemResult>> VincularAsync(string cpf, int orgId, Guid admissaoId, AdmissaoOrigemInput input);

    /// <summary>Retorna o vínculo de origem de uma admissão, ou <c>null</c> no retorno se não houver.</summary>
    Task<ApiGenericResult<AdmissaoOrigemResult>> ObterVinculoAsync(string cpf, int orgId, Guid admissaoId);
}
