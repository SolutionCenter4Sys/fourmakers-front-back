using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using Colaborador.Domain.Interfaces.Services;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Colaborador.Cidadania;
using DataTransferObject.Domain.Usuario;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Colaborador.API.Controllers
{
    [Authorize]
    [ApiController]
    [HandleException]
    [Route("api/Colaborador/[controller]")]
    [LogAction]
    public class CidadaniaController : ControllerBase
    {
        private readonly ICidadaniaService _cidadaniaService;
        private readonly UsuarioLogadoDTO _usuarioLogado;

        public CidadaniaController(ICidadaniaService cidadaniaService, IAspNetUser aspNetUser)
        {
            _cidadaniaService = cidadaniaService;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
        }

        [HttpGet("ListarCidadanias")]
        public async Task<ActionResult<ApiGenericResult<List<CidadaniaDTO>>>> ListarCidadanias()
        {
            var result = await _cidadaniaService.ListarCidadanias();
            return Ok(result);
        }

        [HttpGet("ListarStatusCidadania")]
        public async Task<ActionResult<ApiGenericResult<List<CidadaniaStatusDTO>>>> ListarStatusCidadania()
        {
            var result = await _cidadaniaService.ListarStatusCidadania();
            return Ok(result);
        }

        [HttpPost("InserirCidadaniaColaborador")]
        public async Task<ActionResult<ApiGenericResult<CidadaniaColaboradorDTO>>> InserirCidadaniaColaborador([FromBody] CidadaniaColaboradorParam param)
        {
            var result = await _cidadaniaService.InserirCidadaniaColaborador(param.CidadaniaId, param.CidadaniaStatusId, _usuarioLogado.Cpf);
            return Ok(result);
        }

        [HttpPost("AtualizarCidadaniaColaborador")]
        public async Task<ActionResult<ApiGenericResult<CidadaniaColaboradorDTO>>> AtualizarCidadaniaColaborador([FromBody] CidadaniaColaboradorParam param)
        {
            var result = await _cidadaniaService.AtualizarCidadaniaColaborador(param.CidadaniaStatusId, param.CidadaniaId, _usuarioLogado.Cpf);
            return Ok(result);
        }

        [HttpDelete("RemoverCidadaniaColaborador")]
        public async Task<ActionResult<ApiGenericResult<bool>>> RemoverCidadaniaColaborador([FromQuery] int id)
        {
            var result = await _cidadaniaService.RemoverCidadaniaColaborador(id, _usuarioLogado.Cpf);
            return Ok(result);
        }
    }
}