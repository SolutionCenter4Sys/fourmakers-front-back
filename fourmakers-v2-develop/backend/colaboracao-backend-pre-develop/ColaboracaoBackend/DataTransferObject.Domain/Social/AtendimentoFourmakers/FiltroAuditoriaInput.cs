namespace DataTransferObject.Domain.Social.AtendimentoFourmakers;

public class FiltroAuditoriaInput
{
    public int? Limite { get; set; }
    public int? Offset { get; set; }
    public string DataInicio { get; set; }
    public string DataFim { get; set; }
    public string Categoria { get; set; }
    public string Busca { get; set; }
    public string Formato { get; set; }
}
