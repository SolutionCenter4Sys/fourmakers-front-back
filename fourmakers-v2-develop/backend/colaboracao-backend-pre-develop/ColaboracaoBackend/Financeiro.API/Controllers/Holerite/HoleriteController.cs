using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using DataTransferObject.Domain.Financeiro.Holerite;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Usuario;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using LoteFilaResult = DataTransferObject.Domain.Financeiro.Holerite.LoteFilaResult;
using Financeiro.Domain.Interfaces.Holerite;

using Logs.Infra.Attributes;

namespace Financeiro.API.Controllers.Holerite
{
    [Authorize]
    [Route("api/Financeiro/[controller]")]
    [ApiController]
    [LogAction]
    public class HoleriteController : ControllerBase
    {
        private readonly IHoleriteService _holeriteService;
        private readonly UsuarioLogadoDTO _usuarioLogado;
        private readonly IStringLocalizer<FourmakersCoreMessage> _stringLocalizerFourmakersCore;
        private readonly IStringLocalizer<ApontamentoMessage> _stringLocalizerApontamento;

        public HoleriteController(
            IHoleriteService holeriteService,
            IAspNetUser aspNetUser,
            IStringLocalizer<FourmakersCoreMessage> stringLocalizerFourmakersCore,
            IStringLocalizer<ApontamentoMessage> stringLocalizerApontamento)
        {
            _holeriteService = holeriteService;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
            _stringLocalizerFourmakersCore = stringLocalizerFourmakersCore;
            _stringLocalizerApontamento = stringLocalizerApontamento;
        }

        [HttpPost("ProcessarHolerite")]
        public async Task<ActionResult<ApiGenericResult>> ProcessarHolerite(string loteId, TipoProcessamentoHoleriteEnum tipoProcessamento = TipoProcessamentoHoleriteEnum.Mensal)
        {
            var result = new ApiGenericResult();

            try
            {
                result.Sucesso = await _holeriteService.ProcessarHoleriteAsync(
                    loteId,
                    _usuarioLogado.Cpf,
                    _usuarioLogado.OrgId,
                    tipoProcessamento
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

        [HttpPost("SumarioHolerite")]
        public async Task<ActionResult<ApiGenericResult<SumarioHoleriteResult>>> SumarioHolerite(IFormFile arquivoPdf)
        {
            var result = new ApiGenericResult<SumarioHoleriteResult>();

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

                // Processar sumário do holerite
                var sumario = await _holeriteService.ObterSumarioHoleriteAsync(arquivoBytes, _usuarioLogado.Cpf, _usuarioLogado.OrgId);

                if (sumario == null)
                {
                    result.Sucesso = false;
                    result.Mensagem = "Erro ao processar o sumário do holerite.";
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
                result.Mensagem = $"Erro ao processar sumário do holerite: {ex.Message}";
                return StatusCode(500, result);
            }
        }

        [HttpGet("ListarLotes")]
        public async Task<ActionResult<ApiGenericResult<List<LoteFilaResult>>>> ListarLotes()
        {
            var result = new ApiGenericResult<List<LoteFilaResult>>();

            try
            {
                var lotes = await _holeriteService.BuscarLotesPorOrgAsync(_usuarioLogado.OrgId);

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
        public async Task<ActionResult<ApiGenericResult<DetalhesLoteHoleriteResult>>> DetalharLote(string loteId)
        {
            var result = new ApiGenericResult<DetalhesLoteHoleriteResult>();

            try
            {
                var lote = await _holeriteService.DetalharLoteAsync(loteId);

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
                await _holeriteService.DeletarLoteAsync(loteId);

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

        [HttpPost("DeletarLoteAdiantamento")]
        public async Task<ActionResult<ApiGenericResult>> DeletarLoteAdiantamento(string loteId)
        {
            var result = new ApiGenericResult();

            try
            {
                await _holeriteService.DeletarLoteAdiantamentoAsync(loteId, _usuarioLogado.OrgId);

                result.Sucesso = true;
                result.Mensagem = "Lote de adiantamento deletado com sucesso.";

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
                result.Mensagem = $"Erro ao deletar lote de adiantamento: {ex.Message}";
                return StatusCode(500, result);
            }
        }
        
        [HttpPost("DeletarLoteFerias")]
        public async Task<ActionResult<ApiGenericResult>> DeletarLoteFerias(string loteId)
        {
            var result = new ApiGenericResult();

            try
            {
                await _holeriteService.DeletarLoteFeriasAsync(loteId, _usuarioLogado.OrgId);

                result.Sucesso = true;
                result.Mensagem = "Lote de férias deletado com sucesso.";

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
                result.Mensagem = $"Erro ao deletar lote de férias: {ex.Message}";
                return StatusCode(500, result);
            }
        }
        
        [HttpPost("DeletarLoteDecimoTerceiro")]
        public async Task<ActionResult<ApiGenericResult>> DeletarLoteDecimoTerceiro(string loteId)
        {
            var result = new ApiGenericResult();

            try
            {
                await _holeriteService.DeletarLoteDecimoTerceiroAsync(loteId, _usuarioLogado.OrgId);

                result.Sucesso = true;
                result.Mensagem = "Lote de décimo terceiro deletado com sucesso.";

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
                result.Mensagem = $"Erro ao deletar lote de décimo terceiro: {ex.Message}";
                return StatusCode(500, result);
            }
        }
        
        [HttpPost("DeletarLoteAdiantamentoDecimoTerceiro")]
        public async Task<ActionResult<ApiGenericResult>> DeletarLoteAdiantamentoDecimoTerceiro(string loteId)
        {
            var result = new ApiGenericResult();

            try
            {
                await _holeriteService.DeletarLoteAdiantamentoDecimoTerceiroAsync(loteId, _usuarioLogado.OrgId);

                result.Sucesso = true;
                result.Mensagem = "Lote de adiantamento de décimo terceiro deletado com sucesso.";

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
                result.Mensagem = $"Erro ao deletar lote de adiantamento do décimo terceiro: {ex.Message}";
                return StatusCode(500, result);
            }
        }
        
        [HttpPost("DeletarLoteInformeDeRendimentos")]
        public async Task<ActionResult<ApiGenericResult>> DeletarLoteInformeDeRendimentos(string loteId)
        {
            var result = new ApiGenericResult();

            try
            {
                await _holeriteService.DeletarLoteInformeDeRendimentoAsync(loteId, _usuarioLogado.OrgId);

                result.Sucesso = true;
                result.Mensagem = "Lote de adiantamento de décimo terceiro deletado com sucesso.";

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
                result.Mensagem = $"Erro ao deletar lote de adiantamento do décimo terceiro: {ex.Message}";
                return StatusCode(500, result);
            }
        }

        [HttpPost("ReprocessarItens")]
        public async Task<ActionResult<ApiGenericResult<ReprocessarItensHoleriteResult>>> ReprocessarItens([FromBody] ReprocessarItensHoleriteInput request)
        {
            var result = new ApiGenericResult<ReprocessarItensHoleriteResult>();

            try
            {
                if (request?.ItensLoteId == null || request.ItensLoteId.Count == 0)
                {
                    result.Sucesso = false;
                    result.Mensagem = "É necessário informar ao menos um item de lote para reprocessamento.";
                    return BadRequest(result);
                }

                result.Retorno = await _holeriteService.ReprocessarItensHoleriteAsync(
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