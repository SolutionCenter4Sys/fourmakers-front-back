using DataTransferObject.Domain.Base;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Projeto.Domain.Interfaces.Services
{
    public interface IExtracaoProjetoService
    {
        Task<ApiGenericResult<FileContentResult>> ExportarProjetosExcel(string cpf, int orgId);
    }
}