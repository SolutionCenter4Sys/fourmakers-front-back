using DataTransferObject.Domain;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Candidato;
using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.Nivel;
using DataTransferObject.Domain.Vaga;
using DataTransferObject.Domain.Vaga.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.Social
{
    public interface IComentarioCandidaturaRepository
    {
        Task<ComentarioCandidaturaDTO> GetByIdAsync(string id);
        Task<IEnumerable<ComentarioCandidaturaDTO>> GetByColaboradorCodigoAsync(string colaboradorCodigo);
        Task AddAsync(ComentarioCandidaturaDTO comentario, bool transacaoAberta = false);
        Task UpdateAsync(ComentarioCandidaturaDTO comentario);
        Task<IEnumerable<ComentarioCandidaturaDTO>> GetByCandidaturaIdAsync(string candidatoVagaId);
        Task<IEnumerable<CandidaturaArquivosDTO>> GetArquivosByComentarioIdAsync(string comentarioId);
    }
}
