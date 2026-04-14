using System;
using System.Threading.Tasks;
using Admissao.Domain.Interfaces.Service;
using Core.Domain.SRS;
using Logs.Infra.Attributes;

namespace Admissao.Domain.Impl.Service;

[LogDomainClass]
public class AdmissaoValidatorService : IAdmissaoValidatorService
{
    private readonly IAdmissaoPipelineRepository _pipelineRepository;
    private readonly IAdmissaoStatusRepository _statusRepository;
    private readonly IAdmissaoColaboradorRepository _colaboradorRepository;

    public AdmissaoValidatorService(
        IAdmissaoPipelineRepository pipelineRepository,
        IAdmissaoStatusRepository statusRepository,
        IAdmissaoColaboradorRepository colaboradorRepository)
    {
        _pipelineRepository = pipelineRepository;
        _statusRepository = statusRepository;
        _colaboradorRepository = colaboradorRepository;
    }

    public async Task ValidarPipelineExisteAsync(Guid pipelineId, int orgId)
    {
        if (pipelineId == Guid.Empty)
            throw new InvalidOperationException("Pipeline de admissão é obrigatório.");

        var pipeline = await _pipelineRepository.ObterPorIdAsync(pipelineId, orgId);
        if (pipeline == null)
            throw new InvalidOperationException($"Pipeline de admissão não encontrado na organização (id {pipelineId}).");
        if (!pipeline.Ativo)
            throw new InvalidOperationException($"Pipeline de admissão está inativo (id {pipelineId}).");
    }

    public async Task ValidarStatusExisteAsync(Guid statusId, int orgId)
    {
        if (statusId == Guid.Empty)
            throw new InvalidOperationException("Status de admissão é obrigatório.");

        var status = await _statusRepository.ObterPorIdAsync(statusId, orgId);
        if (status == null)
            throw new InvalidOperationException($"Status de admissão não encontrado na organização (id {statusId}).");
        if (!status.Ativo)
            throw new InvalidOperationException($"Status de admissão está inativo (id {statusId}).");
    }

    public async Task ValidarColaboradorExisteAsync(string codigoInternoColaborador)
    {
        if (string.IsNullOrWhiteSpace(codigoInternoColaborador))
            return;

        var existe = await _colaboradorRepository.ExisteAsync(codigoInternoColaborador.Trim());
        if (!existe)
            throw new InvalidOperationException($"Colaborador não encontrado ou inativo (código: {codigoInternoColaborador}).");
    }
}
