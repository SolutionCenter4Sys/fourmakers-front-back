using DataTransferObject.Domain.Financeiro.IntegracaoContabil;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.Financeiro.IntegracaoContabil
{
    public interface IIntegracaoContabilRepository
    {
        Task<RemessaContabilDTO> GerarRemessaContabilMensalFolhaPontoERubrica(string cnpj, string competencia, int orgId);
        Task<RemessaContabilDTO> GerarRemessaContabilMensalReembolso(string cnpj, string competencia, int orgId);
        Task<string> BuscarOuCriarVigenciaAsync(string competencia);
        Task GravarLogRemessaContabilAsync(string vigenciaId, string cnpj, int orgId, object idsRegistros, string codigoInternoColaboradorCriacao);
    }
}