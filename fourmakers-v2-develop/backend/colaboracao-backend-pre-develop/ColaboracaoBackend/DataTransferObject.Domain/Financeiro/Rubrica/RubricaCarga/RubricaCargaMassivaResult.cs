using System.Collections.Generic;

namespace DataTransferObject.Domain.Financeiro.Rubrica.RubricaCarga;

public class RubricaCargaMassivaResult
{
    public int TotalLinhas { get; set; }
    public int LinhasProcessadas { get; set; }
    public int LinhasComErro { get; set; }
    public int LinhasDuplicadas { get; set; }
    public List<string> MensagensErro { get; set; } = new List<string>();
    public List<string> MensagensAlerta { get; set; } = new List<string>();
    public bool Sucesso { get; set; }
    public string CargaId { get; set; }
    public List<RubricaCargaItemProcessadoDTO> ItensProcessados { get; set; } = new List<RubricaCargaItemProcessadoDTO>();
}