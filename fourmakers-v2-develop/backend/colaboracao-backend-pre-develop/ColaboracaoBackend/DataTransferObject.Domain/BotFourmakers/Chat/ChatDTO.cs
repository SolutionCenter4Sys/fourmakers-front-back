
using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.BotFourmakers.Chat;

public class ChatDTO
{
    public int Id { get; set; }
    public string Mensagem { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; }
    
    [JsonIgnore]
    public string CodigoInternoColaborador { get; set; } = string.Empty;
}