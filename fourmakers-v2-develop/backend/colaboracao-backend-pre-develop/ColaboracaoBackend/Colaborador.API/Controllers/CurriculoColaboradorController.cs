using Colaboracao.Core;
using Colaboracao.Core.Exceptions;
using Colaboracao.Core.Interfaces;
using Colaborador.API.DTOs;
using Colaborador.Domain.Interfaces.Services;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Log;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;
using DataTransferObject.Domain.Colaborador;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using ClosedXML.Excel;
using System.Linq;

namespace Colaborador.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [LogAction]
    public class CurriculoColaboradorController : Controller
    {
        private readonly ILogCore _log;
        private readonly IAspNetUser _aspNetUser;
        private readonly ILinkedinService _linkedinService;
        private readonly IImportacaoColaboradorService _importacaoColaboradorService;

        public CurriculoColaboradorController(
            ILogCore log,
            IAspNetUser aspNetUser,
            ILinkedinService linkedinService,
            IImportacaoColaboradorService importacaoColaboradorService)
        {
            _log = log;
            _aspNetUser = aspNetUser;
            _linkedinService = linkedinService;
            _importacaoColaboradorService = importacaoColaboradorService;
        }

        [HttpPost("ColetarLinkedinColaborador")]
        public async Task<ActionResult<StatusResult>> ColetarLinkedinColaborador([FromBody] ProfileLinkedinParam param)
        {
            try
            {
                var ret = await _linkedinService.ColetarLinkedinColaborador(param.profileUrl, _aspNetUser.GetUsuarioLogado().Cpf);
                return StatusCode(200, ret);
            }
            catch (ValidationException)
            {
                return NotFound(new StatusResult
                {
                    Sucesso = false,
                    Mensagem = "Url incorreta ou não encontrada"
                });
            }
            catch (Exception ex)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(ex.StackTrace, LevelsEnum.Critical);
                return StatusCode(500, new StatusResult
                {
                    Sucesso = false,
                    Mensagem = "Erro interno no servidor"
                });
            }
        }
        
        [HttpPost("SincronizarCurriculoColaborador")]
        public async Task<ActionResult<StatusResult>> SincronizarCurriculoColaborador([FromForm] DocumentoCurriculoParam param)
        {
            try
            {
                byte[] arquivoBytes;
                using (var memoryStream = new MemoryStream())
                {
                    await param.File.CopyToAsync(memoryStream);
                    arquivoBytes = memoryStream.ToArray();
                }
                var ret = await _linkedinService.SincronizarPerfilLinkedinPorDocumento(arquivoBytes, _aspNetUser.GetUsuarioLogado().Cpf);
                return StatusCode(200, ret);
            }
            catch (ValidationException ex)
            {
                return NotFound(new StatusResult
                {
                    Sucesso = false,
                    Mensagem = ex.Message
                });
            }
            catch (Exception ex)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(ex.StackTrace, LevelsEnum.Critical);
                return StatusCode(500, new StatusResult
                {
                    Sucesso = false,
                    Mensagem = "Erro interno no servidor"
                });
            }
        }

        [HttpPost("SincronizarPerfilLinkedin")]
        public async Task<ActionResult<StatusResult>> SincronizarPerfilLinkedin(ProfileLinkedinParam param)
        {
            try
            {
                var ret = await _linkedinService.SincronizarPerfilLinkedin(param.profileUrl, _aspNetUser.GetUsuarioLogado().Cpf, param.useRapidAPI);
                return StatusCode(200, ret);
            }
            catch (ValidationException)
            {
                return NotFound(new StatusResult
                {
                    Sucesso = false,
                    Mensagem = "Url incorreta ou não encontrada"
                });
            }
            catch (Exception ex)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(ex.StackTrace, LevelsEnum.Critical);
                return StatusCode(500, new StatusResult
                {
                    Sucesso = false,
                    Mensagem = "Erro interno no servidor"
                });
            }
        }

        [HttpPost("SincronizarPerfilLinkedinServicoExterno")]
        public async Task<ActionResult<StatusResult>> SincronizarPerfilLinkedinServicoExterno(string codigoInternoColaborador, [FromBody] ProfileLinkedinParam param)
        {
            try
            {
                var ret = await _linkedinService.SincronizarPerfilLinkedinServicoExterno(_aspNetUser.GetUsuarioLogado().Cpf, param.profileUrl, codigoInternoColaborador, _aspNetUser.GetUsuarioLogado().OrgId);
                return StatusCode(200, ret);
            }
            catch (ValidationException)
            {
                return NotFound(new StatusResult
                {
                    Sucesso = false,
                    Mensagem = "Url incorreta ou não encontrada"
                });
            }
            catch (Exception ex)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(ex.StackTrace, LevelsEnum.Critical);
                return StatusCode(500, new StatusResult
                {
                    Sucesso = false,
                    Mensagem = "Erro interno no servidor"
                });
            }
        }

        [Authorize]
        [HttpPost("ImportarColaborador")]
        public async Task<ActionResult<StatusResult>> ImportarColaborador([FromForm] ImportarColaboradorParam param)
        {
            var result = new ApiGenericResult<bool>();
            try
            {
                byte[] arquivoBytes;
                using (var memoryStream = new MemoryStream())
                {
                    await param.File.CopyToAsync(memoryStream);
                    arquivoBytes = memoryStream.ToArray();
                }
                var ret = await _importacaoColaboradorService.ImportarColaborador(arquivoBytes, _aspNetUser.GetUsuarioLogado().OrgId, _aspNetUser.GetUsuarioLogado().Cpf);
                return StatusCode(200, new { codigoColaborador = ret });
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

        [Authorize]
        [HttpPost("ImportarColaboradorLinkedin")]
        public async Task<ActionResult<StatusResult>> ImportarColaboradorLinkedin([FromBody] ImportarColaboradorLinkedinParam param)
        {
            var result = new ApiGenericResult<bool>();
            try
            {
                var ret = await _importacaoColaboradorService.ImportarColaboradorLinkedin(param.PerfilIN, _aspNetUser.GetUsuarioLogado().OrgId, _aspNetUser.GetUsuarioLogado().Cpf);
                return StatusCode(200, new { codigoColaborador = ret });
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

        [Authorize]
        [HttpPost("SincronizarColaboradorBancoTalentos")]
        public async Task<ActionResult<ApiGenericResult<ColaboradorDTO>>> SincronizarColaboradorBancoTalentos([FromBody] ImportarColaboradorLinkedinParam param)
        {
            var result = new ApiGenericResult<ColaboradorDTO>();
            try
            {
                var ret = await _importacaoColaboradorService.SincronizarColaboradorBancoTalentos(param.PerfilIN, _aspNetUser.GetUsuarioLogado().OrgId, _aspNetUser.GetUsuarioLogado().Token);
                return StatusCode(200, ret);
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

        [Authorize]
        [HttpPost("ImportarColaboradorLote")]
        public async Task<ActionResult<StatusResult>> ImportarColaboradorLote([FromForm] ArquivoZipParam param)
        {
            var result = new ApiGenericResult<ProcessamentoZipResult>();
            try
            {
                if (param.File == null || param.File.Length == 0)
                {
                    result.Sucesso = false;
                    result.Mensagem = "Nenhum arquivo foi enviado.";
                    return BadRequest(result);
                }

                // Verificar se é um arquivo ZIP
                if (!param.File.ContentType.Equals("application/zip", StringComparison.OrdinalIgnoreCase) &&
                    !param.File.FileName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                {
                    result.Sucesso = false;
                    result.Mensagem = "O arquivo enviado não é um arquivo ZIP válido.";
                    return BadRequest(result);
                }

                result = await _importacaoColaboradorService.ImportarColaboradorLote(param.File, _aspNetUser.GetUsuarioLogado().OrgId, _aspNetUser.GetUsuarioLogado().Cpf);
                return StatusCode(200, result);
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

        [Authorize]
        [HttpPost("ImportarColaboradorLoteLinkedin")]
        public async Task<ActionResult<StatusResult>> ImportarColaboradorLoteLinkedin([FromForm] ArquivoExcelParam param)
        {
            var result = new StatusResult();
            try
            {
                if (param.File == null || param.File.Length == 0)
                {
                    result.Sucesso = false;
                    result.Mensagem = "Nenhum arquivo foi enviado.";
                    return BadRequest(result);
                }

                // Verificar se é um arquivo Excel
                var allowedExtensions = new[] { ".xlsx", ".xls" };
                var fileExtension = Path.GetExtension(param.File.FileName).ToLowerInvariant();
                
                if (!allowedExtensions.Contains(fileExtension))
                {
                    result.Sucesso = false;
                    result.Mensagem = "O arquivo enviado não é um arquivo Excel válido (.xlsx ou .xls).";
                    return BadRequest(result);
                }

                // Ler o arquivo Excel
                var strings = new List<string>();
                using (var stream = param.File.OpenReadStream())
                using (var workbook = new XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheet(1); // Primeira planilha
                    var usedRange = worksheet.RangeUsed();
                    
                    if (usedRange != null)
                    {
                        // Ler todas as células com conteúdo
                        foreach (var row in usedRange.Rows())
                        {
                            foreach (var cell in row.Cells())
                            {
                                var cellValue = cell.Value.ToString();
                                if (!string.IsNullOrWhiteSpace(cellValue))
                                {
                                    // Extrair apenas o final da URL do LinkedIn
                                    var linkedinId = ExtrairLinkedinId(cellValue);
                                    if (!string.IsNullOrWhiteSpace(linkedinId))
                                    {
                                        strings.Add(linkedinId);
                                    }
                                }
                            }
                        }
                    }
                }

                if (strings.Count == 0)
                {
                    result.Sucesso = false;
                    result.Mensagem = "Nenhuma string encontrada na planilha.";
                    return BadRequest(result);
                }

                result = await _importacaoColaboradorService.ImportarColaboradorLoteLinkedin(strings, _aspNetUser.GetUsuarioLogado().OrgId, _aspNetUser.GetUsuarioLogado().Cpf);

                return StatusCode(200, result);
            }
            catch (Exception ex)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(ex.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = "Erro interno no servidor";
                return StatusCode(500, result);
            }
        }

        private string ExtrairLinkedinId(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return string.Empty;

            try
            {
                // Remover espaços em branco
                url = url.Trim();

                // Verificar se é uma URL do LinkedIn
                if (url.Contains("linkedin.com/in/"))
                {
                    // Extrair a parte após "linkedin.com/in/"
                    var startIndex = url.IndexOf("linkedin.com/in/") + "linkedin.com/in/".Length;
                    if (startIndex < url.Length)
                    {
                        var linkedinId = url.Substring(startIndex);
                        
                        // Remover parâmetros de query string se houver
                        var queryIndex = linkedinId.IndexOf('?');
                        if (queryIndex > 0)
                        {
                            linkedinId = linkedinId.Substring(0, queryIndex);
                        }
                        
                        // Remover barras finais se houver
                        linkedinId = linkedinId.TrimEnd('/');
                        
                        return linkedinId;
                    }
                }
                
                // Se não for uma URL completa, retornar o valor como está (pode ser apenas o ID)
                return url;
            }
            catch (Exception ex)
            {
                _log.Log($"Erro ao extrair LinkedIn ID de '{url}': {ex.Message}", LevelsEnum.Error);
                return string.Empty;
            }
        }
    }
}