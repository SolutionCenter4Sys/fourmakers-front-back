using System.Collections.Generic;

namespace DataTransferObject.Domain.SRS.AdmissaoPipeline;

/// <summary>Request para <b>criar</b> pipeline (<c>tb_admissao_pipeline</c>). Para atualizar use <see cref="AdmissaoPipelineAtualizarInput"/>.</summary>
public class AdmissaoPipelineInput
{
    public string Nome { get; set; }

    public string Descricao { get; set; }

    public bool Ativo { get; set; } = true;

    /// <summary>Versão lógica do pipeline (evolução sem perder histórico).</summary>
    public int Versao { get; set; } = 1;

    /// <summary>Etapas em <c>tb_admissao_pipeline_status</c>. <c>null</c> = sem etapas; lista (mesmo vazia) grava após criar o cabeçalho.</summary>
    public List<AdmissaoPipelineStatusItemInput> StatusItens { get; set; }
}
