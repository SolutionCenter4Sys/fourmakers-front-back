using System;

namespace DataTransferObject.Domain.MapaDeRelacionamento.VCX;

/// <summary>
/// Registro de log de alteração de Nota de bastidores (Objeto/Alteracao deserializados para o front).
/// </summary>
public class VCXNotaBastidoresLogDTO
{
    public Guid Id { get; set; }
    public Guid ColaboradorCodigoInternoColaboradorAlterador { get; set; }
    public Guid OrganogramaPosicaoId { get; set; }
    public string Acao { get; set; } = string.Empty;
    public VCXNotaBastidoresDTO? Objeto { get; set; }
    public VCXNotaBastidoresDTO? Alteracao { get; set; }
    public DateTime DataAlteracao { get; set; }
}
