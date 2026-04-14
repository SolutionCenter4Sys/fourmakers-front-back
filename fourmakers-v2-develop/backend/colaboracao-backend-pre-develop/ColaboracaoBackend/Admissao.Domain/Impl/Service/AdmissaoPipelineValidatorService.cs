using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Admissao.Domain.Interfaces.Service;
using Core.Domain.SRS;
using Core.Domain.Usuario.Permissao;
using DataTransferObject.Domain.SRS.AdmissaoPipeline;
using Logs.Infra.Attributes;

namespace Admissao.Domain.Impl.Service;

[LogDomainClass]
public class AdmissaoPipelineValidatorService : IAdmissaoPipelineValidatorService
{
    private readonly IFuncionalidadeSistemaRepository _funcionalidadeSistemaRepository;
    private readonly IAdmissaoStatusRepository _admissaoStatusRepository;

    public AdmissaoPipelineValidatorService(
        IFuncionalidadeSistemaRepository funcionalidadeSistemaRepository,
        IAdmissaoStatusRepository admissaoStatusRepository)
    {
        _funcionalidadeSistemaRepository = funcionalidadeSistemaRepository;
        _admissaoStatusRepository = admissaoStatusRepository;
    }

    public async Task ValidarAcessoListarAsync(string cpf, int orgId)
    {
        //if (!await _funcionalidadeSistemaRepository.ValidaAcessoFuncionalidade(cpf, orgId, FuncionalidadeSistemaEnum.ADMISSAO_PIPELINE_LISTAR))
        //    throw new UnauthorizedAccessException("Acesso negado. Funcionalidade ADMISSAO_PIPELINE_LISTAR necessária.");
        await Task.CompletedTask;
    }

    public async Task ValidarAcessoCriarEditarAsync(string cpf, int orgId)
    {
        //if (!await _funcionalidadeSistemaRepository.ValidaAcessoFuncionalidade(cpf, orgId, FuncionalidadeSistemaEnum.ADMISSAO_PIPELINE_CRIAR_EDITAR))
        //    throw new UnauthorizedAccessException("Acesso negado. Funcionalidade ADMISSAO_PIPELINE_CRIAR_EDITAR necessária.");
        await Task.CompletedTask;
    }

    public async Task ValidarStatusItensAsync(int orgId, IReadOnlyList<AdmissaoPipelineStatusItemInput> itens)
    {
        if (itens == null) return;

        var msgPura = ValidarRegrasPuras(itens);
        if (msgPura != null)
            throw new InvalidOperationException(msgPura);

        foreach (var item in itens)
        {
            var st = await _admissaoStatusRepository.ObterPorIdAsync(item.AdmissaoStatusId, orgId);
            if (st == null)
                throw new InvalidOperationException($"Status de admissão não encontrado na organização (id {item.AdmissaoStatusId}).");
        }
    }

    /// <summary>Validações puras (sem I/O): estrutura e consistência dos itens.</summary>
    private static string ValidarRegrasPuras(IReadOnlyList<AdmissaoPipelineStatusItemInput> itens)
    {
        if (itens.Count == 0) return null;
        if (itens.Any(i => i.AdmissaoStatusId == Guid.Empty))
            return "Cada item deve informar admissaoStatusId válido.";
        if (itens.Select(i => i.AdmissaoStatusId).Distinct().Count() != itens.Count)
            return "Não é permitido repetir o mesmo status no pipeline.";
        if (itens.Select(i => i.Ordem).Distinct().Count() != itens.Count)
            return "Não é permitido repetir a mesma ordem no pipeline.";
        if (itens.Any(i => i.Ordem <= 0))
            return "Ordem deve ser maior que zero.";
        if (itens.Count(i => i.StatusInicial) != 1)
            return "Deve haver exatamente um status inicial no pipeline.";
        if (itens.Any(i => i.StatusInicial && i.StatusFinal))
            return "Um status não pode ser marcado como inicial e final ao mesmo tempo.";
        return null;
    }
}
