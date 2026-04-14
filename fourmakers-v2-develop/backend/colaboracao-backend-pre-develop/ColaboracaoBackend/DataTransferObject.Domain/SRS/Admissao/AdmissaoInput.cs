using System;

namespace DataTransferObject.Domain.SRS.Admissao;

/// <summary>Request para criar ou atualizar uma admissão (<c>tb_admissao</c>).</summary>
public class AdmissaoInput
{
    /// <summary>Pipeline de admissão ao qual este processo pertence.</summary>
    public Guid AdmissaoPipelineId { get; set; }

    /// <summary>Status atual do processo de admissão.</summary>
    public Guid AdmissaoStatusId { get; set; }

    /// <summary>Código interno do colaborador alvo da admissão (pode ser nulo enquanto não houver vínculo).</summary>
    public string CodigoInternoColaborador { get; set; }

    public DateTime DataInicio { get; set; }

    public DateTime? DataFim { get; set; }

    public string Observacao { get; set; }

    public bool Ativo { get; set; } = true;
}
