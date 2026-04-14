using System;

namespace DataTransferObject.Domain.SRS.AdmissaoHistoricoStatus;

/// <summary>Movimentação de status de admissão (<c>tb_admissao_historico_status</c>).</summary>
public class AdmissaoHistoricoStatusResult
{
    public Guid Id { get; set; }

    public int TbOrgId { get; set; }

    public Guid AdmissaoId { get; set; }

    public Guid? AdmissaoStatusOrigemId { get; set; }

    public Guid AdmissaoStatusDestinoId { get; set; }

    /// <summary>Código interno do colaborador que realizou a movimentação.</summary>
    public string CodigoInternoColaborador { get; set; }

    public DateTime DataMovimentacao { get; set; }

    public string Observacao { get; set; }

    public DateTime DataCriacao { get; set; }

    public DateTime DataAtualizacao { get; set; }
}
