using ApiClient.Domain.Interfaces;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using DataTransferObject.Domain.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UploadFiles.API.DTOs;

using Logs.Infra.Attributes;

namespace UploadFiles.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [LogAction]
    public class ArchiveController : ControllerBase
    {
        private readonly IUploadFiles _uploadFiles;
        private readonly IUsuarioClient _usuarioClient;

        public ArchiveController(IUploadFiles uploadFiles, IUsuarioClient usuarioClient)
        {
            _uploadFiles = uploadFiles;
            _usuarioClient = usuarioClient;
        }

        [Authorize]
        [HttpPost("UploadFile")]
        public async Task<ActionResult<StatusResult>> UploadFileAutorizacao()
        {
            return await UploadFileBase();
        }

        private async Task<ActionResult<StatusResult>> UploadFileBase()
        {
            var ret = new StatusResult();
            try
            {
                byte[] arquivo = null;
                bool resultado = false;
                var filePath = Path.GetTempFileName();

                foreach (var formFile in Request.Form.Files)
                {
                    if (formFile.Length > 0)
                    {
                        using (var inputStream = new FileStream(filePath, FileMode.Create))
                        {
                            await formFile.CopyToAsync(inputStream);
                            arquivo = new byte[inputStream.Length];
                            inputStream.Seek(0, SeekOrigin.Begin);
                            inputStream.Read(arquivo, 0, arquivo.Length);

                            if (inputStream.Length > 0)
                            {
                                resultado = await _uploadFiles.UploadFile(inputStream, formFile.FileName);
                            }
                        }
                    }
                }

                if (resultado)
                {
                    ret.Sucesso = true;
                }
                else
                {
                    ret.Sucesso = false;
                }

                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [Authorize]
        [HttpPost("DeleteFile")]
        public async Task<ActionResult<StatusResult>> DeleteFile(ParamUploadFile param)
        {
            var ret = new StatusResult();
            try
            {
                var resultado = await _uploadFiles.DeleteFile(param.KeyName);

                if (resultado)
                {
                    ret.Sucesso = true;
                }
                else
                {
                    ret.Sucesso = false;
                }

                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [Authorize]
        [HttpPost("RenameFiles")]
        public async Task<ActionResult<StatusResult>> RenameFiles([FromBody] RenomearArquivosRequest request)
        {
            var ret = new StatusResult();
            try
            {
                if (request?.Renomes == null || request.Renomes.Count == 0)
                {
                    ret.Sucesso = false;
                    ret.Mensagem = "Informe ao menos um item em 'renomes' com nomeAtual e nomeNovo.";
                    return BadRequest(ret);
                }

                var renames = request.Renomes
                    .Where(r => !string.IsNullOrWhiteSpace(r.NomeAtual) && !string.IsNullOrWhiteSpace(r.NomeNovo))
                    .Select(r => (r.NomeAtual.Trim(), r.NomeNovo.Trim()))
                    .ToList();

                if (renames.Count == 0)
                {
                    ret.Sucesso = false;
                    ret.Mensagem = "Nenhum par nomeAtual/nomeNovo válido informado.";
                    return BadRequest(ret);
                }

                await _uploadFiles.RenameFilesAsync(renames);
                ret.Sucesso = true;
                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpGet("{token}/{*path}")]
        public async Task<ActionResult> GetFile(string token, string path)
        {
            return await ProcessarGetFile(token, path, false);
        }

        [HttpGet("tokenfile/{token}/{*path}")]
        public async Task<ActionResult> GetFileByToken(string token, string path)
        {
            return await ProcessarGetFile(token, path, true);
        }

        private async Task<ActionResult> ProcessarGetFile(string tokenOrPath, string path, bool isTokenFile = false)
        {
            var ret = new StatusResult();
            try
            {
                Stream resultado;
                if (isTokenFile)
                {
                    resultado = await _uploadFiles.GetFileByToken(tokenOrPath, path);
                }
                else
                {
                    byte[] resBytes = System.Convert.FromBase64String(Uri.EscapeUriString(tokenOrPath));
                    string decodedToken = System.Text.ASCIIEncoding.ASCII.GetString(resBytes);
                    if (decodedToken != VariaveisDeAmbienteUtil.GetVariavelDeAmbiente("TOKEN_SISTEMA_COLABORACAO"))
                    {
                        var tokenValidationResponse = await _usuarioClient.ShowMe(decodedToken);
                        if (tokenValidationResponse == null)
                        {
                            return StatusCode(401);
                        }
                    }
                    resultado = await _uploadFiles.GetFile(path);
                }

                if (resultado == null)
                    return StatusCode(404, "Not found");

                var contentType = ObterContentType(path);
                return File(resultado, contentType);
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode(401);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        private string ObterContentType(string path)
        {
            switch (path.Split('.')[1].ToLower())
            {
                case "png":
                    return "image/png";
                case "jpg":
                case "jpeg":
                    return "image/jpeg";
                case "pdf":
                    return "application/pdf";
                case "doc":
                    return "application/msword";
                case "docx":
                    return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                case "xls":
                    return "application/vnd.ms-excel";
                case "xlsx":
                    return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                case "xlsm":
                    return "application/vnd.ms-excel.sheet.macroEnabled.12";
                case "xlsb":
                    return "application/vnd.ms-excel.sheet.binary.macroEnabled.12";
                case "csv":
                    return "text/csv";
                case "rtf":
                    return "application/rtf";
                case "odt":
                    return "application/vnd.oasis.opendocument.text";
                case "mp3":
                    return "audio/mpeg";
                case "m4a":
                    return "audio/m4a";
                case "mp4":
                    return "video/mp4";
                case "ogg":
                    return "audio/ogg";
                default:
                    return "application/octet-stream";
            }
        }
    }
}
