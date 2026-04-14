using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Projeto;
using DataTransferObject.Domain.Projeto.ClienteOrgCrud;
using DataTransferObject.Domain.Usuario;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Projeto.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Projeto.API.Controllers
{
    [Authorize]
    [Route("api/Projeto/Cliente")]
    [HandleException]
    [ApiController]
    [LogAction]
    public class ClienteOrgController : ControllerBase
    {
        private readonly IClienteOrgService _clienteOrgService;
        private readonly IClienteOrgCrudService _clienteOrgCrudService;
        private readonly UsuarioLogadoDTO _usuarioLogado;

        public ClienteOrgController(IClienteOrgService clienteOrgService, IClienteOrgCrudService clienteOrgCrudService, IAspNetUser aspNetUser)
        {
            _clienteOrgService = clienteOrgService;
            _clienteOrgCrudService = clienteOrgCrudService;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
        }

        [HttpGet("ListarClientesOrg")]
        public async Task<ActionResult<List<ClienteOrgDTO>>> ListarClientesOrg(string codigoClienteFiltro, string codigoGerenteProjeto)
        {
            var statusResult = new StatusResult();
            try
            {
                var listaClientesOrg = await _clienteOrgService.ListarClientesOrg(_usuarioLogado.OrgId, codigoClienteFiltro, codigoGerenteProjeto);
                return listaClientesOrg;
            }
            catch (Exception e)
            {
                statusResult.Sucesso = false;
                statusResult.Mensagem = e.Message;
                return StatusCode(500, statusResult);
            }
        }

        [HttpGet("ListarClienteOrgCrud")]
        public async Task<ActionResult> ListarClienteOrgCrud()
        {
            var perfis = await _clienteOrgCrudService.ListarClienteOrgCruds(_usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(perfis);
        }

        [HttpGet("ObterClienteOrgPorId")]
        public async Task<ActionResult> ObterClienteOrgPorId(Guid id)
        {
            var perfil = await _clienteOrgCrudService.ObterClienteOrgCrudPorId(id, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(perfil);
        }

        [HttpPost("InserirClienteOrg")]
        public async Task<ActionResult> InserirClienteOrg([FromBody] ClienteOrgCrudInput param)
        {
            var result = await _clienteOrgCrudService.InserirClienteOrgCrud(param, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(result);
        }

        [HttpPut("AtualizarClienteOrg")]
        public async Task<ActionResult> AtualizarClienteOrg(Guid id, [FromBody] ClienteOrgCrudInput param)
        {
            var result = await _clienteOrgCrudService.AtualizarClienteOrgCrud(param, id, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(result);
        }

        [HttpDelete("DeletarClienteOrg")]
        public async Task<ActionResult> DeletarClienteOrg(Guid id)
        {
            var result = await _clienteOrgCrudService.DeletarClienteOrgCrud(id, _usuarioLogado.Cpf, _usuarioLogado.OrgId);

            if (!result.Sucesso)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
    }
}