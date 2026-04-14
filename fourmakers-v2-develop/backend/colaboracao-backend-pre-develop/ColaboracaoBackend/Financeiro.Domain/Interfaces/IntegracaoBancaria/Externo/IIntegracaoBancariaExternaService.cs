using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Financeiro.IntegracaoBancaria.Externo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Financeiro.Domain.Interfaces.IntegracaoBancaria.Externo
{
    public interface IIntegracaoBancariaExternaService
    {
        Task<ApiGenericResult<List<ReembolsoPagoExternoDTO>>> BuscarReembolsosPagosViaCNAB(string tokenSistema);

        Task<ApiGenericResult<List<HoleriteLiquidoConciliadoExternoDTO>>> ListarHoleritesLiquidosConciliacaoFolhaPontoAsync(
            string tokenSistema,
            int mes,
            int ano,
            string codDiretoria);
    }
}
