using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain.SRS.AdmissaoPipeline;

namespace Core.Domain.SRS;

public interface IAdmissaoPipelineRepository
{
    /// <summary>Lista pipelines; cada <see cref="AdmissaoPipelineResult.StatusItens"/> é preenchido em lote (2 queries).</summary>
    Task<IEnumerable<AdmissaoPipelineResult>> ListarAsync(int orgId, bool somenteAtivos = true, int? cursor = null, int? limite = null, string busca = null);

    /// <summary>Lista pipelines retornando apenas <c>Id</c> e <c>Nome</c> (1 query, sem etapas).</summary>
    Task<IEnumerable<AdmissaoPipelineSummaryResult>> ListarSummaryAsync(int orgId, bool somenteAtivos = true);
    Task<AdmissaoPipelineResult> ObterPorIdAsync(Guid id, int orgId);
    Task<AdmissaoPipelineResult> ObterPorNomeAsync(string nome, int orgId);
    Task<Guid> InserirAsync(AdmissaoPipelineInput input, string alteradorCpf, int orgId);
    Task<bool> AtualizarAsync(Guid id, AdmissaoPipelineAtualizarInput input, string alteradorCpf, int orgId);
    Task<bool> InativarAsync(Guid id, string alteradorCpf, int orgId);
}
