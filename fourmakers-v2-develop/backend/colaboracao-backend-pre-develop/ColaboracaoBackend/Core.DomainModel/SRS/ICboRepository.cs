using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain.SRS.Cbo;

namespace Core.Domain.SRS;

public interface ICboRepository
{
    Task<IEnumerable<CboResult>> ListarAsync(int orgId, bool somenteAtivos = true, int? cursor = null, int? limite = null, string busca = null);
    Task<CboResult> ObterPorIdAsync(Guid id, int orgId);
    Task<CboResult> ObterPorCodigoAsync(string codigo, int orgId);
    Task<Guid> InserirAsync(CboInput input, string alteradorCpf, int orgId);
    Task<bool> AtualizarAsync(Guid id, CboInput input, string alteradorCpf, int orgId);
}
