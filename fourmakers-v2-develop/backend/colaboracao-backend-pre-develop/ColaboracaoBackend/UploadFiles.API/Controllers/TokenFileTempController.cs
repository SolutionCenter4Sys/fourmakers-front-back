using Colaboracao.Core;
using DataTransferObject.Domain.Arquivo.TokenFileTemp;
using DataTransferObject.Domain.Base;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using UploadFiles.Domain.Interfaces.Services;

namespace UploadFiles.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/Archive/[controller]")]
    [HandleException]
    [LogAction]
    public class TokenFileTempController : ControllerBase
    {
        private readonly ITokenFileTempService _tokenFileTempService;
        public const long MaxBytes = 100L * 1024 * 1024;

        public TokenFileTempController(ITokenFileTempService tokenFileTempService)
        {
            _tokenFileTempService = tokenFileTempService;
        }

        [HttpGet("Listar")]
        public async Task<ActionResult<ApiGenericResult<List<TokenFileTempDTO>>>> Listar()
        {
            var result = await _tokenFileTempService.ListarAsync();
            if (!result.Sucesso)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("Obter/{token}")]
        public async Task<ActionResult<ApiGenericResult<TokenFileTempDTO>>> Obter(string token)
        {
            var result = await _tokenFileTempService.ObterAsync(token);
            if (!result.Sucesso)
                return BadRequest(result);
            return Ok(result);
        }

        /// <summary>
        /// Envia um arquivo (PDF, PNG ou JPG) para o S3 e registra em tb_token_files_temp.
        /// Form: <c>arquivo</c> (obrigatório), <c>base</c> (opcional — um único segmento; vazio usa <c>temp</c>).
        /// Chave S3: <c>arquivo/{base}/{guid}_{yyyyMMddHHmmssfff}{ext}</c>. Máx. 100 MB.
        /// </summary>
        [RequestSizeLimit(MaxBytes)]
        [RequestFormLimits(MultipartBodyLengthLimit = MaxBytes)]
        [HttpPost("Inserir")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ApiGenericResult<TokenFileTempDTO>>> Inserir(
            IFormFile arquivo,
            [FromForm(Name = "base")] string arquivoBase = null)
        {
            if (arquivo == null || arquivo.Length == 0)
                return BadRequest(new ApiGenericResult<TokenFileTempDTO> { Sucesso = false, Mensagem = "Nenhum arquivo enviado." });

            using var stream = arquivo.OpenReadStream();
            var result = await _tokenFileTempService.InserirComUploadAsync(
                stream,
                arquivo.FileName,
                arquivo.Length,
                arquivoBase);

            if (!result.Sucesso)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPut("Atualizar/{token}")]
        public async Task<ActionResult<ApiGenericResult>> Atualizar(string token, [FromBody] TokenFileTempAtualizarDTO dto)
        {
            var result = await _tokenFileTempService.AtualizarAsync(token, dto);
            if (!result.Sucesso)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("Excluir/{token}")]
        public async Task<ActionResult<ApiGenericResult>> Excluir(string token)
        {
            var result = await _tokenFileTempService.ExcluirAsync(token);
            if (!result.Sucesso)
                return BadRequest(result);
            return Ok(result);
        }
    }
}
