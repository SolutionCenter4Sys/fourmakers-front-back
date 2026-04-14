using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Usuario.GestaoDeAcesso.Recurso;
using DataTransferObject.Domain.Usuario;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Usuario.Domain.Interfaces.Services.GestaoDeAcesso.Recurso;

namespace Usuario.API.Controllers.GestaoDeAcesso
{
    [Authorize]
    [Route("api/Usuario/GestaoDeAcesso/[controller]")]
    [HandleException]
    [LogAction]
    [ApiController]
    public class RecursoController : ControllerBase
    {
        private readonly IRecursoService _recursoService;
        private readonly UsuarioLogadoDTO _usuarioLogado;

        public RecursoController(IRecursoService recursoService, IAspNetUser aspNetUser)
        {
            _recursoService = recursoService;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
        }

        [HttpGet("ListarRecursos")]
        public async Task<ActionResult> ListarRecursos()
        {
            var perfis = await _recursoService.ListarRecursos(_usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(perfis);
        }

        [HttpGet("ListarRecursosVisaoMenu")]
        public async Task<ActionResult> ListarRecursosVisaoMenu()
        {
            var perfis = await _recursoService.ListarRecursosVisaoMenu(_usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(perfis);
        }

        [HttpPost("InserirRecursos")]
        public async Task<ActionResult> InserirRecursos([FromBody] List<RecursoInput> recursos, bool forcarExclusao)
        {
            var result = await _recursoService.InserirRecursos(recursos, forcarExclusao, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(result);
        }

        [HttpPut("AtualizarRecurso")]
        public async Task<ActionResult> AtualizarRecurso([FromBody] RecursoInput param, string codigoRecurso)
        {
            var result = await _recursoService.AtualizarRecurso(param, param.CodigoRecurso, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(result);
        }

        [HttpDelete("DeletarRecurso")]
        public async Task<ActionResult> DeletarRecurso(string codigoRecurso, bool forcarExclusao)
        {
            var result = await _recursoService.DeletarRecurso(codigoRecurso, forcarExclusao, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(result);
        }


        [HttpDelete("DeletarTodosRecurso")]
        public async Task<ActionResult> DeletarTodosRecurso(string codigoRecurso)
        {
            var result = await _recursoService.DeletarTodosRecurso(_usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(result);
        }

        [HttpGet("ObterUltimaInsercaoRecursoMenuLog")]
        public async Task<IActionResult> ObterUltimaInsercaoRecursoMenuLog()
        {
            var resultado = await _recursoService.ObterUltimaInsercaoRecursoMenuLog(_usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(resultado);
        }

    }
}
