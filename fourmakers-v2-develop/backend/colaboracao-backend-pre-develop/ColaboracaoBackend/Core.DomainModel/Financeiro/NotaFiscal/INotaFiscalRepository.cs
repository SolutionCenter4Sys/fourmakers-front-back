using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain.Financeiro.NotaFiscal;

namespace Core.Domain.Financeiro.NotaFiscal;

public interface INotaFiscalRepository
{
    Task<Guid> InserirNotaFiscalAsync(int vigenciaMes, int vigenciaAno, string codigoInternoColaborador, int orgId, decimal valor, string numeroNf, decimal? valorAnalise);
    Task<NotaFiscalResult> ObterNotaFiscalPorIdAsync(Guid id);
    Task<IEnumerable<NotaFiscalResult>> MudarStatusNotaFiscalPorListaDeIdsAsync(List<Guid> ids, NotaFiscalStatusEnum notaFiscalStatusId, string motivoReprovacao, string codigoInternoColaboradorAlteracao);
    Task<IEnumerable<NotaFiscalResult>> ListarNotasFiscaisPorVigencia(string filtro, NotaFiscalStatusEnum? statusId, int? mes, int? ano, List<string>? diretorias, string? documentoColaborador, string codigoInternoColaborador, int orgId, int cursor, int? limite, string numeroNf = "");
    Task<IEnumerable<NotaFiscalResult>> ListarNotaFiscaisPorListaDeIdsAsync(List<Guid> ids);
    Task EditarNotaFiscalPorIdAsync(Guid id, string numeroNf, DateTime? dataEmissaoNotaFiscal, decimal? valor, string urlNotaFiscalDownload, NotaFiscalStatusEnum? notaFiscalStatusId, string codigoInternoColaboradorAlteracao);
    Task<IEnumerable<NotaFiscalStatusDTO>> ListarNotaFiscalStatus();
    Task<List<Guid>> ListarIdsNotasFiscaisPendentesParaEmissao(int orgId, int mes, int ano, string? codigoDiretoria);
    Task<string> BuscarCodigoInternoColaboradorPorExterno(string codigoColaboradorExterno, int orgId);

}