using System;

namespace DataTransferObject.Domain.Social.AtendimentoFourmakers;

public class KbFonteResult
{
    public string FonteId { get; set; }
    public string Titulo { get; set; }
    public string TipoFonte { get; set; }
    public int ChunkCount { get; set; }
    public string AreaId { get; set; }
    public string AreaNome { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime DataAlteracao { get; set; }
}
