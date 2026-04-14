namespace DataTransferObject.Domain.SRS;

/// <summary>
/// Linha de tb_parametrizacao_simulador (1 por organização).
/// </summary>
public class ParametrizacaoSimuladorResult
{
    public int TbOrgId { get; set; }
    public decimal PorcentagemMinimaPiso { get; set; }
    public decimal PorcentagemExcedenteCusto { get; set; }
    public decimal PorcentagemMargemCusto { get; set; }
    public int QuantidadeMaximaCalculos { get; set; }
    public int QuantidadeHorasCusto { get; set; }
}
