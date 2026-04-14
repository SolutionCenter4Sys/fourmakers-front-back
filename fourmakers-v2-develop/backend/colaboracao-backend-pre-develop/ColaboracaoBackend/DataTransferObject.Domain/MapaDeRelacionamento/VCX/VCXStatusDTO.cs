using System;

namespace DataTransferObject.Domain.MapaDeRelacionamento.VCX;

public class VCXStatusDTO
{
    public Guid Id { get; set; }
    public string Descricao { get; set; } = string.Empty;
}
