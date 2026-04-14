using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Social.API.Controllers;
using Social.Domain.Interfaces;
using DataTransferObject.Domain;
using Xunit;
using Colaboracao.Core.Interfaces;

namespace Social.API.Tests.Controllers
{
    public class ComentariosCandidaturaControllerTests
    {
        private readonly Mock<IComentarioCandidaturaService> _serviceMock;
        private readonly Mock<IAspNetUser> _aspNetUser;
        private readonly ComentariosCandidaturaController _controller;

        public ComentariosCandidaturaControllerTests()
        {
            _serviceMock = new Mock<IComentarioCandidaturaService>();
            _aspNetUser = new Mock<IAspNetUser>();
            _controller = new ComentariosCandidaturaController(_serviceMock.Object, _aspNetUser.Object);
        }

        [Fact]
        public async Task GetById_QuandoComentarioExiste_DeveRetornarOk()
        {
            // Arrange
            var id = "123";
            var comentario = new ComentarioCandidaturaDTO
            {
                Id = id,
                Texto = "Teste",
                DataCriacao = DateTime.UtcNow,
                CodigoInternoColaborador = "456",
                CandidaturaId = "789"
            };

            _serviceMock.Setup(x => x.GetByIdAsync(id))
                .ReturnsAsync(comentario);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<ComentarioCandidaturaDTO>(okResult.Value);
            Assert.Equal(id, returnValue.Id);
            _serviceMock.Verify(x => x.GetByIdAsync(id), Times.Once);
        }

        [Fact]
        public async Task GetById_QuandoComentarioNaoExiste_DeveRetornarNotFound()
        {
            // Arrange
            var id = "123";
            _serviceMock.Setup(x => x.GetByIdAsync(id))
                .ReturnsAsync((ComentarioCandidaturaDTO)null);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
            _serviceMock.Verify(x => x.GetByIdAsync(id), Times.Once);
        }

        [Fact]
        public async Task GetByColaboradorCodigo_QuandoExistemComentarios_DeveRetornarOk()
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

            _serviceMock.Setup(x => x.GetByColaboradorCodigoAsync(colaboradorCodigo))
                .ReturnsAsync(comentarios);

            // Act
            var result = await _controller.GetByColaboradorCodigo(colaboradorCodigo);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<ComentarioCandidaturaDTO>>(okResult.Value);
            Assert.Equal(2, returnValue.Count());
            _serviceMock.Verify(x => x.GetByColaboradorCodigoAsync(colaboradorCodigo), Times.Once);
        }

        [Fact]
        public async Task GetByCandidaturaId_QuandoExistemComentarios_DeveRetornarOk()
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

            _serviceMock.Setup(x => x.GetByCandidaturaIdAsync(CandidaturaId))
                .ReturnsAsync(comentarios);

            // Act
            var result = await _controller.GetByCandidaturaId(CandidaturaId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<ComentarioCandidaturaDTO>>(okResult.Value);
            Assert.Equal(2, returnValue.Count());
            _serviceMock.Verify(x => x.GetByCandidaturaIdAsync(CandidaturaId), Times.Once);
        }

        [Fact]
        public async Task Create_ComParametrosValidos_DeveRetornarCreated()
        {
            // Arrange
            var dto = new CriarComentarioCandidaturaDTO
            {
                Comentario = "Teste",
                CandidaturaId = "456"
            };

            var comentarioCriado = new ComentarioCandidaturaDTO
            {
                Id = "789",
                Texto = dto.Comentario,
                DataCriacao = DateTime.UtcNow,
                CodigoInternoColaborador = "1",
                CandidaturaId = dto.CandidaturaId
            };

            _serviceMock.Setup(x => x.CreateAsync(dto, "1"))
                .ReturnsAsync(comentarioCriado);

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnValue = Assert.IsType<ComentarioCandidaturaDTO>(createdResult.Value);
            Assert.Equal(comentarioCriado.Id, returnValue.Id);
            Assert.Equal("GetById", createdResult.ActionName);
            _serviceMock.Verify(x => x.CreateAsync(dto, "1"), Times.Once);
        }

        [Fact]
        public async Task Create_QuandoOcorreErro_DeveRetornarBadRequest()
        {
            // Arrange
            var dto = new CriarComentarioCandidaturaDTO
            {
                Comentario = "Teste",
                CandidaturaId = "456"
            };

            _serviceMock.Setup(x => x.CreateAsync(dto, "1"))
                .ThrowsAsync(new Exception("Erro ao criar comentário"));

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Erro ao criar comentário", badRequestResult.Value);
            _serviceMock.Verify(x => x.CreateAsync(dto, "1"), Times.Once);
        }

        [Fact]
        public async Task Update_QuandoComentarioExiste_DeveRetornarOk()
        {
            // Arrange
            var id = "123";
            var dto = new AtualizarComentarioCandidaturaDTO
            {
                Comentario = "Comentário atualizado"
            };

            var comentarioAtualizado = new ComentarioCandidaturaDTO
            {
                Id = id,
                Texto = dto.Comentario,
                DataCriacao = DateTime.UtcNow,
                DataAlteracao = DateTime.UtcNow,
                CodigoInternoColaborador = "456",
                CandidaturaId = "789"
            };

            _serviceMock.Setup(x => x.UpdateAsync(id, dto))
                .ReturnsAsync(comentarioAtualizado);

            // Act
            var result = await _controller.Update(id, dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<ComentarioCandidaturaDTO>(okResult.Value);
            Assert.Equal(id, returnValue.Id);
            Assert.Equal(dto.Comentario, returnValue.Texto);
            _serviceMock.Verify(x => x.UpdateAsync(id, dto), Times.Once);
        }

        [Fact]
        public async Task Update_QuandoOcorreErro_DeveRetornarBadRequest()
        {
            // Arrange
            var id = "123";
            var dto = new AtualizarComentarioCandidaturaDTO
            {
                Comentario = "Comentário atualizado"
            };

            _serviceMock.Setup(x => x.UpdateAsync(id, dto))
                .ThrowsAsync(new Exception("Erro ao atualizar comentário"));

            // Act
            var result = await _controller.Update(id, dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Erro ao atualizar comentário", badRequestResult.Value);
            _serviceMock.Verify(x => x.UpdateAsync(id, dto), Times.Once);
        }
    }
} 