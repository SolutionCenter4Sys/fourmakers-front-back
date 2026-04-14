using System.Collections.Generic;

namespace DataTransferObject.Domain.Social.AtendimentoFourmakers;

public class AtualizarAssistenteConfigInput
{
    public string NomeAssistente { get; set; }
    public string MensagemBoasVindas { get; set; }
    public List<string> AcoesRapidas { get; set; }
    public int? LimiarSimilaridadeChamadoPercent { get; set; }
    public int? RagTopK { get; set; }
    public double? RagSimilaridadeMinima { get; set; }
    public string InstrucaoSistemaExtra { get; set; }
}
