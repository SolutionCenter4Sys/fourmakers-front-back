using DataTransferObject.Domain.Base;
using Firebase.API.DTOs;
using Firebase.Domain.Interfaces.Services;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Firebase.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [LogAction]
    public class FirebaseController : ControllerBase
    {
        private readonly IFirebaseService _firebaseService;

        public FirebaseController(IFirebaseService firebaseService)
        {
            _firebaseService = firebaseService;
        }

        [HttpPost("EnviaPushDoFirebaseParaApenasUmColaborador")]
        public async Task<ActionResult<StatusResult>> EnviaPush(ParamFirebase paramFirebase)
        {
            var ret = new StatusResult();
            try
            {
                var retFirebaseService = await _firebaseService.EnviaPush(paramFirebase.Token, paramFirebase.Titulo, paramFirebase.Mensagem);
                ret.Sucesso = retFirebaseService;
                if (ret.Sucesso)
                {
                    ret.Mensagem = "Notificação do Firebase enviada com sucesso!";
                }
                else
                {
                    ret.Mensagem = "Houve algum erro no envio da notificação do Firebase!";
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

        [HttpPost("EnviaPushDoFirebaseEmLote")]
        public async Task<ActionResult<StatusResult>> EnviaPushEmLote(ParamListaFirebase param)
        {
            var ret = new StatusResult();
            try
            {
                var retFirebaseService = await _firebaseService.EnviaPushEmLote(param.Tokens, param.Titulo, param.Mensagem);
                ret.Sucesso = retFirebaseService;
                if (ret.Sucesso)
                {
                    ret.Mensagem = "Notificação do Firebase enviada com sucesso!";
                }
                else
                {
                    ret.Mensagem = "Houve algum erro no envio da notificação do Firebase!";
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
    }
}