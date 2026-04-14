using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Contratacao;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SRS.Domain.Interfaces.Service
{
    public interface IDiretoriosService
    {
        Task<ApiGenericResult<List<DiretorioDTO>>> ListarDiretoriosAsync();
        Task<ApiGenericResult<DiretorioDTO>> ObterDiretorioPorIdAsync(Guid id);
    }
}
