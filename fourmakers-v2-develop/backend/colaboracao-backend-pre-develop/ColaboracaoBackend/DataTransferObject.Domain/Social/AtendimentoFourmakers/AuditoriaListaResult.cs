using System.Collections.Generic;

namespace DataTransferObject.Domain.Social.AtendimentoFourmakers;

public class AuditoriaListaResult
{
    public List<AuditoriaItemResult> Itens { get; set; }
    public bool TemMais { get; set; }
    public int Limite { get; set; }
    public int Offset { get; set; }
}
