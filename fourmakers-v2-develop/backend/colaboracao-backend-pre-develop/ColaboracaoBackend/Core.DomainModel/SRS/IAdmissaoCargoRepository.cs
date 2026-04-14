using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain.SRS.AdmissaoCargo;

namespace Core.Domain.SRS;

public interface IAdmissaoCargoRepository
{
    Task<IEnumerable<AdmissaoCargoResult>> ListarAsync(int orgId, bool somenteAtivos = true, int? cursor = null, int? limite = null, string busca = null);
    /// <summary>Cargos da org com registro em tb_admissao_remuneracao_clt.</summary>
    Task<IEnumerable<AdmissaoCargoResult>> ListarComRemuneracaoCadastradaAsync(int orgId, bool somenteAtivos = true, int? cursor = null, int? limite = null, string busca = null);
    Task<AdmissaoCargoResult> ObterPorIdAsync(Guid id, int orgId);
    Task<AdmissaoCargoResult> ObterPorDescricaoAsync(string descricao, int orgId);
    Task<bool> ExisteAtivoPorIdEOrgAsync(Guid id, int orgId);
    Task<Guid> InserirAsync(AdmissaoCargoInput input, string alteradorCpf, int orgId);
    Task<bool> AtualizarAsync(Guid id, AdmissaoCargoInput input, string alteradorCpf, int orgId);
}
