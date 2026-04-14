using DataTransferObject.Domain.Contratacao;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.Contratacao
{
    public interface ISistemasLiberadosRepository
    {
        Task<IEnumerable<SistemaLiberadoDTO>> ListarSistemasLiberadosAsync();
        Task<SistemaLiberadoDTO> ObterSistemaLiberadoPorIdAsync(Guid id);
        Task<bool> ExisteSistemaLiberadoAsync(Guid id);
    }
}
