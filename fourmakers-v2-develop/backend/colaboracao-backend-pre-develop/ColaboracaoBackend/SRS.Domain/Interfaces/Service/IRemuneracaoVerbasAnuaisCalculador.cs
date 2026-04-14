using DataTransferObject.Domain.Calculos;

namespace SRS.Domain.Interfaces.Service
{
    /// <summary>
    /// Calculador de verbas anuais (13º, FGTS, férias) mensalizadas e anuais.
    /// Isolado para reutilização na remuneração proposta e na pretendida.
    /// </summary>
    public interface IRemuneracaoVerbasAnuaisCalculador
    {
        RemuneracaoVerbasAnuaisResult Calcular(
            decimal salarioLiquidoClt,
            decimal cltBruto,
            int numeroDependentes,
            InssTabelaDTO tabelaInss,
            IrrfTabelaDTO tabelaIrrf,
            IrrfReducaoDTO tabelaReducaoIrrf);
    }
}
