using DataTransferObject.Domain.Usuario.GestaoDeAcesso.Recurso;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.Usuario.GestaoDeAcesso
{
    public interface IRecursoOrgDisponivelRepository
    {
        Task<IEnumerable<RecursoOrgDisponivelDTO>> ConfigurarRecursoOrgDisponivelAsync(IEnumerable<RecursoOrgDisponivelDTO> recursos);
        Task<IEnumerable<RecursoOrgDisponivelDTO>> ListarRecursoOrgDisponivelAsync();
    }

}
