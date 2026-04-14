using System.Collections.Generic;
using System.Threading.Tasks;
using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados;
using DataTransferObject.Domain.Usuario;
using Logs.Infra.Attributes;
using MapaDeAlocacao.Domain.Interfaces.GestaoDeAlocados;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MapaDeAlocacao.API.Controllers.GestaoDeAlocados
{
    [Route("api/GestaoDeAlocados/[controller]")]
    [ApiController]
    [HandleException]
    [Authorize]
    [LogAction]
    public class FeedbackLabIAController : ControllerBase
    {
        private readonly IFeedbackLabIAService _feedbackLabIAService;
        private readonly UsuarioLogadoDTO _usuarioLogado;

        public FeedbackLabIAController(IFeedbackLabIAService feedbackLabIAService, IAspNetUser aspNetUser)
        {
            _feedbackLabIAService = feedbackLabIAService;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
        }
        // GET
        
        [HttpPost("InserirFeedbackLabIA")]
        public async Task<ActionResult<FeedbackLabIADTO>> InserirFeedbackLabIA([FromBody] FeedbackLabIAParam param)
        {
            var _codigoColaborador = _usuarioLogado.Cpf;
            
            ApiGenericResult<FeedbackLabIADTO> result = await _feedbackLabIAService.InserirFeedbackLabIA(param,  _codigoColaborador);
            return Ok(result);
        }
        
        [HttpGet("ListarFeedbackLabIA")]
        public async Task<ActionResult<List<FeedbackLabIADTO>>> ListarFeedbackLabIA()
        {
            ApiGenericResult<List<FeedbackLabIADTO>> result = await _feedbackLabIAService.ListarFeedbacksLabIa();
            return Ok(result);
        }
    }

}