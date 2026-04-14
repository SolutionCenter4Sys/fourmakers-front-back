using DataTransferObject.Domain.Calculos;

namespace SRS.Domain.Interfaces.Service
{
    /// <summary>
    /// Serviço responsável pelos cálculos de tributos CLT (INSS e IRRF).
    /// Isolado para reutilização em salário líquido, férias, 13º e simulações.
    /// </summary>
    public interface ICalculoTributosCltService
    {
        decimal CalcularInss(decimal salarioBruto, InssTabelaDTO tabelaInss);
        decimal CalcularBaseIrrf(decimal salarioBruto, decimal descontoInss, int numeroDependentes, IrrfTabelaDTO tabelaIrrf);
        decimal CalcularIrrf(decimal baseCalculo, IrrfTabelaDTO tabelaIrrf, decimal salarioBrutoCLT, IrrfReducaoDTO tabelaReducaoIrrf = null);
    }
}
