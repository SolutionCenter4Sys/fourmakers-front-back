using System;
using System.Collections.Generic;

namespace DataTransferObject.Domain.SRS.AdmissaoPipeline;

/// <summary>Pipeline de admissão por organização (<c>tb_admissao_pipeline</c>).</summary>
public class AdmissaoPipelineResult
{
    public Guid Id { get; set; }

    /// <summary>Organização (<c>tb_org</c>).</summary>
    public int OrgId { get; set; }

    public string Nome { get; set; }
    public string Descricao { get; set; }
    public bool Ativo { get; set; }
    public int Versao { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime DataAlteracao { get; set; }

    /// <summary>Preenchido em listagem, <c>ObterPorId</c> e após POST/PUT quando aplicável.</summary>
    public List<AdmissaoPipelineStatusResult> StatusItens { get; set; }
}
