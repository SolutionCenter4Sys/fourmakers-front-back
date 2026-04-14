using DataTransferObject.Domain.Base;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Usuario.Domain.Interfaces.Services
{
    public interface IExtracaoUsuarioService
    {
        Task<ApiGenericResult<FileContentResult>> RelatorioColaboradores(string cpf, int orgId);
    }
}