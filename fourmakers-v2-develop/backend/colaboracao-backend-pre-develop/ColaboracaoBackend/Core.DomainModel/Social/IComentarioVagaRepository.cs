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
    public interface IComentarioVagaRepository
    {
        Task<ComentarioVagaDTO> GetByIdAsync(string id);
        Task<IEnumerable<ComentarioVagaDTO>> GetByVagaIdAsync(string colaboradorCodigo);
        Task AddAsync(ComentarioVagaDTO comentario, bool transacaoAberta = false);
        Task UpdateAsync(ComentarioVagaDTO comentario);
    }
}
