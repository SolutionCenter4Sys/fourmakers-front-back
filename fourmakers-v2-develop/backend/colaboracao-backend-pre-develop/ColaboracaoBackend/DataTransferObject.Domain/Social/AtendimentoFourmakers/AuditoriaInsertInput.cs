namespace DataTransferObject.Domain.Social.AtendimentoFourmakers;

public class AuditoriaInsertInput
{
    public string CodigoInternoColaborador { get; set; }
    public string ActorLabel { get; set; }
    public string Acao { get; set; }
    public string Detalhe { get; set; }
    public string Payload { get; set; }
    public string TipoEntidade { get; set; }
    public string EntidadeId { get; set; }
}
