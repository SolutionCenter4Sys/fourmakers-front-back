using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Marketing.Comunicacao.Tag;
using DataTransferObject.Domain.Usuario;
using Marketing.Domain.Interfaces.Comunicacao.Tag;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Marketing.API.Controllers.Comunicacao.Tag
{
    [Authorize]
    [Route("api/Marketing/Comunicacao/[controller]")]
    [HandleException]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly IComunicacaoTagService _service;
        private readonly UsuarioLogadoDTO _usuarioLogado;

        public TagController(
            IComunicacaoTagService service,
            IAspNetUser aspNetUser)
        {
            _service = service;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
        }

        [HttpGet]
        public async Task<ActionResult> ObterTodasTags()
        {
            var resultado = await _service.ObterTodasTagsAsync(_usuarioLogado.OrgId);
            return Ok(resultado);
        }

        [HttpPut("{tagId}")]
        public async Task<ActionResult> AtualizarTag(string tagId, [FromBody] AtualizarTagRequestDTO request)
        {
            var resultado = await _service.AtualizarTagAsync(
                tagId,
                request,
                _usuarioLogado.OrgId
            );

            return Ok(resultado);
        }

        [HttpDelete("{tagId}")]
        public async Task<ActionResult> RemoverTag(string tagId)
        {
            var resultado = await _service.RemoverTagAsync(
                tagId,
                _usuarioLogado.OrgId
            );

            return Ok(resultado);
        }
    }
}
