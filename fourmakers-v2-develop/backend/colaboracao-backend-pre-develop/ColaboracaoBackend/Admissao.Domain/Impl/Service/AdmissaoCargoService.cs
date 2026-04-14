using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colaboracao.Core;
using Core.Domain.SRS;
using Core.Domain.Usuario.Permissao;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Log;
using DataTransferObject.Domain.SRS.AdmissaoCargo;
using DataTransferObject.Domain.Usuario;
using Logs.Infra.Attributes;
using Admissao.Domain.Interfaces.Service;

namespace Admissao.Domain.Impl.Service;

[LogDomainClass]
public class AdmissaoCargoService : IAdmissaoCargoService
{
    private readonly IAdmissaoCargoRepository _repository;
    private readonly ICboRepository _cboRepository;
    private readonly IFuncionalidadeSistemaRepository _funcionalidadeSistemaRepository;
    private readonly ILogCore _log;

    public AdmissaoCargoService(
        IAdmissaoCargoRepository repository,
        ICboRepository cboRepository,
        IFuncionalidadeSistemaRepository funcionalidadeSistemaRepository,
        ILogCore log)
    {
        _repository = repository;
        _cboRepository = cboRepository;
        _funcionalidadeSistemaRepository = funcionalidadeSistemaRepository;
        _log = log;
    }

    /// <summary>Garante CBO obrigatório e existente em tb_admissao_cbo (evita erro de FK).</summary>
    private async Task<string> ValidarCboObrigatorioAsync(AdmissaoCargoInput input, int orgId)
    {
        if (input == null) return "Dados são obrigatórios.";
        if (input.CboId == Guid.Empty)
            return "É obrigatório informar um CBO cadastrado (cboId).";
        var cbo = await _cboRepository.ObterPorIdAsync(input.CboId, orgId);
        if (cbo == null)
            return "CBO não encontrado (verifique o id em tb_admissao_cbo).";
        return null;
    }

    private async Task ValidarAcessoListarAsync(string cpf, int orgId)
    {
        if (!await _funcionalidadeSistemaRepository.ValidaAcessoFuncionalidade(cpf, orgId, FuncionalidadeSistemaEnum.ADMISSAO_CARGO_LISTAR))
            throw new UnauthorizedAccessException("Acesso negado. Funcionalidade ADMISSAO_CARGO_LISTAR necessária.");
    }

    private async Task ValidarAcessoCriarEditarAsync(string cpf, int orgId)
    {
        if (!await _funcionalidadeSistemaRepository.ValidaAcessoFuncionalidade(cpf, orgId, FuncionalidadeSistemaEnum.ADMISSAO_CARGO_CRIAR_EDITAR))
            throw new UnauthorizedAccessException("Acesso negado. Funcionalidade ADMISSAO_CARGO_CRIAR_EDITAR necessária.");
    }

    public async Task<ApiGenericResult<List<AdmissaoCargoResult>>> ListarAsync(string cpf, int orgId, bool somenteAtivos = true, int? cursor = null, int? limite = null, string busca = null)
    {
        var result = new ApiGenericResult<List<AdmissaoCargoResult>>();
        try
        {
            await ValidarAcessoListarAsync(cpf, orgId);
            var lista = await _repository.ListarAsync(orgId, somenteAtivos, cursor, limite, busca);
            result.Retorno = lista?.ToList() ?? new List<AdmissaoCargoResult>();
            result.Sucesso = true;
            return result;
        }
        catch (UnauthorizedAccessException) { throw; }
        catch (Exception ex)
        {
            _log.Log($"Erro ao listar cargos de admissão: {ex.Message}", LevelsEnum.Error);
            result.Sucesso = false;
            result.Mensagem = "Erro ao listar cargos de admissão.";
            return result;
        }
    }

    public async Task<ApiGenericResult<List<AdmissaoCargoResult>>> ListarComRemuneracaoCadastradaAsync(string cpf, int orgId, bool somenteAtivos = true, int? cursor = null, int? limite = null, string busca = null)
    {
        var result = new ApiGenericResult<List<AdmissaoCargoResult>>();
        try
        {
            await ValidarAcessoListarAsync(cpf, orgId);
            var lista = await _repository.ListarComRemuneracaoCadastradaAsync(orgId, somenteAtivos, cursor, limite, busca);
            result.Retorno = lista?.ToList() ?? new List<AdmissaoCargoResult>();
            result.Sucesso = true;
            return result;
        }
        catch (UnauthorizedAccessException) { throw; }
        catch (Exception ex)
        {
            _log.Log($"Erro ao listar cargos de admissão com remuneração CLT: {ex.Message}", LevelsEnum.Error);
            result.Sucesso = false;
            result.Mensagem = "Erro ao listar cargos de admissão com remuneração cadastrada.";
            return result;
        }
    }

