using System;
using System.Text.Json.Serialization;
using DataTransferObject.Domain.BotFourmakers.Chat;
using DataTransferObject.Domain.BotFourmakers.Feedback;

namespace DataTransferObject.Domain.BotFourmakers.Questao;

public class QuestaoDTO
{
    public int Id { get; set; }
    public int ChatId { get; set; }
    public string Mensagem { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; } 
    public ChatDTO? Chat { get; set; } = null;
    public FeedbackDTO? Feedback { get; set; } = null;
    public int Tipo { get; set; }
    [JsonIgnore]
    public int? ReferenciaRespostaId { get; set; }
    [JsonIgnore]
    public string Query { get; set; } = string.Empty;
    public string QueryHabilidades { get; set; } = string.Empty;

}