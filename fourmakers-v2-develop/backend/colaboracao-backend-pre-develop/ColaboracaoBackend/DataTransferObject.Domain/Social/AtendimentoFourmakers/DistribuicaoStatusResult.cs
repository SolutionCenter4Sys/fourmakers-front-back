using System.Collections.Generic;

namespace DataTransferObject.Domain.Social.AtendimentoFourmakers;

public class DistribuicaoStatusItem
{
    public string Nome { get; set; }
    public int Valor { get; set; }
    public string Cor { get; set; }
}

public class DistribuicaoStatusResult
{
    public List<DistribuicaoStatusItem> Itens { get; set; }
    public int Total { get; set; }
}
