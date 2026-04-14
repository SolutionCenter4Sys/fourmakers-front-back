using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colaboracao.Core;
using Core.Domain.SRS;
using Core.Domain.Usuario.Permissao;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Log;
using DataTransferObject.Domain.SRS.Cbo;
using DataTransferObject.Domain.Usuario;
using Logs.Infra.Attributes;
using Admissao.Domain.Interfaces.Service;

namespace Admissao.Domain.Impl.Service;

[LogDomainClass]
public class CboService : ICboService
{
    private readonly ICboRepository _cboRepository;
    private readonly IFuncionalidadeSistemaRepository _funcionalidadeSistemaRepository;
    private readonly ILogCore _log;

    public CboService(ICboRepository cboRepository, IFuncionalidadeSistemaRepository funcionalidadeSistemaRepository, ILogCore log)
    {
        _cboRepository = cboRepository;
        _funcionalidadeSistemaRepository = funcionalidadeSistemaRepository;
        _log = log;
    }

    private async Task ValidarAcessoListarAsync(string cpf, int orgId)
    {
        if (!await _funcionalidadeSistemaRepository.ValidaAcessoFuncionalidade(cpf, orgId, FuncionalidadeSistemaEnum.ADMISSAO_CBO_LISTAR))
            throw new UnauthorizedAccessException("Acesso negado.");
    }

    private async Task ValidarAcessoCriarEditarAsync(string cpf, int orgId)
    {
        if (!await _funcionalidadeSistemaRepository.ValidaAcessoFuncionalidade(cpf, orgId, FuncionalidadeSistemaEnum.ADMISSAO_CBO_CRIAR_EDITAR))
            throw new UnauthorizedAccessException("Acesso negado.");
    }

    public async Task<ApiGenericResult<List<CboResult>>> ListarAsync(string cpf, int orgId, bool somenteAtivos = false, int? cursor = null, int? limite = null, string busca = null)
    {
        var result = new ApiGenericResult<List<CboResult>>();
        try
        {
            await ValidarAcessoListarAsync(cpf, orgId);
            var lista = await _cboRepository.ListarAsync(orgId, somenteAtivos, cursor, limite, busca);
            result.Retorno = lista?.ToList() ?? new List<CboResult>();
            result.Sucesso = true;
            return result;
        }
        catch (UnauthorizedAccessException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _log.Log($"Erro ao listar CBO: {ex.Message}", LevelsEnum.Error);
            _log.Log(ex.StackTrace, LevelsEnum.Error);
            result.Sucesso = false;
            result.Mensagem = "Erro ao listar CBO.";
            return result;
        }
    }

    public async Task<ApiGenericResult<CboResult>> ObterPorIdAsync(string cpf, int orgId, Guid id)
    {
        var result = new ApiGenericResult<CboResult>();
        try
        {
            await ValidarAcessoListarAsync(cpf, orgId);
            if (id == Guid.Empty)
            {
                result.Sucesso = false;
                result.Mensagem = "ID inválido.";
                return result;
            }
            var item = await _cboRepository.ObterPorIdAsync(id, orgId);
            if (item == null)
            {
                result.Sucesso = false;
                result.Mensagem = "CBO não encontrado.";
                return result;
            }
            result.Retorno = item;
            result.Sucesso = true;
            return result;
        }
        catch (UnauthorizedAccessException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _log.Log($"Erro ao obter CBO por id: {ex.Message}", LevelsEnum.Error);
            result.Sucesso = false;
            result.Mensagem = "Erro ao obter CBO.";
            return result;
        }
    }

    public async Task<ApiGenericResult<CboResult>> ObterPorCodigoAsync(string cpf, int orgId, string codigo)
    {
        var result = new ApiGenericResult<CboResult>();
        try
        {
            await ValidarAcessoListarAsync(cpf, orgId);
            if (string.IsNullOrWhiteSpace(codigo))
            {
                result.Sucesso = false;
                result.Mensagem = "Código é obrigatório.";
                return result;
            }
            var item = await _cboRepository.ObterPorCodigoAsync(codigo.Trim(), orgId);
            if (item == null)
            {
                result.Sucesso = false;
                result.Mensagem = "CBO não encontrado.";
                return result;
            }
            result.Retorno = item;
            result.Sucesso = true;
            return result;
        }
        catch (UnauthorizedAccessException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _log.Log($"Erro ao obter CBO por código: {ex.Message}", LevelsEnum.Error);
            result.Sucesso = false;
            result.Mensagem = "Erro ao obter CBO.";
            return result;
        }
    }

