using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using DataTransferObject.Domain.Financeiro.NF.NotaFiscalRubrica;

namespace DataTransferObject.Domain.Financeiro.NotaFiscal;

public class NotaFiscalResult : NotaFiscalBase
{
    public Guid Id  { get; set; }
    [JsonIgnore]
    public string? CodigoInternoColaboradorCriacao  { get; set; }
    [JsonIgnore]
    public string? CodigoInternoColaboradorAlteracao { get; set; }
    [JsonIgnore]
    public string Email { get; set; } = null;
    public string NomeColaborador { get; set; }
    public string DocumentoColaborador { get; set; }
    
    public string NotaFiscalStatusDescricao { get; set; }
    public IEnumerable<NotaFiscalRubricaResult> Rubricas { get; set; } = [];
    public decimal? SumarioValorTotalDeRubricas { get; set; } = 0M;

}