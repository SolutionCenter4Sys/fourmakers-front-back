using DataTransferObject.Domain.Calculos;
using SRS.Domain.Interfaces.Service;
using System;

namespace SRS.Domain.Impl.Service
{
    /// <summary>
    /// Implementação dos cálculos de INSS e IRRF para CLT.
    /// Fórmulas centralizadas para reutilização em salário líquido, férias e simulações.
    /// </summary>
    public class TributosCltService : ICalculoTributosCltService
    {
        public decimal CalcularInss(decimal salarioBruto, InssTabelaDTO tabelaInss)
        {
            decimal baseCalculo = salarioBruto > tabelaInss.TetoInss ? tabelaInss.TetoInss : salarioBruto;

            decimal faixa1ParcelaDeduzir = tabelaInss.Faixa1ParcelaDeduzir > 0 ? tabelaInss.Faixa1ParcelaDeduzir : 0m;
            decimal faixa2ParcelaDeduzir = tabelaInss.Faixa2ParcelaDeduzir > 0 ? tabelaInss.Faixa2ParcelaDeduzir : 24.32m;
            decimal faixa3ParcelaDeduzir = tabelaInss.Faixa3ParcelaDeduzir > 0 ? tabelaInss.Faixa3ParcelaDeduzir : 111.40m;
            decimal faixa4ParcelaDeduzir = tabelaInss.Faixa4ParcelaDeduzir > 0 ? tabelaInss.Faixa4ParcelaDeduzir : 198.49m;

            decimal descontoInss;
            if (baseCalculo <= 1518.00m)
                descontoInss = baseCalculo * (tabelaInss.Faixa1Aliquota / 100) - faixa1ParcelaDeduzir;
            else if (baseCalculo <= 2793.88m)
                descontoInss = baseCalculo * (tabelaInss.Faixa2Aliquota / 100) - faixa2ParcelaDeduzir;
            else if (baseCalculo <= 4190.83m)
                descontoInss = baseCalculo * (tabelaInss.Faixa3Aliquota / 100) - faixa3ParcelaDeduzir;
            else if (baseCalculo <= 8157.41m)
                descontoInss = baseCalculo * (tabelaInss.Faixa4Aliquota / 100) - faixa4ParcelaDeduzir;
            else
                descontoInss = tabelaInss.TetoInss * (tabelaInss.Faixa4Aliquota / 100) - faixa4ParcelaDeduzir;

            return Math.Max(0, Math.Round(descontoInss, 2));
        }

        public decimal CalcularBaseIrrf(decimal salarioBruto, decimal descontoInss, int numeroDependentes, IrrfTabelaDTO tabelaIrrf)
        {
            var deducaoPorDependente = tabelaIrrf.DeducaoPorDependente * numeroDependentes;
            var baseCalculo = salarioBruto - descontoInss - deducaoPorDependente;
            return Math.Max(0, Math.Round(baseCalculo, 2));
        }

        public decimal CalcularIrrf(decimal baseCalculo, IrrfTabelaDTO tabelaIrrf, decimal salarioBrutoCLT, IrrfReducaoDTO tabelaReducaoIrrf = null)
        {
            decimal descontoIrrf;
            if (baseCalculo <= tabelaIrrf.Faixa1Max)
                descontoIrrf = 0;
            else if (baseCalculo <= tabelaIrrf.Faixa2Max)
                descontoIrrf = (baseCalculo * (tabelaIrrf.Faixa2Aliquota / 100)) - tabelaIrrf.Faixa2Deducao;
            else if (baseCalculo <= tabelaIrrf.Faixa3Max)
                descontoIrrf = (baseCalculo * (tabelaIrrf.Faixa3Aliquota / 100)) - tabelaIrrf.Faixa3Deducao;
            else if (baseCalculo <= tabelaIrrf.Faixa4Max)
                descontoIrrf = (baseCalculo * (tabelaIrrf.Faixa4Aliquota / 100)) - tabelaIrrf.Faixa4Deducao;
            else
                descontoIrrf = (baseCalculo * (tabelaIrrf.Faixa5Aliquota / 100)) - tabelaIrrf.Faixa5Deducao;

            if (tabelaReducaoIrrf != null)
            {
                decimal reducao = CalcularReducaoIrrf(salarioBrutoCLT, tabelaReducaoIrrf);
                descontoIrrf = Math.Max(0, descontoIrrf - reducao);
            }

            return Math.Max(0, Math.Round(descontoIrrf, 2));
        }

        private static decimal CalcularReducaoIrrf(decimal salarioBrutoCLT, IrrfReducaoDTO tabelaReducaoIrrf)
        {
            if (salarioBrutoCLT <= tabelaReducaoIrrf.Faixa1Max)
                return tabelaReducaoIrrf.Faixa1DescontoMaximo;
            if (salarioBrutoCLT <= tabelaReducaoIrrf.Faixa2Max)
            {
                decimal reducao = tabelaReducaoIrrf.Faixa2ValorBase - (tabelaReducaoIrrf.Faixa2Coeficiente * salarioBrutoCLT);
                return Math.Max(0, reducao);
            }
            return 0;
        }
    }
}
