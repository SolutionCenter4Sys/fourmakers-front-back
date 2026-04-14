using System;

namespace DataTransferObject.Domain.MapaDeRelacionamento.VCX;

public class VCXImpactoDTO
{
    public Guid Id { get; set; }
    public string Descricao { get; set; } = string.Empty;
}
