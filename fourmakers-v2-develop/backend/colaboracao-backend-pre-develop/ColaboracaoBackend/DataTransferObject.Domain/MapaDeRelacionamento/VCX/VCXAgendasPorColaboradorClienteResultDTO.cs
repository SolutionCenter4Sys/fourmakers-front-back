using System.Collections.Generic;

namespace DataTransferObject.Domain.MapaDeRelacionamento.VCX;

/// <summary>
/// Resultado de agendas por colaborador e cliente: antigas (já realizadas) e novas (futuras).
/// </summary>
public class VCXAgendasPorColaboradorClienteResultDTO
{
    public List<VCXAgendaItemDTO> AgendasAntigas { get; set; } = new();
    public List<VCXAgendaItemDTO> AgendasNovas { get; set; } = new();
}
