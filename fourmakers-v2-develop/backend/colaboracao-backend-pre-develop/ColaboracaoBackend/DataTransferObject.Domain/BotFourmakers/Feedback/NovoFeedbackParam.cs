namespace DataTransferObject.Domain.BotFourmakers.Feedback;

public class NovoFeedbackParam
{
    public int QuestaoId { get; set; }
    public bool Feedback { get; set; }
    public string? Comentario { get; set; }
}