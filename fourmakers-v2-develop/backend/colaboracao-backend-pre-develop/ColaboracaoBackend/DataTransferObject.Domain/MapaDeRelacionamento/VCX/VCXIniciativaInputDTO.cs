using System;

namespace DataTransferObject.Domain.MapaDeRelacionamento.VCX;

public class VCXIniciativaInputDTO
{
    /// <summary>Obrigatório apenas para atualização (PUT).</summary>
    public Guid? Id { get; set; }
    public Guid OrganogramaPosicaoId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public Guid? VcxStatusId { get; set; }
    public Guid? VcxTemasId { get; set; }
}
