using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colaboracao.Core;
using Core.Domain.SRS;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Log;
using DataTransferObject.Domain.SRS.AdmissaoPipeline;
using Logs.Infra.Attributes;
using Admissao.Domain.Interfaces.Service;

namespace Admissao.Domain.Impl.Service;

[LogDomainClass]
public class AdmissaoPipelineService : IAdmissaoPipelineService
{
    private readonly IAdmissaoPipelineRepository _repository;
    private readonly IAdmissaoPipelineStatusRepository _pipelineStatusRepository;
    private readonly IAdmissaoPipelineValidatorService _validator;
    private readonly ILogCore _log;

    public AdmissaoPipelineService(
        IAdmissaoPipelineRepository repository,
        IAdmissaoPipelineStatusRepository pipelineStatusRepository,
        IAdmissaoPipelineValidatorService validator,
        ILogCore log)
    {
        _repository = repository;
        _pipelineStatusRepository = pipelineStatusRepository;
        _validator = validator;
        _log = log;
    }

    private async Task AnexarStatusItensNoResultAsync(int orgId, AdmissaoPipelineResult retorno)
    {
        if (retorno == null) return;
        var lista = await _pipelineStatusRepository.ListarPorPipelineAsync(orgId, retorno.Id, somenteAtivos: false);
        retorno.StatusItens = lista?.ToList() ?? new List<AdmissaoPipelineStatusResult>();
    }

    public async Task<ApiGenericResult<List<AdmissaoPipelineResult>>> ListarAsync(string cpf, int orgId, bool somenteAtivos = true, int? cursor = null, int? limite = null, string busca = null)
    {
        var result = new ApiGenericResult<List<AdmissaoPipelineResult>>();
        try
        {
            await _validator.ValidarAcessoListarAsync(cpf, orgId);
            var lista = await _repository.ListarAsync(orgId, somenteAtivos, cursor, limite, busca);
            result.Retorno = lista?.ToList() ?? new List<AdmissaoPipelineResult>();
            result.Sucesso = true;
            return result;
        }
        catch (UnauthorizedAccessException) { throw; }
        catch (Exception ex)
        {
            _log.Log($"Erro ao listar pipelines de admissão: {ex.Message}", LevelsEnum.Error);
            result.Sucesso = false;
            result.Mensagem = "Erro ao listar pipelines de admissão.";
            return result;
        }
    }

    public async Task<ApiGenericResult<List<AdmissaoPipelineSummaryResult>>> ListarSummaryAsync(string cpf, int orgId, bool somenteAtivos = true)
    {
        var result = new ApiGenericResult<List<AdmissaoPipelineSummaryResult>>();
        try
        {
            await _validator.ValidarAcessoListarAsync(cpf, orgId);
            var lista = await _repository.ListarSummaryAsync(orgId, somenteAtivos);
            result.Retorno = lista?.ToList() ?? new List<AdmissaoPipelineSummaryResult>();
            result.Sucesso = true;
            return result;
        }
        catch (UnauthorizedAccessException) { throw; }
        catch (Exception ex)
        {
            _log.Log($"Erro ao listar resumo de pipelines de admissão: {ex.Message}", LevelsEnum.Error);
            result.Sucesso = false;
            result.Mensagem = "Erro ao listar pipelines de admissão.";
            return result;
        }
    }

    public async Task<ApiGenericResult<AdmissaoPipelineResult>> ObterPorIdAsync(string cpf, int orgId, Guid id)
    {
        var result = new ApiGenericResult<AdmissaoPipelineResult>();
        try
        {
            await _validator.ValidarAcessoListarAsync(cpf, orgId);
            if (id == Guid.Empty) { result.Sucesso = false; result.Mensagem = "ID inválido."; return result; }
            var item = await _repository.ObterPorIdAsync(id, orgId);
            if (item == null) { result.Sucesso = false; result.Mensagem = "Pipeline de admissão não encontrado."; return result; }
            await AnexarStatusItensNoResultAsync(orgId, item);
            result.Retorno = item;
            result.Sucesso = true;
            return result;
        }
        catch (UnauthorizedAccessException) { throw; }
        catch (Exception ex)
        {
            _log.Log($"Erro ao obter pipeline de admissão: {ex.Message}", LevelsEnum.Error);
            result.Sucesso = false;
            result.Mensagem = "Erro ao obter pipeline de admissão.";
            return result;
        }
    }

