using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Marketing.Comunicacao.Publicacao;
using DataTransferObject.Domain.Usuario;
using Marketing.Domain.Interfaces.Comunicacao.Publicacao;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Marketing.API.Controllers.Comunicacao.PublicacaoComentario
{
    [Authorize]
    [Route("api/Marketing/Comunicacao/[controller]")]
    [HandleException]
    [ApiController]
    public class PublicacaoComentarioController : ControllerBase
    {
        private readonly IComunicacaoPublicacaoService _service;
        private readonly UsuarioLogadoDTO _usuarioLogado;

        public PublicacaoComentarioController(
            IComunicacaoPublicacaoService service,
            IAspNetUser aspNetUser)
        {
            _service = service;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
        }

        [HttpPost("Comentarios")]
        public async Task<ActionResult> AdicionarComentario([FromBody] InserirComentarioRequestDTO request)
        {
            if (request == null)
                return BadRequest(new ApiGenericResult<Guid> { Sucesso = false, Mensagem = "Request obrigatório." });
            if (request.PublicacaoId == Guid.Empty)
                return BadRequest(new ApiGenericResult<Guid> { Sucesso = false, Mensagem = "PublicacaoId obrigatório." });
            if (string.IsNullOrWhiteSpace(request.Conteudo))
                return BadRequest(new ApiGenericResult<Guid> { Sucesso = false, Mensagem = "Conteudo do comentário obrigatório." });

            var resultado = await _service.AdicionarComentarioAsync(
                _usuarioLogado.Cpf,
                _usuarioLogado.OrgId,
                request
            );
            return Ok(resultado);
        }

        [HttpPut("Comentarios")]
        public async Task<ActionResult> AtualizarComentario([FromBody] AtualizarComentarioRequestDTO request)
        {
            var resultado = await _service.AtualizarComentarioAsync(
                _usuarioLogado.Cpf,
                _usuarioLogado.OrgId,
                request
            );
            return Ok(resultado);
        }

        [HttpDelete("{publicacaoId}/Comentarios/{comentarioId}")]
        public async Task<ActionResult> RemoverComentario(string publicacaoId, string comentarioId)
        {
            var resultado = await _service.RemoverComentarioAsync(
                publicacaoId,
                comentarioId,
                _usuarioLogado.Cpf,
                _usuarioLogado.OrgId
            );
            return Ok(resultado);
        }

        [HttpPost("Comentarios/Interacao")]
        public async Task<ActionResult> AdicionarInteracaoComentario([FromBody] AdicionarInteracaoComentarioRequestDTO request)
        {
            if (request == null)
                return BadRequest(new ApiGenericResult { Sucesso = false, Mensagem = "Request obrigatório." });
            if (request.ComentarioId == Guid.Empty)
                return BadRequest(new ApiGenericResult { Sucesso = false, Mensagem = "ComentarioId obrigatório." });
            if (request.Emoji != null && request.Emoji.Length > 50)
                return BadRequest(new ApiGenericResult { Sucesso = false, Mensagem = "Emoji deve ter no máximo 50 caracteres." });

            var resultado = await _service.AdicionarInteracaoComentarioAsync(
                _usuarioLogado.Cpf,
                _usuarioLogado.OrgId,
                request
            );
            return Ok(resultado);
        }

        [HttpDelete("Comentarios/Interacao")]
        public async Task<ActionResult> RemoverInteracaoComentario([FromBody] RemoverInteracaoComentarioRequestDTO request)
        {
            if (request == null)
                return BadRequest(new ApiGenericResult { Sucesso = false, Mensagem = "Request obrigatório." });
            if (request.ComentarioId == Guid.Empty)
                return BadRequest(new ApiGenericResult { Sucesso = false, Mensagem = "ComentarioId obrigatório." });

            var resultado = await _service.RemoverInteracaoComentarioAsync(
                _usuarioLogado.Cpf,
                _usuarioLogado.OrgId,
                request
            );
            return Ok(resultado);
        }
    }
}
