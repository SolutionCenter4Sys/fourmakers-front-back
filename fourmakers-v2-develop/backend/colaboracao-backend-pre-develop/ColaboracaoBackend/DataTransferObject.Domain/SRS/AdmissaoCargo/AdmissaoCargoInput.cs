using System;

namespace DataTransferObject.Domain.SRS.AdmissaoCargo;

/// <summary>Request para criar ou atualizar cargo de admissão.</summary>
public class AdmissaoCargoInput
{
    public string Descricao { get; set; }

    /// <summary>ID do CBO em tb_admissao_cbo (obrigatório na API ao criar ou atualizar).</summary>
    public Guid CboId { get; set; }

    public bool Ativo { get; set; } = true;
}
