using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.SRS.AdmissaoPipeline;

namespace Admissao.Domain.Interfaces.Service;

public interface IAdmissaoPipelineService
{
    Task<ApiGenericResult<List<AdmissaoPipelineResult>>> ListarAsync(string cpf, int orgId, bool somenteAtivos = true, int? cursor = null, int? limite = null, string busca = null);
    Task<ApiGenericResult<List<AdmissaoPipelineSummaryResult>>> ListarSummaryAsync(string cpf, int orgId, bool somenteAtivos = true);
    Task<ApiGenericResult<AdmissaoPipelineResult>> ObterPorIdAsync(string cpf, int orgId, Guid id);
    Task<ApiGenericResult<AdmissaoPipelineResult>> InserirAsync(string cpf, int orgId, AdmissaoPipelineInput input);
    Task<ApiGenericResult<AdmissaoPipelineResult>> AtualizarAsync(string cpf, int orgId, Guid id, AdmissaoPipelineAtualizarInput input);
    Task<ApiGenericResult<object>> InativarAsync(string cpf, int orgId, Guid id);
}
