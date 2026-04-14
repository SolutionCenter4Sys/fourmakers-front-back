using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Social.AtendimentoFourmakers;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Social.Domain.Interfaces.AtendimentoFourmakers;

namespace Social.API.Controllers
{
    [HandleException]
    [Route("api/Social/[controller]")]
    [ApiController]
    [Authorize]
    [LogAction]
    public class KnowledgeBaseController : ControllerBase
    {
        private readonly IKnowledgeBaseService _service;
        private readonly DataTransferObject.Domain.Usuario.UsuarioLogadoDTO _usuarioLogado;

        public KnowledgeBaseController(IKnowledgeBaseService service, IAspNetUser aspNetUser)
        {
            _service = service;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
        }

        [HttpPost("Ingerir")]
        public async Task<ActionResult> Ingerir([FromBody] IngerirDocumentoInput input)
        {
            var result = await _service.IngerirAsync(input, _usuarioLogado.Cpf, null, _usuarioLogado.OrgId);
            return Ok(result);
        }

        [HttpDelete("RemoverFonte")]
        public async Task<ActionResult> RemoverFonte([FromBody] RemoverFonteInput input)
        {
            var result = await _service.RemoverFonteAsync(input, _usuarioLogado.Cpf, null, _usuarioLogado.OrgId);
            return Ok(result);
        }

        [HttpGet("Totais")]
        public async Task<ActionResult> Totais()
        {
            var result = await _service.ObterTotaisAsync(_usuarioLogado.OrgId);
            return Ok(result);
        }

        [HttpGet("Listar")]
        public async Task<ActionResult> Listar()
        {
            var result = await _service.ListarFontesAsync(_usuarioLogado.OrgId);
            return Ok(result);
        }

        [HttpGet("{fonteId}")]
        public async Task<ActionResult> ObterPorFonteId(string fonteId)
        {
            var result = await _service.ObterFontePorIdAsync(fonteId, _usuarioLogado.OrgId);
            return Ok(result);
        }
    }
}
