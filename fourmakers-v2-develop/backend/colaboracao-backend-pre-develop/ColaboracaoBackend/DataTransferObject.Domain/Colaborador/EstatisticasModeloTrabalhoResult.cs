using DataTransferObject.Domain.Base;
using System.Collections.Generic;

public class EstatisticasModeloTrabalhoResult : StatusResult
{
    public List<ModeloTrabalhoDTO> ModelosTrabalho { get; set; } = new List<ModeloTrabalhoDTO>();
}

public class ModeloTrabalhoDTO
{
    public string ModeloTrabalho { get; set; }
    public long QtdUsuarios { get; set; }
    public List<DiasPorSemanaDTO> DiasPorSemana { get; set; } = null;
}

public class DiasPorSemanaDTO
{
    public long Dias { get; set; }
    public long QtdUsuarios { get; set; }
}