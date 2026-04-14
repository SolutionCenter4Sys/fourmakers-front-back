using System;

namespace DataTransferObject.Domain.MapaDeRelacionamento.VCX;

public class OrganogramaPosicaoOrcamentoHistoricoResponseDTO
{
    public string Id { get; set; }
    public string OrganogramaPosicaoId { get; set; }
    public int OrgId { get; set; }
    public decimal Orcamento { get; set; }
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public string CodigoInternoColaboradorAlterador { get; set; }
    public string NomeColaboradorAlterador { get; set; }
    public DateTime DataCriacao { get; set; }
}

public class OrganogramaPosicaoOrcamentoHistoricoInserirParamDTO
{
    public decimal Orcamento { get; set; }
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
}
