using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using Colaborador.Domain.Interfaces.Services;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Escolaridade;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Colaborador.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [LogAction]
    public class EscolaridadeColaboradorController : Controller
    {
        private readonly IEscolaridadeColaboradorService _escolaridadeColaboradorService;
        private readonly ITokens _tokens;
        private readonly IAspNetUser _aspNetUser;
        private readonly ILogCore _log;
        private readonly IConfiguration _configuration;

        public EscolaridadeColaboradorController(IEscolaridadeColaboradorService escolaridadeColaborador, ITokens tokens, IAspNetUser aspNetUser, ILogCore log, IConfiguration configuration)
        {
            _escolaridadeColaboradorService = escolaridadeColaborador;
            _tokens = tokens;
            _aspNetUser = aspNetUser;
            _log = log;
            _configuration = configuration;
        }

        [Authorize]
        [HttpPost("AdicionarEscolaridadeColaborador")]
        public async Task<ActionResult<EscolaridadeResult>> AdicionarEscolaridadeColaborador()
        {
            var ret = new EscolaridadeResult();
            try
            {
                StringValues escolaridadeParam;
                Request.Form.TryGetValue("escolaridadeParam", out escolaridadeParam);
                AddEscolaridadeDTO param = JsonConvert.DeserializeObject<AddEscolaridadeDTO>(escolaridadeParam);

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

                ret.escolaridade = await _escolaridadeColaboradorService.AdicionarNovaEscolaridade(param.FormacaoId, param.Instituicao, param.DataInicio, param.DataTermino, param.Descricao, _aspNetUser.GetUsuarioLogado().Cpf, param.TipoDiplomaId, imagem, fileType);

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
        [HttpGet("ListarEscolaridadeColaborador")]
        public ActionResult<ListaEscolaridadeColaboradorResult> ListarEscolaridadeColaborador(string busca, int cursor, int limite)
        {
            var ret = new ListaEscolaridadeColaboradorResult();
            try
            {
                ret.Escolaridade = _escolaridadeColaboradorService.ListarEscolaridadeColaborador(busca, cursor, limite, _aspNetUser.GetUsuarioLogado().Cpf);

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
        [HttpGet("GetEscolaridadeColaboradorById")]
        public ActionResult<EscolaridadeResult> GetEscolaridadeColaboradorById(long id)
        {
            var ret = new EscolaridadeResult();
            try
            {
                var retService = _escolaridadeColaboradorService.GetEscolaridadeColaboradorById(id);

                return Ok(retService);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [Authorize]
        [HttpPost("RemoverEscolaridadeColaborador")]
        public ActionResult<StatusResult> RemoverEscolaridadeColaborador(RemoverEscolaridadeColaboradorDTO param)
        {
            var ret = new StatusResult();
            try
            {
                _escolaridadeColaboradorService.RemoverEscolaridadeColaborador(param.EscolaridadeId, _aspNetUser.GetUsuarioLogado().Cpf);
                ret.Sucesso = true;
                return Ok(ret);
            }
            catch (KeyNotFoundException e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
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
        [HttpPost("AtualizaEscolaridadeColaborador")]
        public async Task<ActionResult<EscolaridadeResult>> AtualizaEscolaridadeColaborador()
        {
            var ret = new EscolaridadeResult();
            try
            {
                StringValues escolaridadeParam;
                Request.Form.TryGetValue("escolaridadeParam", out escolaridadeParam);
                EscolaridadeDTO param = JsonConvert.DeserializeObject<EscolaridadeDTO>(escolaridadeParam);

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

                ret.escolaridade = await _escolaridadeColaboradorService.AtualizarEscolaridade(param.Id, param.FormacaoId, param.Instituicao, param.DataInicio, param.DataTermino, param.Descricao, (int)param.TipoDiplomaId, _aspNetUser.GetUsuarioLogado().Cpf, imagem, fileType);
                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }
    }
}