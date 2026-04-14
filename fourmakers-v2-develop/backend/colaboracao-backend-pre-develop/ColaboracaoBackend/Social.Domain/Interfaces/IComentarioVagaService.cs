using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain;

namespace Social.Domain.Interfaces
{
    public interface IComentarioVagaService
    {
        Task<ComentarioVagaDTO> GetByIdAsync(string id);
        Task<IEnumerable<ComentarioVagaDTO>> GetByVagaIdAsync(string vagaId);
        Task<ComentarioVagaDTO> CreateAsync(CriarComentarioVagaDTO dto, string codColaborador);
        Task<ComentarioVagaDTO> UpdateAsync(string id, AtualizarComentarioVagaDTO dto);
    }
} 