using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.GestaoPessoa.Pdi;
using DataTransferObject.Domain.Usuario;
using GestaoPessoa.Domain.Interfaces.Services.Pdi;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoPessoa.API.Controllers.Pdi
{
    [Authorize]
    [ApiController]
    [Route("api/GestaoPessoa/Pdi/[controller]")]
    [HandleException]
    [LogAction]
    public class MetricasController : ControllerBase
    {
        private readonly IPdiMetricasService _service;
        private readonly UsuarioLogadoDTO _usuarioLogado;

        public MetricasController(IPdiMetricasService service, IAspNetUser aspNetUser)
        {
            _service = service;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();                       
        }

        /// <summary>Métricas PDI no escopo do usuário (org + restrição DIRETORIA). Filtros + paginação da lista: Pagina (default 1), TamanhoPagina (default 25, máx. 200).</summary>
        [HttpGet]
        public async Task<ActionResult> ObterMetricasPorOrg([FromQuery] PdiMetricasFiltroRequestDTO filtro)
        {
            var result = await _service.ObterMetricasPorOrgAsync(_usuarioLogado.Cpf, _usuarioLogado.OrgId, filtro);
            if (!result.Sucesso)
                return BadRequest(result);
            return Ok(result);
        }

        /// <summary>Lista gestores no escopo (org + diretoria). Query: pagina (default 1), tamanhoPagina (default 25, máx. 200).</summary>
        [HttpGet("gestores")]
        public async Task<ActionResult> ListarGestores([FromQuery] int? pagina, [FromQuery] int? tamanhoPagina)
        {
            var result = await _service.ListarGestoresOrgAsync(_usuarioLogado.Cpf, _usuarioLogado.OrgId, pagina, tamanhoPagina);
            if (!result.Sucesso)
                return BadRequest(result);
            return Ok(result);
        }

        /// <summary>Unidades (diretorias) no escopo da org do usuário. Query: pagina, tamanhoPagina (mesmos defaults).</summary>
        [HttpGet("unidades")]
        public async Task<ActionResult> ListarUnidades([FromQuery] int? pagina, [FromQuery] int? tamanhoPagina)
        {
            var result = await _service.ListarUnidadesMetricasAsync(_usuarioLogado.Cpf, _usuarioLogado.OrgId, pagina, tamanhoPagina);
            if (!result.Sucesso)
                return BadRequest(result);
            return Ok(result);
        }

        /// <summary>Colaboradores da org no escopo (cpfGestor/codDiretoria opcionais). pagina, tamanhoPagina opcionais.</summary>
        [HttpGet("colaboradores")]
        public async Task<ActionResult> ListarColaboradores(
            [FromQuery] string? cpfGestor,
            [FromQuery] string? codDiretoria,
            [FromQuery] int? pagina,
            [FromQuery] int? tamanhoPagina)
        {
            var result = await _service.ListarColaboradoresMetricasAsync(
                _usuarioLogado.Cpf, _usuarioLogado.OrgId, cpfGestor, codDiretoria, pagina, tamanhoPagina);
            if (!result.Sucesso)
                return BadRequest(result);
            return Ok(result);
        }

        /// <summary>Exporta PDIs selecionados em CSV (mesmo escopo/filtros da busca).</summary>
        [HttpPost("exportar")]
        public async Task<IActionResult> Exportar([FromBody] PdiMetricasExportRequestDTO request)
        {
            var result = await _service.ExportarMetricasCsvAsync(_usuarioLogado.Cpf, _usuarioLogado.OrgId, request);
            if (!result.Sucesso)
                return BadRequest(result);
            var bytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(result.Retorno.ConteudoCsv)).ToArray();
            return File(bytes, "text/csv; charset=utf-8", result.Retorno.NomeArquivo);
        }

        /// <summary>Visão colaborador: métricas dos próprios PDIs (big numbers, ativos e históricos).</summary>
        [HttpGet("colaborador")]
        public async Task<ActionResult> ObterMetricasColaborador()
        {
            var result = await _service.ObterMetricasColaboradorAsync(_usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(result);
        }

        /// <summary>Visão gestor: métricas do time (próprios + subordinados).</summary>
        [HttpGet("gestor")]
        public async Task<ActionResult> ObterMetricasGestor()
        {
            var result = await _service.ObterMetricasGestorAsync(_usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(result);
        }

        /// <summary>Visão gestor: métricas de um colaborador específico do time (colaboradorId = codigo_interno/cpf do colaborador).</summary>
        [HttpGet("gestor/colaborador/{colaboradorId}")]
        public async Task<ActionResult> ObterMetricasGestorPorColaborador(string colaboradorId)
        {
            var result = await _service.ObterMetricasGestorPorColaboradorAsync(colaboradorId, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
            if (!result.Sucesso)
                return BadRequest(result);
            return Ok(result);
        }
    }
}
