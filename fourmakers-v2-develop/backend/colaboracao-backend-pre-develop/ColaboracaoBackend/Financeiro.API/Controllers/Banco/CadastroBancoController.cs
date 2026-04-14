using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Financeiro.Banco.CadastroBanco;
using DataTransferObject.Domain.Usuario;
using Financeiro.Domain.Interfaces.Banco.CadastroBanco;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Financeiro.API.Controllers.Banco
{
    [Authorize]
    [Route("api/Banco/[controller]")]
    [HandleException]
    [ApiController]
    public class CadastroBancoController : ControllerBase
    {
        private readonly ICadastroBancoService _cadastroBancoService;
        private readonly UsuarioLogadoDTO _usuarioLogado;

        public CadastroBancoController(ICadastroBancoService cadastroBancoService, IAspNetUser aspNetUser)
        {
            _cadastroBancoService = cadastroBancoService;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
        }

        [HttpGet("ListarBancos")]
        public async Task<ActionResult> ListarBancos()
        {
            var perfis = await _cadastroBancoService.ListarCadastroBancos(_usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(perfis);
        }

        [HttpGet("ObterBancoPorCodigo")]
        public async Task<ActionResult> ObterBancoPorCodigo(string codigoBanco)
        {
            var perfil = await _cadastroBancoService.ObterCadastroBancoPorCodigo(codigoBanco, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(perfil);
        }

        [HttpPost("InserirBanco")]
        public async Task<ActionResult> InserirBanco([FromBody] CadastroBancoInput param)
        {
            var result = await _cadastroBancoService.InserirCadastroBanco(param, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(result);
        }

        // Não vamos dar update nos cadastros de banco por enquanto
        //[HttpPut("AtualizarCadastroBanco")]
        //public async Task<ActionResult> AtualizarCadastroBanco(Guid id, [FromBody] CadastroBancoInput param)
        //{
        //    var result = await _cadastroBancoService.AtualizarCadastroBanco(param, id, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
        //    return Ok(result);
        //}

        [HttpDelete("DeletarBanco")]
        public async Task<ActionResult> DeletarBanco(string codigoBanco)
        {
            var result = await _cadastroBancoService.DeletarCadastroBanco(codigoBanco, _usuarioLogado.Cpf, _usuarioLogado.OrgId);

            if (!result.Sucesso)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
    }
}
