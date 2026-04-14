using DataTransferObject.Domain.Base;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Financeiro.Domain.Interfaces.IntegracaoContabil
{
    public interface IIntegracaoContabilService
    {
        Task<ApiGenericResult<FileContentResult>> GerarRemessaContabilMensalFolhaPontoERubricaAsync(string cnpj, string competencia, string cpfUsuario, int orgId);
    }
}