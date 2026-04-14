using System.Threading.Tasks;
using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Colaborador.Campanha._2025_01_COLETA_PERFIL_COLABORADOR;
using DataTransferObject.Domain.Usuario;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Questionario.Domain.Interfaces;
using Logs.Infra.Attributes;

namespace Foursys.API.Controllers;

[Authorize]
[Route("api/Fourmakers/[controller]")]
[ApiController]
[HandleException]
[LogAction]
public class CampanhaController : ControllerBase
{
    private readonly IQuestionarioService _questionarioService;
    private readonly UsuarioLogadoDTO _usuarioLogadoDTO;

    public CampanhaController(IQuestionarioService questionarioService, IAspNetUser aspNetUser)
    {
        _questionarioService = questionarioService;
        _usuarioLogadoDTO = aspNetUser.GetUsuarioLogado();
    }
    
    [DisableRequestSizeLimit]
    [HttpPost("ColetaPerfilColaboradorCampanha")]
    public async Task<ActionResult<ApiGenericResult<string>>> ColetaPerfilColaboradorCampanha([FromBody] ColetaPerfilColaboradorCampanhaRootDTO root)
    {
        var coletaResult = await _questionarioService.ColetarPerfilColaboradorAsync(root, _usuarioLogadoDTO.Cpf, _usuarioLogadoDTO.OrgId);
        return Ok(coletaResult);
    }
}