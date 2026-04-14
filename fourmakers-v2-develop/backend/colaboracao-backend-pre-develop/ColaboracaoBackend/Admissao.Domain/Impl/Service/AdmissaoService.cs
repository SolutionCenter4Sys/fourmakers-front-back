using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colaboracao.Core;
using Core.Domain.SRS;
using Core.Domain.Usuario.Permissao;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Log;
using DataTransferObject.Domain.SRS.Admissao;
using DataTransferObject.Domain.SRS.AdmissaoHistoricoStatus;
using Logs.Infra.Attributes;
using Admissao.Domain.Interfaces.Service;

namespace Admissao.Domain.Impl.Service;

[LogDomainClass]
public class AdmissaoService : IAdmissaoService
{
    private readonly IAdmissaoRepository _repository;
    private readonly IAdmissaoHistoricoStatusRepository _historicoRepository;
    private readonly IAdmissaoValidatorService _validator;
    private readonly IFuncionalidadeSistemaRepository _funcionalidadeSistemaRepository;
    private readonly ILogCore _log;

    public AdmissaoService(
        IAdmissaoRepository repository,
        IAdmissaoHistoricoStatusRepository historicoRepository,
        IAdmissaoValidatorService validator,
        IFuncionalidadeSistemaRepository funcionalidadeSistemaRepository,
        ILogCore log)
    {
        _repository = repository;
        _historicoRepository = historicoRepository;
        _validator = validator;
        _funcionalidadeSistemaRepository = funcionalidadeSistemaRepository;
        _log = log;
    }

    private async Task ValidarAcessoListarAsync(string cpf, int orgId)
    {
        //if (!await _funcionalidadeSistemaRepository.ValidaAcessoFuncionalidade(cpf, orgId, FuncionalidadeSistemaEnum.ADMISSAO_LISTAR))
        //    throw new UnauthorizedAccessException("Acesso negado. Funcionalidade ADMISSAO_LISTAR necessária.");
        await Task.CompletedTask;
    }

    private async Task ValidarAcessoCriarEditarAsync(string cpf, int orgId)
    {
        //if (!await _funcionalidadeSistemaRepository.ValidaAcessoFuncionalidade(cpf, orgId, FuncionalidadeSistemaEnum.ADMISSAO_CRIAR_EDITAR))
        //    throw new UnauthorizedAccessException("Acesso negado. Funcionalidade ADMISSAO_CRIAR_EDITAR necessária.");
        await Task.CompletedTask;
    }

    public async Task<ApiGenericResult<List<AdmissaoResult>>> ListarAsync(string cpf, int orgId, bool somenteAtivos = true, int? cursor = null, int? limite = null, Guid? pipelineId = null, Guid? statusId = null)
    {
        var result = new ApiGenericResult<List<AdmissaoResult>>();
        try
        {
            await ValidarAcessoListarAsync(cpf, orgId);
            var lista = await _repository.ListarAsync(orgId, somenteAtivos, cursor, limite, pipelineId, statusId);
            result.Retorno = lista?.ToList() ?? new List<AdmissaoResult>();
            result.Sucesso = true;
            return result;
        }
        catch (UnauthorizedAccessException) { throw; }
        catch (Exception ex)
        {
            _log.Log($"Erro ao listar admissões: {ex.Message}", LevelsEnum.Error);
            result.Sucesso = false;
            result.Mensagem = "Erro ao listar admissões.";
            return result;
        }
    }

    public async Task<ApiGenericResult<AdmissaoResult>> ObterPorIdAsync(string cpf, int orgId, Guid id)
    {
        var result = new ApiGenericResult<AdmissaoResult>();
        try
        {
            await ValidarAcessoListarAsync(cpf, orgId);
            if (id == Guid.Empty) { result.Sucesso = false; result.Mensagem = "ID inválido."; return result; }
            var item = await _repository.ObterPorIdAsync(id, orgId);
            if (item == null) { result.Sucesso = false; result.Mensagem = "Admissão não encontrada."; return result; }
            result.Retorno = item;
            result.Sucesso = true;
            return result;
        }
        catch (UnauthorizedAccessException) { throw; }
        catch (Exception ex)
        {
            _log.Log($"Erro ao obter admissão: {ex.Message}", LevelsEnum.Error);
            result.Sucesso = false;
            result.Mensagem = "Erro ao obter admissão.";
            return result;
        }
    }

    public async Task<ApiGenericResult<AdmissaoResult>> InserirAsync(string cpf, int orgId, AdmissaoInput input)
    {
        var result = new ApiGenericResult<AdmissaoResult>();
        try
        {
            await ValidarAcessoCriarEditarAsync(cpf, orgId);
            if (input == null) { result.Sucesso = false; result.Mensagem = "Dados são obrigatórios."; return result; }
            if (input.DataInicio == default) { result.Sucesso = false; result.Mensagem = "Data de início é obrigatória."; return result; }
            await _validator.ValidarPipelineExisteAsync(input.AdmissaoPipelineId, orgId);
            await _validator.ValidarStatusExisteAsync(input.AdmissaoStatusId, orgId);
            await _validator.ValidarColaboradorExisteAsync(input.CodigoInternoColaborador);

            var id = await _repository.InserirAsync(input, cpf, orgId);
            result.Retorno = await _repository.ObterPorIdAsync(id, orgId);
            result.Sucesso = true;
            result.Mensagem = "Admissão criada com sucesso.";
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
            _log.Log($"Erro ao inserir admissão: {ex.Message}", LevelsEnum.Error);
            result.Sucesso = false;
            result.Mensagem = "Erro ao criar admissão.";
            return result;
        }
    }

