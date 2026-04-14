using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Financeiro.IntegracaoBancaria.CnabOrg;
using DataTransferObject.Domain.Usuario;
using Financeiro.Domain.Interfaces.IntegracaoBancaria.CnabOrg;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Financeiro.API.Controllers.IntegracaoBancaria
{
    [Authorize]
    [Route("api/Financeiro/IntegracaoBancaria/[controller]")]
    [HandleException]
    [ApiController]
    public class CnabOrgController : ControllerBase
    {
        private readonly ICnabOrgService _cnabOrgService;
        private readonly UsuarioLogadoDTO _usuarioLogado;

        public CnabOrgController(ICnabOrgService cnabOrgService, IAspNetUser aspNetUser)
        {
            _cnabOrgService = cnabOrgService;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
        }

        [HttpGet("ListarCnabOrgs")]
        public async Task<ActionResult> ListarCnabOrgs()
        {
            var perfis = await _cnabOrgService.ListarCnabOrgs(_usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(perfis);
        }

        [HttpGet("ObterCnabOrgPorId")]
        public async Task<ActionResult> ObterCnabOrgPorId(Guid id)
        {
            var perfil = await _cnabOrgService.ObterCnabOrgPorId(id, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(perfil);
        }

        [HttpPost("InserirCnabOrg")]
        public async Task<ActionResult> InserirCnabOrg([FromBody] CnabOrgInput param)
        {
            var result = await _cnabOrgService.InserirCnabOrg(param, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(result);
        }

        [HttpPut("AtualizarCnabOrg")]
        public async Task<ActionResult> AtualizarCnabOrg(Guid id, [FromBody] CnabOrgInput param)
        {
            var result = await _cnabOrgService.AtualizarCnabOrg(param, id, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(result);
        }

        [HttpDelete("DeletarCnabOrg")]
        public async Task<ActionResult> DeletarCnabOrg(Guid id)
        {
            var result = await _cnabOrgService.DeletarCnabOrg(id, _usuarioLogado.Cpf, _usuarioLogado.OrgId);

            if (!result.Sucesso)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpGet("ListarDiretoriasDisponiveis")]
        public async Task<ActionResult> ListarDiretoriasDisponiveis()
        {
            var result = await _cnabOrgService.ListarDiretoriasDisponiveis(_usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(result);
        }
    }
}
