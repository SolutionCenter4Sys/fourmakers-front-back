using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.SRS.AdmissaoStatus;

namespace Admissao.Domain.Interfaces.Service;

public interface IAdmissaoStatusService
{
    Task<ApiGenericResult<List<AdmissaoStatusResult>>> ListarAsync(string cpf, int orgId, bool somenteAtivos = true, int? cursor = null, int? limite = null, string busca = null);
    Task<ApiGenericResult<AdmissaoStatusResult>> ObterPorIdAsync(string cpf, int orgId, Guid id);
    Task<ApiGenericResult<AdmissaoStatusResult>> InserirAsync(string cpf, int orgId, AdmissaoStatusInput input);
    Task<ApiGenericResult<AdmissaoStatusResult>> AtualizarAsync(string cpf, int orgId, Guid id, AdmissaoStatusInput input);
    Task<ApiGenericResult<bool>> ExcluirAsync(string cpf, int orgId, Guid id);
}