    public async Task<ApiGenericResult<AdmissaoResult>> AtualizarAsync(string cpf, int orgId, Guid id, AdmissaoUpdateInput input)
    {
        var result = new ApiGenericResult<AdmissaoResult>();
        try
        {
            await ValidarAcessoCriarEditarAsync(cpf, orgId);
            if (id == Guid.Empty) { result.Sucesso = false; result.Mensagem = "ID inválido."; return result; }
            if (input == null) { result.Sucesso = false; result.Mensagem = "Dados são obrigatórios."; return result; }
            await _validator.ValidarPipelineExisteAsync(input.AdmissaoPipelineId, orgId);
            await _validator.ValidarColaboradorExisteAsync(input.CodigoInternoColaborador);

            var existente = await _repository.ObterPorIdAsync(id, orgId);
            if (existente == null) { result.Sucesso = false; result.Mensagem = "Admissão não encontrada."; return result; }

            var atualizou = await _repository.AtualizarAsync(id, input, cpf, orgId);
            if (!atualizou) { result.Sucesso = false; result.Mensagem = "Não foi possível atualizar a admissão."; return result; }

            result.Retorno = await _repository.ObterPorIdAsync(id, orgId);
            result.Sucesso = true;
            result.Mensagem = "Admissão atualizada com sucesso.";
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
            _log.Log($"Erro ao atualizar admissão: {ex.Message}", LevelsEnum.Error);
            result.Sucesso = false;
            result.Mensagem = "Erro ao atualizar admissão.";
            return result;
        }
    }

    public async Task<ApiGenericResult<bool>> InativarAsync(string cpf, int orgId, Guid id)
    {
        var result = new ApiGenericResult<bool>();
        try
        {
            await ValidarAcessoCriarEditarAsync(cpf, orgId);
            if (id == Guid.Empty) { result.Sucesso = false; result.Mensagem = "ID inválido."; result.Retorno = false; return result; }

            var inativou = await _repository.InativarAsync(id, cpf, orgId);
            if (!inativou) { result.Sucesso = false; result.Mensagem = "Admissão não encontrada ou já inativa."; result.Retorno = false; return result; }

            result.Retorno = true;
            result.Sucesso = true;
            result.Mensagem = "Admissão inativada com sucesso.";
            return result;
        }
        catch (UnauthorizedAccessException) { throw; }
        catch (Exception ex)
        {
            _log.Log($"Erro ao inativar admissão: {ex.Message}", LevelsEnum.Error);
            result.Sucesso = false;
            result.Mensagem = "Erro ao inativar admissão.";
            result.Retorno = false;
            return result;
        }
    }

    public async Task<ApiGenericResult<AdmissaoHistoricoStatusResult>> MoverStatusAsync(string cpf, int orgId, Guid admissaoId, AdmissaoHistoricoStatusInput input)
    {
        var result = new ApiGenericResult<AdmissaoHistoricoStatusResult>();
        try
        {
            await ValidarAcessoCriarEditarAsync(cpf, orgId);
            if (admissaoId == Guid.Empty) { result.Sucesso = false; result.Mensagem = "ID da admissão inválido."; return result; }
            if (input == null) { result.Sucesso = false; result.Mensagem = "Dados são obrigatórios."; return result; }
            await _validator.ValidarStatusExisteAsync(input.AdmissaoStatusDestinoId, orgId);

            var admissao = await _repository.ObterPorIdAsync(admissaoId, orgId);
            if (admissao == null) { result.Sucesso = false; result.Mensagem = "Admissão não encontrada."; return result; }
            if (!admissao.Ativo) { result.Sucesso = false; result.Mensagem = "Não é possível mover o status de uma admissão inativa."; return result; }

            var statusOrigemId = admissao.AdmissaoStatusId;

            var atualizou = await _repository.AtualizarStatusAsync(admissaoId, input.AdmissaoStatusDestinoId, cpf, orgId);
            if (!atualizou) { result.Sucesso = false; result.Mensagem = "Não foi possível atualizar o status da admissão."; return result; }

            var historicoId = await _historicoRepository.InserirAsync(input, admissaoId, statusOrigemId, cpf, orgId);
            result.Retorno = await _historicoRepository.ObterPorIdAsync(historicoId, orgId);
            result.Sucesso = true;
            result.Mensagem = "Status da admissão movimentado com sucesso.";
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
            _log.Log($"Erro ao mover status da admissão: {ex.Message}", LevelsEnum.Error);
            result.Sucesso = false;
            result.Mensagem = "Erro ao mover status da admissão.";
            return result;
        }
    }
}
