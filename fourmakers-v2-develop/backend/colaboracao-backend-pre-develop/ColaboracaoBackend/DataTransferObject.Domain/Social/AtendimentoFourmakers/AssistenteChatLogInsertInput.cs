namespace DataTransferObject.Domain.Social.AtendimentoFourmakers;

public class AssistenteChatLogInsertInput
{
    public string CodigoInternoColaborador { get; set; }
    public string MensagemUsuario { get; set; }
    public string MensagemAssistente { get; set; }
    public string Fontes { get; set; }
    public int QtdChunks { get; set; }
    public double? SimilaridadeMax { get; set; }
    public bool ExibirChamado { get; set; }
}
