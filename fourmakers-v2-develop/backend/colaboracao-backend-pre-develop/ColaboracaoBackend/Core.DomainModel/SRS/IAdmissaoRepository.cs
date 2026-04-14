using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain.SRS.Admissao;

namespace Core.Domain.SRS;

public interface IAdmissaoRepository
{
    Task<IEnumerable<AdmissaoResult>> ListarAsync(int orgId, bool somenteAtivos = true, int? cursor = null, int? limite = null, Guid? pipelineId = null, Guid? statusId = null);
    Task<AdmissaoResult> ObterPorIdAsync(Guid id, int orgId);
    Task<Guid> InserirAsync(AdmissaoInput input, string alteradorCpf, int orgId);
    Task<bool> AtualizarAsync(Guid id, AdmissaoUpdateInput input, string alteradorCpf, int orgId);
    Task<bool> InativarAsync(Guid id, string alteradorCpf, int orgId);

    /// <summary>Atualiza somente o status corrente da admissão (usado ao registrar movimentação).</summary>
    Task<bool> AtualizarStatusAsync(Guid id, Guid novoStatusId, string alteradorCpf, int orgId);
}
