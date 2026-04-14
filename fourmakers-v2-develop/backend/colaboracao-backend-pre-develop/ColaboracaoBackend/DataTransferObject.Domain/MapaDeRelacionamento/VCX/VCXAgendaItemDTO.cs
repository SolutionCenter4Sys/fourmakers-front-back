using System;

namespace DataTransferObject.Domain.MapaDeRelacionamento.VCX;

/// <summary>
/// Item de agenda comercial para listagem VCX (colaborador + cliente).
/// </summary>
public class VCXAgendaItemDTO
{
    public int Id { get; set; }
    public string? GraphEventId { get; set; }
    public string? CodColaboradorCriador { get; set; }
    public string? NomeCompletoColaboradorCriador { get; set; }
    public string? CodigoCliente { get; set; }
    public int? TipoInteracao { get; set; }
    public DateTime? DataCriacao { get; set; }
    public DateTime? DataAtualizacao { get; set; }
    public DateTime? DataAgendada { get; set; }
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public string? Status { get; set; }
    public string? Localizacao { get; set; }
    public string? LinkReuniao { get; set; }
    public int QuantidadeParticipantes { get; set; }
    public string? Titulo { get; set; }
    public string? Descricao { get; set; }
    public int? AgendaPaiId { get; set; }
}
