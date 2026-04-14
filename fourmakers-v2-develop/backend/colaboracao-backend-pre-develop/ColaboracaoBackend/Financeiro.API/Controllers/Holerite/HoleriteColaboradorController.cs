using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Financeiro.Holerite;
using DataTransferObject.Domain.Usuario;
using Financeiro.Domain.Interfaces.Holerite;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Logs.Infra.Attributes;

namespace Financeiro.API.Controllers.Holerite;

[ApiController]
[HandleException]
[Authorize]
[Route("api/Financeiro/Holerite/Colaborador")]
[LogAction]
public class HoleriteColaboradorController : ControllerBase
{
    private readonly IHoleriteColaboradorService _holeriteColaboradorService;
    private readonly UsuarioLogadoDTO _usuarioLogado;

    public HoleriteColaboradorController(IHoleriteColaboradorService holeriteColaboradorService, IAspNetUser aspNetuser)
    {
        _holeriteColaboradorService = holeriteColaboradorService;
        _usuarioLogado = aspNetuser.GetUsuarioLogado();
    }

    [HttpGet("ListarHoleritesColaboradorPorAno")]
    public async Task<ActionResult<ApiGenericResult<IEnumerable<HoleriteColaboradorDTO>>>> ListarHoleritesColaboradorPorAno([FromQuery] string codigoInternoColaborador, int ano)
    {
        var result = await _holeriteColaboradorService.ListarHoleritesColaboradorPorAnoAsync(codigoInternoColaborador, ano, _usuarioLogado.OrgId);
        return Ok(result);
    }
    
    [HttpGet("AssinarHoleritePorLoteId")]
    public async Task<ActionResult<ApiGenericResult>> AssinarHoleritePorLoteId([FromQuery] string itemLoteId)
    {
        var result = await _holeriteColaboradorService.AssinarHoleritePorLoteId(itemLoteId, _usuarioLogado.Token, _usuarioLogado.Cpf, _usuarioLogado.OrgId);

        return Ok(result);
    }
}