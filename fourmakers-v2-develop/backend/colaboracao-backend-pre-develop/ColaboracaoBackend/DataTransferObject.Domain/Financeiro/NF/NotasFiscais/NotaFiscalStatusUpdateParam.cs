using System;
using System.Collections.Generic;

namespace DataTransferObject.Domain.Financeiro.NotaFiscal;

public class NotaFiscalStatusUpdateParam
{
    public List<Guid> Ids { get; set; }
    public string MotivoReprovacao { get; set; } = null;
}