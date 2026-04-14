using Colaboracao.Core;
using DataTransferObject.Domain;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Match;
using DataTransferObject.Domain.Log;
using DataTransferObject.Domain.SRS;
using DataTransferObject.Domain.SRS.Candidate;
using DataTransferObject.Domain.SRS.Vagas;
using DataTransferObject.Domain.Vaga;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfil;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SRS.API.Controllers
{
    public partial class VagaController
    {

        [HttpGet("ListarVagasCadastradasFourmakers")]
        public async Task<ActionResult<ApiGenericResult<IEnumerable<ListarVagasCadastradasFourmakersResult>>>> ListarVagasCadastradasFourmakers(int limite, int cursor, string codCliente = null, string gestorExternoPerfilId = null, string busca = null)
        {
            try
            {
                var retorno = new ApiGenericResult<IEnumerable<ListarVagasCadastradasFourmakersResult>>();
                var usuarioLogado = _aspNetUser.GetUsuarioLogado();
                var lista = await _vagaService.ListarVagasCadastradas(limite, cursor, usuarioLogado.OrgId, codCliente, gestorExternoPerfilId, busca);
                retorno.Retorno = lista.ToList();
                return retorno;
            }
            catch (UnauthorizedAccessException e)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, new ApiGenericResult<string> { Sucesso = false, Mensagem = "Acesso não autorizado!" });
            }
            catch (ArgumentException e)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new ApiGenericResult<string> { Sucesso = false, Mensagem = e.Message });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiGenericResult<string> { Sucesso = false, Mensagem = e.Message + ". " + e.InnerException.Message });
            }
        }

        [HttpGet("ListarClientesComVagasVigentes")]
        public async Task<ActionResult<ApiGenericResult<List<DropDownItemDTO>>>> ListarClientesComVagasVigentes()
        {
            try
            {
                var usuarioLogado = _aspNetUser.GetUsuarioLogado();
                var result = await _vagaService.ListarClientesComVagasVigentes(usuarioLogado.OrgId);
                return result;
            }
            catch (Exception e)
            {
                return StatusCode(500, new ApiGenericResult<List<DropDownItemDTO>> { Sucesso = false, Mensagem = e.Message });
            }
        }

        [HttpGet("ListarGestorExternoPerfilComVagasVigentesPorCliente")]
        public async Task<ActionResult<ApiGenericResult<List<DropDownItemDTO>>>> ListarGestorExternoPerfilComVagasVigentesPorCliente(string codigoCliente)
        {
            try
            {
                var usuarioLogado = _aspNetUser.GetUsuarioLogado();
                var result = await _vagaService.ListarGestorExternoPerfilComVagasVigentesPorCliente(codigoCliente, usuarioLogado.OrgId);
                return result;
            }
            catch (Exception e)
            {
                return StatusCode(500, new ApiGenericResult<List<DropDownItemDTO>> { Sucesso = false, Mensagem = e.Message });
            }
        }

        [HttpPost("RecomendarCandidatosPorEmail")]
        public async Task<ActionResult<ApiGenericResult>> RecomendarCandidatosPorEmail(RecomendarCandidatosParam param)
        {
            var result = new ApiGenericResult<bool>();
            try
            {
                return await _vagaService.RecomendarCandidatosPorEmail(param);
            }
            catch (UnauthorizedAccessException e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception e) when (e is ArgumentException || e is ApplicationException)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return BadRequest(result);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        [HttpGet("ListarVagasPipeline")]
        public async Task<ActionResult<ApiGenericResult<List<ListarVagasEmBancoDeTalentosResult>>>> ListarVagasPipeline(string dataInicio, string dataFim, string cliente, int cursor, int limite, string busca)
        {
            var result = new ApiGenericResult<bool>();
            try
            {
                return await _vagaService.ListarVagasPipeline(cursor, limite, busca, cliente, dataInicio, dataFim, _aspNetUser.GetUsuarioLogado()?.OrgId);
            }
            catch (UnauthorizedAccessException e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception e) when (e is ArgumentException || e is ApplicationException)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return BadRequest(result);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        [HttpGet("ObterVagaRecrutamentoPorCodigo/{codigo}")]
        public async Task<ActionResult<ApiGenericResult<VagaRecrutamentoDTO>>> ObterVagaRecrutamentoPorCodigo(int codigo)
        {
            var result = new ApiGenericResult<bool>();
            try
            {
                return await _vagaService.ObterVagaRecrutamentoPorCodigo(codigo);
            }
            catch (UnauthorizedAccessException e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception e) when (e is ArgumentException || e is ApplicationException)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return BadRequest(result);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        [HttpGet("ObterVagaRecrutamentoPorId/{vagaId}")]
        public async Task<ActionResult<ApiGenericResult<VagaRecrutamentoDTO>>> ObterVagaRecrutamentoPorId(string vagaId)
        {
            var result = new ApiGenericResult<bool>();
            try
            {
                return await _vagaService.ObterVagaRecrutamentoPorId(vagaId);
            }
            catch (UnauthorizedAccessException e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception e) when (e is ArgumentException || e is ApplicationException)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return BadRequest(result);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        /// <summary>
        /// Lista o histórico de alterações de status da vaga: para qual status foi em cada alteração e quanto tempo ficou no status anterior.
        /// </summary>
        [HttpGet("ListarHistoricoStatusVaga/{vagaId}")]
        public async Task<ActionResult<ApiGenericResult<IEnumerable<HistoricoStatusVagaItemDTO>>>> ListarHistoricoStatusVaga(string vagaId)
        {
            try
            {
                return await _vagaService.ListarHistoricoStatusVaga(vagaId);
            }
            catch (UnauthorizedAccessException e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                return StatusCode(StatusCodes.Status401Unauthorized, new ApiGenericResult<IEnumerable<HistoricoStatusVagaItemDTO>> { Sucesso = false, Mensagem = e.Message });
            }
            catch (Exception e) when (e is ArgumentException || e is ApplicationException)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                return BadRequest(new ApiGenericResult<IEnumerable<HistoricoStatusVagaItemDTO>> { Sucesso = false, Mensagem = e.Message });
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                return StatusCode(500, new ApiGenericResult<IEnumerable<HistoricoStatusVagaItemDTO>> { Sucesso = false, Mensagem = e.Message });
            }
        }

        [HttpPut("AtualizarVagaRecrutamentoPorId")]
        public async Task<ActionResult<ApiGenericResult<VagaRecrutamentoDTO>>> AtualizarVagaRecrutamentoPorId([FromBody] AtualizarVagaRecrutamentoDTO vaga)
        {
            var result = new ApiGenericResult<bool>();
            try
            {
                var usuarioLogado = _aspNetUser.GetUsuarioLogado();
                if (usuarioLogado == null)
                {
                    result.Sucesso = false;
                    result.Mensagem = "Usuário não autenticado";
                    return StatusCode(401, result);
                }

                return await _vagaService.AtualizarVagaRecrutamentoPorId(vaga, usuarioLogado.Cpf);
            }
            catch (UnauthorizedAccessException e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception e) when (e is ArgumentException || e is ApplicationException)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return BadRequest(result);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        [AllowAnonymous]
        [HttpGet("ObterVagaAnonymous/{codigo}")]
        public async Task<ActionResult<VagaAnonymousDTO>> ObterVagaAnonymous(int codigo)
        {
            try
            {
                return await _vagaService.ObterVagaRecrutamentoPorCodigoAnonymous(codigo);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                return StatusCode(500, "Erro interno");
            }
        }

        [HttpPost("InserirVagaRecrutamento")]
        public async Task<ActionResult<ApiGenericResult<VagaRecrutamentoDTO>>> InserirVaga(InserirVagaRecrutamentoDTO vaga)
        {
            var result = new ApiGenericResult<bool>();
            try
            {
                return await _vagaService.InserirVagaRecrutamento(vaga, _aspNetUser.GetUsuarioLogado().Cpf);
            }
            catch (UnauthorizedAccessException e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception e) when (e is ArgumentException || e is ApplicationException)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return BadRequest(result);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        [HttpPut("AtualizarVagaRecrutamento")]
        public async Task<ActionResult<ApiGenericResult>> AtualizarVaga(VagaVindaDeGestorExternoPerfil vaga)
        {
            var result = new ApiGenericResult();
            try
            {
                await _vagaService.AtualizarVagaRecrutamento(vaga, _aspNetUser.GetUsuarioLogado().Cpf, true);
                return NoContent();
            }
            catch (UnauthorizedAccessException e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception e) when (e is ArgumentException || e is ApplicationException)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return BadRequest(result);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        [HttpDelete("CancelarVagaRecrutamento/{codigo}")]
        public async Task<ActionResult<ApiGenericResult<bool>>> CancelarVaga(int codigo)
        {
            var result = new ApiGenericResult();
            try
            {
                return await _vagaService.CancelarVagaRecrutamento(codigo, _aspNetUser.GetUsuarioLogado().Cpf);
            }
            catch (UnauthorizedAccessException e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception e) when (e is ArgumentException || e is ApplicationException)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return BadRequest(result);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        [HttpPost("ListarVagasRecrutamento")]
        public async Task<ActionResult<ApiGenericResult<IEnumerable<VagaRecrutamentoDTO>>>> ListarVagasRecrutamento(
            [FromBody] ListarVagasRecrutamentoParam param)
        {
            var result = new ApiGenericResult<bool>();
            try
            {
                var resultado = await _vagaService.ListarVagasRecrutamento(param.Limite, param.Cursor, param.Busca, param.Status, _aspNetUser.GetUsuarioLogado()?.OrgId, param.DataInicio, param.DataFim);
                return Ok(resultado);
            }
            catch (UnauthorizedAccessException e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception e) when (e is ArgumentException || e is ApplicationException)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return BadRequest(result);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        [HttpPost("CandidatarSe")]
        public async Task<ActionResult<ApiGenericResult<IEnumerable<VagaRecrutamentoDTO>>>> CandidatarSe(CandidatarSeRecrutamentoParam candidatarSeRecrutamentoParam)
        {
            var result = new ApiGenericResult<bool>();
            try
            {
                await _vagaService.CandidatarSe(candidatarSeRecrutamentoParam);
                result.Mensagem = "Sucesso";
                return Ok(result);
            }
            catch (UnauthorizedAccessException e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception e) when (e is ArgumentException || e is ApplicationException)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return BadRequest(result);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        [HttpGet("ListarOpcoesContato")]
        public async Task<ActionResult<ApiGenericResult<IEnumerable<OpcaoContatoDTO>>>> ListarOpcoesContato()
        {
            var result = new ApiGenericResult<IEnumerable<OpcaoContatoDTO>>();
            try
            {
                result = await _vagaService.ListarOpcoesContato();
                return Ok(result);
            }
            catch (UnauthorizedAccessException e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception e) when (e is ArgumentException || e is ApplicationException)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return BadRequest(result);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        [HttpGet("ListarCandidatosInscritos")]
        public async Task<ActionResult<ApiGenericResult<IEnumerable<ListarCandidatosInscritosResult>>>> ListarCandidatosInscritos(string vagaId, string busca, string dataInicio, string dataFim, int cursor, int limite, bool? qualificados = null, int? diasUltimaAlteracao = null, string? LocalizacaoCidade = null, string? LocalizacaoEstado = null)
        {
            var result = new ApiGenericResult<IEnumerable<ListarCandidatosInscritosResult>>();

            try
            {
                var usuarioLogado = _aspNetUser.GetUsuarioLogado();
                result.Retorno = await _vagaService.ListarCandidatosInscritos(vagaId, busca, dataInicio, dataFim, cursor, limite, usuarioLogado.Cpf, qualificados, diasUltimaAlteracao, LocalizacaoCidade, LocalizacaoEstado);
                result.Sucesso = true;
                return Ok(result);
            }
            catch (UnauthorizedAccessException e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception e) when (e is ArgumentException || e is ApplicationException)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return BadRequest(result);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        [HttpGet("ListarStatusVagaRecrutamento")]
        public async Task<ActionResult<ApiGenericResult<IEnumerable<StatusVagaRecrutamentoDTO>>>> ListarStatusVagaRecrutamento()
        {
            var result = new ApiGenericResult<IEnumerable<StatusVagaRecrutamentoDTO>>();
            try
            {
                var usuarioLogado = _aspNetUser.GetUsuarioLogado();

                result.Retorno = await _vagaService.ListarStatusVagaRecrutamento(usuarioLogado.OrgId);
                return Ok(result);
            }
            catch (UnauthorizedAccessException e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception e) when (e is ArgumentException || e is ApplicationException)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return BadRequest(result);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        [HttpPut("AtualizarOrdemStatusVagaRecrutamento")]
        public async Task<ActionResult<ApiGenericResult<bool>>> AtualizarOrdemStatusVagaRecrutamento([FromBody] AtualizarOrdemStatusVagaRecrutamentoParam param)
        {
            var result = new ApiGenericResult<bool>();
            try
            {
                var usuarioLogado = _aspNetUser.GetUsuarioLogado();
                if (usuarioLogado == null)
                {
                    result.Sucesso = false;
                    result.Mensagem = "Usuário não autenticado";
                    return StatusCode(401, result);
                }

                result = await _vagaService.AtualizarOrdemStatusVagaRecrutamento(param, usuarioLogado.Cpf);
                return Ok(result);
            }
            catch (UnauthorizedAccessException e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception e) when (e is ArgumentException || e is ApplicationException)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return BadRequest(result);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        [HttpPost("MudarStatusVaga")]
        public async Task<ActionResult<ApiGenericResult<bool>>> MudarStatusVaga(MudarStatusVagaRecrutamentoParam mudarStatusVagaRecrutamentoParam)
        {
            var result = new ApiGenericResult<bool>();
            try
            {
                await _vagaService.MudarStatusVaga(mudarStatusVagaRecrutamentoParam, _aspNetUser.GetUsuarioLogado().Cpf, _aspNetUser.GetUsuarioLogado().OrgId);
                result.Retorno = true;
                result.Mensagem = "Sucesso";
                return Ok(result);
            }
            catch (UnauthorizedAccessException e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception e) when (e is ArgumentException || e is ApplicationException)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return BadRequest(result);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        [HttpPost("GravarPerdaVaga")]
        public async Task<ActionResult<ApiGenericResult<bool>>> GravarPerdaVaga(GravarPerdaVagaParam gravarPerdaVagaParam)
        {
            var result = new ApiGenericResult<bool>();
            try
            {
                var usuarioLogado = _aspNetUser.GetUsuarioLogado();
                await _vagaService.GravarPerdaVaga(gravarPerdaVagaParam.CodigoVaga, gravarPerdaVagaParam.IdMotivoPerda, gravarPerdaVagaParam.Comentario, usuarioLogado.Cpf, usuarioLogado.OrgId);
                result.Sucesso = true;
                result.Mensagem = "Vaga marcada como perdida com sucesso.";
                return Ok(result);
            }
            catch (UnauthorizedAccessException e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception e) when (e is ArgumentException || e is ApplicationException)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return BadRequest(result);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        [HttpPost("MudarStatusCandidatura")]
        public async Task<ActionResult<ApiGenericResult<MudarStatusCandidaturaResultDTO>>> MudarStatusCandidatura(MudarStatusCandidaturaRecrutamentoParam mudarStatusCandidaturaRecrutamentoParam)
        {
            var result = new ApiGenericResult<MudarStatusCandidaturaResultDTO>();
            try
            {
                (string comentarioId, string msg) comentarioMsg = await _vagaService.MudarStatusCandidatura(
                                          mudarStatusCandidaturaRecrutamentoParam.IdCandidatura,
                                          mudarStatusCandidaturaRecrutamentoParam.CodigoStatus,
                                          _aspNetUser.GetUsuarioLogado().Cpf,
                                          mudarStatusCandidaturaRecrutamentoParam.Comentario);

                result.Sucesso = true;
                result.Mensagem = comentarioMsg.msg;
                result.Retorno = new MudarStatusCandidaturaResultDTO
                {
                    CandidaturaId = mudarStatusCandidaturaRecrutamentoParam.IdCandidatura,
                    ComentarioId = comentarioMsg.comentarioId
                };

                return Ok(result);

            }
            catch (UnauthorizedAccessException e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception e) when (e is ArgumentException || e is ApplicationException)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return BadRequest(result);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        [HttpGet("ListarStatusCandidaturaRecrutamento")]
        public async Task<ActionResult<ApiGenericResult<IEnumerable<StatusCandidaturaRecrutamentoDTO>>>> ListarStatusCandidaturaRecrutamento()
        {
            var result = new ApiGenericResult<IEnumerable<StatusCandidaturaRecrutamentoDTO>>();
            try
            {
                result.Retorno = await _vagaService.ListarStatusCandidaturaRecrutamento();
                return Ok(result);
            }
            catch (UnauthorizedAccessException e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception e) when (e is ArgumentException || e is ApplicationException)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return BadRequest(result);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        [HttpGet("ListarMotivosDescandidatura")]
        public async Task<ActionResult<ApiGenericResult<IEnumerable<MotivoDescandidatarDTO>>>> ListarMotivosDescandidatura()
        {
            var result = new ApiGenericResult<IEnumerable<MotivoDescandidatarDTO>>();
            try
            {
                result.Retorno = await _vagaService.ListarMotivosDescandidatura();
                return Ok(result);
            }
            catch (UnauthorizedAccessException e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception e) when (e is ArgumentException || e is ApplicationException)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return BadRequest(result);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        [HttpGet("ListarMotivosPerdaVaga")]
        public async Task<ActionResult<ApiGenericResult<IEnumerable<MotivoPerdaVagaDTO>>>> ListarMotivosPerdaVaga()
        {
            var result = new ApiGenericResult<IEnumerable<MotivoPerdaVagaDTO>>();
            try
            {
                result = await _vagaService.ListarMotivosPerdaVaga();
                return Ok(result);
            }
            catch (UnauthorizedAccessException e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception e) when (e is ArgumentException || e is ApplicationException)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return BadRequest(result);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        [HttpPost("DescandidatarSe")]
        public async Task<ActionResult<ApiGenericResult<IEnumerable<MotivoDescandidatarDTO>>>> DescandidatarSe(DescandidatarSeVagasRecrutamentoParam param)
        {
            var result = new ApiGenericResult<bool>();
            try
            {
                await _vagaService.DescandidatarSe(param.idCandidatura, param.idMotivoDescandidatura);
                result.Sucesso = true;
                return Ok(result);
            }
            catch (UnauthorizedAccessException e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception e) when (e is ArgumentException || e is ApplicationException)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return BadRequest(result);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        [HttpPost("CandidatarOutraPessoa")]
        public async Task<ActionResult<ApiGenericResult<IEnumerable<VagaRecrutamentoDTO>>>> CandidatarOutraPessoa(CandidatarOutraPessoaRecrutamentoParam candidatarOutraPessoaRecrutamentoParam)
        {
            var result = new ApiGenericResult<bool>();
            try
            {
                await _vagaService.CandidatarOutraPessoa(candidatarOutraPessoaRecrutamentoParam);
                result.Mensagem = "Sucesso";
                return Ok(result);
            }
            catch (UnauthorizedAccessException e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception e) when (e is ArgumentException || e is ApplicationException)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return BadRequest(result);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        [HttpPost("ListarCandidatosAderentes")]
        public async Task<ActionResult<ApiGenericResult<IEnumerable<ListarCandidatosAderentesResult>>>> ListarCandidatosAderentes([FromBody] ListarCandidatosAderentesParam param)
        {
            var result = new ApiGenericResult<IEnumerable<ListarCandidatosAderentesResult>>();

            try
            {
                result.Retorno = await _vagaService.ListarCandidatosAderentes(param);
                result.Sucesso = true;
                return Ok(result);
            }
            catch (UnauthorizedAccessException e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception e) when (e is ArgumentException || e is ApplicationException)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return BadRequest(result);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        [HttpPost("ListarCandidatosAderentesEInscritos")]
        public async Task<ActionResult<ApiGenericResult<ListarCandidatosAderentesEInscritosResult>>> ListarCandidatosAderentesEInscritos(
            [FromBody] ListarCandidatosAderentesEInscritosParam param)
        {
            var result = new ApiGenericResult<ListarCandidatosAderentesEInscritosResult>();

            try
            {
                var candidatosAderentesParam = new ListarCandidatosAderentesParam
                {
                    VagaId = param.VagaId,
                    Cursor = param.Cursor,
                    Limite = param.Limite,
                    Busca = param.Busca,
                    Origens = param.Origens,
                    PesoDisponibilidades = param.PesoDisponibilidades,
                    PesoDominiosNegocio = param.PesoDominiosNegocio,
                    PesoHardSkills = param.PesoHardSkills,
                    PesoIdiomas = param.PesoIdiomas,
                    PesoMetodologias = param.PesoMetodologias,
                    PesoSoftSkills = param.PesoSoftSkills,
                    LocalizacaoCidade = param.LocalizacaoCidade,
                    LocalizacaoEstado = param.LocalizacaoEstado
                };

                var candidatosAderentesTask = _vagaService.ListarCandidatosAderentes(candidatosAderentesParam);
                var candidatosInscritosTask = _vagaService.ListarCandidatosInscritos(param.VagaId, param.Busca, param.DataInicio, param.DataFim, param.Cursor, param.Limite, _aspNetUser.GetUsuarioLogado().Cpf, null, null, param.LocalizacaoCidade, param.LocalizacaoEstado);

                await Task.WhenAll(candidatosAderentesTask, candidatosInscritosTask);

                var candidatosAderentes = await candidatosAderentesTask;
                var candidatosInscritos = await candidatosInscritosTask;

                var totalCandidatosAderentes = candidatosAderentes?.Count() ?? 0;
                var totalCandidatosInscritos = await _vagaService.CountCandidatosInscritos(param.VagaId);

                var combinedResult = new ListarCandidatosAderentesEInscritosResult
                {
                    CandidatosAderentes = candidatosAderentes,
                    CandidatosInscritos = candidatosInscritos,
                    TotalCandidatosAderentes = totalCandidatosAderentes,
                    TotalCandidatosInscritos = totalCandidatosInscritos.TotalCandidatosInscritos
                };

                result.Retorno = combinedResult;
                result.Sucesso = true;
                return Ok(result);
            }
            catch (UnauthorizedAccessException e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception e) when (e is ArgumentException || e is ApplicationException)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return BadRequest(result);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        [HttpGet("ObterTotaisInscritos")]
        public async Task<ActionResult<ApiGenericResult<ObterTotaisInscritosResult>>> ObterTotaisInscritos(
            [FromQuery] string vagaId)
        {
            var result = new ApiGenericResult<ObterTotaisInscritosResult>();

            try
            {
                var totais = await _vagaService.ObterTotaisInscritos(vagaId);

                result.Retorno = totais;
                result.Sucesso = true;
                return Ok(result);
            }
            catch (UnauthorizedAccessException e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception e) when (e is ArgumentException || e is ApplicationException)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return BadRequest(result);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        [HttpGet("ListarDisponibilidadesEntrevista")]
        public async Task<ActionResult<ApiGenericResult<IEnumerable<DisponibilidadeEntrevistaDTO>>>> ListarDisponibilidadesEntrevista()
        {
            try
            {
                var retorno = new ApiGenericResult<IEnumerable<DisponibilidadeEntrevistaDTO>>();
                var lista = await _vagaService.ListarDisponibilidadesEntrevista();
                retorno.Retorno = lista.ToList();
                return retorno;
            }
            catch (UnauthorizedAccessException e)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, new ApiGenericResult<string> { Sucesso = false, Mensagem = "Acesso não autorizado!" });
            }
            catch (ArgumentException e)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new ApiGenericResult<string> { Sucesso = false, Mensagem = e.Message });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiGenericResult<string> { Sucesso = false, Mensagem = e.Message + ". " + e.InnerException?.Message });
            }
        }



        [HttpPost("RankCandidates")]
        public async Task<ActionResult<ApiGenericResult<List<DataTransferObject.Domain.Match.CandidatosMatchResponse>>>> RankCandidates([FromBody] DataTransferObject.Domain.Match.CandidatosMatchRequest request)
        {
            try
            {
                var retorno = new ApiGenericResult<List<DataTransferObject.Domain.Match.CandidatosMatchResponse>>();
                var result = await _vagaService.RankCandidates(request);
                retorno.Retorno = result;
                return retorno;
            }
            catch (UnauthorizedAccessException e)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, new ApiGenericResult<string> { Sucesso = false, Mensagem = "Acesso não autorizado!" });
            }
            catch (ArgumentException e)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new ApiGenericResult<string> { Sucesso = false, Mensagem = e.Message });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiGenericResult<string> { Sucesso = false, Mensagem = e.Message + ". " + e.InnerException?.Message });
            }
        }

        [HttpGet("ObterUltimaCandidaturaPorCodColaborador")]
        public async Task<ActionResult<ApiGenericResult<ListarCandidaturasPorCodCandidatoResult>>> ObterUltimaCandidaturaPorCodColaborador([FromQuery] string codColaborador)
        {
            var result = new ApiGenericResult<ListarCandidaturasPorCodCandidatoResult>();
            try
            {
                result.Retorno = await _vagaService.ObterUltimaCandidaturaPorCodColaborador(codColaborador);
                result.Sucesso = true;
                return Ok(result);
            }
            catch (UnauthorizedAccessException e)
            {
                result.Sucesso = false;
                result.Mensagem = "Acesso não autorizado!";
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (ArgumentException e)
            {
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(StatusCodes.Status400BadRequest, result);
            }
            catch (Exception e)
            {
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }

        [HttpPut("InserirInformacoesComplementaresVagaRecrutamento")]
        public async Task<ActionResult<ApiGenericResult<object>>> InserirInformacoesComplementaresVagaRecrutamento([FromBody] InserirInformacoesComplementaresVagaRecrutamentoParam param)
        {
            var result = new ApiGenericResult<object>();
            try
            {
                await _vagaService.InserirInformacoesComplementaresVagaRecrutamento(param, _aspNetUser.GetUsuarioLogado().Cpf);
                result.Sucesso = true;
                result.Mensagem = "Informações complementares inseridas com sucesso";
                return Ok(result);
            }
            catch (UnauthorizedAccessException e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception e) when (e is ArgumentException || e is ApplicationException)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return BadRequest(result);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        [HttpGet("ListarTiposVaga")]
        public async Task<ActionResult<ApiGenericResult<IEnumerable<TipoVagaDTO>>>> ListarTiposVaga()
        {
            var result = new ApiGenericResult<IEnumerable<TipoVagaDTO>>();
            try
            {
                result.Retorno = await _vagaService.ListarTiposVaga();
                result.Sucesso = true;
                return Ok(result);
            }
            catch (UnauthorizedAccessException e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception e) when (e is ArgumentException || e is ApplicationException)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return BadRequest(result);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        [HttpGet("ListarNiveisVaga")]
        public async Task<ActionResult<ApiGenericResult<IEnumerable<NivelVagaDTO>>>> ListarNiveisVaga()
        {
            var result = new ApiGenericResult<IEnumerable<NivelVagaDTO>>();
            try
            {
                result.Retorno = await _vagaService.ListarNiveisVaga();
                result.Sucesso = true;
                return Ok(result);
            }
            catch (UnauthorizedAccessException e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception e) when (e is ArgumentException || e is ApplicationException)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return BadRequest(result);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        [HttpGet("ListarTiposContratacao")]
        public async Task<ActionResult<ApiGenericResult<IEnumerable<TipoContratacaoDTO>>>> ListarTiposContratacao()
        {
            var result = new ApiGenericResult<IEnumerable<TipoContratacaoDTO>>();
            try
            {
                result.Retorno = await _vagaService.ListarTiposContratacao();
                result.Sucesso = true;
                return Ok(result);
            }
            catch (UnauthorizedAccessException e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception e) when (e is ArgumentException || e is ApplicationException)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return BadRequest(result);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        [HttpGet("ListarUnidades")]
        public async Task<ActionResult<ApiGenericResult<IEnumerable<UnidadeDTO>>>> ListarUnidades()
        {
            var result = new ApiGenericResult<IEnumerable<UnidadeDTO>>();
            try
            {
                result.Retorno = await _vagaService.ListarUnidades();
                result.Sucesso = true;
                return Ok(result);
            }
            catch (UnauthorizedAccessException e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception e) when (e is ArgumentException || e is ApplicationException)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return BadRequest(result);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        [HttpPost("ListarVagasRecrutamentoEPerfis")]
        public async Task<ActionResult<ApiGenericResult<ListarVagasRecrutamentoEPerfisResult>>> ListarVagasRecrutamentoEPerfis(
            [FromBody] ListarVagasRecrutamentoEPerfisParam param)
        {
            var result = new ApiGenericResult<ListarVagasRecrutamentoEPerfisResult>();
            try
            {
                var usuarioLogado = _aspNetUser.GetUsuarioLogado();
                
                var vagasResult = await _vagaService.ListarVagasRecrutamento(
                    param.Limite, param.Cursor, param.Busca, param.Status, usuarioLogado?.OrgId, param.DataInicio, param.DataFim);
                var contadorVagasPorStatus = await _vagaService.CountVagasRecrutamentoPorStatus(usuarioLogado?.OrgId, param.Status, param.DataInicio, param.DataFim);
                
                var perfisResult = await _service.ListarPerfis(param.DataInicio, param.DataFim, param.Cliente, _aspNetUser.GetUsuarioLogado().Cpf, param.Limite, param.Cursor, param.Busca, _aspNetUser.GetUsuarioLogado().OrgId);
                var perfisCount = await _service.CountPerfis(_aspNetUser.GetUsuarioLogado().OrgId);
                
                var totalVagasPorStatus = new Dictionary<string, int>();

                foreach (var contador in contadorVagasPorStatus.ContadoresPorStatus)
                {
                    totalVagasPorStatus[contador.CodigoStatus.ToString()] = contador.Quantidade;
                }

                var combinedResult = new ListarVagasRecrutamentoEPerfisResult
                {
                    VagasRecrutamento = vagasResult.Sucesso ? vagasResult.Retorno : new List<VagaRecrutamentoDTO>(),
                    TotalVagas = totalVagasPorStatus,
                    Perfis = perfisResult.Sucesso ? perfisResult.Retorno : new List<ListarPerfisResult>(),
                    TotalPerfis = perfisCount          
                };
                
                result.Sucesso = true;
                result.Retorno = combinedResult;
                return Ok(result);
            }
            catch (UnauthorizedAccessException e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception e) when (e is ArgumentException || e is ApplicationException)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return BadRequest(result);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        [HttpGet("ListarTiposEmpregosLinkedin")]
        public async Task<ActionResult<ApiGenericResult<IEnumerable<TipoEmpregoLinkedin>>>> ListarTiposEmpregosLinkedin()
        {
            var result = new ApiGenericResult<IEnumerable<TipoEmpregoLinkedin>>();
            try
            {
                result.Retorno = await _vagaService.ListarTiposEmpregosLinkedin();
                result.Sucesso = true;
                return Ok(result);
            }
            catch (UnauthorizedAccessException e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception e) when (e is ArgumentException || e is ApplicationException)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return BadRequest(result);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        [HttpGet("ListarNiveisExperienciaLinkedin")]
        public async Task<ActionResult<ApiGenericResult<IEnumerable<NivelExperienciaLinkedin>>>> ListarNiveisExperienciaLinkedin()
        {
            var result = new ApiGenericResult<IEnumerable<NivelExperienciaLinkedin>>();
            try
            {
                result.Retorno = await _vagaService.ListarNiveisExperienciaLinkedin();
                result.Sucesso = true;
                return Ok(result);
            }
            catch (UnauthorizedAccessException e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception e) when (e is ArgumentException || e is ApplicationException)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return BadRequest(result);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        [HttpGet("RelatorioVagas")]
        public async Task<IActionResult> RelatorioVagas(DateTime? dataInicio, DateTime? dataFim)
        {
            var fim = dataFim ?? DateTime.Now;
            var inicio = dataInicio ?? fim.AddDays(-30);

            var usuarioLogado = _aspNetUser.GetUsuarioLogado();
            var fileResult = await _vagaService.RelatorioVagas(usuarioLogado.OrgId, inicio, fim);

            if (!fileResult.Sucesso)
                return NoContent();

            return File(fileResult.Retorno.FileContents, fileResult.Retorno.ContentType, fileResult.Retorno.FileDownloadName);
        }

        [HttpGet("RelatorioProdutividade")]
        public async Task<IActionResult> RelatorioProdutividade(DateTime? dataInicio, DateTime? dataFim)
        {
            var fim = dataFim ?? DateTime.Now;
            var inicio = dataInicio ?? fim.AddDays(-30);

            var usuarioLogado = _aspNetUser.GetUsuarioLogado();
            var fileResult = await _vagaService.RelatorioProdutividade(usuarioLogado.OrgId, inicio, fim);

            if (!fileResult.Sucesso)
                return NoContent();

            return File(fileResult.Retorno.FileContents, fileResult.Retorno.ContentType, fileResult.Retorno.FileDownloadName);
        }

        [HttpGet("RelatorioVagasCandidaturas")]
        public async Task<IActionResult> RelatorioVagasCandidaturas(DateTime? dataInicio, DateTime? dataFim)
        {
            var fim = dataFim ?? DateTime.Now;
            var inicio = dataInicio ?? fim.AddDays(-30);

            var usuarioLogado = _aspNetUser.GetUsuarioLogado();
            var fileResult = await _vagaService.RelatorioVagasCandidaturas(inicio, fim, usuarioLogado.OrgId);

            if (!fileResult.Sucesso)
                return NoContent();

            return File(fileResult.Retorno.FileContents, fileResult.Retorno.ContentType, fileResult.Retorno.FileDownloadName);
        }

        [HttpGet("BuscarTemplateDescricaoVaga")]
        public async Task<ActionResult<ApiGenericResult<TemplateDescricaoVagaDTO>>> BuscarTemplateDescricaoVaga()
        {
            var result = new ApiGenericResult<TemplateDescricaoVagaDTO>();
            try
            {
                var usuarioLogado = _aspNetUser.GetUsuarioLogado();

                result.Retorno = await _vagaService.BuscarTemplateDescricaoVaga(usuarioLogado.OrgId);

                if (result.Retorno == null)
                {
                    result.Sucesso = false;
                    result.Mensagem = $"Não foi encontrado nenhuma descricao de template para a org id {usuarioLogado.OrgId}.";
                    return result;
                }

                result.Sucesso = true;
                return result;
            }
            catch (Exception e)
            {
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        [HttpPost("AdicionarTemplateDescricaoVaga")]
        public async Task<ActionResult<ApiGenericResult<string>>> AdicionarTemplateDescricaoVaga([FromBody] AdicionarTemplateDescricaoVagaRequest request)
        {
            var result = new ApiGenericResult<string>();

            try
            {
                var usuarioLogado = _aspNetUser.GetUsuarioLogado();

                await _vagaService.AdicionarTemplateDescricaoVaga(
                    usuarioLogado.OrgId,
                    request.TextoIntroducao,
                    request.TextoFinalizacao
                );

                result.Sucesso = true;
                result.Retorno = "Template de descrição da vaga adicionado com sucesso.";

                return Ok(result);
            }
            catch (Exception e)
            {
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        [HttpPut("AtualizarTemplateDescricaoVaga")]
        public async Task<ActionResult<ApiGenericResult<string>>> AtualizarTemplateDescricaoVaga([FromBody] AtualizarTemplateDescricaoVagaDTO dto)
        {
            var result = new ApiGenericResult<string>();

            try
            {
                var usuarioLogado = _aspNetUser.GetUsuarioLogado();

                await _vagaService.AtualizarTemplateDescricaoVaga(
                    usuarioLogado.OrgId,
                    dto.TextoIntroducao,
                    dto.TextoFinalizacao
                );

                result.Sucesso = true;
                result.Retorno = "Template atualizado com sucesso.";
                return Ok(result);
            }
            catch (Exception e)
            {
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        [HttpDelete("ExcluirTemplateDescricaoVaga")]
        public async Task<ActionResult<ApiGenericResult<string>>> ExcluirTemplateDescricaoVaga()
        {
            var result = new ApiGenericResult<string>();

            try
            {
                var usuarioLogado = _aspNetUser.GetUsuarioLogado();

                await _vagaService.ExcluirTemplateDescricaoVaga(usuarioLogado.OrgId);

                result.Sucesso = true;
                result.Retorno = "Template excluído com sucesso.";
                return Ok(result);
            }
            catch (Exception e)
            {
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return BadRequest(result);
            }
        }

        [HttpPost("AdicionarRecrutadorVaga")]
        public async Task<ActionResult<ApiGenericResult<ListarVagasRecrutamentoEPerfisResult>>> AdicionarRecrutadorVaga(string vagaId, string codInternoColaboradorRecrutador)
        {
            var result = new ApiGenericResult<string>();
            try
            {
                var retorno = await _vagaService.AdicionarRecrutadorVaga(vagaId, codInternoColaboradorRecrutador);

                if (!retorno.Sucesso)
                {
                    result.Sucesso = false;
                    result.Retorno = retorno.Retorno;
                    return Ok(result);
                }

                result.Sucesso = true;
                result.Retorno = retorno.Retorno;
                return Ok(result);  
            }
            catch (UnauthorizedAccessException e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception e) when (e is ArgumentException || e is ApplicationException)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return BadRequest(result);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        [HttpGet("ListarVagasRecrutamentoPorParent")]
        public async Task<ActionResult<ApiGenericResult<IEnumerable<VagaRecrutamentoDTO>>>> ListarVagasRecrutamentoPorParent(string vagaIdParent)
        {
            var result = new ApiGenericResult<IEnumerable<VagaRecrutamentoDTO>>();

            try
            {
                var usuarioLogado = _aspNetUser.GetUsuarioLogado();
                result.Retorno = await _vagaService.ListarVagasRecrutamentoPorParent(vagaIdParent, usuarioLogado.OrgId);
                result.Sucesso = true;
                return Ok(result);
            }
            catch (UnauthorizedAccessException e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (ArgumentException e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return BadRequest(result);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        [HttpGet("ListarVagasRecrutamentoPorParentEmAndamento")]
        public async Task<ActionResult<ApiGenericResult<IEnumerable<VagaRecrutamentoDTO>>>> ListarVagasRecrutamentoPorParentEmAndamento(string vagaIdParent)
        {
            var result = new ApiGenericResult<IEnumerable<VagaRecrutamentoDTO>>();

            try
            {
                var usuarioLogado = _aspNetUser.GetUsuarioLogado();
                result.Retorno = await _vagaService.ListarVagasRecrutamentoPorParentEmAndamento(vagaIdParent, usuarioLogado.OrgId);
                result.Sucesso = true;
                return Ok(result);
            }
            catch (UnauthorizedAccessException e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (ArgumentException e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return BadRequest(result);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }
    }
}