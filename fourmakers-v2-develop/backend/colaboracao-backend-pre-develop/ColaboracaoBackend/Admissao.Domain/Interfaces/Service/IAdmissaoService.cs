using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.SRS.Admissao;
using DataTransferObject.Domain.SRS.AdmissaoHistoricoStatus;

namespace Admissao.Domain.Interfaces.Service;

public interface IAdmissaoService
{
    Task<ApiGenericResult<List<AdmissaoResult>>> ListarAsync(string cpf, int orgId, bool somenteAtivos = true, int? cursor = null, int? limite = null, Guid? pipelineId = null, Guid? statusId = null);
    Task<ApiGenericResult<AdmissaoResult>> ObterPorIdAsync(string cpf, int orgId, Guid id);
    Task<ApiGenericResult<AdmissaoResult>> InserirAsync(string cpf, int orgId, AdmissaoInput input);
    Task<ApiGenericResult<AdmissaoResult>> AtualizarAsync(string cpf, int orgId, Guid id, AdmissaoUpdateInput input);
    Task<ApiGenericResult<bool>> InativarAsync(string cpf, int orgId, Guid id);

    /// <summary>
    /// Move o status da admissão: atualiza <c>tb_admissao.tb_admissao_status_id</c>
    /// e grava o registro em <c>tb_admissao_historico_status</c>.
    /// O status de origem é lido automaticamente do registro atual.
    /// </summary>
    Task<ApiGenericResult<AdmissaoHistoricoStatusResult>> MoverStatusAsync(string cpf, int orgId, Guid admissaoId, AdmissaoHistoricoStatusInput input);
}
