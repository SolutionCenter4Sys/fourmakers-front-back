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
    public class MaterialAreaController : ControllerBase
    {
        private readonly IMaterialAreaService _service;
        private readonly DataTransferObject.Domain.Usuario.UsuarioLogadoDTO _usuarioLogado;

        public MaterialAreaController(IMaterialAreaService service, IAspNetUser aspNetUser)
        {
            _service = service;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
        }

        [HttpGet("Listar")]
        public async Task<ActionResult> Listar()
        {
            var result = await _service.ListarAsync(_usuarioLogado.OrgId);
            return Ok(result);
        }

        [HttpPost("Criar")]
        public async Task<ActionResult> Criar([FromBody] CriarMaterialAreaInput input)
        {
            var result = await _service.CriarAsync(input, _usuarioLogado.OrgId);
            return Ok(result);
        }

        [HttpPut("Atualizar/{id}")]
        public async Task<ActionResult> Atualizar(string id, [FromBody] AtualizarMaterialAreaInput input)
        {
            var result = await _service.AtualizarAsync(id, input, _usuarioLogado.OrgId);
            return Ok(result);
        }

        [HttpDelete("Remover/{id}")]
        public async Task<ActionResult> Remover(string id)
        {
            var result = await _service.RemoverAsync(id, _usuarioLogado.OrgId);
            return Ok(result);
        }
    }
}
