using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain.Financeiro.Reembolso.Verba;

namespace Core.Domain.Reembolso.Verba;

public interface IVerbaLogRepository
{
    Task InserirLogAsync(string regra, string acao, string valorAnterior, string novoValor, string codigoInternoColaborador, int orgId);
    Task<List<VerbaLogDTO>> ListarLogsAsync(int orgId);
}