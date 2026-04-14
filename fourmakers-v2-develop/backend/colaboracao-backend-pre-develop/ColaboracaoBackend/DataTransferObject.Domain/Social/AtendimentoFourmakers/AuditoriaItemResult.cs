using System;

namespace DataTransferObject.Domain.Social.AtendimentoFourmakers;

public class AuditoriaItemResult
{
    public string Id { get; set; }
    public DateTime DataCriacao { get; set; }
    public string ActorLabel { get; set; }
    public string Acao { get; set; }
    public string Detalhe { get; set; }
    public string Categoria { get; set; }
    public string TipoEntidade { get; set; }
    public string EntidadeId { get; set; }
}
