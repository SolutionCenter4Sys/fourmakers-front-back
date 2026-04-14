using DataTransferObject.Domain.Contratacao;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.Contratacao
{
    public interface IGruposEmailsRepository
    {
        Task<IEnumerable<GrupoEmailDTO>> ListarGruposEmailsAsync();
        Task<GrupoEmailDTO> ObterGrupoEmailPorIdAsync(Guid id);
        Task<bool> ExisteGrupoEmailAsync(Guid id);
    }
}
