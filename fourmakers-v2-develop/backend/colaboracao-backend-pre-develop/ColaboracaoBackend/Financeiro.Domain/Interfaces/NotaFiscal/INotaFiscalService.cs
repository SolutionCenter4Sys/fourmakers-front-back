using Colaboracao.Helper.Enum;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Financeiro.NotaFiscal;
using DataTransferObject.Domain.Financeiro.Rubrica.RubricaColaboradorLiberacao;

namespace Financeiro.Domain.Interfaces.NotaFiscal;

public interface INotaFiscalService
{
    Task<ApiGenericResult<IEnumerable<NotaFiscalResult>>> ListarNotasFiscaisPorVigenciaAsync(string? filtro, NotaFiscalStatusEnum? statusId, int? mes, int? ano, string? codDiretoria, string? documentoColaborador, string? codigoInternoColaborador, int cursor, int limite, int orgId, string? cpfGestor, string numeroNf = "");
    Task<ApiGenericResult> AprovarNotasFiscais(NotaFiscalStatusUpdateParam param, string? cpfRequest, int orgId);
    Task<ApiGenericResult> ReprovarNotasFiscais(NotaFiscalStatusUpdateParam param, string? cpfRequest, int orgId);
    Task<ApiGenericResult<IEnumerable<NotaFiscalStatusDTO>>> ListarNotaFiscalStatus();
    Task<ApiGenericResult<NotaFiscalResult>> UploadNotaFiscal(UploadNotaFiscalParam input, string cpfRequest, bool useTransaction = true);
    Task<ApiGenericResult<NotaFiscalResult>> CancelarEnvioNf(Guid notaFiscalId, string cpfRequest);
    Task<ApiGenericResult<LiberarEmissaoDeNfsResult>> LiberarEmissaoDeNotasFiscaisPorVigencia(int orgId, int mes, int ano, string? codigoDiretoria, bool enviarEmail, string cpfRequest);
    Task<ApiGenericResult> GerarPagamentoDeSolicitacoesPorIds(NotaFiscalStatusUpdateParam param, string? cpfRequest, int orgId, bool validaAcesso = true);
    Task<ApiGenericResult<List<RubricaColaboradorLiberacaoNfDTO>>> ListarRubricasColaboradorParaLiberacaoDeNf(string codigoInternoColaborador, int? mes, int? ano, int orgId);
    Task<ApiGenericResult<NotaFiscalResult>> InserirNotaFiscal(InserirNotaFiscalParam input, string codigoInternoColaborador, int orgId);
    Task<ApiGenericResult<IEnumerable<NotaFiscalResult>>> ListarNotasFiscaisPorVigenciaComColaboradoresSemNFAsync(string? filtro, NotaFiscalStatusEnum? statusId, int? mes, int? ano, string? codDiretoria, string? documentoColaborador, string? codigoInternoColaborador, int cursor, int limite, int orgId, string? cpfGestor, string numeroNf = "");
}