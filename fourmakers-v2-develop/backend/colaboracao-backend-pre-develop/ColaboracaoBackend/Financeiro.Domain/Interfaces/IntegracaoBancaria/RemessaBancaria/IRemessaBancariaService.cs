using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Financeiro.IntegracaoBancaria.RemessaBancaria;
using System.Threading.Tasks;

namespace Financeiro.Domain.Interfaces.IntegracaoBancaria.RemessaBancaria
{
    public interface IRemessaBancariaService
    {
        Task<ApiGenericResult<ListarRemessasCnabResult>> ProcessarRemessaBancariaCNAB(GerarRemessaBancariaRequest request, string cpfRequest, int orgId);
        Task<ApiGenericResult<ListarSolicitacoesPagamentoCNABResult>> ListarSolicitacoesPagamentoCNAB(string tipoRemessa,string codDiretoria, string cpfRequest, int orgId);
        Task<ApiGenericResult<ListarRemessasCnabResult>> ListarRemessasCnab(int orgId, string tipoRemessa, string mesAnoProcessamento = null, string status = null);
        Task<ApiGenericResult<List<string>>> BuscarModulosRemessaOrg(int orgId, string codigoInternoColaborador);
    }
}
