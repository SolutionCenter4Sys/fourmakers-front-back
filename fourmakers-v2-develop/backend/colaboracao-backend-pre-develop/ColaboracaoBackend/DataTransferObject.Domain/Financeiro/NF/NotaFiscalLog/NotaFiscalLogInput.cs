using System;

namespace DataTransferObject.Domain.Financeiro.NotaFiscal;

public class NotaFiscalLogInput
{
    public Guid Id { get; set; }
    public Guid? NotaFiscalId { get; set; } = null;
    public NotaFiscalStatusEnum? NotaFiscalStatusAnteriorId { get; set; } = null;
    public NotaFiscalStatusEnum? NotaFiscalStatusNovoId { get; set; } = null;
    public string NumeroNf { get; set; } = null;
    public DateTime? DataEmissaoNotaFiscal { get; set; } = null;
    public string Observacao { get; set; } = null;
    public DateTime? DataCriacao { get; set; } = null;
    public DateTime? DataAlteracao { get; set; } = null;
    public Guid? CodigoInternoColaboradorCriacao  { get; set; } = Guid.Empty;
    public Guid? CodigoInternoColaboradorAlteracao { get; set; } = Guid.Empty;
}