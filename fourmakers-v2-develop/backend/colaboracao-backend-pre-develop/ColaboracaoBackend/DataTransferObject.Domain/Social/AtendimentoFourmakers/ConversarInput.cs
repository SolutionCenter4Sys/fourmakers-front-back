using System.Collections.Generic;

namespace DataTransferObject.Domain.Social.AtendimentoFourmakers;

public class ConversarInput
{
    public string Mensagem { get; set; }
    public List<HistoricoMensagemInput> Historico { get; set; }
}
