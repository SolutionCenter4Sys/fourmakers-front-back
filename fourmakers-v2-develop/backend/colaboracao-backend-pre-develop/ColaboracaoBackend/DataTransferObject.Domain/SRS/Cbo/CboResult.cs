using System;

namespace DataTransferObject.Domain.SRS.Cbo;

/// <summary>
/// Response de um CBO (Código Brasileiro de Ocupações).
/// </summary>
public class CboResult
{
    public Guid Id { get; set; }

    /// <summary>Organização (tb_org).</summary>
    public int TbOrgId { get; set; }

    public string Codigo { get; set; }
    public string Titulo { get; set; }

    /// <summary>Descrição detalhada (ex.: Classificação Brasileira de Ocupações).</summary>
    public string Descricao { get; set; }

    public bool Ativo { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime DataAlteracao { get; set; }
}
