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
    public class ChamadoController : ControllerBase
    {
        private readonly IChamadoService _service;
        private readonly DataTransferObject.Domain.Usuario.UsuarioLogadoDTO _usuarioLogado;

        public ChamadoController(IChamadoService service, IAspNetUser aspNetUser)
        {
            _service = service;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
        }

        [HttpPost("Criar")]
        public async Task<ActionResult> Criar([FromBody] CriarChamadoInput input)
        {
            var result = await _service.CriarAsync(input, _usuarioLogado.Cpf, null, _usuarioLogado.OrgId);
            return Ok(result);
        }

        [HttpPut("Atualizar/{id}")]
        public async Task<ActionResult> Atualizar(string id, [FromBody] AtualizarChamadoInput input)
        {
            var result = await _service.AtualizarAsync(id, input, _usuarioLogado.Cpf, null, _usuarioLogado.OrgId);
            return Ok(result);
        }

        [HttpGet("Listar")]
        public async Task<ActionResult> Listar([FromQuery] string status = null)
        {
            var result = await _service.ListarAsync(_usuarioLogado.Cpf, _usuarioLogado.OrgId, status);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> ObterPorId(string id)
        {
            var result = await _service.ObterPorIdAsync(id, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(result);
        }

        [HttpGet("DistribuicaoStatus")]
        public async Task<ActionResult> DistribuicaoStatus()
        {
            var result = await _service.ObterDistribuicaoStatusAsync(_usuarioLogado.OrgId);
            return Ok(result);
        }
    }
}
