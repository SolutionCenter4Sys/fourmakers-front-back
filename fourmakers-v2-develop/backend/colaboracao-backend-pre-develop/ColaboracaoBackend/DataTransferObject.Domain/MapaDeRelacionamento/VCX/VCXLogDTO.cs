using System;

namespace DataTransferObject.Domain.MapaDeRelacionamento.VCX;

public class VCXLogDTO
{
    public Guid Id { get; set; }
    public Guid ColaboradorCodigoInternoColaboradorAlterador { get; set; }
    public Guid OrganogramaPosicaoId { get; set; }
    public string Acao { get; set; } = string.Empty; // INSERT, UPDATE, DELETE
    public string? Objeto { get; set; }  // valor antigo (JSON)
    public string? Alteracao { get; set; } // valor novo (JSON)
    public DateTime DataAlteracao { get; set; }
}
