using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Financeiro.Reembolso.Verba;

namespace Financeiro.Domain.Interfaces.Reembolso.Verba;

public interface IVerbaPersonalizadaService
{
    Task<ApiGenericResult<List<SimpleColaboradorDTO>>> ListarColaboradoresAlocadosPorClienteOuProjeto(int orgId, string? clienteId, string? projetoId);
    Task<ApiGenericResult<List<VerbaPersonalizadaCompletaDTO>>> InserirListaDeVerbasPersonalizadas(InserirVerbaPersonalizadaParam param, int orgId);
    Task<ApiGenericResult<List<VerbaPersonalizadaCompletaDTO>>> ListarVerbasPersonalizadasPorOrg(int orgId, string? clienteId, string? projetoId);
    Task<ApiGenericResult<bool>> InativarVerbaPersonalizada(VerbaPersonalizadaInput verbaPersonalizada, int orgId);
}