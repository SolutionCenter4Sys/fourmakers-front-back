using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Projeto;
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
    [Route("api/Projeto/[controller]")]
    [ApiController]
    [LogAction]
    public class AtividadeProjetoController : ControllerBase
    {
        private readonly IAtividadeProjetoService _atividadeProjetoService;
        private readonly UsuarioLogadoDTO _usuarioLogado;
        public AtividadeProjetoController(IAtividadeProjetoService atividadeProjetoService, IAspNetUser usuarioLogado)
        {
            _atividadeProjetoService = atividadeProjetoService;
            _usuarioLogado = usuarioLogado.GetUsuarioLogado();
        }
        [HttpGet("ListarAtividades")]
        public async Task<ActionResult<ApiGenericResult<List<AtividadeProjetoDTO>>>> ListarAtividades()
        {
            var ret = new ApiGenericResult<List<AtividadeProjetoDTO>>();
            try
            {
                ret.Retorno = await _atividadeProjetoService.ListarAtividades(_usuarioLogado.OrgId);
                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }
    }
}