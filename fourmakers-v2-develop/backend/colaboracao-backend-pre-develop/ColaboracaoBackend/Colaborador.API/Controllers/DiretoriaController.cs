using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using Colaborador.Domain.Interfaces.Services;
using DataTransferObject.Domain.Colaborador;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Colaborador.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/Colaborador/[controller]")]
    [LogAction]
    public class DiretoriaController : Controller
    {
        private readonly IColaboradorDiretoriaService _colaboradorDiretoriaService;
        private readonly IAspNetUser _aspNetUser;

        public DiretoriaController(IColaboradorDiretoriaService colaboradorDiretoriaService, IAspNetUser aspNetUser)
        {
            _colaboradorDiretoriaService = colaboradorDiretoriaService;
            _aspNetUser = aspNetUser;
        }

        [HttpGet("ListarDiretoriaDosColaboradores")]
        public async Task<ListarDiretoriaDosColaboradoresResult> ListarDiretoriaDosColaboradores()
        {
            var ret = new ListarDiretoriaDosColaboradoresResult();
            try
            {
                var usuarioLogado = _aspNetUser.GetUsuarioLogado();
                ret.DiretoriaColaborador = await _colaboradorDiretoriaService.ListarDiretoriaDosColaboradoresAsync(usuarioLogado.OrgId, usuarioLogado.Cpf);
                return ret;
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