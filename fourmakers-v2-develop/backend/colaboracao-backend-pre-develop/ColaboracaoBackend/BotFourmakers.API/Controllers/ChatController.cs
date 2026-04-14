using BotFourmakers.Domain.Interfaces;
using BotFourmakers.Domain.Interfaces.Chat;
using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.BotFourmakers;
using DataTransferObject.Domain.BotFourmakers.Chat;
using DataTransferObject.Domain.Usuario;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BotFourmakers.API.Controllers;

[Authorize]
[Route("api/BotFourmakers/[controller]")]
[HandleException]
[LogAction]
public class ChatController : ControllerBase
{
    private UsuarioLogadoDTO _usuarioLogado;
    private readonly IChatServices _chatServices;

    public ChatController(IAspNetUser aspNetUser, IChatServices chatServices)
    {
        _usuarioLogado = aspNetUser.GetUsuarioLogado();
        _chatServices = chatServices;
    }

    [HttpGet("Listar")]
    public async Task<ActionResult<ApiGenericResult<List<ChatDTO>>>> Listar()
    {
        return Ok(await _chatServices.ListarAsync(_usuarioLogado.Cpf, _usuarioLogado.OrgId));
    }
    
    [HttpDelete("Remover")]
    public async Task<ActionResult<ApiGenericResult<ChatDTO>>> Remover([FromQuery] int chatId)
    {
        return Ok(await _chatServices.RemoverAsync(chatId, _usuarioLogado.Cpf));
    }
}