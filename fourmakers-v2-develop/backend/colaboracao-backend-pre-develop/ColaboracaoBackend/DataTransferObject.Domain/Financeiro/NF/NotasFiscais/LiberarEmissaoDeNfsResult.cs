using System.Collections.Generic;

namespace DataTransferObject.Domain.Financeiro.NotaFiscal;

public class LiberarEmissaoDeNfsResult
{
    public List<string> RubricaCcntabilErroLog { get; set; } = new List<string>();
    public List<string> RubricaContabilSucessoLog { get; set; } = new List<string>();
    public int QuantidadeErros { get; set; } = 0;
    public int QuantidadeSucesso { get; set; } = 0;
    public int QuantidadeDeNfsLiberadas { get; set; } = 0;
}