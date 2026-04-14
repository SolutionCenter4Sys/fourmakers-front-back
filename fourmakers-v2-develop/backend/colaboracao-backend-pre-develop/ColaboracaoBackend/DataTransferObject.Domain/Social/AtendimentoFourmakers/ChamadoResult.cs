using System;

namespace DataTransferObject.Domain.Social.AtendimentoFourmakers;

public class ChamadoResult
{
    public string Id { get; set; }
    public int TbOrgId { get; set; }
    public string CodigoInternoColaborador { get; set; }
    public string CooperadoLabel { get; set; }
    public string Assunto { get; set; }
    public string Descricao { get; set; }
    public string Status { get; set; }
    public string Prioridade { get; set; }
    public string NotasResolucao { get; set; }
    public string RespostaPublica { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime DataAlteracao { get; set; }
}
