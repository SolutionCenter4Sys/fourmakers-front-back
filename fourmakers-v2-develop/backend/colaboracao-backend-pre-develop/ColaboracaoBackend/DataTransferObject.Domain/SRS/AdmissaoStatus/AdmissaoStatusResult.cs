using System;

namespace DataTransferObject.Domain.SRS.AdmissaoStatus;

/// <summary>Status de admissão (tb_admissao_status).</summary>
public class AdmissaoStatusResult
{
    public Guid Id { get; set; }

    /// <summary>Organização (tb_org).</summary>
    public int TbOrgId { get; set; }

    public string Descricao { get; set; }

    public int? Codigo { get; set; }

    public int Ordem { get; set; }

    public bool Ativo { get; set; }

    public DateTime DataCriacao { get; set; }

    public DateTime DataAlteracao { get; set; }
}