    public async Task<ApiGenericResult<AdmissaoPipelineResult>> InserirAsync(string cpf, int orgId, AdmissaoPipelineInput input)
    {
        var result = new ApiGenericResult<AdmissaoPipelineResult>();
        try
        {
            await _validator.ValidarAcessoCriarEditarAsync(cpf, orgId);
            if (input == null) { result.Sucesso = false; result.Mensagem = "Dados são obrigatórios."; return result; }
            if (string.IsNullOrWhiteSpace(input.Nome)) { result.Sucesso = false; result.Mensagem = "Nome é obrigatório."; return result; }
            var existente = await _repository.ObterPorNomeAsync(input.Nome, orgId);
            if (existente != null) { result.Sucesso = false; result.Mensagem = "Já existe pipeline com este nome na organização."; return result; }
            await _validator.ValidarStatusItensAsync(orgId, input.StatusItens);

            var id = await _repository.InserirAsync(input, cpf, orgId);
            if (input.StatusItens != null)
                await _pipelineStatusRepository.SubstituirPorPipelineAsync(orgId, id, input.StatusItens);

            result.Retorno = await _repository.ObterPorIdAsync(id, orgId);
            await AnexarStatusItensNoResultAsync(orgId, result.Retorno);
            result.Sucesso = true;
            result.Mensagem = "Pipeline de admissão criado com sucesso.";
            return result;
        }
        catch (UnauthorizedAccessException) { throw; }
        catch (InvalidOperationException ex)
        {
            result.Sucesso = false;
            result.Mensagem = ex.Message;
            return result;
        }
        catch (Exception ex)
        {
            _log.Log($"Erro ao inserir pipeline de admissão: {ex.Message}", LevelsEnum.Error);
            result.Sucesso = false;
            result.Mensagem = "Erro ao criar pipeline de admissão.";
            return result;
        }
    }

    public async Task<ApiGenericResult<AdmissaoPipelineResult>> AtualizarAsync(string cpf, int orgId, Guid id, AdmissaoPipelineAtualizarInput input)
    {
        var result = new ApiGenericResult<AdmissaoPipelineResult>();
        try
        {
            await _validator.ValidarAcessoCriarEditarAsync(cpf, orgId);
            if (id == Guid.Empty) { result.Sucesso = false; result.Mensagem = "ID inválido."; return result; }
            if (input == null) { result.Sucesso = false; result.Mensagem = "Dados são obrigatórios."; return result; }
            if (string.IsNullOrWhiteSpace(input.Nome)) { result.Sucesso = false; result.Mensagem = "Nome é obrigatório."; return result; }
            var atual = await _repository.ObterPorIdAsync(id, orgId);
            if (atual == null) { result.Sucesso = false; result.Mensagem = "Pipeline de admissão não encontrado."; return result; }
            var outroMesmoNome = await _repository.ObterPorNomeAsync(input.Nome, orgId);
            if (outroMesmoNome != null && outroMesmoNome.Id != id) { result.Sucesso = false; result.Mensagem = "Já existe outro pipeline com este nome."; return result; }
            await _validator.ValidarStatusItensAsync(orgId, input.StatusItens);

            var atualizou = await _repository.AtualizarAsync(id, input, cpf, orgId);
            if (!atualizou) { result.Sucesso = false; result.Mensagem = "Não foi possível atualizar."; return result; }
            if (input.StatusItens != null)
                await _pipelineStatusRepository.SubstituirPorPipelineAsync(orgId, id, input.StatusItens);

            result.Retorno = await _repository.ObterPorIdAsync(id, orgId);
            await AnexarStatusItensNoResultAsync(orgId, result.Retorno);
            result.Sucesso = true;
            result.Mensagem = "Pipeline de admissão atualizado com sucesso.";
            return result;
        }
        catch (UnauthorizedAccessException) { throw; }
        catch (InvalidOperationException ex)
        {
            result.Sucesso = false;
            result.Mensagem = ex.Message;
            return result;
        }
        catch (Exception ex)
        {
            _log.Log($"Erro ao atualizar pipeline de admissão: {ex.Message}", LevelsEnum.Error);
            result.Sucesso = false;
            result.Mensagem = "Erro ao atualizar pipeline de admissão.";
            return result;
        }
    }

    public async Task<ApiGenericResult<object>> InativarAsync(string cpf, int orgId, Guid id)
    {
        var result = new ApiGenericResult<object>();
        try
        {
            await _validator.ValidarAcessoCriarEditarAsync(cpf, orgId);
            if (id == Guid.Empty) { result.Sucesso = false; result.Mensagem = "ID inválido."; return result; }
            var ok = await _repository.InativarAsync(id, cpf, orgId);
            if (!ok) { result.Sucesso = false; result.Mensagem = "Pipeline de admissão não encontrado."; return result; }
            result.Sucesso = true;
            result.Mensagem = "Pipeline inativado com sucesso.";
            return result;
        }
        catch (UnauthorizedAccessException) { throw; }
        catch (Exception ex)
        {
            _log.Log($"Erro ao inativar pipeline de admissão: {ex.Message}", LevelsEnum.Error);
            result.Sucesso = false;
            result.Mensagem = "Erro ao inativar pipeline de admissão.";
            return result;
        }
    }
}
