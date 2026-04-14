using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Projeto.GestorExterno;
using DataTransferObject.Domain.Usuario;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Projeto.Domain.Interfaces.Services;
using System.Threading.Tasks;

namespace Projeto.API.Controllers
{
    [Authorize]
    [Route("api/GestaoDeAlocados/[controller]")]
    [HandleException]
    [ApiController]
    [LogAction]
    public class GestorExternoController : ControllerBase
    {
        private readonly IGestorExternoService _gestorExternoService;
        private readonly UsuarioLogadoDTO _usuarioLogado;

        public GestorExternoController(IGestorExternoService gestorExternoService, IAspNetUser aspNetUser)
        {
            _gestorExternoService = gestorExternoService;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
        }

        [HttpGet("ListarGestoresExterno")]
        public async Task<ActionResult> ListarGestoresExterno(string codigoCliente, string busca)
        {
            var perfis = await _gestorExternoService.ListarGestoresExternos(_usuarioLogado.Cpf, codigoCliente, _usuarioLogado.OrgId, busca);
            return Ok(perfis);
        }

        [HttpGet("ObterGestorExternoPorId")]
        public async Task<ActionResult> ObterGestorExternoPorCodigo(string codGestorExterno)
        {
            var perfil = await _gestorExternoService.ObterGestorExternoPorCodigo(codGestorExterno, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(perfil);
        }

        [HttpPost("InserirGestorExterno")]
        public async Task<ActionResult> InserirGestorExterno([FromBody] GestorExternoInput param)
        {
            var result = await _gestorExternoService.InserirGestorExterno(param, _usuarioLogado.Cpf, _usuarioLogado.Token, _usuarioLogado.OrgId);
            return Ok(result);
        }

        [HttpPut("AtualizarGestorExterno")]
        public async Task<ActionResult> AtualizarGestorExterno(string codGestorExterno, [FromBody] GestorExternoInput param)
        {
            var result = await _gestorExternoService.AtualizarGestorExterno(param, codGestorExterno, _usuarioLogado.Cpf, _usuarioLogado.Token, _usuarioLogado.OrgId);
            return Ok(result);
        }

        [HttpDelete("DeletarGestorExterno")]
        public async Task<ActionResult> DeletarGestorExterno(string codGestorExterno)
        {
            var result = await _gestorExternoService.DeletarGestorExterno(codGestorExterno, _usuarioLogado.Cpf, _usuarioLogado.OrgId);

            if (!result.Sucesso)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
    }
}