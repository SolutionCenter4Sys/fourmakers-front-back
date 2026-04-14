using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.BotFourmakers.ChatIA;

public class ChatIAQuestionReponseDTO
{
    public string Response { get; set; }
    [JsonPropertyName("query_mapa_alocacao")]
    public string QueryMapaAlocacao  { get; set; }
    [JsonPropertyName("query_habilidade")]
    public string QueryHabilidade  { get; set; }
}