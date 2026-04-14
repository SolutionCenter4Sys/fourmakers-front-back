using System;

namespace DataTransferObject.Domain.Social.AtendimentoFourmakers;

public class AssistenteChatLogResult
{
    public string Id { get; set; }
    public int TbOrgId { get; set; }
    public string CodigoInternoColaborador { get; set; }
    public string MensagemUsuario { get; set; }
    public string MensagemAssistente { get; set; }
    public string Fontes { get; set; }
    public int QtdChunks { get; set; }
    public double? SimilaridadeMax { get; set; }
    public bool ExibirChamado { get; set; }
    public string Feedback { get; set; }
    public DateTime? FeedbackEm { get; set; }
    public string StatusCuradoria { get; set; }
    public string NotaCurador { get; set; }
    public DateTime DataCriacao { get; set; }
}
