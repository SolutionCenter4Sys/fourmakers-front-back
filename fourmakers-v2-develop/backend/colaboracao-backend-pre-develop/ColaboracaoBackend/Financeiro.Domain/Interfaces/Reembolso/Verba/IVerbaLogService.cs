using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Financeiro.Reembolso.Verba;

namespace Financeiro.Domain.Interfaces.Reembolso.Verba;

public interface IVerbaLogService
{
    Task InserirLogAsync(string regra, string acao, string valorAnterior, string novoValor, string codigoInternoColaborador, int orgId);
    Task<ApiGenericResult<List<VerbaLogDTO>>> ListarLogsAsync(int orgId);
}