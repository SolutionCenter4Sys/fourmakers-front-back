using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using Colaborador.Domain.Interfaces.Services.BancoDeTalentos;
using DataTransferObject.Domain.BancoDeTalentos;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Log;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfil;
using DataTransferObject.Domain.Vaga;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Colaborador.API.Controllers
{
    [Route("api/Colaborador/[controller]")]
    [HandleException]
    [ApiController]
    [Authorize]
    [LogAction]
    public class BancoDeTalentosController : Controller
    {
        private readonly IAspNetUser _aspNetUser;
        private readonly IBancoDeTalentosService _bancoTalentosService;
        private readonly ILogCore _log;

        public BancoDeTalentosController(
            IAspNetUser aspNetUser,
            IBancoDeTalentosService bancoTalentosService,
            ILogCore log)
        {
            _aspNetUser = aspNetUser;
            _bancoTalentosService = bancoTalentosService;
            _log = log;
        }

        [HttpGet("BuscarPessoasQueEuCadastrei")]
        public async Task<ActionResult<StatusResult>> BuscarPessoasQueEuCadastrei(string busca, int cursor, int limite)
        {
            return StatusCode(200, await _bancoTalentosService.BuscarPessoasQueEuCadastrei(_aspNetUser.GetUsuarioLogado().Cpf, _aspNetUser.GetUsuarioLogado().OrgId, busca, cursor, limite));
        }

        [HttpGet("BuscarMeusLotes")]
        public async Task<ActionResult<StatusResult>> BuscarMeusLotes()
        {
            return StatusCode(200, await _bancoTalentosService.BuscarMeusLotes(_aspNetUser.GetUsuarioLogado().Cpf, _aspNetUser.GetUsuarioLogado().OrgId));
        }

        [HttpGet("BuscarInformacoesLote")]
        public async Task<ActionResult<StatusResult>> BuscarInformacoesLote(string idLote)
        {
            return StatusCode(200, await _bancoTalentosService.BuscarInformacoesLote(idLote));
        }

        [HttpGet("BuscarPessoasCadastradasPorColaborador")]
        public async Task<ActionResult<StatusResult>> BuscarPessoasCadastradasPorColaborador(string codColaborador, string busca, int cursor, int limite)
        {
            return StatusCode(200, await _bancoTalentosService.BuscarPessoasQueEuCadastrei(codColaborador, _aspNetUser.GetUsuarioLogado().OrgId, busca, cursor, limite));
        }

        [HttpGet("BuscarRecrutadoresQuantidadeCadastrada")]
        public async Task<ActionResult<StatusResult>> BuscarRecrutadoresQuantidadeCadastrada()
        {
            return StatusCode(200, await _bancoTalentosService.BuscarRecrutadoresQuantidadeCadastrada());
        }

        [HttpGet("BuscarBancoTalentos")]
        public async Task<ActionResult<StatusResult>> BuscarBancoTalentos(string busca, int cursor, int limite)
        {
            return StatusCode(200, await _bancoTalentosService.BuscarBancoTalentos(_aspNetUser.GetUsuarioLogado().OrgId, busca, cursor, limite));
        }

        [HttpPost("BuscarBancoTalentosComMatch")]
        public async Task<ActionResult<StatusResult>> BuscarBancoTalentosComMatch([FromQuery] int limite, [FromQuery] int cursor, GestorExternoPerfilInput gestorExternoPerfilInput)
        {
            return StatusCode(200, await _bancoTalentosService.BuscarBancoTalentosComMatch(gestorExternoPerfilInput, _aspNetUser.GetUsuarioLogado().OrgId, limite, cursor));
        }

        [HttpPost("BuscarBancoTalentosComPromptMatch")]
        public async Task<ActionResult<ApiGenericResult<PromptMatchResultSnakeCase>>> BuscarBancoTalentosComPromptMatch([FromQuery] int limite, [FromQuery] int cursor, [FromQuery] string? idVaga, ExtrairPerfilDeUmPromptRequest request)
        {
            var result = new ApiGenericResult<PromptMatchResultSnakeCase>();
            try
            {
                var usuarioLogado = _aspNetUser.GetUsuarioLogado();
                var codigoInternoColaborador = !string.IsNullOrEmpty(usuarioLogado.CodColaborador) ? usuarioLogado.CodColaborador : usuarioLogado.Cpf;
                result.Retorno = await _bancoTalentosService.BuscarBancoTalentosComPromptMatchSnakeCase(request, usuarioLogado.OrgId, limite, cursor, codigoInternoColaborador, idVaga);
                result.Sucesso = true;
                result.Mensagem = "Busca realizada com sucesso.";
                return StatusCode(StatusCodes.Status200OK, result);
            }
            catch (UnauthorizedAccessException e)
            {
                _log.Log($"[{GetType().Name}] {e.GetType().Name}: {e.Message}", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception e) when (e is ArgumentException || e is ApplicationException || e is InvalidOperationException)
            {
                _log.Log($"[{GetType().Name}] {e.GetType().Name}: {e.Message}", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return BadRequest(result);
            }
            catch (Exception e)
            {
                _log.Log($"[{GetType().Name}] {e.GetType().Name}: {e.Message}", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        [HttpPost("BuscarPessoasCadastradasPorOrg")]
        public async Task<ActionResult<StatusResult>> BuscarPessoasCadastradasPorOrg(BuscarPessoasCadastradasPorOrgInput input)
        {
            var usuarioLogado = _aspNetUser.GetUsuarioLogado();
            var resultado = await _bancoTalentosService.BuscarPessoasCadastradasPorOrg(usuarioLogado.Cpf, usuarioLogado.OrgId, input);
            return StatusCode(200, resultado);
        }
    }
}