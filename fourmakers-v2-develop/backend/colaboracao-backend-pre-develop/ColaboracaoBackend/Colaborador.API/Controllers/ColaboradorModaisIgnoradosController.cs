using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using Colaborador.Domain.Interfaces.Services;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Colaborador;
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
    [Route("api/Colaborador/[controller]")]
    [HandleException]
    [LogAction]
    public class ColaboradorModaisIgnoradosController : ControllerBase
    {
        private readonly IColaboradorModaisIgnoradosService _colaboradorModaisIgnoradosService;
        private readonly UsuarioLogadoDTO _usuarioLogado;

        public ColaboradorModaisIgnoradosController(IColaboradorModaisIgnoradosService colaboradorModaisIgnoradosService, IAspNetUser aspNetUser)
        {
            _colaboradorModaisIgnoradosService = colaboradorModaisIgnoradosService;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
        }

        /// <summary>Lista as tags dos modais ignorados do usuário logado (Cpf/OrgId do token).</summary>
        [HttpGet("ListarMeusIgnorados")]
        public async Task<ActionResult<ApiGenericResult<List<string>>>> ListarMeusIgnorados()
        {
            var result = await _colaboradorModaisIgnoradosService.ListarMeusIgnoradosAsync(_usuarioLogado.Cpf, _usuarioLogado.OrgId);
            if (!result.Sucesso)
                return BadRequest(result);
            return Ok(result);
        }

        /// <summary>Marca um modal como ignorado. Body: { "tag": "NOME_DA_TAG" }. Retorna a lista de tags; se já existir, apenas retorna a lista (nunca duplica).</summary>
        [HttpPost("Ignorar")]
        public async Task<ActionResult<ApiGenericResult<List<string>>>> Ignorar([FromBody] ColaboradorModaisIgnoradosRequestDTO request)
        {
            var result = await _colaboradorModaisIgnoradosService.IgnoreDialogAsync(_usuarioLogado.Cpf, _usuarioLogado.OrgId, request?.Tag);
            if (!result.Sucesso)
                return BadRequest(result);
            return Ok(result);
        }
    }
}
