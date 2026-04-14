using Colaboracao.Helper.Enum;
using Core.Domain;
using Core.Domain.Reembolso.Verba;
using DataTransferObject.Domain.Financeiro.Reembolso.Verba;
using Financeiro.Domain.Interfaces.Reembolso.Validadores;
using Logs.Infra.Attributes;

namespace Financeiro.Domain.Services.Reembolso.Validadores;

[LogDomainClass]
public class VerbaPersonalizadaValidadorService : IVerbaPersonalizadaValidadorService
{
    private readonly IVerbaRepository _verbaRepository;
    private readonly IVerbaPersonalizadaRepository _verbaPersonalizadaRepository;

    public VerbaPersonalizadaValidadorService(IVerbaRepository verbaRepository, IVerbaPersonalizadaRepository verbaPersonalizadaRepository)
    {
        _verbaRepository = verbaRepository;
        _verbaPersonalizadaRepository = verbaPersonalizadaRepository;
    }

    public async Task ValidaVerbaPersonalizada(VerbaPersonalizadaInput input, CRUDEnum operacao, int orgId)
    {
        if (operacao == CRUDEnum.Create)
        {
            await ValidaVerba(input.VerbaId);
        }

        if (operacao == CRUDEnum.Update || operacao == CRUDEnum.Create)
        {
            ValidaValorDaVerbaPersonalizada(input.Valor);
            AjustaClienteEProjetoId(input);
            await ValidaSeExisteCombinacao(input, orgId);
        }

        if (operacao == CRUDEnum.Update || operacao == CRUDEnum.Delete) 
        {
            await ValidaSeExisteParaEdicao(input.Id);
        }
    }

    private async Task ValidaVerba(int verbaId)
    {
        if (verbaId == null || verbaId == 0)
        {
            throw new ArgumentException("ID da verba vazio");
        }
        var buscarVerba = await _verbaRepository.ObterPorIdAsync(verbaId);
        if (buscarVerba == null)
        {
            throw new ArgumentException("Verba não encontrada.");
        }
    }

    private void ValidaValorDaVerbaPersonalizada(decimal valor)
    {
        if (valor <= 0)
        {
            throw new ArgumentException("Valor da verba vazio");
        }
    }

    private void AjustaClienteEProjetoId(VerbaPersonalizadaInput input)
    {
        if (!string.IsNullOrEmpty(input.ClienteId) && !string.IsNullOrEmpty(input.ProjetoId))
        {
            input.ClienteId = null;
        }
    }

    private async Task ValidaSeExisteParaEdicao(int? id)
    {
        if (id == 0 || id == null)
        {
            throw new ArgumentException("ID da verba personalizada vazia");
        }

        var buscaVerbaPersonalizada = await _verbaPersonalizadaRepository.BuscarVerbaPersonalizadaPorId(id.Value);
        if (buscaVerbaPersonalizada == null)
        {
            throw new ArgumentException("Verba personalizada não encontrada");
        }
    }

    private async Task ValidaSeExisteCombinacao(VerbaPersonalizadaInput input, int orgId)
    {
        var verificaSeJaExiste = await _verbaPersonalizadaRepository.BuscarVerbasPersonalizadaPorOrg(orgId, input.VerbaId,  input.ClienteId, input.ProjetoId, input.CodigoInternoColaborador);
        if (verificaSeJaExiste != null)
        {
            throw new ArgumentException("Já existe um registro com o mesmo colaborador, ou uma regra geral cadastrada.");
        }
    }
}