    public async Task<ApiGenericResult<CboResult>> InserirAsync(string cpf, int orgId, CboInput input)
    {
        var result = new ApiGenericResult<CboResult>();
        try
        {
            await ValidarAcessoCriarEditarAsync(cpf, orgId);
            if (input == null)
            {
                result.Sucesso = false;
                result.Mensagem = "Dados do CBO são obrigatórios.";
                return result;
            }
            if (string.IsNullOrWhiteSpace(input.Codigo))
            {
                result.Sucesso = false;
                result.Mensagem = "Código CBO é obrigatório.";
                return result;
            }
            if (string.IsNullOrWhiteSpace(input.Titulo))
            {
                result.Sucesso = false;
                result.Mensagem = "Título é obrigatório.";
                return result;
            }
            var existente = await _cboRepository.ObterPorCodigoAsync(input.Codigo.Trim(), orgId);
            if (existente != null)
            {
                result.Sucesso = false;
                result.Mensagem = "Já existe um CBO com este código.";
                return result;
            }
            var id = await _cboRepository.InserirAsync(input, cpf, orgId);
            var criado = await _cboRepository.ObterPorIdAsync(id, orgId);
            result.Retorno = criado;
            result.Sucesso = true;
            result.Mensagem = "CBO criado com sucesso.";
            return result;
        }
        catch (UnauthorizedAccessException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _log.Log($"Erro ao inserir CBO: {ex.Message}", LevelsEnum.Error);
            _log.Log(ex.StackTrace, LevelsEnum.Error);
            result.Sucesso = false;
            result.Mensagem = "Erro ao criar CBO.";
            return result;
        }
    }

    public async Task<ApiGenericResult<CboResult>> AtualizarAsync(string cpf, int orgId, Guid id, CboInput input)
    {
        var result = new ApiGenericResult<CboResult>();
        try
        {
            await ValidarAcessoCriarEditarAsync(cpf, orgId);
            if (id == Guid.Empty)
            {
                result.Sucesso = false;
                result.Mensagem = "ID inválido.";
                return result;
            }
            if (input == null)
            {
                result.Sucesso = false;
                result.Mensagem = "Dados do CBO são obrigatórios.";
                return result;
            }
            if (string.IsNullOrWhiteSpace(input.Codigo))
            {
                result.Sucesso = false;
                result.Mensagem = "Código CBO é obrigatório.";
                return result;
            }
            if (string.IsNullOrWhiteSpace(input.Titulo))
            {
                result.Sucesso = false;
                result.Mensagem = "Título é obrigatório.";
                return result;
            }
            var existente = await _cboRepository.ObterPorIdAsync(id, orgId);
            if (existente == null)
            {
                result.Sucesso = false;
                result.Mensagem = "CBO não encontrado.";
                return result;
            }
            var outroComMesmoCodigo = await _cboRepository.ObterPorCodigoAsync(input.Codigo.Trim(), orgId);
            if (outroComMesmoCodigo != null && outroComMesmoCodigo.Id != id)
            {
                result.Sucesso = false;
                result.Mensagem = "Já existe outro CBO com este código.";
                return result;
            }
            var atualizou = await _cboRepository.AtualizarAsync(id, input, cpf, orgId);
            if (!atualizou)
            {
                result.Sucesso = false;
                result.Mensagem = "Não foi possível atualizar o CBO.";
                return result;
            }
            result.Retorno = await _cboRepository.ObterPorIdAsync(id, orgId);
            result.Sucesso = true;
            result.Mensagem = "CBO atualizado com sucesso.";
            return result;
        }
        catch (UnauthorizedAccessException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _log.Log($"Erro ao atualizar CBO: {ex.Message}", LevelsEnum.Error);
            result.Sucesso = false;
            result.Mensagem = "Erro ao atualizar CBO.";
            return result;
        }
    }
}
