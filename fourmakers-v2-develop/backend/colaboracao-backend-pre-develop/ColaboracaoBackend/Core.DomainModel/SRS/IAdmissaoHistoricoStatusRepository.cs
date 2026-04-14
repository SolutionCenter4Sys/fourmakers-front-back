using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain.SRS.AdmissaoHistoricoStatus;

namespace Core.Domain.SRS;

public interface IAdmissaoHistoricoStatusRepository
{
    Task<IEnumerable<AdmissaoHistoricoStatusResult>> ListarPorAdmissaoAsync(Guid admissaoId, int orgId, int? cursor = null, int? limite = null);
    Task<AdmissaoHistoricoStatusResult> ObterPorIdAsync(Guid id, int orgId);
    Task<Guid> InserirAsync(AdmissaoHistoricoStatusInput input, Guid admissaoId, Guid? statusOrigemId, string codigoInternoColaborador, int orgId);
}