    public async Task<ApiGenericResult<AdmissaoCargoResult>> ObterPorIdAsync(string cpf, int orgId, Guid id)
    {
        var result = new ApiGenericResult<AdmissaoCargoResult>();
        try
        {
            await ValidarAcessoListarAsync(cpf, orgId);
            if (id == Guid.Empty) { result.Sucesso = false; result.Mensagem = "ID inválido."; return result; }
            var item = await _repository.ObterPorIdAsync(id, orgId);
            if (item == null) { result.Sucesso = false; result.Mensagem = "Cargo de admissão não encontrado."; return result; }
            result.Retorno = item;
            result.Sucesso = true;
            return result;
        }
        catch (UnauthorizedAccessException) { throw; }
        catch (Exception ex)
        {
            _log.Log($"Erro ao obter cargo de admissão: {ex.Message}", LevelsEnum.Error);
            result.Sucesso = false;
            result.Mensagem = "Erro ao obter cargo de admissão.";
            return result;
        }
    }

    public async Task<ApiGenericResult<AdmissaoCargoResult>> InserirAsync(string cpf, int orgId, AdmissaoCargoInput input)
    {
        var result = new ApiGenericResult<AdmissaoCargoResult>();
        try
        {
            await ValidarAcessoCriarEditarAsync(cpf, orgId);
            if (input == null) { result.Sucesso = false; result.Mensagem = "Dados são obrigatórios."; return result; }
            var msgCbo = await ValidarCboObrigatorioAsync(input, orgId);
            if (msgCbo != null) { result.Sucesso = false; result.Mensagem = msgCbo; return result; }
            if (string.IsNullOrWhiteSpace(input.Descricao)) { result.Sucesso = false; result.Mensagem = "Descrição é obrigatória."; return result; }
            var existente = await _repository.ObterPorDescricaoAsync(input.Descricao, orgId);
            if (existente != null) { result.Sucesso = false; result.Mensagem = "Já existe cargo de admissão com esta descrição."; return result; }
            var id = await _repository.InserirAsync(input, cpf, orgId);
            result.Retorno = await _repository.ObterPorIdAsync(id, orgId);
            result.Sucesso = true;
            result.Mensagem = "Cargo de admissão criado com sucesso.";
            return result;
        }
        catch (UnauthorizedAccessException) { throw; }
        catch (Exception ex)
        {
            _log.Log($"Erro ao inserir cargo de admissão: {ex.Message}", LevelsEnum.Error);
            result.Sucesso = false;
            result.Mensagem = "Erro ao criar cargo de admissão.";
            return result;
        }
    }

    public async Task<ApiGenericResult<AdmissaoCargoResult>> AtualizarAsync(string cpf, int orgId, Guid id, AdmissaoCargoInput input)
    {
        var result = new ApiGenericResult<AdmissaoCargoResult>();
        try
        {
            await ValidarAcessoCriarEditarAsync(cpf, orgId);
            if (id == Guid.Empty) { result.Sucesso = false; result.Mensagem = "ID inválido."; return result; }
            if (input == null) { result.Sucesso = false; result.Mensagem = "Dados são obrigatórios."; return result; }
            var msgCbo = await ValidarCboObrigatorioAsync(input, orgId);
            if (msgCbo != null) { result.Sucesso = false; result.Mensagem = msgCbo; return result; }
            if (string.IsNullOrWhiteSpace(input.Descricao)) { result.Sucesso = false; result.Mensagem = "Descrição é obrigatória."; return result; }
            var existente = await _repository.ObterPorIdAsync(id, orgId);
            if (existente == null) { result.Sucesso = false; result.Mensagem = "Cargo de admissão não encontrado."; return result; }
            var outroMesmaDescricao = await _repository.ObterPorDescricaoAsync(input.Descricao, orgId);
            if (outroMesmaDescricao != null && outroMesmaDescricao.Id != id) { result.Sucesso = false; result.Mensagem = "Já existe outro cargo com esta descrição."; return result; }
            var atualizou = await _repository.AtualizarAsync(id, input, cpf, orgId);
            if (!atualizou) { result.Sucesso = false; result.Mensagem = "Não foi possível atualizar."; return result; }
            result.Retorno = await _repository.ObterPorIdAsync(id, orgId);
            result.Sucesso = true;
            result.Mensagem = "Cargo de admissão atualizado com sucesso.";
            return result;
        }
        catch (UnauthorizedAccessException) { throw; }
        catch (Exception ex)
        {
            _log.Log($"Erro ao atualizar cargo de admissão: {ex.Message}", LevelsEnum.Error);
            result.Sucesso = false;
            result.Mensagem = "Erro ao atualizar cargo de admissão.";
            return result;
        }
    }
}
