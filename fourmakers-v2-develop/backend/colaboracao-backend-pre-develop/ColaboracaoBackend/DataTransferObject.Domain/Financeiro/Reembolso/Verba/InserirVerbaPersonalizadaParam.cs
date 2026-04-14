using System.Collections.Generic;

namespace DataTransferObject.Domain.Financeiro.Reembolso.Verba;

public class InserirVerbaPersonalizadaParam
{
    public string ProjetoId { get; set; }
    public string ClienteId { get; set; }
    public List<VerbaPersonalizadaCompletaDTO> VerbasCustomizadas { get; set; }
}