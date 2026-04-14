using Apontamento.Domain.Interfaces.Service;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Usuario;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Apontamento.API.Controllers
{
    [Authorize]
    [Route("api/Apontamento/[controller]")]
    [ApiController]
    [LogAction]
    public class PeriodoFechadoController : ControllerBase
    {
        private readonly IPeriodoFechadoService _periodoFechadoService;
        private readonly UsuarioLogadoDTO _usuarioLogado;

        public PeriodoFechadoController(IPeriodoFechadoService periodoFechadoService, IAspNetUser aspNetUser)
        {
            _periodoFechadoService = periodoFechadoService;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
        }

        [HttpPost("FecharAlterarPeriodo")]
        public ActionResult<StatusResult> FecharAlterarPeriodo(DateTime dataFim)
        {
            var result = new StatusResult();
            try
            {
                var ret = _periodoFechadoService.FecharAlterarPeriodo(dataFim, _usuarioLogado.OrgId, _usuarioLogado.Cpf);
                return Ok(ret);
            }
            catch (Exception e)
            {
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        [HttpGet("BuscaPeriodoFechado")]
        public async Task<ActionResult<StatusResult>> BuscaPeriodoFechado()
        {
            var result = new StatusResult();
            try
            {
                var ret = await _periodoFechadoService.BuscaPeriodoFechado(_usuarioLogado.OrgId);
                return Ok(ret);
            }
            catch (Exception e)
            {
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }
    }
}