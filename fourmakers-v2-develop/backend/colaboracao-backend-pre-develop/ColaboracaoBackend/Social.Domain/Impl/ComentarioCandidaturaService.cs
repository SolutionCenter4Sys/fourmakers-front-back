using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain;
using Social.Domain.Interfaces;
using Core.Domain.Social;
using Core.DomainModel;

using Logs.Infra.Attributes;

namespace Social.Domain.Impl
{
    [LogDomainClass]
    public class ComentarioCandidaturaService : IComentarioCandidaturaService
    {
        private readonly IComentarioCandidaturaRepository _repository;
        private readonly ICandidaturaRepository _candidaturaRepository;

        public ComentarioCandidaturaService(IComentarioCandidaturaRepository repository, ICandidaturaRepository candidaturaRepository)
        {
            _repository = repository;
            _candidaturaRepository = candidaturaRepository;
        }

        public async Task<ComentarioCandidaturaDTO> GetByIdAsync(string id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<ComentarioCandidaturaDTO>> GetByColaboradorCodigoAsync(string colaboradorCodigo)
        {
            return await _repository.GetByColaboradorCodigoAsync(colaboradorCodigo);
        }

        public async Task<IEnumerable<ComentarioCandidaturaDTO>> GetByCandidaturaIdAsync(string candidatoVagaId)
        {
            return await _repository.GetByCandidaturaIdAsync(candidatoVagaId);
        }

        public async Task<ComentarioCandidaturaDTO> CreateAsync(CriarComentarioCandidaturaDTO dto, string codColaborador)
        {
            if(!await _candidaturaRepository.EstaCandidaturaExiste(dto.CandidaturaId))
                throw new ApplicationException("Id Candidatura invalido");

            var comentario = new ComentarioCandidaturaDTO
            {
                Id = Guid.NewGuid().ToString(),
                Texto = dto.Comentario,
                CodigoInternoColaborador = codColaborador,
                CandidaturaId = dto.CandidaturaId,
                DataCriacao = DateTime.UtcNow
            };

            await _repository.AddAsync(comentario);
            return comentario;
        }

        public async Task<ComentarioCandidaturaDTO> UpdateAsync(string id, AtualizarComentarioCandidaturaDTO dto)
        {
            var comentario = await _repository.GetByIdAsync(id);
            if (comentario == null)
                throw new ApplicationException("Comentário não encontrado");

            comentario.Texto = dto.Comentario;
            comentario.DataAlteracao = DateTime.UtcNow;

            await _repository.UpdateAsync(comentario);
            return comentario;
        }
    }
} 