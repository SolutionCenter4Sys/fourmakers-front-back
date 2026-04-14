using System;

namespace DataTransferObject.Domain.SRS.AdmissaoOrigem;

/// <summary>
/// Request para vincular um processo de admissão a uma vaga ou candidatura do módulo de recrutamento.
/// Envie <c>TbVagaId</c> quando <c>OrigemTipo = VAGA</c>
/// e <c>TbCandidatoVagaId</c> quando <c>OrigemTipo = CANDIDATURA</c>.
/// </summary>
public class AdmissaoOrigemInput
{
    public AdmissaoOrigemTipoEnum OrigemTipo { get; set; }

    /// <summary>ID da vaga (tb_vaga.id). Obrigatório quando OrigemTipo = VAGA.</summary>
    public Guid? TbVagaId { get; set; }

    /// <summary>ID da candidatura (tb_candidato_vaga.id). Obrigatório quando OrigemTipo = CANDIDATURA.</summary>
    public string TbCandidatoVagaId { get; set; }
}
