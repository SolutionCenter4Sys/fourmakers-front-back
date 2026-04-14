using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Contratacao;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SRS.Domain.Interfaces.Service
{
    public interface ISistemasLiberadosService
    {
        Task<ApiGenericResult<List<SistemaLiberadoDTO>>> ListarSistemasLiberadosAsync();
        Task<ApiGenericResult<SistemaLiberadoDTO>> ObterSistemaLiberadoPorIdAsync(Guid id);
    }
}
