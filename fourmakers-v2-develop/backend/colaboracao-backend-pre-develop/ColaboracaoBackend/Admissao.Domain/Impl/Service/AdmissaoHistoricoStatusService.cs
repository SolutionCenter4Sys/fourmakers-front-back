using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colaboracao.Core;
using Core.Domain.SRS;
using Core.Domain.Usuario.Permissao;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Log;
using DataTransferObject.Domain.SRS.AdmissaoHistoricoStatus;
using Logs.Infra.Attributes;
using Admissao.Domain.Interfaces.Service;

namespace Admissao.Domain.Impl.Service;

[LogDomainClass]
public class AdmissaoHistoricoStatusService : IAdmissaoHistoricoStatusService
{
    private readonly IAdmissaoHistoricoStatusRepository _repository;
    private readonly IFuncionalidadeSistemaRepository _funcionalidadeSistemaRepository;
    private readonly ILogCore _log;

    public AdmissaoHistoricoStatusService(
        IAdmissaoHistoricoStatusRepository repository,
        IFuncionalidadeSistemaRepository funcionalidadeSistemaRepository,
        ILogCore log)
    {
        _repository = repository;
        _funcionalidadeSistemaRepository = funcionalidadeSistemaRepository;
        _log = log;
    }

    private async Task ValidarAcessoListarAsync(string cpf, int orgId)
    {
        //if (!await _funcionalidadeSistemaRepository.ValidaAcessoFuncionalidade(cpf, orgId, FuncionalidadeSistemaEnum.ADMISSAO_LISTAR))
        //    throw new UnauthorizedAccessException("Acesso negado. Funcionalidade ADMISSAO_LISTAR necessária.");
        await Task.CompletedTask;
    }

    public async Task<ApiGenericResult<List<AdmissaoHistoricoStatusResult>>> ListarPorAdmissaoAsync(string cpf, int orgId, Guid admissaoId, int? cursor = null, int? limite = null)
    {
        var result = new ApiGenericResult<List<AdmissaoHistoricoStatusResult>>();
        try
        {
            await ValidarAcessoListarAsync(cpf, orgId);
            if (admissaoId == Guid.Empty) { result.Sucesso = false; result.Mensagem = "ID da admissão inválido."; return result; }
            var lista = await _repository.ListarPorAdmissaoAsync(admissaoId, orgId, cursor, limite);
            result.Retorno = lista?.ToList() ?? new List<AdmissaoHistoricoStatusResult>();
            result.Sucesso = true;
            return result;
        }
        catch (UnauthorizedAccessException) { throw; }
        catch (Exception ex)
        {
            _log.Log($"Erro ao listar histórico de status da admissão: {ex.Message}", LevelsEnum.Error);
            result.Sucesso = false;
            result.Mensagem = "Erro ao listar histórico de status.";
            return result;
        }
    }

    public async Task<ApiGenericResult<AdmissaoHistoricoStatusResult>> ObterPorIdAsync(string cpf, int orgId, Guid id)
    {
        var result = new ApiGenericResult<AdmissaoHistoricoStatusResult>();
        try
        {
            await ValidarAcessoListarAsync(cpf, orgId);
            if (id == Guid.Empty) { result.Sucesso = false; result.Mensagem = "ID inválido."; return result; }
            var item = await _repository.ObterPorIdAsync(id, orgId);
            if (item == null) { result.Sucesso = false; result.Mensagem = "Registro de histórico não encontrado."; return result; }
            result.Retorno = item;
            result.Sucesso = true;
            return result;
        }
        catch (UnauthorizedAccessException) { throw; }
        catch (Exception ex)
        {
            _log.Log($"Erro ao obter histórico de status: {ex.Message}", LevelsEnum.Error);
            result.Sucesso = false;
            result.Mensagem = "Erro ao obter histórico de status.";
            return result;
        }
    }
}
