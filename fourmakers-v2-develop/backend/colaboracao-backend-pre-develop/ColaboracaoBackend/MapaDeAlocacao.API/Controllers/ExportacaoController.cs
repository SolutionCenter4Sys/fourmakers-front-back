using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.MapaDeAlocacao;
using DataTransferObject.Domain.Usuario;
using Logs.Infra.Attributes;
using MapaDeAlocacao.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Threading.Tasks;

namespace MapaDeAlocacao.API.Controllers
{
    [Authorize]
    [HandleException]
    [Route("api/MapaDeAlocacao/[controller]")]
    [ApiController]
    [LogAction]
    public class ExportacaoController : Controller
    {
        private readonly IMapaDeAlocacaoService _mapaDeAlocacaoService;
        private readonly IAspNetUser aspNetUser;
        private readonly UsuarioLogadoDTO _usuarioLogado;
        private readonly IProjetoMapaDeAlocacaoService _projetoMapaDeAlocacaoService;
        private readonly IExtracaoAlocacaoService _extracaoAlocacaoService;
        public ExportacaoController(IMapaDeAlocacaoService mapaDeAlocacaoService, IAspNetUser aspNetUser, IProjetoMapaDeAlocacaoService projetoMapaDeAlocacaoService, IExtracaoAlocacaoService extracaoAlocacaoService)
        {
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
            _extracaoAlocacaoService = extracaoAlocacaoService;
        }

        [HttpGet("RelatorioAlocacoes")]
        public async Task<FileResult> RelatorioAlocacoes()
        {
            var fileResult = await _extracaoAlocacaoService.RelatorioAlocacoes(_usuarioLogado.Cpf, _usuarioLogado.OrgId);

            if (!fileResult.Sucesso)
                throw (new Exception(fileResult.Mensagem));

            return File(fileResult.Retorno.FileContents, fileResult.Retorno.ContentType, fileResult.Retorno.FileDownloadName);
        }

        [HttpPost("RelatorioAlocacoesPorPesquisa")]
        public async Task<FileResult> RelatorioAlocacoesPorPesquisa([FromBody] ListarAlocacoesColabETbdInput dto)
        {
            var fileResult = await _extracaoAlocacaoService.RelatorioAlocacoesPorPesquisa(dto, _usuarioLogado.Cpf, _usuarioLogado.OrgId);

            if (!fileResult.Sucesso)
                throw (new Exception(fileResult.Mensagem));

            return File(fileResult.Retorno.FileContents, fileResult.Retorno.ContentType, fileResult.Retorno.FileDownloadName);
        }
    }
}