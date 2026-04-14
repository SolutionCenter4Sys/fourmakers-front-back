using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Social.Domain.Impl;
using Social.Domain.Interfaces;
using DataTransferObject.Domain;
using Xunit;
using Core.Domain.Social;
using Core.DomainModel;

namespace Social.Domain.Tests.Services
{
    public class ComentarioCandidaturaServiceTests
    {
        private readonly Mock<IComentarioCandidaturaRepository> _repositoryMock;
        private readonly Mock<ICandidaturaRepository> _candidaturaRepositoryMock;
        private readonly IComentarioCandidaturaService _service;

        public ComentarioCandidaturaServiceTests()
        {
            _repositoryMock = new Mock<IComentarioCandidaturaRepository>();
            _candidaturaRepositoryMock = new Mock<ICandidaturaRepository>();
            _service = new ComentarioCandidaturaService(_repositoryMock.Object, _candidaturaRepositoryMock.Object);
        }

        [Fact]
        public async Task GetByIdAsync_QuandoComentarioExiste_DeveRetornarComentario()
        {
            // Arrange
            var id = "123";
            var comentarioDto = new ComentarioCandidaturaDTO
            {
                Id = id,
                Texto = "Teste",
                DataCriacao = DateTime.UtcNow,
                CodigoInternoColaborador = "456",
                CandidaturaId = "789"
            };

            _repositoryMock.Setup(x => x.GetByIdAsync(id))
                .ReturnsAsync(comentarioDto);

            // Act
            var result = await _service.GetByIdAsync(id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            _repositoryMock.Verify(x => x.GetByIdAsync(id), Times.Once);
        }

        [Fact]
        public async Task GetByColaboradorCodigoAsync_QuandoExistemComentarios_DeveRetornarLista()
        {
            // Arrange
            var colaboradorCodigo = "123";
            var comentarios = new List<ComentarioCandidaturaDTO>
            {
                new ComentarioCandidaturaDTO
                {
                    Id = "1",
                    Texto = "Teste 1",
                    DataCriacao = DateTime.UtcNow,
                    CodigoInternoColaborador = colaboradorCodigo,
                    CandidaturaId = "456"
                },
                new ComentarioCandidaturaDTO
                {
                    Id = "2",
                    Texto = "Teste 2",
                    DataCriacao = DateTime.UtcNow,
                    CodigoInternoColaborador = colaboradorCodigo,
                    CandidaturaId = "789"
                }
            };

            _repositoryMock.Setup(x => x.GetByColaboradorCodigoAsync(colaboradorCodigo))
                .ReturnsAsync(comentarios);

            // Act
            var result = await _service.GetByColaboradorCodigoAsync(colaboradorCodigo);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            _repositoryMock.Verify(x => x.GetByColaboradorCodigoAsync(colaboradorCodigo), Times.Once);
        }

        [Fact]
        public async Task GetByCandidaturaIdAsync_QuandoExistemComentarios_DeveRetornarLista()
        {
            // Arrange
            var CandidaturaId = "123";
            var comentarios = new List<ComentarioCandidaturaDTO>
            {
                new ComentarioCandidaturaDTO
                {
                    Id = "1",
                    Texto = "Teste 1",
                    DataCriacao = DateTime.UtcNow,
                    CodigoInternoColaborador = "456",
                    CandidaturaId = CandidaturaId
                },
                new ComentarioCandidaturaDTO
                {
                    Id = "2",
                    Texto = "Teste 2",
                    DataCriacao = DateTime.UtcNow,
                    CodigoInternoColaborador = "789",
                    CandidaturaId = CandidaturaId
                }
            };

            _repositoryMock.Setup(x => x.GetByCandidaturaIdAsync(CandidaturaId))
                .ReturnsAsync(comentarios);

            // Act
            var result = await _service.GetByCandidaturaIdAsync(CandidaturaId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            _repositoryMock.Verify(x => x.GetByCandidaturaIdAsync(CandidaturaId), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ComParametrosValidos_DeveCriarComSucesso()
        {
            // Arrange
            var dto = new CriarComentarioCandidaturaDTO
            {
                Comentario = "Teste",
                CandidaturaId = "456"
            };

            _repositoryMock.Setup(x => x.AddAsync(It.IsAny<ComentarioCandidaturaDTO>(), It.IsAny<bool>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.CreateAsync(dto, "1");

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.Id);
            Assert.Equal(dto.Comentario, result.Texto);
            Assert.Equal(dto.CandidaturaId, result.CandidaturaId);
            _repositoryMock.Verify(x => x.AddAsync(It.IsAny<ComentarioCandidaturaDTO>(), It.IsAny<bool>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_QuandoComentarioExiste_DeveAtualizarComSucesso()
        {
            // Arrange
            var id = "123";
            var dto = new AtualizarComentarioCandidaturaDTO
            {
                Comentario = "Comentário atualizado"
            };

            var comentarioExistente = new ComentarioCandidaturaDTO
            {
                Id = id,
                Texto = "Comentário original",
                DataCriacao = DateTime.UtcNow,
                CodigoInternoColaborador = "456",
                CandidaturaId = "789"
            };

            _repositoryMock.Setup(x => x.GetByIdAsync(id))
                .ReturnsAsync(comentarioExistente);

            _repositoryMock.Setup(x => x.UpdateAsync(It.IsAny<ComentarioCandidaturaDTO>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.UpdateAsync(id, dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal(dto.Comentario, result.Texto);
            Assert.NotNull(result.DataAlteracao);
            _repositoryMock.Verify(x => x.GetByIdAsync(id), Times.Once);
            _repositoryMock.Verify(x => x.UpdateAsync(It.IsAny<ComentarioCandidaturaDTO>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_QuandoComentarioNaoExiste_DeveLancarExcecao()
        {
            // Arrange
            var id = "123";
            var dto = new AtualizarComentarioCandidaturaDTO
            {
                Comentario = "Comentário atualizado"
            };

            _repositoryMock.Setup(x => x.GetByIdAsync(id))
                .ReturnsAsync((ComentarioCandidaturaDTO)null);

            // Act & Assert
            await Assert.ThrowsAsync<ApplicationException>(() => _service.UpdateAsync(id, dto));
            _repositoryMock.Verify(x => x.GetByIdAsync(id), Times.Once);
            _repositoryMock.Verify(x => x.UpdateAsync(It.IsAny<ComentarioCandidaturaDTO>()), Times.Never);
        }
    }
} 