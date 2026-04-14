using System.Collections.Generic;

namespace DataTransferObject.Domain.Financeiro.Reembolso.Verba;

public class VerbaPersonalizadaCompletaDTO
{
    public VerbaPersonalizadaDTO VerbaGeral { get; set; }
    public List<VerbaPersonalizadaColaboradorDTO> ExcecoesColaborador { get; set; }
}