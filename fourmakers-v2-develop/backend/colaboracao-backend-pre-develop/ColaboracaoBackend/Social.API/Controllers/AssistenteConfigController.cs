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
    public class AssistenteConfigController : ControllerBase
    {
        private readonly IAssistenteConfigService _service;
        private readonly DataTransferObject.Domain.Usuario.UsuarioLogadoDTO _usuarioLogado;

        public AssistenteConfigController(IAssistenteConfigService service, IAspNetUser aspNetUser)
        {
            _service = service;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
        }

        [HttpGet("Obter")]
        public async Task<ActionResult> Obter()
        {
            var result = await _service.ObterAsync(_usuarioLogado.OrgId);
            return Ok(result);
        }

        [HttpPut("Atualizar")]
        public async Task<ActionResult> Atualizar([FromBody] AtualizarAssistenteConfigInput input)
        {
            var result = await _service.AtualizarAsync(input, _usuarioLogado.Cpf, null, _usuarioLogado.OrgId);
            return Ok(result);
        }
    }
}
