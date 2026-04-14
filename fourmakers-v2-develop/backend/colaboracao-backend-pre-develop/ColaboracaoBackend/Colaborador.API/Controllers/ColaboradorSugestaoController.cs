using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using Colaborador.Domain.Interfaces.Services;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Usuario;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Colaborador.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/Colaborador/[controller]")]
    [LogAction]
    public class SugestaoController : Controller
    {
        private readonly IColaboradorSugestaoService _colaboradorSugestaoService;
        private readonly UsuarioLogadoDTO _usuarioLogado;

        public SugestaoController(IColaboradorSugestaoService colaboradorSugestaoService, IAspNetUser aspNetUser, ILogCore log, IConfiguration configuration)
        {
            _colaboradorSugestaoService = colaboradorSugestaoService;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
        }

        [HttpGet("ListarEmpresasRelacionadas")]
        public async Task<ActionResult<ApiGenericResult<List<string>>>> ListarEmpresasRelacionadas(String nomeEmpresa, int limite, int cursor)
        {
            var ret = new ApiGenericResult<List<string>>();
            try
            {
                ret.Retorno = await _colaboradorSugestaoService.ListarEmpresasRelacionadas(nomeEmpresa, limite, cursor, _usuarioLogado.OrgId);
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