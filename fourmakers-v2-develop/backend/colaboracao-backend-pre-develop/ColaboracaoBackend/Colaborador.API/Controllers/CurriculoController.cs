using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using Colaborador.Domain.Interfaces.Services;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Colaborador;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Colaborador.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [LogAction]
    public class CurriculoController : Controller
    {
        private readonly IColaboradorService _colaboradorService;
        private readonly IAspNetUser _aspNetUser;

        public CurriculoController(IColaboradorService colaboradorService, IAspNetUser aspNetUser)
        {
            _colaboradorService = colaboradorService;
            _aspNetUser = aspNetUser;
        }

        [HttpPost("AlterarCurriculoColaborador")]
        public async Task<ActionResult<StatusResult>> AlterarCurriculoColaborador()
        {
            var ret = new StatusResult();
            var httpRequest = HttpContext.Request;

            try
            {
                byte[] file = null;
                var filePath = Path.GetTempFileName();
                foreach (var formFile in Request.Form.Files)
                {
                    if (formFile.Length > 0)
                    {
                        using (var inputStream = new FileStream(filePath, FileMode.Create))
                        {
                            switch (formFile.ContentType)
                            {
                                case "application/pdf":
                                    await formFile.CopyToAsync(inputStream);
                                    file = new byte[inputStream.Length];
                                    inputStream.Seek(0, SeekOrigin.Begin);
                                    inputStream.Read(file, 0, file.Length);
                                    break;

                                default:
                                    throw new Exception("Formato inválido do certificado.");
                            }
                        }
                    }
                    await _colaboradorService.AlterarCurriculoColaborador(file, _aspNetUser.GetUsuarioLogado().Cpf);
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

        [HttpGet("GetCurriculoColaborador")]
        public async Task<GetCurriculoColaboradorResult> GetCurriculoColaborador()
        {
            var ret = new GetCurriculoColaboradorResult();
            try
            {
                var retService = await _colaboradorService.GetCurriculoColaborador(_aspNetUser.GetUsuarioLogado().Cpf);
                if (!string.IsNullOrEmpty(retService))
                {
                    ret.file = retService;
                    return ret;
                }
                else
                {
                    throw new Exception("não há arquivo!");
                }
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return ret;
            }
        }
        [HttpPost("InsereCurriculoColaborador")]
        public async Task<GetCurriculoColaboradorResult> InsereCurriculoColaborador()
        {
            var ret = new GetCurriculoColaboradorResult();
            try
            {
                byte[] file = null;
                var filePath = Path.GetTempFileName();
                foreach (var formFile in Request.Form.Files)
                {
                    if (formFile.Length > 0)
                    {
                        using (var inputStream = new FileStream(filePath, FileMode.Create))
                        {
                            switch (formFile.ContentType)
                            {
                                case "application/pdf":
                                    await formFile.CopyToAsync(inputStream);
                                    file = new byte[inputStream.Length];
                                    inputStream.Seek(0, SeekOrigin.Begin);
                                    inputStream.Read(file, 0, file.Length);
                                    break;

                                default:
                                    throw new Exception("Formato inválido do certificado.");
                            }
                        }
                    }
                }
                var retService = await _colaboradorService.InsereCurriculoColaborador(file, _aspNetUser.GetUsuarioLogado().Cpf);

                if (retService != null)
                {
                    ret.file = retService;
                    return ret;
                }
                ret.Sucesso = false;
                return null;
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return ret;
            }
        }
    }
}