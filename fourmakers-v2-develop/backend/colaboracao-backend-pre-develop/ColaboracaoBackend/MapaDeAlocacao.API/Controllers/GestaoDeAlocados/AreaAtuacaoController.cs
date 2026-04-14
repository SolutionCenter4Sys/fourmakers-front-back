using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Usuario;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Projeto.Domain.Interfaces.Services;
using System.Threading.Tasks;

namespace Projeto.API.Controllers
{
    [Authorize]
    [Route("api/GestaoDeAlocados/[controller]")]
    [HandleException]
    [ApiController]
    [LogAction]
    public class AreaAtuacaoController : ControllerBase
    {
        private readonly IAreaAtuacaoService _areaAtuacaoService;
        private readonly UsuarioLogadoDTO _usuarioLogado;

        public AreaAtuacaoController(IAreaAtuacaoService areaAtuacaoService, IAspNetUser aspNetUser)
        {
            _areaAtuacaoService = areaAtuacaoService;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
        }

        [HttpGet("ListarAreasAtuacao")]
        public async Task<ActionResult> ListarAreasAtuacao()
        {
            var listaAreasAtuacao = await _areaAtuacaoService.ListarAreasAtuacao(_usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(listaAreasAtuacao);
        }
    }
}