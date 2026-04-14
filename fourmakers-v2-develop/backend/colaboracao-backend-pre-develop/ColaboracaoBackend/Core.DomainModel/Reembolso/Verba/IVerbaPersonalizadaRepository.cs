using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Financeiro.Reembolso.Verba;

namespace Core.Domain.Reembolso.Verba;

public interface IVerbaPersonalizadaRepository
{
    Task<List<SimpleColaboradorDTO>> ListarColaboradoresAlocadosPorClienteOuProjeto(int orgId, string? clienteId, string? projetoId);
    Task<VerbaPersonalizadaColaboradorDTO> InserirVerbaPersonalizada(int orgId, string? codigoInternoColaborador, int verbaId, string? projetoId, string? clienteId, decimal valor, bool ativo, bool custoCliente);
    Task<VerbaPersonalizadaColaboradorDTO> EditarVerbaPersonalizada(int id, string? codigoInternoColaborador, decimal valor, bool ativo, bool custoCliente);
    Task<List<VerbaPersonalizadaColaboradorDTO>> ListarVerbasPersonalizadasPorOrg(int orgId, string? clienteId, string? projetoId);
    Task<VerbaPersonalizadaColaboradorDTO> BuscarVerbaPersonalizadaPorId(int id);
    Task<VerbaPersonalizadaColaboradorDTO> BuscarVerbasPersonalizadaPorOrg(int orgId, int verbaId, string? clienteId, string? projetoId, string? codigoInternoColaborador);
    Task<bool> InativarVerbaPersonalizadaPorId(int id);
}