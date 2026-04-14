namespace DataTransferObject.Domain.Social.AtendimentoFourmakers;

public class KbChunkSimilarResult
{
    public string Id { get; set; }
    public string Conteudo { get; set; }
    public double Similaridade { get; set; }
    public string TipoFonte { get; set; }
    public string FonteId { get; set; }
    public string Titulo { get; set; }
    public int IndiceChunk { get; set; }
}
