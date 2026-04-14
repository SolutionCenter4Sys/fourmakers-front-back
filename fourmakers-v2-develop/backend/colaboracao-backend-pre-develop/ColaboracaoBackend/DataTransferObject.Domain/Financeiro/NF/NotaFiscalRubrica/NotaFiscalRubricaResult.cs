using System;

namespace DataTransferObject.Domain.Financeiro.NF.NotaFiscalRubrica;

public class NotaFiscalRubricaResult : NotaFiscalRubricaBase
{
   public string RubricaDescricao  { get; set; }
   public string Tipo { get; set; }
   public string CodigoRubrica { get; set; }
   public string Natureza { get; set; }
}