using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Financeiro.NotaFiscal;

namespace Financeiro.Domain.Interfaces.NotaFiscal;

public interface INotaFiscalExternoService
{
    Task<ApiGenericResult<IEnumerable<NotaFiscalResult>>> ProcessarNotaFiscalAprovadaAsync(string tokenSistema, string competencia, bool atualizarParaPago, string? codDiretoria, string? documentoColaborador, string? codigoColaboradorExternoAprovador = "", string numeroNf = "");
    Task<ApiGenericResult<LiberarEmissaoDeNfsResult>> LiberarEmissaoDeNotasFiscaisPorVigenciaExterno(string tokenSistema, string competencia, string? codDiretoria, bool enviarEmail, string codigoColaboradorExternoEmissao);
}