using System;

namespace DataTransferObject.Domain.MapaDeRelacionamento.VCX;

public class VCXTemaDTO
{
    public Guid Id { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public int OrgId { get; set; }
}
