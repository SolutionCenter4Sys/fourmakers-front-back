using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colaboracao.Core;
using Core.Domain.SRS;
using Core.Domain.Usuario.Permissao;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Log;
using DataTransferObject.Domain.SRS.AdmissaoStatus;
using DataTransferObject.Domain.Usuario;
using Logs.Infra.Attributes;
using Admissao.Domain.Interfaces.Service;

namespace Admissao.Domain.Impl.Service;

[LogDomainClass]
public class AdmissaoStatusService : IAdmissaoStatusService
{
    private readonly IAdmissaoStatusRepository _repository;
    private readonly IFuncionalidadeSistemaRepository _funcionalidadeSistemaRepository;
    private readonly ILogCore _log;

    public AdmissaoStatusService(
        IAdmissaoStatusRepository repository,
        IFuncionalidadeSistemaRepository funcionalidadeSistemaRepository,
        ILogCore log)
    {
        _repository = repository;
        _funcionalidadeSistemaRepository = funcionalidadeSistemaRepository;
        _log = log;
    }

    private async Task ValidarAcessoListarAsync(string cpf, int orgId)
    {
        //if (!await _funcionalidadeSistemaRepository.ValidaAcessoFuncionalidade(cpf, orgId, FuncionalidadeSistemaEnum.ADMISSAO_STATUS_LISTAR))
        //    throw new UnauthorizedAccessException("Acesso negado. Funcionalidade ADMISSAO_STATUS_LISTAR necessária.");
    }

    private async Task ValidarAcessoCriarEditarAsync(string cpf, int orgId)
    {
        //if (!await _funcionalidadeSistemaRepository.ValidaAcessoFuncionalidade(cpf, orgId, FuncionalidadeSistemaEnum.ADMISSAO_STATUS_CRIAR_EDITAR))
        //    throw new UnauthorizedAccessException("Acesso negado. Funcionalidade ADMISSAO_STATUS_CRIAR_EDITAR necessária.");
    }

    public async Task<ApiGenericResult<List<AdmissaoStatusResult>>> ListarAsync(string cpf, int orgId, bool somenteAtivos = true, int? cursor = null, int? limite = null, string busca = null)
    {
        var result = new ApiGenericResult<List<AdmissaoStatusResult>>();
        try
        {
            await ValidarAcessoListarAsync(cpf, orgId);
            var lista = await _repository.ListarAsync(orgId, somenteAtivos, cursor, limite, busca);
            result.Retorno = lista?.ToList() ?? new List<AdmissaoStatusResult>();
            result.Sucesso = true;
            return result;
        }
        catch (UnauthorizedAccessException) { throw; }
        catch (Exception ex)
        {
            _log.Log($"Erro ao listar status de admissão: {ex.Message}", LevelsEnum.Error);
            result.Sucesso = false;
            result.Mensagem = "Erro ao listar status de admissão.";
            return result;
        }
    }

    public async Task<ApiGenericResult<AdmissaoStatusResult>> ObterPorIdAsync(string cpf, int orgId, Guid id)
    {
        var result = new ApiGenericResult<AdmissaoStatusResult>();
        try
        {
            await ValidarAcessoListarAsync(cpf, orgId);
            if (id == Guid.Empty) { result.Sucesso = false; result.Mensagem = "ID inválido."; return result; }
            var item = await _repository.ObterPorIdAsync(id, orgId);
            if (item == null) { result.Sucesso = false; result.Mensagem = "Status de admissão não encontrado."; return result; }
            result.Retorno = item;
            result.Sucesso = true;
            return result;
        }
        catch (UnauthorizedAccessException) { throw; }
        catch (Exception ex)
        {
            _log.Log($"Erro ao obter status de admissão: {ex.Message}", LevelsEnum.Error);
            result.Sucesso = false;
            result.Mensagem = "Erro ao obter status de admissão.";
            return result;
        }
    }

