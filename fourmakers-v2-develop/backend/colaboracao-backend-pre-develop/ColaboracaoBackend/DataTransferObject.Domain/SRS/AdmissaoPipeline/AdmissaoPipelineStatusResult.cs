using System;

namespace DataTransferObject.Domain.SRS.AdmissaoPipeline;

/// <summary>Linha de <c>tb_admissao_pipeline_status</c> com dados do status.</summary>
public class AdmissaoPipelineStatusResult
{
    public Guid Id { get; set; }

    public Guid AdmissaoPipelineId { get; set; }

    public Guid AdmissaoStatusId { get; set; }

    public string StatusDescricao { get; set; }

    public int? StatusCodigo { get; set; }

    public int Ordem { get; set; }

    public bool Obrigatorio { get; set; }

    public bool? PermiteRetroceder { get; set; }

    public bool StatusInicial { get; set; }

    public bool StatusFinal { get; set; }

    public bool Ativo { get; set; }

    public DateTime DataCriacao { get; set; }

    public DateTime DataAlteracao { get; set; }
}
