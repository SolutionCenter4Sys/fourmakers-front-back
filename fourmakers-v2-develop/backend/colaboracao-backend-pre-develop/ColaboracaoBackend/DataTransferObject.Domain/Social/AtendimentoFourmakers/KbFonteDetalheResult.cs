using System;
using System.Collections.Generic;

namespace DataTransferObject.Domain.Social.AtendimentoFourmakers;

public class KbFonteDetalheResult
{
    public string FonteId { get; set; }
    public string Titulo { get; set; }
    public string TipoFonte { get; set; }
    public string AreaId { get; set; }
    public string AreaNome { get; set; }
    public int ChunkCount { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime DataAlteracao { get; set; }
    public List<KbChunkResumoResult> Chunks { get; set; }
}

public class KbChunkResumoResult
{
    public string Id { get; set; }
    public int IndiceChunk { get; set; }
    public string Conteudo { get; set; }
    public DateTime DataCriacao { get; set; }
}
