using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados;
using DataTransferObject.Domain.Usuario;
using Logs.Infra.Attributes;
using MapaDeAlocacao.Domain.Interfaces.GestaoDeAlocados;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MapaDeAlocacao.API.Controllers.GestaoDeAlocados
{
    [Route("api/GestaoDeAlocados/[controller]")]
    [ApiController]
    [HandleException]
    [Authorize]
    [LogAction]
    public class ParceirosController : ControllerBase
    {
        private readonly UsuarioLogadoDTO _usuarioLogado;
        private readonly IParceirosService _parceirosService;

        public ParceirosController(IAspNetUser aspNetUser, IParceirosService parceirosService)
        {
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
            _parceirosService = parceirosService;
        }
        
        [HttpPost("InserirArquivo")]
        public async Task<ActionResult<ParceiroArchiveDTO>> InserirArquivo()
        {
            var ret = new ParceiroArchiveDTO();
            
            try
            {
                StringValues parceirosArquivoParam;
                Request.Form.TryGetValue("parceirosArquivoParam", out parceirosArquivoParam);
                
                
                ParceiroArchiveParam param = JsonConvert.DeserializeObject<ParceiroArchiveParam>(parceirosArquivoParam);
                byte[] imagem = null;
                var filePath = Path.GetTempFileName();
                var fileType = ".png";
                
                foreach (var formFile in Request.Form.Files)
                {
                    if (formFile.Length > 0)
                    {
                        using (var inputStream = new FileStream(filePath, FileMode.Create))
                        {
                            if (!String.IsNullOrEmpty(formFile.ContentType))
                            {
                                switch (formFile.ContentType)
                                {
                                    case "image/jpg":
                                        fileType = ".jpg";
                                        break;

                                    case "image/jpeg":
                                        fileType = ".jpeg";
                                        break;

                                    case "application/pdf":
                                        fileType = ".pdf";
                                        break;
                                }
                            }
                            await formFile.CopyToAsync(inputStream);
                            imagem = new byte[inputStream.Length];
                            inputStream.Seek(0, SeekOrigin.Begin);
                            inputStream.Read(imagem, 0, imagem.Length);
                        }
                    }
                }
                
                
                // await using var ms = new MemoryStream();
                // await upload.File.CopyToAsync(ms);
                param.bytes = imagem;
                // param.ParceiroID = ParceiroID;

                var result = await _parceirosService.InserirArquivo(param);
                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500, new { sucesso = false, mensagem = e.Message });
            }
        }

        [HttpGet("BuscarArquivoPorId")]
        public async Task<ActionResult<ParceiroArchiveDTO>> BuscarArquivoPorId(string arquivoId)
        {
            ApiGenericResult<ParceiroArchiveDTO>  result = await _parceirosService.BuscarArquivoPorId(arquivoId);
            
            return Ok(result);
        }
        
        [HttpGet("BuscarArquivosPorParceiroId")]
        public async Task<ActionResult<List<ParceiroArchiveDTO>>> BuscarArquivosPorParceiroId(string parceiroID)
        {
            ApiGenericResult<List<ParceiroArchiveDTO>>  result = await _parceirosService.BuscarArquivosPorParceiroId(parceiroID);
            
            return Ok(result);
        }
        
        [HttpPost("InserirParceiro")]
        public async Task<ActionResult<ApiGenericResult<ParceiroParamDTO>>> InserirParceiro([FromBody] ParceiroInserirParam param)
            => Ok(await _parceirosService.InserirParceiro(param, _usuarioLogado.Cpf));

        [HttpGet("BuscarParceiroPorId")]
        public async Task<ActionResult<ApiGenericResult<ParceiroDTO>>> BuscarParceiroPorId(string parceiroID)
            => Ok(await _parceirosService.BuscarParceiroPorId(parceiroID));

        [HttpGet("BuscarTodosParceiros")]
        public async Task<ActionResult<ApiGenericResult<List<ParceiroDTO>>>> BuscarTodosParceiros(int? orgId = null, string? filtro = null, string? bucket = null)
                 => Ok(await _parceirosService.BuscarTodosParceiros(orgId, filtro, bucket));

        [HttpPut("AtualizarParceiro")]
        public async Task<ActionResult<ApiGenericResult<ParceiroParamDTO>>> AtualizarParceiro(string parceiroID, [FromBody] ParceiroIDParam param)
            => Ok(await _parceirosService.AtualizarParceiro(parceiroID, param, _usuarioLogado.Cpf));

        [HttpDelete("DeletarParceiro")]
        public async Task<ActionResult<ApiGenericResult<bool>>> DeletarParceiro(string parceiroID)
            => Ok(await _parceirosService.DeletarParceiro(parceiroID));

        [HttpPost("InserirContrato")]
        public async Task<ActionResult<ApiGenericResult<ParceiroGestaoContratoDTO>>> InserirDocumentos([FromBody] ParceiroGestaoContratoParceiroIDParam param)
            => Ok(await _parceirosService.InserirContrato(param));

        [HttpPut("AtualizarContrato")]
        public async Task<ActionResult<ApiGenericResult<ParceiroGestaoContratoDTO>>> AtualizarContrato(ParceiroGestaoContratoParam param)
            => Ok(await _parceirosService.AtualizarContrato(param));

        [HttpDelete("DeletarContrato")]
        public async Task<ActionResult<ApiGenericResult<bool>>> DeletarContrato(string ID)
            => Ok(await _parceirosService.DeletarContrato(ID));

        [HttpDelete("DeletarArquivo")]
        public async Task<ActionResult<ApiGenericResult<bool>>> DeletarArquivo(string ID)
            => Ok(await _parceirosService.DeletarArquivo(ID));

        [HttpGet("BuscarGestaoContratoPorId")]
        public async Task<ActionResult<ApiGenericResult<ParceiroGestaoContratoDTO>>> BuscarGestaoContratoPorId(string gestaoContratoID)
         => Ok(await _parceirosService.BuscarGestaoContratoPorId(gestaoContratoID));
        
        [HttpGet("RelatorioParceriaAliancas")]
        public async Task<IActionResult> RelatorioParceriaAliancas()
        {
            // Nao tera filtro de data no momento
            //var fim = dataFim ?? DateTime.Now;
            //var inicio = dataInicio ?? fim.AddDays(-30);

            var fileResult = await _parceirosService.RelatorioParceriaAliancas(_usuarioLogado.OrgId);

            if (!fileResult.Sucesso)
                return NoContent();

            return File(fileResult.Retorno.FileContents, fileResult.Retorno.ContentType, fileResult.Retorno.FileDownloadName);
        }

        [HttpPost("ImportacaoPlanilhaContrato")]
        public async Task<ActionResult<ApiGenericResult<(int Sucesso, int Erros, List<string> Mensagens)>>> ImportacaoPlanilhaContrato([FromBody] ImportacaoPlanilhaParam param)
    => Ok(await _parceirosService.ImportacaoPlanilhaContrato(param.CaminhoArquivo, _usuarioLogado.Cpf, _usuarioLogado.OrgId));

    }

}
