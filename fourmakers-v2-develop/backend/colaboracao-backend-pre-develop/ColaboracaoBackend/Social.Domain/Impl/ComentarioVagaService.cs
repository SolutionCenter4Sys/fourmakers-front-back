using Core.Domain.Social;
using Core.Domain.Vaga;
using DataTransferObject.Domain;
using Social.Domain.Interfaces;

using Logs.Infra.Attributes;

namespace Social.Domain.Impl
{
    [LogDomainClass]
    public class ComentarioVagaService : IComentarioVagaService
    {
        private readonly IComentarioVagaRepository _repository;
        private readonly IVagaFourmakersRepository _vagaFourmakersRepository;

        public ComentarioVagaService(IComentarioVagaRepository repository, IVagaFourmakersRepository vagaFourmakersRepository)
        {
            _repository = repository;
            _vagaFourmakersRepository = vagaFourmakersRepository;
        }

        public async Task<ComentarioVagaDTO> GetByIdAsync(string id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<ComentarioVagaDTO>> GetByVagaIdAsync(string vagaId)
        {
            return await _repository.GetByVagaIdAsync(vagaId);
        }

        public async Task<ComentarioVagaDTO> CreateAsync(CriarComentarioVagaDTO dto, string codColaborador)
        {
            if(!await _vagaFourmakersRepository.EstaVagaExistePorId(dto.VagaId.ToString()))
                throw new ApplicationException("Id vaga invalido");

            var comentario = new ComentarioVagaDTO
            {
                Id = Guid.NewGuid().ToString(),
                Texto = dto.Comentario,
                CodigoInternoColaborador = codColaborador,
                VagaId = dto.VagaId,
                DataCriacao = DateTime.UtcNow
            };

            await _repository.AddAsync(comentario);
            return comentario;
        }

        public async Task<ComentarioVagaDTO> UpdateAsync(string id, AtualizarComentarioVagaDTO dto)
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