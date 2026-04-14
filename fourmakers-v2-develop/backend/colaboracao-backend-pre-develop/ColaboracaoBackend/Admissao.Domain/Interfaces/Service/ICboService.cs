using System;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.SRS.Cbo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Admissao.Domain.Interfaces.Service;

public interface ICboService
{
    Task<ApiGenericResult<List<CboResult>>> ListarAsync(string cpf, int orgId, bool somenteAtivos = true, int? cursor = null, int? limite = null, string busca = null);
    Task<ApiGenericResult<CboResult>> ObterPorIdAsync(string cpf, int orgId, Guid id);
    Task<ApiGenericResult<CboResult>> ObterPorCodigoAsync(string cpf, int orgId, string codigo);
    Task<ApiGenericResult<CboResult>> InserirAsync(string cpf, int orgId, CboInput input);
    Task<ApiGenericResult<CboResult>> AtualizarAsync(string cpf, int orgId, Guid id, CboInput input);
}
