using System;

namespace DataTransferObject.Domain.MapaDeRelacionamento.VCX;

public class VCXDorDTO
{
    public Guid Id { get; set; }
    public Guid OrganogramaPosicaoId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime DataAlteracao { get; set; }
    public Guid? VcxImpactosId { get; set; }
    public Guid? VcxUrgenciasId { get; set; }
    public string? VcxImpactosDescricao { get; set; }
    public string? VcxUrgenciasDescricao { get; set; }
}
