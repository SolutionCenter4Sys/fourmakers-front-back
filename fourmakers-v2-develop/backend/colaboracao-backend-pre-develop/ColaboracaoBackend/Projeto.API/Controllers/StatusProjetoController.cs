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
    public class StatusProjetoController : ControllerBase
    {
        private readonly IStatusProjetoService _statusProjetoService;
        private readonly UsuarioLogadoDTO _usuarioLogado;
        public StatusProjetoController(IAspNetUser aspNetUser, IStatusProjetoService statusProjetoService)
        {
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
            _statusProjetoService = statusProjetoService;
        }

        [HttpGet("ListarStatus")]
        public async Task<ActionResult<ApiGenericResult<List<StatusProjetosDTO>>>> ListarStatus()
        {
            var ret = new ApiGenericResult<List<StatusProjetosDTO>>();
            try
            {
                ret.Retorno = await _statusProjetoService.ListarStatus(_usuarioLogado.OrgId);
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