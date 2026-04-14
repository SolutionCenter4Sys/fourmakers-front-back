using System;
using System.Threading.Tasks;
using Colaboracao.Core;
using Core.Domain.SRS;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Log;
using DataTransferObject.Domain.SRS.AdmissaoOrigem;
using Logs.Infra.Attributes;
using Admissao.Domain.Interfaces.Service;

namespace Admissao.Domain.Impl.Service;

[LogDomainClass]
public class AdmissaoOrigemService : IAdmissaoOrigemService
{
    private readonly IAdmissaoOrigemRepository _origemRepository;
    private readonly IAdmissaoRepository _admissaoRepository;
    private readonly ILogCore _log;

    public AdmissaoOrigemService(
        IAdmissaoOrigemRepository origemRepository,
        IAdmissaoRepository admissaoRepository,
        ILogCore log)
    {
        _origemRepository = origemRepository;
        _admissaoRepository = admissaoRepository;
        _log = log;
    }

    public async Task<ApiGenericResult<AdmissaoOrigemResult>> VincularAsync(string cpf, int orgId, Guid admissaoId, AdmissaoOrigemInput input)
    {
        var result = new ApiGenericResult<AdmissaoOrigemResult>();
        try
        {
            if (admissaoId == Guid.Empty) { result.Sucesso = false; result.Mensagem = "ID da admissão inválido."; return result; }
            if (input == null) { result.Sucesso = false; result.Mensagem = "Dados são obrigatórios."; return result; }

            if (!ValidarConsistencia(input, out var mensagemValidacao))
            {
                result.Sucesso = false;
                result.Mensagem = mensagemValidacao;
                return result;
            }

            var admissao = await _admissaoRepository.ObterPorIdAsync(admissaoId, orgId);
            if (admissao == null) { result.Sucesso = false; result.Mensagem = "Admissão não encontrada."; return result; }

            var id = await _origemRepository.InserirAsync(admissaoId, input, cpf);
            result.Retorno = await _origemRepository.ObterPorAdmissaoAsync(admissaoId);
            result.Sucesso = true;
            result.Mensagem = "Vínculo registrado com sucesso.";
            return result;
        }
        catch (Exception ex)
        {
            _log.Log($"Erro ao vincular origem da admissão: {ex.Message}", LevelsEnum.Error);
            result.Sucesso = false;
            result.Mensagem = "Erro ao registrar vínculo de origem.";
            return result;
        }
    }

    public async Task<ApiGenericResult<AdmissaoOrigemResult>> ObterVinculoAsync(string cpf, int orgId, Guid admissaoId)
    {
        var result = new ApiGenericResult<AdmissaoOrigemResult>();
        try
        {
            if (admissaoId == Guid.Empty) { result.Sucesso = false; result.Mensagem = "ID da admissão inválido."; return result; }

            result.Retorno = await _origemRepository.ObterPorAdmissaoAsync(admissaoId);
            result.Sucesso = true;
            return result;
        }
        catch (Exception ex)
        {
            _log.Log($"Erro ao obter vínculo de origem da admissão: {ex.Message}", LevelsEnum.Error);
            result.Sucesso = false;
            result.Mensagem = "Erro ao obter vínculo de origem.";
            return result;
        }
    }

    private static bool ValidarConsistencia(AdmissaoOrigemInput input, out string mensagem)
    {
        if (input.OrigemTipo == AdmissaoOrigemTipoEnum.VAGA && !input.TbVagaId.HasValue)
        {
            mensagem = "TbVagaId é obrigatório quando OrigemTipo = VAGA.";
            return false;
        }
        if (input.OrigemTipo == AdmissaoOrigemTipoEnum.CANDIDATURA && string.IsNullOrWhiteSpace(input.TbCandidatoVagaId))
        {
            mensagem = "TbCandidatoVagaId é obrigatório quando OrigemTipo = CANDIDATURA.";
            return false;
        }
        mensagem = null;
        return true;
    }
}
