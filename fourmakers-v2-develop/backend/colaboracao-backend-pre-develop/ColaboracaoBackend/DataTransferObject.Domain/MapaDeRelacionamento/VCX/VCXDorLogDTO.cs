using System;

namespace DataTransferObject.Domain.MapaDeRelacionamento.VCX;

/// <summary>
/// Registro de log de alteração de Dor, com Objeto e Alteracao deserializados para o front exibir.
/// </summary>
public class VCXDorLogDTO
{
    public Guid Id { get; set; }
    public Guid ColaboradorCodigoInternoColaboradorAlterador { get; set; }
    public Guid OrganogramaPosicaoId { get; set; }
    public string Acao { get; set; } = string.Empty; // INSERT, UPDATE, DELETE
    /// <summary>Valor anterior (para UPDATE/DELETE).</summary>
    public VCXDorDTO? Objeto { get; set; }
    /// <summary>Valor novo (para INSERT/UPDATE).</summary>
    public VCXDorDTO? Alteracao { get; set; }
    public DateTime DataAlteracao { get; set; }
}
