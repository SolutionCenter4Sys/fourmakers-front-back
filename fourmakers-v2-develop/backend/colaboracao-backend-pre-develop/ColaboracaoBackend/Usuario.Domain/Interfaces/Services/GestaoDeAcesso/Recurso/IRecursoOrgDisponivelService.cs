using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Usuario.GestaoDeAcesso.Recurso;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Usuario.Domain.Interfaces.Services.GestaoDeAcesso.Recurso
{
    public interface IRecursoOrgDisponivelService
    {
        Task<ApiGenericResult<IEnumerable<RecursoOrgDisponivelDTO>>> ConfigurarRecursoOrgDisponivel(List<RecursoOrgDisponivelDTO> listRecursoOrgDisponivelDTO, string cpfRequest, int orgId);
        Task<ApiGenericResult<IEnumerable<RecursoOrgDisponivelDTO>>> ListarRecursoOrgDisponivelAsync(string cpfRequest, int orgId);
        Task<ApiGenericResult<IEnumerable<RecursoOrgDisponivelDTO>>> ObterUltimaInsercaoRecursoOrgDisponivelLog(string cpfRequest, int orgId);
    }
}
