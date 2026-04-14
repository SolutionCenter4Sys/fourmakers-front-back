using DataTransferObject.Domain.Calculos;
using SRS.Domain.Interfaces.Service;

namespace SRS.Domain.Impl.Service
{
    /// <summary>
    /// Calcula 13º mensal/anual, FGTS mensal/anual e férias (líquido e mensal)
    /// a partir do salário líquido CLT e do bruto CLT, reutilizando tributos (INSS/IRRF).
    /// </summary>
    public class RemuneracaoVerbasAnuaisCalculador : IRemuneracaoVerbasAnuaisCalculador
    {
        private readonly ICalculoTributosCltService _tributosCltService;

        public RemuneracaoVerbasAnuaisCalculador(ICalculoTributosCltService tributosCltService)
        {
            _tributosCltService = tributosCltService;
        }

        public RemuneracaoVerbasAnuaisResult Calcular(
            decimal salarioLiquidoClt,
            decimal cltBruto,
            int numeroDependentes,
            InssTabelaDTO tabelaInss,
            IrrfTabelaDTO tabelaIrrf,
            IrrfReducaoDTO tabelaReducaoIrrf)
        {
            decimal decimoTerceiroMensal = salarioLiquidoClt / 12m;
            decimal fgtsMensal = (cltBruto * 0.08m) / 12m;
            decimal decimoTerceiroAnual = salarioLiquidoClt;
            decimal fgtsAnual = cltBruto * 0.08m;

            decimal feriasBruto = cltBruto * (1m + (1m / 3m));
            decimal descontoInssFerias = _tributosCltService.CalcularInss(feriasBruto, tabelaInss);
            decimal baseCalculoIrrfFerias = _tributosCltService.CalcularBaseIrrf(feriasBruto, descontoInssFerias, numeroDependentes, tabelaIrrf);
            decimal descontoIrrfFerias = _tributosCltService.CalcularIrrf(baseCalculoIrrfFerias, tabelaIrrf, feriasBruto, tabelaReducaoIrrf);
            decimal feriasLiquido = feriasBruto - descontoInssFerias - descontoIrrfFerias;
            decimal feriasMensal = feriasLiquido / 12m;

            return new RemuneracaoVerbasAnuaisResult
            {
                DecimoTerceiroMensal = decimoTerceiroMensal,
                FgtsMensal = fgtsMensal,
                FeriasMensal = feriasMensal,
                FeriasLiquido = feriasLiquido,
                DecimoTerceiroAnual = decimoTerceiroAnual,
                FgtsAnual = fgtsAnual
            };
        }
    }
}
