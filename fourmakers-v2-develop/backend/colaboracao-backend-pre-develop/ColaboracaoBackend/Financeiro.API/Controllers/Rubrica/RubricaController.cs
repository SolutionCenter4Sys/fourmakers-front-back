using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Financeiro.Rubrica.Rubrica;
using DataTransferObject.Domain.Usuario;
using Financeiro.Domain.Interfaces.Rubrica.Rubrica;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Logs.Infra.Attributes;

namespace Financeiro.API.Controllers.Rubrica
{
    [Authorize]
    [Route("api/Financeiro/Rubrica")]
    [HandleException]
    [ApiController]
    [LogAction]
    public class RubricaController : ControllerBase
    {
        private readonly IRubricaService _rubricaService;
        private readonly UsuarioLogadoDTO _usuarioLogado;

        public RubricaController(IRubricaService rubricaService, IAspNetUser aspNetUser)
        {
            _rubricaService = rubricaService;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
        }

        [HttpGet("ListarRubricas")]
        public async Task<ActionResult> ListarRubricas([FromQuery] bool? somenteComTemplates)
        {
            var perfis = await _rubricaService.ListarRubricas(_usuarioLogado.Cpf, _usuarioLogado.OrgId, somenteComTemplates?? false);
            return Ok(perfis);
        }

        [HttpGet("ObterRubricaPorId")]
        public async Task<ActionResult> ObterRubricaPorId(Guid id)
        {
            var perfil = await _rubricaService.ObterRubricaPorId(id, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(perfil);
        }

        [HttpPost("InserirRubrica")]
        public async Task<ActionResult> InserirRubrica([FromBody] RubricaInput param)
        {
            var result = await _rubricaService.InserirRubrica(param, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(result);
        }

        [HttpPut("AtualizarRubrica")]
        public async Task<ActionResult> AtualizarRubrica(Guid id, [FromBody] RubricaInput param)
        {
            var result = await _rubricaService.AtualizarRubrica(param, id, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(result);
        }

        [HttpDelete("DeletarRubrica")]
        public async Task<ActionResult> DeletarRubrica(Guid id)
        {
            var result = await _rubricaService.DeletarRubrica(id, _usuarioLogado.Cpf, _usuarioLogado.OrgId);

            if (!result.Sucesso)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
    }
}
