using System;

namespace DataTransferObject.Domain.Financeiro.Rubrica.RubricaCarga;

public class RubricaCargaOrigemPdfDTO
{
    public string PathPdf { get; set; } = string.Empty;
    public Guid RubricaId { get; set; }
}