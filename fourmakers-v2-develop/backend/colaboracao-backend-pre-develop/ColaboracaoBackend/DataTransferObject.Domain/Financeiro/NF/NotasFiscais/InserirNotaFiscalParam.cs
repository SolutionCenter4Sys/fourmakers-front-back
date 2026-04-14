using System;
using System.Collections.Generic;
using DataTransferObject.Domain.Arquivo;

namespace DataTransferObject.Domain.Financeiro.NotaFiscal;

public class InserirNotaFiscalParam
{
    public Base64DTO Base64Objeto { get; set; }
    public string NumeroNf { get; set; }
    public int VigenciaMes  { get; set; }
    public int VigenciaAno { get; set; }
    public List<Guid> ListaDeIdsRubricasLiberacao { get; set; }
}