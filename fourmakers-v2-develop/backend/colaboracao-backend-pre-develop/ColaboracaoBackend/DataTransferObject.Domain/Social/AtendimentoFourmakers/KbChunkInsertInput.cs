namespace DataTransferObject.Domain.Social.AtendimentoFourmakers;

public class KbChunkInsertInput
{
    public string Conteudo { get; set; }
    public string EmbeddingJson { get; set; }
    public string TipoFonte { get; set; }
    public string FonteId { get; set; }
    public string Titulo { get; set; }
    public int IndiceChunk { get; set; }
}
