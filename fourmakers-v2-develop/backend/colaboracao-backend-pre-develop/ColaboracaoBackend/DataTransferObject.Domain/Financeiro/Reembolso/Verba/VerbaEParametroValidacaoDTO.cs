using System.Collections.Generic;
using DataTransferObject.Domain.Financeiro.Reembolso.Parametro;

namespace DataTransferObject.Domain.Financeiro.Reembolso.Verba;

public class VerbaEParametroValidacaoDTO
{
    public ParametroReembolsoDTO ParametroReembolso { get; set; }
    public List<VerbaSimplificadoDTO> Verbas { get; set; }
}