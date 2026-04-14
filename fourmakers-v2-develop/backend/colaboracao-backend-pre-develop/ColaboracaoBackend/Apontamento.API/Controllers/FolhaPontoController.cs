using Apontamento.Domain.Interfaces.Service;
using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using DataTransferObject.Domain.Apontamento.FolhaPonto;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Usuario;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Apontamento.API.Controllers
{
    [Authorize]
    [Route("api/Apontamento/[controller]")]
    [ApiController]
    [LogAction]
    public class FolhaPontoController : ControllerBase
    {
        private readonly IFolhaPontoService _folhaPontoService;
        private readonly UsuarioLogadoDTO _usuarioLogado;
        private readonly IStringLocalizer<FourmakersCoreMessage> _stringLocalizerFourmakersCore;
        private readonly IStringLocalizer<ApontamentoMessage> _stringLocalizerApontamento;

        public FolhaPontoController(
            IFolhaPontoService folhaPontoService,
            IAspNetUser aspNetUser,
            IStringLocalizer<FourmakersCoreMessage> stringLocalizerFourmakersCore,
            IStringLocalizer<ApontamentoMessage> stringLocalizerApontamento)
        {
            _folhaPontoService = folhaPontoService;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
            _stringLocalizerFourmakersCore = stringLocalizerFourmakersCore;
            _stringLocalizerApontamento = stringLocalizerApontamento;
        }

        [HttpPost("ProcessarFolhaPonto")]
        public async Task<ActionResult<ApiGenericResult>> ProcessarFolhaPonto(string loteId)
        {
            var result = new ApiGenericResult();

            try
            {
                result.Sucesso = await _folhaPontoService.ProcessarFolhaPontoAsync(
                    loteId,
                    _usuarioLogado.Cpf,
                    _usuarioLogado.OrgId
                );
                return Ok(result);
            }
            catch (Exception e)
            {
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        [HttpPost("SumarioFolhaPonto")]
        public async Task<ActionResult<ApiGenericResult<SumarioFolhaPontoResult>>> SumarioFolhaPonto(IFormFile arquivoPdf)
        {
            var result = new ApiGenericResult<SumarioFolhaPontoResult>();

            try
            {
                // Validar se o arquivo foi enviado
                if (arquivoPdf == null || arquivoPdf.Length == 0)
                {
                    result.Sucesso = false;
                    result.Mensagem = "Arquivo PDF não enviado.";
                    return BadRequest(result);
                }

                // Validar se é um arquivo PDF
                if (!arquivoPdf.ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase))
                {
                    result.Sucesso = false;
                    result.Mensagem = "O arquivo deve ser um PDF.";
                    return BadRequest(result);
                }

                // Converter arquivo para byte array
                byte[] arquivoBytes;
                using (var memoryStream = new MemoryStream())
                {
                    await arquivoPdf.CopyToAsync(memoryStream);
                    arquivoBytes = memoryStream.ToArray();
                }

                // Processar sumário da folha ponto
                var sumario = await _folhaPontoService.ObterSumarioFolhaPontoAsync(arquivoBytes, _usuarioLogado.Cpf, _usuarioLogado.OrgId);

                if (sumario == null)
                {
                    result.Sucesso = false;
                    result.Mensagem = "Erro ao processar o sumário da folha ponto.";
                    return StatusCode(500, result);
                }

                result.Sucesso = true;
                result.Mensagem = _stringLocalizerFourmakersCore.GetStringOuVazio("QUERY_COMPLETED_SUCCESSFULLY");
                result.Retorno = sumario;

                return Ok(result);
            }
            catch (Exception ex)
            {
                result.Sucesso = false;
                result.Mensagem = $"Erro ao processar sumário da folha ponto: {ex.Message}";
                return StatusCode(500, result);
            }
        }

        [HttpGet("ListarLotes")]
        public async Task<ActionResult<ApiGenericResult<List<LoteFilaResult>>>> ListarLotes()
        {
            var result = new ApiGenericResult<List<LoteFilaResult>>();

            try
            {

                var lotes = await _folhaPontoService.BuscarLotesPorOrgAsync(_usuarioLogado.OrgId);

                result.Sucesso = true;
                result.Mensagem = _stringLocalizerFourmakersCore.GetStringOuVazio("QUERY_COMPLETED_SUCCESSFULLY");
                result.Retorno = lotes;

                return Ok(result);
            }
            catch (Exception ex)
            {
                result.Sucesso = false;
                result.Mensagem = $"Erro ao listar lotes: {ex.Message}";
                return StatusCode(500, result);
            }
        }

        [HttpGet("DetalharLote")]
        public async Task<ActionResult<ApiGenericResult<DetalhesLoteFolhaPontoResult>>> DetalharLote(string loteId)
        {
            var result = new ApiGenericResult<DetalhesLoteFolhaPontoResult>();

            try
            {
                var lote = await _folhaPontoService.DetalharLoteAsync(loteId);

                result.Sucesso = true;
                result.Mensagem = _stringLocalizerFourmakersCore.GetStringOuVazio("QUERY_COMPLETED_SUCCESSFULLY");
                result.Retorno = lote;

                return Ok(result);
            }
            catch (Exception ex)
            {
                result.Sucesso = false;
                result.Mensagem = $"Erro ao detalhar lote: {ex.Message}";
                return StatusCode(500, result);
            }
        }

        [HttpPost("DeletarLote")]
        public async Task<ActionResult<ApiGenericResult>> DeletarLote(string loteId)
        {
            var result = new ApiGenericResult();

            try
            {
                await _folhaPontoService.DeletarLoteAsync(loteId);

                result.Sucesso = true;
                result.Mensagem = _stringLocalizerFourmakersCore.GetStringOuVazio("QUERY_COMPLETED_SUCCESSFULLY");

                return Ok(result);
            }
            catch (Exception ex)
            {
                result.Sucesso = false;
                result.Mensagem = $"Erro ao deletar lote: {ex.Message}";
                return StatusCode(500, result);
            }
        }

        [HttpPost("ReprocessarItens")]
        public async Task<ActionResult<ApiGenericResult<ReprocessarItensFolhaPontoResult>>> ReprocessarItens([FromBody] ReprocessarItensFolhaPontoInput request)
        {
            var result = new ApiGenericResult<ReprocessarItensFolhaPontoResult>();

            try
            {
                if (request?.ItensLoteId == null || request.ItensLoteId.Count == 0)
                {
                    result.Sucesso = false;
                    result.Mensagem = "É necessário informar ao menos um item de lote para reprocessamento.";
                    return BadRequest(result);
                }

                result.Retorno = await _folhaPontoService.ReprocessarItensFolhaPontoAsync(
                    request.ItensLoteId,
                    _usuarioLogado.Cpf,
                    _usuarioLogado.OrgId
                );
                result.Sucesso = true;
                result.Mensagem = "Reprocessamento iniciado com sucesso.";

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                result.Sucesso = false;
                result.Mensagem = ex.Message;
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                result.Sucesso = false;
                result.Mensagem = $"Erro ao iniciar reprocessamento: {ex.Message}";
                return StatusCode(500, result);
            }
        }
    }
} 