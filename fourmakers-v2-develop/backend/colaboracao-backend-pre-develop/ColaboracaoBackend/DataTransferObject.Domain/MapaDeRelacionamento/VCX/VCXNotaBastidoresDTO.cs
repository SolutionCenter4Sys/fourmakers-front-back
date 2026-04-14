using System;

namespace DataTransferObject.Domain.MapaDeRelacionamento.VCX;

public class VCXNotaBastidoresDTO
{
    public Guid Id { get; set; }
    public Guid OrganogramaPosicaoId { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; }
    public DateTime DataAlteracao { get; set; }
}
