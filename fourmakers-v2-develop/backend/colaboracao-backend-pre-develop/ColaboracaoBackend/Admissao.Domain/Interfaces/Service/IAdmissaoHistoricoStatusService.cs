using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.SRS.AdmissaoHistoricoStatus;

namespace Admissao.Domain.Interfaces.Service;

public interface IAdmissaoHistoricoStatusService
{
    Task<ApiGenericResult<List<AdmissaoHistoricoStatusResult>>> ListarPorAdmissaoAsync(string cpf, int orgId, Guid admissaoId, int? cursor = null, int? limite = null);
    Task<ApiGenericResult<AdmissaoHistoricoStatusResult>> ObterPorIdAsync(string cpf, int orgId, Guid id);
}
