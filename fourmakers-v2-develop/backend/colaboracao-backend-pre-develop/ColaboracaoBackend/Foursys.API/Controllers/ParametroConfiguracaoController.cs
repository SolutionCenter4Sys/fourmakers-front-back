using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Fourmakers.ParametroConfiguracao;
using DataTransferObject.Domain.Usuario;
using Foursys.Domain.Interfaces.Services;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Foursys.API.Controllers
{
    [Authorize]
    [Route("api/Fourmakers/[controller]")]
    [HandleException]
    [ApiController]
    [LogAction]
    public class ParametroConfiguracaoController : ControllerBase
    {
        private readonly IParametroConfiguracaoService _parametroConfiguracaoService;
        private readonly UsuarioLogadoDTO _usuarioLogado;

        public ParametroConfiguracaoController(IParametroConfiguracaoService parametroConfiguracaoService, IAspNetUser aspNetUser)
        {
            _parametroConfiguracaoService = parametroConfiguracaoService;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
        }

        [HttpGet("ListarParametroConfiguracaoDoUsuarioLogado")]
        public async Task<ActionResult> ListarParametroConfiguracaoDoUsuarioLogado()
        {
            var parametros = await _parametroConfiguracaoService.ListarParametroConfiguracaoDoUsuarioLogado(_usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(parametros);
        }

        [HttpGet("ListarParametroConfiguracao")]
        public async Task<ActionResult> ListarParametroConfiguracao(int orgId)
        {
            var parametros = await _parametroConfiguracaoService.ListarParametroConfiguracao(_usuarioLogado.Cpf, _usuarioLogado.OrgId, orgId);
            return Ok(parametros);
        }

        [HttpGet("ObterParametroConfiguracaoPorId")]
        public async Task<ActionResult> ObterParametroConfiguracaoPorId(string id, int orgId)
        {
            var parametro = await _parametroConfiguracaoService.ObterParametroConfiguracaoPorId(id, _usuarioLogado.Cpf, _usuarioLogado.OrgId, orgId);
            return Ok(parametro);
        }

        [HttpPost("InserirParametroConfiguracao")]
        public async Task<ActionResult> InserirParametroConfiguracao([FromBody] ParametroConfiguracaoInput param, int orgId)
        {
            var result = await _parametroConfiguracaoService.InserirParametroConfiguracao(param, _usuarioLogado.Cpf, _usuarioLogado.OrgId, orgId);
            return Ok(result);
        }

        [HttpPut("AtualizarParametroConfiguracao")]
        public async Task<ActionResult> AtualizarParametroConfiguracao(Guid id, int orgId, [FromBody] ParametroConfiguracaoInput param)
        {
            var result = await _parametroConfiguracaoService.AtualizarParametroConfiguracao(param, id, _usuarioLogado.Cpf, _usuarioLogado.OrgId, orgId);
            return Ok(result);
        }

        [HttpDelete("DeletarParametroConfiguracao")]
        public async Task<ActionResult> DeletarParametroConfiguracao(string id, int orgId)
        {
            var result = await _parametroConfiguracaoService.DeletarParametroConfiguracao(id, _usuarioLogado.Cpf, _usuarioLogado.OrgId, orgId);

            if (!result.Sucesso)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
    }
}