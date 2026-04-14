using System.Collections.Generic;

namespace DataTransferObject.Domain.MapaDeRelacionamento.VCX;

/// <summary>
/// Resultado combinado: lista de dores e lista de iniciativas para uma posição.
/// </summary>
public class VCXPosicaoDoresIniciativasResultDTO
{
    public List<VCXDorDTO> Dores { get; set; } = new();
    public List<VCXIniciativaDTO> Iniciativas { get; set; } = new();
}
