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
    public class PassaporteColaboradorController : Controller
    {
        private readonly ILogCore _log;
        private readonly IAspNetUser _aspNetUser;
        private readonly IPassaporteColaboradorService _passaporteColaboradorService;

        public PassaporteColaboradorController(
            ILogCore log,
            IAspNetUser aspNetUser,
            IPassaporteColaboradorService passaporteColaboradorService)
        {
            _log = log;
            _aspNetUser = aspNetUser;
            _passaporteColaboradorService = passaporteColaboradorService;
        }

        [HttpPost("AdicionarPassaporte")]
        public async Task<ActionResult<StatusResult>> AdicionarPassaporte([FromBody] PassaporteColaboradorDTO passaporteColaboradorDTO)
        {
            try
            {
                var usuarioLogado = _aspNetUser.GetUsuarioLogado();

                var adicionarPassaporteResult = await _passaporteColaboradorService.AdicionarPassaporteColaborador(
                    usuarioLogado.Cpf, passaporteColaboradorDTO);

                return StatusCode(200, adicionarPassaporteResult);
            }
            catch (Exception ex)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(ex.StackTrace, LevelsEnum.Critical);

                return BadRequest(
                    new
                    {
                        sucesso = false,
                        mensagem = "Erro ao adicionar o passaporte",
                        erros = ex.Message
                    });
            }
        }

        [HttpPut("AlterarPassaporte/{idPassaporteColaborador}")]
        public async Task<ActionResult<StatusResult>> AlterarPassaporte(
            [FromRoute] int idPassaporteColaborador, [FromBody] PassaporteColaboradorDTO passaporteColaboradorDTO)
        {
            try
            {
                var usuarioLogado = _aspNetUser.GetUsuarioLogado();
                passaporteColaboradorDTO.Id = idPassaporteColaborador;

                var alterarPassaporteResult = await _passaporteColaboradorService.AlterarPassaporteColaborador(
                    usuarioLogado.Cpf, passaporteColaboradorDTO);

                return Ok(
                    new
                    {
                        sucesso = alterarPassaporteResult,
                        mensagem = "Passaporte alterado com sucesso"
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
                        mensagem = "Erro ao alterar o passaporte",
                        erros = ex.Message
                    });
            }
        }

        [HttpDelete("RemoverPassaporte/{idPassaporteColaborador}")]
        public async Task<ActionResult<StatusResult>> RemoverPassaporte([FromRoute] int idPassaporteColaborador)
        {
            try
            {
                var usuarioLogado = _aspNetUser.GetUsuarioLogado();

                var removerPassaporteResult = await _passaporteColaboradorService.RemoverPassaporteColaborador(
                    usuarioLogado.Cpf, idPassaporteColaborador);

                return Ok(
                    new
                    {
                        sucesso = removerPassaporteResult,
                        mensagem = "Passaporte removido com sucesso"
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
                        mensagem = "Erro ao remover o passaporte",
                        erros = ex.Message
                    });
            }
        }

        [HttpGet("ObterPassaportes")]
        public async Task<ActionResult<PassaporteColaboradorResult>> ObterPassaportes()
        {
            try
            {
                var usuarioLogado = _aspNetUser.GetUsuarioLogado();
                var passaporteColaboradorResult = new PassaporteColaboradorResult();

                var listaPassaporteResult = await _passaporteColaboradorService.ObterPassaportesPorCpfColaborador(
                    usuarioLogado.Cpf);

                passaporteColaboradorResult.PassaportesColaborador = listaPassaporteResult;

                return StatusCode(200, listaPassaporteResult);
            }
            catch (Exception ex)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(ex.StackTrace, LevelsEnum.Critical);

                return BadRequest(
                    new
                    {
                        sucesso = false,
                        mensagem = "Erro ao buscar passaportes",
                        erros = ex.Message
                    });
            }
        }
    }
}