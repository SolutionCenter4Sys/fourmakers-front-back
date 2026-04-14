using System;

namespace DataTransferObject.Domain.SRS.RemuneracaoClt;

/// <summary>
/// Response de Remuneração CLT (cargo de admissão + faixas + CBO + piso).
/// </summary>
public class RemuneracaoCltResult
{
    public Guid Id { get; set; }
    /// <summary>ID do cargo de admissão (tb_admissao_cargo).</summary>
    public Guid AdmissaoCargoId { get; set; }
    /// <summary>Descrição do cargo de admissão.</summary>
    public string CargoDescricao { get; set; }
    public decimal? Faixa1Inicio { get; set; }
    public decimal? Faixa1Final { get; set; }
    public decimal? Faixa2Inicio { get; set; }
    public decimal? Faixa2Final { get; set; }
    public decimal? Faixa3Inicio { get; set; }
    public decimal? Faixa3Final { get; set; }
    public decimal? Faixa4Inicio { get; set; }
    public decimal? Faixa4Final { get; set; }
    public Guid? CboId { get; set; }
    public string CboCodigo { get; set; }
    public string CboTitulo { get; set; }
    public decimal? Piso { get; set; }
    public bool Ativo { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime DataAlteracao { get; set; }
}