    public async Task<ApiGenericResult<AdmissaoStatusResult>> InserirAsync(string cpf, int orgId, AdmissaoStatusInput input)
    {
        var result = new ApiGenericResult<AdmissaoStatusResult>();
        try
        {
            await ValidarAcessoCriarEditarAsync(cpf, orgId);
            if (input == null) { result.Sucesso = false; result.Mensagem = "Dados são obrigatórios."; return result; }
            if (string.IsNullOrWhiteSpace(input.Descricao)) { result.Sucesso = false; result.Mensagem = "Descrição é obrigatória."; return result; }
            var existente = await _repository.ObterPorDescricaoAsync(input.Descricao, orgId);
            if (existente != null) { result.Sucesso = false; result.Mensagem = "Já existe status de admissão com esta descrição."; return result; }
            if (input.Codigo.HasValue)
            {
                var outroCodigo = await _repository.ObterPorCodigoNaOrgAsync(input.Codigo.Value, orgId);
                if (outroCodigo != null) { result.Sucesso = false; result.Mensagem = "Já existe status com este código na organização."; return result; }
            }
            var id = await _repository.InserirAsync(input, cpf, orgId);
            result.Retorno = await _repository.ObterPorIdAsync(id, orgId);
            result.Sucesso = true;
            result.Mensagem = "Status de admissão criado com sucesso.";
            return result;
        }
        catch (UnauthorizedAccessException) { throw; }
        catch (Exception ex)
        {
            _log.Log($"Erro ao inserir status de admissão: {ex.Message}", LevelsEnum.Error);
            result.Sucesso = false;
            result.Mensagem = "Erro ao criar status de admissão.";
            return result;
        }
    }

    public async Task<ApiGenericResult<AdmissaoStatusResult>> AtualizarAsync(string cpf, int orgId, Guid id, AdmissaoStatusInput input)
    {
        var result = new ApiGenericResult<AdmissaoStatusResult>();
        try
        {
            await ValidarAcessoCriarEditarAsync(cpf, orgId);
            if (id == Guid.Empty) { result.Sucesso = false; result.Mensagem = "ID inválido."; return result; }
            if (input == null) { result.Sucesso = false; result.Mensagem = "Dados são obrigatórios."; return result; }
            if (string.IsNullOrWhiteSpace(input.Descricao)) { result.Sucesso = false; result.Mensagem = "Descrição é obrigatória."; return result; }
            var existente = await _repository.ObterPorIdAsync(id, orgId);
            if (existente == null) { result.Sucesso = false; result.Mensagem = "Status de admissão não encontrado."; return result; }
            var outroMesmaDescricao = await _repository.ObterPorDescricaoAsync(input.Descricao, orgId);
            if (outroMesmaDescricao != null && outroMesmaDescricao.Id != id) { result.Sucesso = false; result.Mensagem = "Já existe outro status com esta descrição."; return result; }
            if (input.Codigo.HasValue)
            {
                var outroCodigo = await _repository.ObterPorCodigoNaOrgAsync(input.Codigo.Value, orgId);
                if (outroCodigo != null && outroCodigo.Id != id) { result.Sucesso = false; result.Mensagem = "Já existe outro status com este código na organização."; return result; }
            }
            var atualizou = await _repository.AtualizarAsync(id, input, cpf, orgId);
            if (!atualizou) { result.Sucesso = false; result.Mensagem = "Não foi possível atualizar."; return result; }
            result.Retorno = await _repository.ObterPorIdAsync(id, orgId);
            result.Sucesso = true;
            result.Mensagem = "Status de admissão atualizado com sucesso.";
            return result;
        }
        catch (UnauthorizedAccessException) { throw; }
        catch (Exception ex)
        {
            _log.Log($"Erro ao atualizar status de admissão: {ex.Message}", LevelsEnum.Error);
            result.Sucesso = false;
            result.Mensagem = "Erro ao atualizar status de admissão.";
            return result;
        }
    }

    public async Task<ApiGenericResult<bool>> ExcluirAsync(string cpf, int orgId, Guid id)
    {
        var result = new ApiGenericResult<bool>();
        try
        {
            await ValidarAcessoCriarEditarAsync(cpf, orgId);
            if (id == Guid.Empty) { result.Sucesso = false; result.Mensagem = "ID inválido."; result.Retorno = false; return result; }
            var excluiu = await _repository.ExcluirLogicamenteAsync(id, cpf, orgId);
            if (!excluiu) { result.Sucesso = false; result.Mensagem = "Status não encontrado ou já inativo."; result.Retorno = false; return result; }
            result.Retorno = true;
            result.Sucesso = true;
            result.Mensagem = "Status de admissão inativado com sucesso.";
            return result;
        }
        catch (UnauthorizedAccessException) { throw; }
        catch (Exception ex)
        {
            _log.Log($"Erro ao excluir status de admissão: {ex.Message}", LevelsEnum.Error);
            result.Sucesso = false;
            result.Mensagem = "Erro ao inativar status de admissão.";
            result.Retorno = false;
            return result;
        }
    }
}
