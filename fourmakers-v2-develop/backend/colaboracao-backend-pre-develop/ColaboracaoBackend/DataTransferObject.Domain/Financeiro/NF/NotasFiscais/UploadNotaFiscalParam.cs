using System;
using DataTransferObject.Domain.Arquivo;

namespace DataTransferObject.Domain.Financeiro.NotaFiscal;

public class UploadNotaFiscalParam
{
    public Base64DTO Base64Objeto { get; set; }
    public string NumeroNf { get; set; }
    public Guid NotaFiscalId { get; set; }
}