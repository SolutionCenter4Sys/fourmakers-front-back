using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colaboracao.Core;
using Core.Domain.SRS;
using Core.Domain.Usuario.Permissao;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Log;
using DataTransferObject.Domain.SRS.RemuneracaoClt;
using DataTransferObject.Domain.Usuario;
using Logs.Infra.Attributes;
using Admissao.Domain.Interfaces.Service;

namespace Admissao.Domain.Impl.Service;

[LogDomainClass]
public class RemuneracaoCltService : IRemuneracaoCltService
{
    private readonly IRemuneracaoCltRepository _repository;
    private readonly IAdmissaoCargoRepository _admissaoCargoRepository;
    private readonly ICboRepository _cboRepository;
    private readonly IFuncionalidadeSistemaRepository _funcionalidadeSistemaRepository;
    private readonly ILogCore _log;

    public RemuneracaoCltService(
        IRemuneracaoCltRepository repository,
        IAdmissaoCargoRepository admissaoCargoRepository,
        ICboRepository cboRepository,
        IFuncionalidadeSistemaRepository funcionalidadeSistemaRepository,
        ILogCore log)
    {
        _repository = repository;
        _admissaoCargoRepository = admissaoCargoRepository;
        _cboRepository = cboRepository;
        _funcionalidadeSistemaRepository = funcionalidadeSistemaRepository;
        _log = log;
    }

    /// <summary>Evita erro de FK: valida tb_admissao_cargo e tb_admissao_cbo (se informado).</summary>
    private async Task<string> ValidarExistenciaCargoECboAsync(RemuneracaoCltInput input, int orgId)
    {
        var cargo = await _admissaoCargoRepository.ObterPorIdAsync(input.AdmissaoCargoId, orgId);
        if (cargo == null)
            return "Cargo de admissão não encontrado (verifique o id em tb_admissao_cargo).";

        if (input.CboId.HasValue && input.CboId.Value != Guid.Empty)
        {
            var cbo = await _cboRepository.ObterPorIdAsync(input.CboId.Value, orgId);
            if (cbo == null)
                return "CBO não encontrado (verifique o id em tb_admissao_cbo).";
        }

        return null;
    }

    private async Task ValidarAcessoListarAsync(string cpf, int orgId)
    {
        if (!await _funcionalidadeSistemaRepository.ValidaAcessoFuncionalidade(cpf, orgId, FuncionalidadeSistemaEnum.ADMISSAO_REMUNERACAO_CLT_LISTAR))
            throw new UnauthorizedAccessException("Acesso negado. Funcionalidade ADMISSAO_REMUNERACAO_CLT_LISTAR necessária.");
    }

    private async Task ValidarAcessoCriarEditarAsync(string cpf, int orgId)
    {
        if (!await _funcionalidadeSistemaRepository.ValidaAcessoFuncionalidade(cpf, orgId, FuncionalidadeSistemaEnum.ADMISSAO_REMUNERACAO_CLT_CRIAR_EDITAR))
            throw new UnauthorizedAccessException("Acesso negado. Funcionalidade ADMISSAO_REMUNERACAO_CLT_CRIAR_EDITAR necessária.");
    }

    public async Task<ApiGenericResult<List<RemuneracaoCltResult>>> ListarAsync(string cpf, int orgId, bool somenteAtivos = true, int? cursor = null, int? limite = null, Guid? admissaoCargoId = null)
    {
        var result = new ApiGenericResult<List<RemuneracaoCltResult>>();
        try
        {
            await ValidarAcessoListarAsync(cpf, orgId);
            var lista = await _repository.ListarAsync(somenteAtivos, cursor, limite, admissaoCargoId);
            result.Retorno = lista?.ToList() ?? new List<RemuneracaoCltResult>();
            result.Sucesso = true;
            return result;
        }
        catch (UnauthorizedAccessException) { throw; }
        catch (Exception ex)
        {
            _log.Log($"Erro ao listar Remuneração CLT: {ex.Message}", LevelsEnum.Error);
            result.Sucesso = false;
            result.Mensagem = "Erro ao listar Remuneração CLT.";
            return result;
        }
    }

    public async Task<ApiGenericResult<RemuneracaoCltResult>> ObterPorIdAsync(string cpf, int orgId, Guid id)
    {
        var result = new ApiGenericResult<RemuneracaoCltResult>();
        try
        {
            await ValidarAcessoListarAsync(cpf, orgId);
            if (id == Guid.Empty) { result.Sucesso = false; result.Mensagem = "ID inválido."; return result; }
            var item = await _repository.ObterPorIdAsync(id);
            if (item == null) { result.Sucesso = false; result.Mensagem = "Remuneração CLT não encontrada."; return result; }
            result.Retorno = item;
            result.Sucesso = true;
            return result;
        }
        catch (UnauthorizedAccessException) { throw; }
        catch (Exception ex)
        {
            _log.Log($"Erro ao obter Remuneração CLT: {ex.Message}", LevelsEnum.Error);
            result.Sucesso = false;
            result.Mensagem = "Erro ao obter Remuneração CLT.";
            return result;
        }
    }

