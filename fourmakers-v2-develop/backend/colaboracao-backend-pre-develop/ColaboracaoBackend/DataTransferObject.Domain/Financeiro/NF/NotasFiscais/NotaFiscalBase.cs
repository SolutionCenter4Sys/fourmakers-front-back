using System;
using Newtonsoft.Json;

namespace DataTransferObject.Domain.Financeiro.NotaFiscal;

public class NotaFiscalBase
{
    public int? VigenciaMes { get; set; } = null;
    public int? VigenciaAno { get; set; } = null;
    public string NumeroNf { get; set; } = null;
    public DateTime? DataEmissaoNotaFiscal { get; set; } = null;
    public Decimal? Valor  { get; set; } = null;
    public Decimal? valorAnalisado  { get; set; } = null;
    public string UrlNotaFiscalDownload { get; set; } = null;
    public NotaFiscalStatusEnum? NotaFiscalStatusId { get; set; } = null;
    public string MotivoReprovacao { get; set; } = null;
}