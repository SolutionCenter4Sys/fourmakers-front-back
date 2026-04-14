using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Usuario;
using Logs.Infra.Attributes;
using MapaDeRelacionamento.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MapaDeRelacionamento.API.Controllers;

[HandleException]
[Route("api/[controller]")]
[ApiController]
[Authorize]
[LogAction]
public class MapaDeRelacionamentoController : ControllerBase
{
    private readonly IMapaDeRelacionamentoService _mapaDeRelacionamentoService;
    UsuarioLogadoDTO _usuarioLogado;

    public MapaDeRelacionamentoController(IMapaDeRelacionamentoService mapaDeRelacionamentoService, IAspNetUser aspNetUser)
    {
        _mapaDeRelacionamentoService = mapaDeRelacionamentoService;
        _usuarioLogado = aspNetUser.GetUsuarioLogado();
    }

    [HttpGet("health")]
    public IActionResult Health() => Ok("MapaDeRelacionamento API");
}
