namespace DataTransferObject.Domain.Social.AtendimentoFourmakers;

public class AssistenteConfigUpdateInput
{
    public string NomeAssistente { get; set; }
    public string MensagemBoasVindas { get; set; }
    public string AcoesRapidasJson { get; set; }
    public double? LimiarSimilaridadeChamado { get; set; }
    public int? RagTopK { get; set; }
    public double? RagSimilaridadeMinima { get; set; }
    public string InstrucaoSistemaExtra { get; set; }
    public string AlteradoPorCodigoInternoColaborador { get; set; }
}
