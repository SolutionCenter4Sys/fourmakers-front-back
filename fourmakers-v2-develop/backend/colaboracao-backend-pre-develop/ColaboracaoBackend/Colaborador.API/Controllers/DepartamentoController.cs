using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using Colaborador.Domain.Interfaces.Services;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Diretoria;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Colaborador.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/Colaborador/[controller]")]
    [LogAction]
    public class DepartamentoController : Controller
    {
        private readonly IColaboradorDepartamentoService _colaboradorDepartamentoService;
        private readonly IAspNetUser _aspNetUser;

        public DepartamentoController(IColaboradorDepartamentoService colaboradorDepartamentoService, IAspNetUser aspNetUser)
        {
            _colaboradorDepartamentoService = colaboradorDepartamentoService;
            _aspNetUser = aspNetUser;
        }

        [HttpGet("ListarDepartamentosDosColaboradores")]
        public async Task<ActionResult<ApiGenericResult<List<DepartamentoColaboradorDTO>>>> ListarDepartamentosDosColaboradores([FromQuery] string codigoDiretoria)
        {
            var ret = new ApiGenericResult<List<DepartamentoColaboradorDTO>>();
            try
            {
                var usuarioLogado = _aspNetUser.GetUsuarioLogado();
                ret.Retorno = await _colaboradorDepartamentoService.ListarDepartamentosDosColaboradoresAsync(usuarioLogado.OrgId, codigoDiretoria, usuarioLogado.Cpf);
                return ret;
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return ret;
            }
        }
    }
}