    public async Task<ApiGenericResult<RemuneracaoCltResult>> ObterPorAdmissaoCargoIdAsync(string cpf, int orgId, Guid admissaoCargoId)
    {
        var result = new ApiGenericResult<RemuneracaoCltResult>();
        try
        {
            await ValidarAcessoListarAsync(cpf, orgId);
            if (admissaoCargoId == Guid.Empty) { result.Sucesso = false; result.Mensagem = "AdmissaoCargoId inválido."; return result; }
            var item = await _repository.ObterPorAdmissaoCargoIdAsync(admissaoCargoId);
            if (item == null) { result.Sucesso = false; result.Mensagem = "Remuneração CLT não encontrada para este cargo de admissão."; return result; }
            result.Retorno = item;
            result.Sucesso = true;
            return result;
        }
        catch (UnauthorizedAccessException) { throw; }
        catch (Exception ex)
        {
            _log.Log($"Erro ao obter Remuneração CLT por cargo: {ex.Message}", LevelsEnum.Error);
            result.Sucesso = false;
            result.Mensagem = "Erro ao obter Remuneração CLT.";
            return result;
        }
    }

    public async Task<ApiGenericResult<RemuneracaoCltResult>> InserirAsync(string cpf, int orgId, RemuneracaoCltInput input)
    {
        var result = new ApiGenericResult<RemuneracaoCltResult>();
        try
        {
            await ValidarAcessoCriarEditarAsync(cpf, orgId);
            if (input == null) { result.Sucesso = false; result.Mensagem = "Dados são obrigatórios."; return result; }
            if (input.AdmissaoCargoId == Guid.Empty) { result.Sucesso = false; result.Mensagem = "Cargo de admissão é obrigatório."; return result; }
            var msgFk = await ValidarExistenciaCargoECboAsync(input, orgId);
            if (msgFk != null) { result.Sucesso = false; result.Mensagem = msgFk; return result; }
            var existente = await _repository.ObterPorAdmissaoCargoIdAsync(input.AdmissaoCargoId);
            if (existente != null) { result.Sucesso = false; result.Mensagem = "Já existe Remuneração CLT para este cargo."; return result; }
            var id = await _repository.InserirAsync(input, cpf);
            var criado = await _repository.ObterPorIdAsync(id);
            result.Retorno = criado;
            result.Sucesso = true;
            result.Mensagem = "Remuneração CLT criada com sucesso.";
            return result;
        }
        catch (UnauthorizedAccessException) { throw; }
        catch (Exception ex)
        {
            _log.Log($"Erro ao inserir Remuneração CLT: {ex.Message}", LevelsEnum.Error);
            result.Sucesso = false;
            result.Mensagem = "Erro ao criar Remuneração CLT.";
            return result;
        }
    }

    public async Task<ApiGenericResult<RemuneracaoCltResult>> AtualizarAsync(string cpf, int orgId, Guid id, RemuneracaoCltInput input)
    {
        var result = new ApiGenericResult<RemuneracaoCltResult>();
        try
        {
            await ValidarAcessoCriarEditarAsync(cpf, orgId);
            if (id == Guid.Empty) { result.Sucesso = false; result.Mensagem = "ID inválido."; return result; }
            if (input == null) { result.Sucesso = false; result.Mensagem = "Dados são obrigatórios."; return result; }
            if (input.AdmissaoCargoId == Guid.Empty) { result.Sucesso = false; result.Mensagem = "Cargo de admissão é obrigatório."; return result; }
            var existente = await _repository.ObterPorIdAsync(id);
            if (existente == null) { result.Sucesso = false; result.Mensagem = "Remuneração CLT não encontrada."; return result; }
            var msgFk = await ValidarExistenciaCargoECboAsync(input, orgId);
            if (msgFk != null) { result.Sucesso = false; result.Mensagem = msgFk; return result; }
            var outroComMesmoCargo = await _repository.ObterPorAdmissaoCargoIdAsync(input.AdmissaoCargoId);
            if (outroComMesmoCargo != null && outroComMesmoCargo.Id != id) { result.Sucesso = false; result.Mensagem = "Já existe Remuneração CLT para este cargo."; return result; }
            var atualizou = await _repository.AtualizarAsync(id, input, cpf);
            if (!atualizou) { result.Sucesso = false; result.Mensagem = "Não foi possível atualizar."; return result; }
            result.Retorno = await _repository.ObterPorIdAsync(id);
            result.Sucesso = true;
            result.Mensagem = "Remuneração CLT atualizada com sucesso.";
            return result;
        }
        catch (UnauthorizedAccessException) { throw; }
        catch (Exception ex)
        {
            _log.Log($"Erro ao atualizar Remuneração CLT: {ex.Message}", LevelsEnum.Error);
            result.Sucesso = false;
            result.Mensagem = "Erro ao atualizar Remuneração CLT.";
            return result;
        }
    }
}
