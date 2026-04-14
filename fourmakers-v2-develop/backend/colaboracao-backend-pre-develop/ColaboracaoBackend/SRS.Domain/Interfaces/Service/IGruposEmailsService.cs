using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Contratacao;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SRS.Domain.Interfaces.Service
{
    public interface IGruposEmailsService
    {
        Task<ApiGenericResult<List<GrupoEmailDTO>>> ListarGruposEmailsAsync();
        Task<ApiGenericResult<GrupoEmailDTO>> ObterGrupoEmailPorIdAsync(Guid id);
    }
}
