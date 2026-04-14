using Financeiro.Domain.Interfaces.Conciliacao;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Financeiro.Conciliacao;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Usuario;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Logs.Infra.Attributes;

namespace Financeiro.API.Controllers.Conciliacao
{
    [Authorize]
    [Route("api/Financeiro/[controller]")]
    [ApiController]
    [LogAction]
    public class ConciliacaoController : ControllerBase
    {
        private readonly IConciliacaoService _conciliacaoService;
        private readonly UsuarioLogadoDTO _usuarioLogado;

        public ConciliacaoController(
            IConciliacaoService conciliacaoService,
            IAspNetUser aspNetUser)
        {
            _conciliacaoService = conciliacaoService;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
        }

        /// <summary>
        /// Cria um novo lote de conciliação de folha ponto
        /// </summary>
        /// <param name="request">Dados para criação do lote</param>
        /// <returns>Resultado da criação do lote</returns>
        [HttpPost("CriarLote")]
        public async Task<ActionResult<ApiGenericResult<SumarioConciliacaoResult>>> CriarLote([FromBody] CriarLoteConciliacaoDTO request)
        {
            var result = new ApiGenericResult<SumarioConciliacaoResult>();

            try
            {

                // Validar campos obrigatórios
                if (string.IsNullOrWhiteSpace(request.Cnpj))
                {
                    result.Sucesso = false;
                    result.Mensagem = "CNPJ é obrigatório.";
                    return BadRequest(result);
                }

                if (string.IsNullOrWhiteSpace(request.Competencia))
                {
                    result.Sucesso = false;
                    result.Mensagem = "Competência é obrigatória.";
                    return BadRequest(result);
                }

                // Criar o lote de conciliação
                var loteResult = await _conciliacaoService.CriarLoteConciliacaoAsync(
                    request.Cnpj,
                    request.Competencia,
                    _usuarioLogado.OrgId,
                    _usuarioLogado.Cpf
                );

                result.Sucesso = true;
                result.Mensagem = "Lote de conciliação criado com sucesso.";
                result.Retorno = loteResult;

                return Ok(result);
            }
            catch (ArgumentException e)
            {
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return BadRequest(result);
            }
            catch (InvalidOperationException e)
            {
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return BadRequest(result);
            }
            catch (Exception e)
            {
                result.Sucesso = false;
                result.Mensagem = "Erro interno do servidor ao criar lote de conciliação.";
                return StatusCode(500, result);
            }
        }

        /// <summary>
        /// Busca os lotes de conciliação de folha ponto
        /// </summary>
        /// <returns>Lista de lotes de conciliação de folha ponto</returns>
        [HttpGet("BuscarLotes")]
        public async Task<ActionResult<ApiGenericResult<List<SumarioConciliacaoResult>>>> BuscarLotes()
        {
            var result = new ApiGenericResult<List<SumarioConciliacaoResult>>();

            try
            {
                var lotes = await _conciliacaoService.BuscarLotesPorOrgAsync(_usuarioLogado.Cpf, _usuarioLogado.OrgId);

                result.Sucesso = true;
                result.Mensagem = "Lotes de conciliação encontrados com sucesso.";
                result.Retorno = lotes;

                return Ok(result);
            }
            catch (Exception e)
            {
                result.Sucesso = false;
                result.Mensagem = "Erro interno do servidor ao buscar lotes de conciliação.";
                return StatusCode(500, result);
            }
        }

        /// <summary>
        /// Busca os itens de conciliação de folha ponto
        /// </summary>
        /// <param name="loteId">ID do lote</param>
        /// <returns>Lista de itens de conciliação de folha ponto</returns>
        [HttpGet("DetalharLote")]
        public async Task<ActionResult<ApiGenericResult<ConciliacaoLoteResult>>> DetalharLote(string loteId)
        {
            var result = new ApiGenericResult<ConciliacaoLoteResult>();

            try
            {
                var itensConciliacao = await _conciliacaoService.BuscarItensConciliacaoColaboradorAsync(loteId, _usuarioLogado.OrgId);

                result.Sucesso = true;
                result.Mensagem = "Itens de conciliação de folha ponto encontrados com sucesso.";
                result.Retorno = itensConciliacao;

                return Ok(result);
            }
            catch (Exception e)
            {
                result.Sucesso = false;
                result.Mensagem = "Erro interno do servidor ao buscar itens de conciliação de folha ponto.";
                return StatusCode(500, result);
            }
        }

        /// <summary>
        /// Exporta as divergências de um lote para um arquivo Excel
        /// </summary>
        /// <param name="loteId">ID do lote</param>
        /// <returns>Arquivo Excel com as divergências</returns>
        [HttpGet("ExportarDivergencias")]
        public async Task<IActionResult> ExportarDivergencias(string loteId)
        {
            try
            {
                // Validar parâmetro obrigatório
                if (string.IsNullOrWhiteSpace(loteId))
                {
                    return BadRequest("ID do lote é obrigatório.");
                }

                var fileResult = await _conciliacaoService.ExportarDivergenciasLoteAsync(loteId, _usuarioLogado.OrgId);

                if (!fileResult.Sucesso)
                {
                    return BadRequest(fileResult.Mensagem);
                }

                return File(fileResult.Retorno.FileContents, fileResult.Retorno.ContentType, fileResult.Retorno.FileDownloadName);
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(500, "Erro interno do servidor ao exportar divergências.");
            }
        }

        /// <summary>
        /// Lista o status de vigência da conciliação para a organização
        /// </summary>
        /// <returns>Lista de CNPJs e vigências com status de processamento</returns>
        [HttpGet("StatusVigencia")]
        public async Task<ActionResult<ApiGenericResult<List<StatusVigenciaConciliacaoDTO>>>> ListarStatusVigencia()
        {
            var result = new ApiGenericResult<List<StatusVigenciaConciliacaoDTO>>();

            try
            {
                var statusVigencia = await _conciliacaoService.ListarStatusVigenciaConciliacaoAsync(_usuarioLogado.OrgId);

                result.Sucesso = true;
                result.Mensagem = "Status de vigência da conciliação encontrado com sucesso.";
                result.Retorno = statusVigencia;

                return Ok(result);
            }
            catch (Exception e)
            {
                result.Sucesso = false;
                result.Mensagem = "Erro interno do servidor ao buscar status de vigência da conciliação.";
                return StatusCode(500, result);
            }
        }

        /// <summary>
        /// Aprova um lote de conciliação de folha ponto
        /// </summary>
        /// <param name="request">Dados para aprovação do lote</param>
        /// <returns>Resultado da aprovação do lote</returns>
        [HttpPost("AprovarLote")]
        public async Task<ActionResult<ApiGenericResult<bool>>> AprovarLote([FromBody] AprovarLoteConciliacaoDTO request)
        {
            var result = new ApiGenericResult<bool>();

            try
            {
                // Validar campos obrigatórios
                if (string.IsNullOrWhiteSpace(request.LoteId))
                {
                    result.Sucesso = false;
                    result.Mensagem = "ID do lote é obrigatório.";
                    return BadRequest(result);
                }

                // Aprovar o lote de conciliação
                var aprovado = await _conciliacaoService.AprovarConciliacaoAsync(_usuarioLogado.OrgId, request.LoteId);

                result.Sucesso = true;
                result.Mensagem = "Lote de conciliação aprovado com sucesso.";
                result.Retorno = aprovado;

                return Ok(result);
            }
            catch (Exception e)
            {
                result.Sucesso = false;
                result.Mensagem = "Erro interno do servidor ao aprovar lote de conciliação.";
                return StatusCode(500, result);
            }
        }
    }
} 