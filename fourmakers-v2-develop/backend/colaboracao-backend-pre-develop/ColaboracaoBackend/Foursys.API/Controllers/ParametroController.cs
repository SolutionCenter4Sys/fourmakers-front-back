using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Fourmakers.Parametro;
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
    public class ParametroController : ControllerBase
    {
        private readonly IParametroService _parametroService;
        private readonly UsuarioLogadoDTO _usuarioLogado;

        public ParametroController(IParametroService parametroService, IAspNetUser aspNetUser)
        {
            _parametroService = parametroService;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
        }

        [HttpGet("ListarParametros")]
        public async Task<ActionResult> ListarParametros()
        {
            var parametros = await _parametroService.ListarParametros(_usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(parametros);
        }

        [HttpGet("ObterParametroPorId")]
        public async Task<ActionResult> ObterParametroPorId(string id)
        {
            var parametro = await _parametroService.ObterParametroPorId(id, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(parametro);
        }

        [HttpPost("InserirParametro")]
        public async Task<ActionResult> InserirParametro([FromBody] ParametroInput param)
        {
            var result = await _parametroService.InserirParametro(param, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(result);
        }

        [HttpPut("AtualizarParametro")]
        public async Task<ActionResult> AtualizarParametro(Guid id, [FromBody] ParametroInput param)
        {
            var result = await _parametroService.AtualizarParametro(param, id, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(result);
        }

        [HttpDelete("DeletarParametro")]
        public async Task<ActionResult> DeletarParametro(string id)
        {
            var result = await _parametroService.DeletarParametro(id, _usuarioLogado.Cpf, _usuarioLogado.OrgId);

            if (!result.Sucesso)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
    }
}