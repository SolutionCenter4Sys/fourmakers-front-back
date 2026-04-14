using System;

namespace DataTransferObject.Domain.SRS.AdmissaoCargo;

/// <summary>Cargo de admissão (tb_admissao_cargo).</summary>
public class AdmissaoCargoResult
{
    public Guid Id { get; set; }

    /// <summary>Organização (tb_org).</summary>
    public int TbOrgId { get; set; }

    public string Descricao { get; set; }

    /// <summary>ID do CBO vinculado (tb_admissao_cbo).</summary>
    public Guid? CboId { get; set; }

    public bool Ativo { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime DataAlteracao { get; set; }
}
