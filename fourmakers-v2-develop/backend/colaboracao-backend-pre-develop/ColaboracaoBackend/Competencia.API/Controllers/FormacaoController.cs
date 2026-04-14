using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Formacao;
using Formacao.Domain.Interfaces.Services;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Formacao.API.Controllers
{
    [Authorize]
    [Route("api/Competencia/[controller]")]
    [ApiController]
    [LogAction]
    public class FormacaoController : ControllerBase
    {
        private readonly IFormacaoService _formacaoService;
        private readonly IAspNetUser _aspNetUser;

        public FormacaoController(IAspNetUser aspNetUser,
            IFormacaoService formacaoService)
        {
            _aspNetUser = aspNetUser;
            _formacaoService = formacaoService;
        }

        [HttpGet("ListarFormacao")]
        public ActionResult<ListaFormacaoResult> ListarFormacao(string busca, int cursor, int limite)
        {
            var ret = new ListaFormacaoResult();
            try
            {
                var retService = _formacaoService.ListarFormacao(busca, cursor, limite);

                ret.Formacao = retService;

                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }
        [HttpGet("GetFormacaoById")]
        public ActionResult<FormacaoResult> GetFormacaoById(long id)
        {
            var ret = new FormacaoResult();
            try
            {
                ret.Formacao = _formacaoService.GetFormacaoById(id);

                return Ok(ret);
            }
            catch (ArgumentNullException e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(404, ret);
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