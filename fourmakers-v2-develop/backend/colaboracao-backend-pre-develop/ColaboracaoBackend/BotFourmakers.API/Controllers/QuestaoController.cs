using BotFourmakers.Domain.Interfaces;
using BotFourmakers.Domain.Interfaces.Questao;
using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.BotFourmakers;
using DataTransferObject.Domain.BotFourmakers.Questao;
using DataTransferObject.Domain.Usuario;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BotFourmakers.API.Controllers;

[Authorize]
[Route("api/BotFourmakers/[controller]")]
[HandleException]
[LogAction]
public class QuestaoController : ControllerBase
{
    private UsuarioLogadoDTO _usuarioLogado;
    private readonly IQuestaoServices _questaoServices;

    public QuestaoController(IAspNetUser aspNetUser, IQuestaoServices questaoServices)
    {
        _usuarioLogado = aspNetUser.GetUsuarioLogado();
        _questaoServices = questaoServices;
    }

    [HttpPost("Inserir")]
    public async Task<ActionResult<ApiGenericResult<QuestaoDTO>>> Inserir([FromBody] NovaQuestaoParam parametros)
    {
        return Ok(await _questaoServices.InserirAsync(parametros.Questao, parametros.ChatId, _usuarioLogado.Cpf, _usuarioLogado.OrgId));
    }
    
    [HttpGet("Listar")]
    public async Task<ActionResult<ApiGenericResult<List<QuestaoDTO>>>> Listar([FromQuery] int chatId)
    {
        return Ok(await _questaoServices.ListarAsync(chatId));
    }
}