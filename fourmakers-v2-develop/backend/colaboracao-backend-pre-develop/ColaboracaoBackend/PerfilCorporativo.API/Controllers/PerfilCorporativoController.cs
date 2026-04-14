using Colaboracao.Core;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerfilCorporativo.Domain.Interfaces;

namespace PerfilCorporativo.API.Controllers;

[HandleException]
[Route("api/PerfilCorporativo/[controller]")]
[ApiController]
[Authorize]
[LogAction]
public class PerfilCorporativoController : ControllerBase
{
    private readonly IPerfilCorporativoService _perfilCorporativoService;

    public PerfilCorporativoController(IPerfilCorporativoService perfilCorporativoService)
    {
        _perfilCorporativoService = perfilCorporativoService;
    }

    [HttpGet("health")]
    public IActionResult Health() => Ok("PerfilCorporativo API");
}
