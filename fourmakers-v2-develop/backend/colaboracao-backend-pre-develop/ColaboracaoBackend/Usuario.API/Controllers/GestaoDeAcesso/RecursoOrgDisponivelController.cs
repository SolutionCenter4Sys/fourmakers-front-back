using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Usuario;
using DataTransferObject.Domain.Usuario.GestaoDeAcesso.Recurso;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Usuario.Domain.Interfaces.Services.GestaoDeAcesso.Recurso;

namespace Usuario.API.Controllers.GestaoDeAcesso
{
    [Authorize]
    [Route("api/Usuario/GestaoDeAcesso/[controller]")]
    [HandleException]
    [LogAction]
    [ApiController]
    public class RecursoOrgDisponivelController : ControllerBase
    {
        private readonly IRecursoOrgDisponivelService _recursoOrgDisponivelService;
        private readonly UsuarioLogadoDTO _usuarioLogado;

        public RecursoOrgDisponivelController(IRecursoOrgDisponivelService recursoOrgDisponivelService, IAspNetUser aspNetUser)
        {
            _recursoOrgDisponivelService = recursoOrgDisponivelService;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
        }

        [HttpPost("ConfigurarRecursoOrgDisponivel")]
        public async Task<IActionResult> ConfigurarRecursoOrgDisponivel([FromBody] List<RecursoOrgDisponivelDTO> input)
        {
            var resultado = await _recursoOrgDisponivelService.ConfigurarRecursoOrgDisponivel(input, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(resultado);
        }

        [HttpGet("ListarRecursoOrgDisponivel")]
        public async Task<IActionResult> ListarRecursoOrgDisponivel()
        {
            var resultado = await _recursoOrgDisponivelService.ListarRecursoOrgDisponivelAsync(_usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(resultado);
        }

        [HttpGet("ObterUltimaInsercaoRecursoOrgLog")]
        public async Task<IActionResult> ObterUltimaInsercaoRecursoOrgLog()
        {
            var resultado = await _recursoOrgDisponivelService.ObterUltimaInsercaoRecursoOrgDisponivelLog(_usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(resultado);
        }


    }
}
