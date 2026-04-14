using Colaboracao.Core.Interfaces;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Projeto.Domain.Interfaces.Services;
using System.Threading.Tasks;

namespace Projeto.API.Controllers
{
    [Authorize]
    [Route("api/Projeto/[controller]")]
    [ApiController]
    [LogAction]
    public class RelatorioController : ControllerBase
    {
        private readonly IExtracaoProjetoService _extracaoProjetoService;
        private readonly IAspNetUser _aspNetUser;
        public RelatorioController(IExtracaoProjetoService extracaoProjetoService, IAspNetUser aspNetUser)
        {
            _extracaoProjetoService = extracaoProjetoService;
            _aspNetUser = aspNetUser;
        }

        [HttpGet("ExportarProjetosExcel")]
        public async Task<IActionResult> ExportarProjetosExcel()
        {
            var fileResult = await _extracaoProjetoService.ExportarProjetosExcel(_aspNetUser.GetUsuarioLogado().Cpf, _aspNetUser.GetUsuarioLogado().OrgId);

            if (!fileResult.Sucesso)
            {
                return StatusCode(500, fileResult.Mensagem);
            }

            return File(fileResult.Retorno.FileContents, fileResult.Retorno.ContentType, fileResult.Retorno.FileDownloadName);
        }
    }
}