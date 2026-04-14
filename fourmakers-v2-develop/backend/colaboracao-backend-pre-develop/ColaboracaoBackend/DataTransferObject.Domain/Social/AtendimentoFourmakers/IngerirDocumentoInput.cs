namespace DataTransferObject.Domain.Social.AtendimentoFourmakers;

public class IngerirDocumentoInput
{
    public string Texto { get; set; }
    public string Titulo { get; set; }
    public string TipoFonte { get; set; }
    public string FonteId { get; set; }
    public bool SubstituirFonte { get; set; }
    public string AreaId { get; set; }
}
