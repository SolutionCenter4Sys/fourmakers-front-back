using System;
using System.Collections.Generic;

namespace DataTransferObject.Domain.Colaborador;

public class ProcessamentoPlanilhaResult
{
    public int TotalLinhas { get; set; }
    public int LinhasAProcessar { get; set; }
    public int LinhasComErro { get; set; }
    public string IdLote { get; set; }
} 