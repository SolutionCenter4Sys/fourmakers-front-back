using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.SRS.AdmissaoCargo;

namespace Admissao.Domain.Interfaces.Service;

public interface IAdmissaoCargoService
{
    Task<ApiGenericResult<List<AdmissaoCargoResult>>> ListarAsync(string cpf, int orgId, bool somenteAtivos = true, int? cursor = null, int? limite = null, string busca = null);
    Task<ApiGenericResult<List<AdmissaoCargoResult>>> ListarComRemuneracaoCadastradaAsync(string cpf, int orgId, bool somenteAtivos = true, int? cursor = null, int? limite = null, string busca = null);
    Task<ApiGenericResult<AdmissaoCargoResult>> ObterPorIdAsync(string cpf, int orgId, Guid id);
    Task<ApiGenericResult<AdmissaoCargoResult>> InserirAsync(string cpf, int orgId, AdmissaoCargoInput input);
    Task<ApiGenericResult<AdmissaoCargoResult>> AtualizarAsync(string cpf, int orgId, Guid id, AdmissaoCargoInput input);
}
