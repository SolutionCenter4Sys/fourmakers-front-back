using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using Colaborador.Domain.Interfaces.Colaborador.DepartamentoOrg;
using DataTransferObject.Domain.Colaborador.DepartamentoOrg;
using DataTransferObject.Domain.Usuario;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Colaborador.API.Controllers.Colaborador
{
    [Authorize]
    [Route("api/Colaborador/[controller]")]
    [HandleException]
    [ApiController]
    [LogAction]
    public class DepartamentoOrgController : ControllerBase
    {
        private readonly IDepartamentoOrgService _departamentoOrgService;
        private readonly UsuarioLogadoDTO _usuarioLogado;

        public DepartamentoOrgController(IDepartamentoOrgService departamentoOrgService, IAspNetUser aspNetUser)
        {
            _departamentoOrgService = departamentoOrgService;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
        }

        [HttpGet("ListarDepartamentoOrgs")]
        public async Task<ActionResult> ListarDepartamentoOrgs()
        {
            var perfis = await _departamentoOrgService.ListarDepartamentoOrgs(_usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(perfis);
        }

        [HttpGet("ObterDepartamentoOrgPorId")]
        public async Task<ActionResult> ObterDepartamentoOrgPorId(Guid id)
        {
            var perfil = await _departamentoOrgService.ObterDepartamentoOrgPorId(id, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(perfil);
        }

        [HttpPost("InserirDepartamentoOrg")]
        public async Task<ActionResult> InserirDepartamentoOrg([FromBody] DepartamentoOrgInput param)
        {
            var result = await _departamentoOrgService.InserirDepartamentoOrg(param, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(result);
        }

        [HttpPut("AtualizarDepartamentoOrg")]
        public async Task<ActionResult> AtualizarDepartamentoOrg(Guid id, [FromBody] DepartamentoOrgInput param)
        {
            var result = await _departamentoOrgService.AtualizarDepartamentoOrg(param, id, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(result);
        }

        [HttpDelete("DeletarDepartamentoOrg")]
        public async Task<ActionResult> DeletarDepartamentoOrg(Guid id)
        {
            var result = await _departamentoOrgService.DeletarDepartamentoOrg(id, _usuarioLogado.Cpf, _usuarioLogado.OrgId);

            if (!result.Sucesso)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
    }
}