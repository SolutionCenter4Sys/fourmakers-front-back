using System;

namespace DataTransferObject.Domain.MapaDeRelacionamento.VCX;

public class VCXIniciativaDTO
{
    public Guid Id { get; set; }
    public Guid OrganogramaPosicaoId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime DataAlteracao { get; set; }
    public Guid? VcxStatusId { get; set; }
    public Guid? VcxTemasId { get; set; }
    public string? VcxStatusDescricao { get; set; }
    public string? VcxTemasDescricao { get; set; }
}
