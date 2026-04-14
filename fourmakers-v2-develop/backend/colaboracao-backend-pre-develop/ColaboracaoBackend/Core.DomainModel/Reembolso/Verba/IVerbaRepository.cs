using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain.Financeiro.Reembolso.Verba;

namespace Core.Domain.Reembolso.Verba;

public interface IVerbaRepository
{
    Task<VerbaDTO> InserirAsync(string categoria, int tipoCusto, string unidade, decimal valor, bool custoCliente, int orgId, bool ativo, string codigoInternoColaborador);
    Task<VerbaDTO> EditarAsync(int id, string categoria, int tipoCusto, string unidade, decimal valor, bool custoCliente, bool ativo, string codigoInternoColaborador);
    Task<List<VerbaDTO>> ListarAsync(int orgId);
    Task<VerbaDTO> ObterPorIdAsync(int id);
    Task<List<VerbaSimplificadoDTO>> ListarSimplificadoAsync(int orgId);
    Task<decimal?> ObterExcecaoDaVerbaPorColaboradorProjetoECliente(int verbaId, int orgId, string codigoCliente, string codigoProjeto, string codigoColaborador);
}