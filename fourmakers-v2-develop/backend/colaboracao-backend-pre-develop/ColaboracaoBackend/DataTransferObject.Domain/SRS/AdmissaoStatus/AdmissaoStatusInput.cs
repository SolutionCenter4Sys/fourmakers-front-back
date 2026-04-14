namespace DataTransferObject.Domain.SRS.AdmissaoStatus;

/// <summary>Request para criar ou atualizar status de admissão (tb_admissao_status).</summary>
public class AdmissaoStatusInput
{
    public string Descricao { get; set; }

    /// <summary>Código opcional (similar a tb_status_vaga.codigo).</summary>
    public int? Codigo { get; set; }

    public int Ordem { get; set; }

    public bool Ativo { get; set; } = true;
}
