using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Financeiro.NF.NotaFiscalRubrica;

public class NotaFiscalRubricaBase
{
    public Guid Id { get; set; }
    [JsonIgnore]
    public Guid NotaFiscalId  { get; set; }
    [JsonIgnore]
    public Guid RubricaColaboradorId  { get; set; } = Guid.Empty;
    public decimal Valor { get; set; }
}