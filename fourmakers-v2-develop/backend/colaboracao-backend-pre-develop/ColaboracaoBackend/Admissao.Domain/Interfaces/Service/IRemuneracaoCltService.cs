using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.SRS.RemuneracaoClt;

namespace Admissao.Domain.Interfaces.Service;

public interface IRemuneracaoCltService
{
    Task<ApiGenericResult<List<RemuneracaoCltResult>>> ListarAsync(string cpf, int orgId, bool somenteAtivos = true, int? cursor = null, int? limite = null, Guid? admissaoCargoId = null);
    Task<ApiGenericResult<RemuneracaoCltResult>> ObterPorIdAsync(string cpf, int orgId, Guid id);
    Task<ApiGenericResult<RemuneracaoCltResult>> ObterPorAdmissaoCargoIdAsync(string cpf, int orgId, Guid admissaoCargoId);
    Task<ApiGenericResult<RemuneracaoCltResult>> InserirAsync(string cpf, int orgId, RemuneracaoCltInput input);
    Task<ApiGenericResult<RemuneracaoCltResult>> AtualizarAsync(string cpf, int orgId, Guid id, RemuneracaoCltInput input);
}
