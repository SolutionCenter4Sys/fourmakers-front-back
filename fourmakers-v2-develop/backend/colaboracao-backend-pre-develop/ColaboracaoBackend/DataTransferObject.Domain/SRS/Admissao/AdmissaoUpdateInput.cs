using System;

namespace DataTransferObject.Domain.SRS.Admissao;

/// <summary>
/// Request para atualizar um processo de admissão existente.
/// Datas e status são imutáveis por este endpoint:
/// datas são definidas na criação e status só pode mudar via MoverStatus.
/// </summary>
public class AdmissaoUpdateInput
{
    /// <summary>Pipeline ao qual o processo pertence.</summary>
    public Guid AdmissaoPipelineId { get; set; }

    /// <summary>Código interno do colaborador alvo (pode ser nulo enquanto não houver vínculo).</summary>
    public string CodigoInternoColaborador { get; set; }

    public string Observacao { get; set; }

    public bool Ativo { get; set; } = true;
}
