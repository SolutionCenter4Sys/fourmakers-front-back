using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Vaga;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Social.Domain.Impl;
using Social.Domain.Interfaces;

namespace Social.API.Controllers
{
    [HandleException]
    [Route("api/Social/[controller]")]
    [ApiController]
    [Authorize]
    [LogAction]
    public class ComentariosCandidaturaController : ControllerBase
    {
        private readonly IComentarioCandidaturaService _service;
        private readonly IAspNetUser _aspNetUser;

        public ComentariosCandidaturaController(IComentarioCandidaturaService service, IAspNetUser aspNetUser)
        {
            _service = service;
            _aspNetUser = aspNetUser;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ComentarioCandidaturaDTO>> GetById(string id)
        {
            var result = new ApiGenericResult<ComentarioCandidaturaDTO>();
            result.Retorno = await _service.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpGet("colaborador/{colaboradorCodigo}")]
        public async Task<ActionResult<IEnumerable<ComentarioCandidaturaDTO>>> GetByColaboradorCodigo(string colaboradorCodigo)
        {
            var result = new ApiGenericResult<IEnumerable<ComentarioCandidaturaDTO>>();
            result.Retorno = await _service.GetByColaboradorCodigoAsync(colaboradorCodigo);
            return Ok(result);
        }

        [HttpGet("candidatura/{candidatoVagaId}")]
        public async Task<ActionResult<IEnumerable<ComentarioCandidaturaDTO>>> GetByCandidaturaId(string candidatoVagaId)
        {
            var result = new ApiGenericResult<IEnumerable<ComentarioCandidaturaDTO>>();
            result.Retorno = await _service.GetByCandidaturaIdAsync(candidatoVagaId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<ApiGenericResult<ComentarioCandidaturaDTO>>> Create(CriarComentarioCandidaturaDTO dto)
        {
            var result = new ApiGenericResult<ComentarioCandidaturaDTO>();
            result.Retorno = await _service.CreateAsync(dto, _aspNetUser.GetUsuarioLogado().Cpf);
            result.Mensagem = "Comentario incluido com sucesso";
            return CreatedAtAction(nameof(GetById), new { id = result.Retorno.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiGenericResult<bool>>> Update(string id, AtualizarComentarioCandidaturaDTO dto)
        {
            var result = new ApiGenericResult<bool>();
            await _service.UpdateAsync(id, dto);
            return Ok(result);
        }
    }
}