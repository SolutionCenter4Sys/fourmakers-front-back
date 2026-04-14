using System.Collections.Generic;
using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Marketing.Comunicacao.Label;
using DataTransferObject.Domain.Usuario;
using Marketing.Domain.Interfaces.Comunicacao.Label;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Marketing.API.Controllers.Comunicacao.Label
{
    [Authorize]
    [Route("api/Marketing/Comunicacao/[controller]")]
    [HandleException]
    [ApiController]
    public class LabelController : ControllerBase
    {
        private readonly IComunicacaoLabelService _service;
        private readonly UsuarioLogadoDTO _usuarioLogado;

        public LabelController(
            IComunicacaoLabelService service,
            IAspNetUser aspNetUser)
        {
            _service = service;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
        }

        [HttpGet]
        public async Task<ActionResult> ObterTodasLabels([FromQuery] List<string> tipos = null)
        {
            var resultado = await _service.ObterTodasLabelsAsync(_usuarioLogado.OrgId, tipos ?? new List<string> { "informativo", "documento" });
            return Ok(resultado);
        }

        [HttpPut("{labelId}")]
        public async Task<ActionResult> AtualizarLabel(string labelId, [FromBody] AtualizarLabelRequestDTO request)
        {
            var resultado = await _service.AtualizarLabelAsync(
                labelId,
                request,
                _usuarioLogado.OrgId
            );

            return Ok(resultado);
        }

        [HttpDelete("{labelId}")]
        public async Task<ActionResult> RemoverLabel(string labelId)
        {
            var resultado = await _service.RemoverLabelAsync(
                labelId,
                _usuarioLogado.OrgId
            );

            return Ok(resultado);
        }
    }
}
