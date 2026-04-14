using Colaboracao.Core;
using Colaborador.Domain.Interfaces.Services;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Log;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Colaborador.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [LogAction]
    public class PaisController : Controller
    {
        private readonly ILogCore _log;
        private readonly IPaisService _paisService;

        public PaisController(ILogCore log,
                              IPaisService paisService)
        {
            _log = log;
            _paisService = paisService;
        }

        [HttpGet("BuscarTodos")]
        public PaisResult BuscarTodos()
        {
            var ret = new PaisResult();
            try
            {
                ret.Pais = _paisService.BuscarTodos();
                return ret;
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                var tempAux = e.InnerException;
                while (tempAux != null)
                {
                    ret.Mensagem += "\n" + tempAux.Message;
                    tempAux = tempAux.InnerException;
                }

                return ret;
            }
        }
    }
}