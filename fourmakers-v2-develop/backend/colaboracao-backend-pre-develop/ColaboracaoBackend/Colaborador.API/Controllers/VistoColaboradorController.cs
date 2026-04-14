using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using Colaborador.Domain.Interfaces.Services;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Log;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Colaborador.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [LogAction]
    public class VistoColaboradorController : Controller
    {
        private readonly ILogCore _log;
        private readonly IAspNetUser _aspNetUser;
        private readonly IVistoColaboradorService _vistoColaboradorService;

        public VistoColaboradorController(
            ILogCore log,
            IAspNetUser aspNetUser,
            IVistoColaboradorService vistoColaboradorService)
        {
            _log = log;
            _aspNetUser = aspNetUser;
            _vistoColaboradorService = vistoColaboradorService;
        }

        [HttpPost("AdicionarVisto")]
        public async Task<ActionResult<StatusResult>> AdicionarVisto([FromBody] VistoColaboradorDTO vistoColaboradorDTO)
        {
            try
            {
                var usuarioLogado = _aspNetUser.GetUsuarioLogado();

                var adicionarVistoResult = await _vistoColaboradorService.AdicionarVistoColaborador(
                    usuarioLogado.Cpf, vistoColaboradorDTO);

                return StatusCode(200, adicionarVistoResult);
            }
            catch (Exception ex)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(ex.StackTrace, LevelsEnum.Critical);

                return BadRequest(
                    new
                    {
                        sucesso = false,
                        mensagem = "Erro ao adicionar o visto",
                        erros = ex.Message
                    });
            }
        }

        [HttpPut("AlterarVisto/{idVistoColaborador}")]
        public async Task<ActionResult<StatusResult>> AlterarVisto(
            [FromRoute] int idVistoColaborador, [FromBody] VistoColaboradorDTO vistoColaboradorDTO)
        {
            try
            {
                var usuarioLogado = _aspNetUser.GetUsuarioLogado();
                vistoColaboradorDTO.Id = idVistoColaborador;

                var alterarVistoResult = await _vistoColaboradorService.AlterarVistoColaborador(
                    usuarioLogado.Cpf, vistoColaboradorDTO);

                return Ok(
                    new
                    {
                        sucesso = alterarVistoResult,
                        mensagem = "Visto alterado com sucesso"
                    });
            }
            catch (Exception ex)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(ex.StackTrace, LevelsEnum.Critical);

                return BadRequest(
                    new
                    {
                        sucesso = false,
                        mensagem = "Erro ao alterar o visto",
                        erros = ex.Message
                    });
            }
        }

        [HttpDelete("RemoverVisto/{idVistoColaborador}")]
        public async Task<ActionResult<StatusResult>> RemoverVisto([FromRoute] int idVistoColaborador)
        {
            try
            {
                var usuarioLogado = _aspNetUser.GetUsuarioLogado();

                var removerVistoResult = await _vistoColaboradorService.RemoverVistoColaborador(
                    usuarioLogado.Cpf, idVistoColaborador);

                return Ok(
                    new
                    {
                        sucesso = removerVistoResult,
                        mensagem = "Visto removido com sucesso"
                    });
            }
            catch (Exception ex)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(ex.StackTrace, LevelsEnum.Critical);

                return BadRequest(
                    new
                    {
                        sucesso = false,
                        mensagem = "Erro ao remover o visto",
                        erros = ex.Message
                    });
            }
        }

        [HttpGet("ObterVistos")]
        public async Task<ActionResult<VistoColaboradorResult>> ObterVistos()
        {
            try
            {
                var usuarioLogado = _aspNetUser.GetUsuarioLogado();
                var vistoColaboradorResult = new VistoColaboradorResult();

                var listaVistoResult = await _vistoColaboradorService.ObterVistosPorCpfColaborador(
                    usuarioLogado.Cpf);

                vistoColaboradorResult.VistosColaborador = listaVistoResult;

                return StatusCode(200, listaVistoResult);
            }
            catch (Exception ex)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(ex.StackTrace, LevelsEnum.Critical);

                return BadRequest(
                    new
                    {
                        sucesso = false,
                        mensagem = "Erro ao buscar vistos",
                        erros = ex.Message
                    });
            }
        }
    }
}