using System;
using System.Collections.Generic;

namespace DataTransferObject.Domain.Colaborador;

public class ProcessamentoPdfResult
{
    public string NomeArquivo { get; set; }
    public bool AProcessar { get; set; }
    public string Erro { get; set; }
}

public class ProcessamentoZipResult
{
    public List<ProcessamentoPdfResult> PdfsProcessados { get; set; } = new List<ProcessamentoPdfResult>();
    public int TotalArquivos { get; set; }
    public int ArquivosAProcessar { get; set; }
    public int ArquivosComErro { get; set; }
    public string IdLote { get; set; }
} 