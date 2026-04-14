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
    public class CuradoriaController : ControllerBase
    {
        private readonly ICuradoriaService _service;
        private readonly DataTransferObject.Domain.Usuario.UsuarioLogadoDTO _usuarioLogado;

        public CuradoriaController(ICuradoriaService service, IAspNetUser aspNetUser)
        {
            _service = service;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
        }

        [HttpPut("AtualizarLog/{id}")]
        public async Task<ActionResult> AtualizarLog(string id, [FromBody] AtualizarCuradoriaInput input)
        {
            var result = await _service.AtualizarLogAsync(id, input, _usuarioLogado.Cpf, null, _usuarioLogado.OrgId);
            return Ok(result);
        }
    }
}
