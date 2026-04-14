using System.Collections.Generic;

namespace DataTransferObject.Domain.MapaDeRelacionamento.VCX;

/// <summary>
/// Resultado combinado: histórico (logs) de dores e de iniciativas para uma posição, ordenados por data de alteração.
/// Objeto e Alteracao vêm deserializados para o front exibir.
/// </summary>
public class VCXHistoricoDoresIniciativasResultDTO
{
    public List<VCXDorLogDTO> DoresLog { get; set; } = new();
    public List<VCXIniciativaLogDTO> IniciativasLog { get; set; } = new();
}
