using DataTransferObject.Domain.Contratacao;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.Contratacao
{
    public interface IDiretoriosRepository
    {
        Task<IEnumerable<DiretorioDTO>> ListarDiretoriosAsync();
        Task<DiretorioDTO> ObterDiretorioPorIdAsync(Guid id);
        Task<bool> ExisteDiretorioAsync(Guid id);
    }
}
