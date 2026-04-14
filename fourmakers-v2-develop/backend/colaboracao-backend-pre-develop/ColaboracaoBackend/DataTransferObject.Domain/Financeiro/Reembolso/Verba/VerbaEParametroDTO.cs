using System.Collections.Generic;
using DataTransferObject.Domain.Financeiro.Reembolso.Parametro;

namespace DataTransferObject.Domain.Financeiro.Reembolso.Verba;

public class VerbaEParametroDTO
{
    public ParametroReembolsoDTO ParametroReembolso { get; set; }
    public List<VerbaDTO> Verbas  { get; set; }
}