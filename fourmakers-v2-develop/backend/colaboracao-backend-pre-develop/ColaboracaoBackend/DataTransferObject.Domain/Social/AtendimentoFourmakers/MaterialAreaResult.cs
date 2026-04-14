using System;

namespace DataTransferObject.Domain.Social.AtendimentoFourmakers;

public class MaterialAreaResult
{
    public string Id { get; set; }
    public int TbOrgId { get; set; }
    public string Nome { get; set; }
    public string Slug { get; set; }
    public int Ordem { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime DataAlteracao { get; set; }
}
