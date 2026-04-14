using System.Collections.Generic;

namespace DataTransferObject.Domain.SRS.AdmissaoPipeline;

/// <summary>
/// Body do PUT: apenas campos aplicados em <c>UPDATE tb_admissao_pipeline</c> e substituição opcional de etapas.
/// Não inclui <c>Id</c> nem <c>OrgId</c> (vêm da rota e do token).
/// </summary>
public class AdmissaoPipelineAtualizarInput
{
    public string Nome { get; set; }

    public string Descricao { get; set; }

    public bool Ativo { get; set; } = true;

    public int Versao { get; set; } = 1;

    /// <summary>Etapas em <c>tb_admissao_pipeline_status</c>. <c>null</c> = mantém as existentes; lista (vazia ou não) substitui todos os vínculos.</summary>
    public List<AdmissaoPipelineStatusItemInput> StatusItens { get; set; }
}
