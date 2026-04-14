using System;

namespace DataTransferObject.Domain.SRS.AdmissaoPipeline;

/// <summary>Um vínculo pipeline ↔ status em <c>tb_admissao_pipeline_status</c>.</summary>
public class AdmissaoPipelineStatusItemInput
{
    /// <summary>ID do status em <c>tb_admissao_status</c> (mesma org).</summary>
    public Guid AdmissaoStatusId { get; set; }

    public int Ordem { get; set; }

    public bool Obrigatorio { get; set; } = true;

    /// <summary>Null = não definido / padrão no fluxo.</summary>
    public bool? PermiteRetroceder { get; set; }

    public bool StatusInicial { get; set; }

    public bool StatusFinal { get; set; }

    public bool Ativo { get; set; } = true;
}
