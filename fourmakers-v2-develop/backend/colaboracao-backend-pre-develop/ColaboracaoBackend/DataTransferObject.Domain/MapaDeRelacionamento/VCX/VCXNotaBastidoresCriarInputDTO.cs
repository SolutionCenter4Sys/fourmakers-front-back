using System;

namespace DataTransferObject.Domain.MapaDeRelacionamento.VCX;

public class VCXNotaBastidoresCriarInputDTO
{
    public Guid OrganogramaPosicaoId { get; set; }
    public string Descricao { get; set; } = string.Empty;
}
