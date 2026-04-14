namespace DataTransferObject.Domain.Social.AtendimentoFourmakers;

public class FiltroLogsInput
{
    public int? Limite { get; set; }
    public int? Offset { get; set; }
    public string StatusCuradoria { get; set; }
    public string Feedback { get; set; }
}
