using BotFourmakers.Domain.Interfaces.Feedback;
using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.BotFourmakers.Feedback;
using DataTransferObject.Domain.Usuario;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BotFourmakers.API.Controllers;

[Authorize]
[Route("api/BotFourmakers/[controller]")]
[HandleException]
[LogAction]
public class FeedbackController : ControllerBase
{
    private readonly IFeedbackIAService _feedbackIaService;
    private readonly UsuarioLogadoDTO _usuarioLogado;

    public FeedbackController(IFeedbackIAService feedbackIaService, IAspNetUser aspNetUser)
    {
        _feedbackIaService = feedbackIaService;
        _usuarioLogado = aspNetUser.GetUsuarioLogado();
    }

    [HttpPost("Inserir")]
    public async Task<ActionResult<ApiGenericResult<FeedbackDTO>>> Inserir([FromBody] NovoFeedbackParam parametros)
    {
        return Ok(await _feedbackIaService.InserirFeedback(parametros, _usuarioLogado.Cpf));
    }
    
    [HttpGet("BuscarPorId")]
    public async Task<ActionResult<ApiGenericResult<FeedbackDTO>>> BuscarPorId([FromQuery] int  feedbackId)
    {
        return Ok(await _feedbackIaService.BuscarFeedbackPorIdAsync(feedbackId));
    }
}