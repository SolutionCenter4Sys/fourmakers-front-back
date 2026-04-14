using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain.SRS.AdmissaoStatus;

namespace Core.Domain.SRS;

public interface IAdmissaoStatusRepository
{
    Task<IEnumerable<AdmissaoStatusResult>> ListarAsync(int orgId, bool somenteAtivos = true, int? cursor = null, int? limite = null, string busca = null);
    Task<AdmissaoStatusResult> ObterPorIdAsync(Guid id, int orgId);
    Task<AdmissaoStatusResult> ObterPorDescricaoAsync(string descricao, int orgId);
    Task<AdmissaoStatusResult> ObterPorCodigoNaOrgAsync(int codigo, int orgId);
    Task<Guid> InserirAsync(AdmissaoStatusInput input, string alteradorCpf, int orgId);
    Task<bool> AtualizarAsync(Guid id, AdmissaoStatusInput input, string alteradorCpf, int orgId);
    /// <summary>Inativa o registro (ativo = 0) e grava log DELETE.</summary>
    Task<bool> ExcluirLogicamenteAsync(Guid id, string alteradorCpf, int orgId);
}
