using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain;
using DataTransferObject.Domain.Base;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Social.Domain.Interfaces;

namespace Social.API.Controllers
{
    [HandleException]
    [Route("api/Social/[controller]")]
    [ApiController]
    [Authorize]
    [LogAction]
    public class ComentariosVagaController : ControllerBase
    {
        private readonly IComentarioVagaService _service;
        private readonly IAspNetUser _aspNetUser;

        public ComentariosVagaController(IComentarioVagaService service, IAspNetUser aspNetUser)
        {
            _service = service;
            _aspNetUser = aspNetUser;
        }

        [HttpGet("comentario")]
        public async Task<ActionResult<ComentarioVagaDTO>> GetById(string comentarioId)
        {
            var result = new ApiGenericResult<ComentarioVagaDTO>();
            result.Retorno = await _service.GetByIdAsync(comentarioId);
            return Ok(result);
        }

        [HttpGet("vaga")]
        public async Task<ActionResult<IEnumerable<ComentarioVagaDTO>>> GetByVagaId(string vagaId)
        {
            var result = new ApiGenericResult<IEnumerable<ComentarioVagaDTO>>();
            result.Retorno = await _service.GetByVagaIdAsync(vagaId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<ApiGenericResult<ComentarioVagaDTO>>> Create(CriarComentarioVagaDTO dto)
        {
            var result = new ApiGenericResult<ComentarioVagaDTO>();
            result.Retorno = await _service.CreateAsync(dto, _aspNetUser.GetUsuarioLogado().Cpf);
            result.Mensagem = "Comentario incluido com sucesso";
            //return CreatedAtAction(nameof(GetById), new { id = result.Retorno.Id }, result);
            return null;
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiGenericResult<bool>>> Update(string id, AtualizarComentarioVagaDTO dto)
        {
            var result = new ApiGenericResult<bool>();
            await _service.UpdateAsync(id, dto);
            return Ok(result);
        }
    }
}