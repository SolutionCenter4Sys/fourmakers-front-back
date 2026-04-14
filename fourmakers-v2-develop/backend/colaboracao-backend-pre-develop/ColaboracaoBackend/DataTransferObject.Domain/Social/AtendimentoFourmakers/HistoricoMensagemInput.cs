using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Social.AtendimentoFourmakers;

public class HistoricoMensagemInput
{
    public string Role { get; set; }

    [JsonPropertyName("content")]
    public string Conteudo { get; set; }
}
