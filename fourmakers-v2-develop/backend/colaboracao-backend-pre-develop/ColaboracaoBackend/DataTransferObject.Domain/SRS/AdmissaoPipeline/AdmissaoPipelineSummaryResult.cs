using System;

namespace DataTransferObject.Domain.SRS.AdmissaoPipeline;

/// <summary>Projeção reduzida de <c>tb_admissao_pipeline</c> para listagens que não necessitam de etapas nem metadados.</summary>
public class AdmissaoPipelineSummaryResult
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
}
