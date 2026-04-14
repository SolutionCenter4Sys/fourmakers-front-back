using System;

namespace DataTransferObject.Domain.SRS.AdmissaoHistoricoStatus;

/// <summary>
/// Request para registrar uma movimentação de status em um processo de admissão.
/// O ID da admissão é informado na URL e o status de origem é lido automaticamente do registro atual.
/// </summary>
public class AdmissaoHistoricoStatusInput
{
    /// <summary>Status para o qual a admissão será movida (obrigatório).</summary>
    public Guid AdmissaoStatusDestinoId { get; set; }

    public string Observacao { get; set; }
}
