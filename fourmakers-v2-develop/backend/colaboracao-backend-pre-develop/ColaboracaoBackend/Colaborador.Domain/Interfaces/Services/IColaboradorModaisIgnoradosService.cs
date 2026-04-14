using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Colaborador.Domain.Interfaces.Services
{
    public interface IColaboradorModaisIgnoradosService
    {
        Task<ApiGenericResult<List<string>>> ListarMeusIgnoradosAsync(string cpf, int orgId);
        Task<ApiGenericResult<List<string>>> IgnoreDialogAsync(string cpf, int orgId, string tag);
    }
}
