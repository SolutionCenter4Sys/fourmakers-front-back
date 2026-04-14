using System;

namespace DataTransferObject.Domain.SRS.RemuneracaoClt;

/// <summary>
/// Request para criar ou atualizar Remuneração CLT (cargo de admissão tb_admissao_cargo + faixas + CBO + piso).
/// </summary>
public class RemuneracaoCltInput
{
    /// <summary>ID do cargo de admissão (tb_admissao_cargo, GUID).</summary>
    public Guid AdmissaoCargoId { get; set; }
    public decimal? Faixa1Inicio { get; set; }
    public decimal? Faixa1Final { get; set; }
    public decimal? Faixa2Inicio { get; set; }
    public decimal? Faixa2Final { get; set; }
    public decimal? Faixa3Inicio { get; set; }
    public decimal? Faixa3Final { get; set; }
    public decimal? Faixa4Inicio { get; set; }
    public decimal? Faixa4Final { get; set; }
    public Guid? CboId { get; set; }
    public decimal? Piso { get; set; }
    public bool Ativo { get; set; } = true;
}
