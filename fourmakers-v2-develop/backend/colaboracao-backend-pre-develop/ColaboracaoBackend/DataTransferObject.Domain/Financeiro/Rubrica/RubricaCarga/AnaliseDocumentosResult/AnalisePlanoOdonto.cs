using System.Collections.Generic;

namespace DataTransferObject.Domain.Financeiro.Rubrica.RubricaCarga.AnaliseDocumentosResult;

public class AnalisePlanoOdonto
{
    public string IdentificacaoDoBeneficiario { get; set; }
    public long NumeroDaCarteirinha { get; set; }
    public long? CarteirinhaDoTitular { get; set; }
    public string NomeDoBeneficiario { get; set; }
}
