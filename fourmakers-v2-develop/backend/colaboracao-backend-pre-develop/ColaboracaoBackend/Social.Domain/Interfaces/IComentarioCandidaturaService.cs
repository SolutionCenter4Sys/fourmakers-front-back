using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain;

namespace Social.Domain.Interfaces
{
    public interface IComentarioCandidaturaService
    {
        Task<ComentarioCandidaturaDTO> GetByIdAsync(string id);

        Task<IEnumerable<ComentarioCandidaturaDTO>> GetByColaboradorCodigoAsync(string colaboradorCodigo);

        Task<IEnumerable<ComentarioCandidaturaDTO>> GetByCandidaturaIdAsync(string candidatoVagaId);

        Task<ComentarioCandidaturaDTO> CreateAsync(CriarComentarioCandidaturaDTO dto, string codColaborador);

        Task<ComentarioCandidaturaDTO> UpdateAsync(string id, AtualizarComentarioCandidaturaDTO dto);
    }
} 