using System.Collections.Generic;

namespace DataTransferObject.Domain.Social.AtendimentoFourmakers;

public class WebhookWhatsAppResult
{
    public bool Ok { get; set; }
    public int Recebidas { get; set; }
    public int Respondidas { get; set; }
    public int ChamadosCriados { get; set; }
    public List<string> Erros { get; set; }
}
