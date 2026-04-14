using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain.SRS.RemuneracaoClt;

namespace Core.Domain.SRS;

public interface IRemuneracaoCltRepository
{
    Task<IEnumerable<RemuneracaoCltResult>> ListarAsync(bool somenteAtivos = true, int? cursor = null, int? limite = null, Guid? admissaoCargoId = null);
    Task<RemuneracaoCltResult> ObterPorIdAsync(Guid id);
    Task<RemuneracaoCltResult> ObterPorAdmissaoCargoIdAsync(Guid admissaoCargoId);
    Task<Guid> InserirAsync(RemuneracaoCltInput input, string alteradorCpf);
    Task<bool> AtualizarAsync(Guid id, RemuneracaoCltInput input, string alteradorCpf);
}
