using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Marketing.Comunicacao.Grupo;
using DataTransferObject.Domain.Usuario;
using Marketing.Domain.Interfaces.Comunicacao.Grupo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Marketing.API.Controllers.Comunicacao.Grupo
{
    [Authorize]
    [Route("api/Marketing/Comunicacao/[controller]")]
    [HandleException]
    [ApiController]
    public class GrupoController : ControllerBase
    {
        private readonly IComunicacaoGrupoService _service;
        private readonly UsuarioLogadoDTO _usuarioLogado;

        public GrupoController(
            IComunicacaoGrupoService service,
            IAspNetUser aspNetUser)
        {
            _service = service;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
        }

        /// <summary>Modelos de contratação distintos entre colaboradores elegíveis (referência para filtro em ColaboradoresDisponiveis).</summary>
        [HttpGet("SugestoesModeloContratacaoColaboradoresDisponiveis")]
        public async Task<ActionResult> ObterSugestoesModeloContratacaoColaboradoresDisponiveis()
        {
            var resultado = await _service.ObterSugestoesModeloContratacaoColaboradoresDisponiveisAsync(_usuarioLogado.OrgId);
            return Ok(resultado);
        }

        /// <summary>Diretorias distintas entre colaboradores elegíveis (referência para filtro em ColaboradoresDisponiveis).</summary>
        [HttpGet("SugestoesDiretoriaColaboradoresDisponiveis")]
        public async Task<ActionResult> ObterSugestoesDiretoriaColaboradoresDisponiveis()
        {
            var resultado = await _service.ObterSugestoesDiretoriaColaboradoresDisponiveisAsync(_usuarioLogado.OrgId);
            return Ok(resultado);
        }

        [HttpGet("ColaboradoresDisponiveis")]
        public async Task<ActionResult> ListarColaboradoresDisponiveis(
            [FromQuery] string filtro = null,
            [FromQuery] List<string> codModeloContratacao = null,
            [FromQuery] List<string> codDiretoria = null)
        {
            var resultado = await _service.ListarColaboradoresDisponiveisParaAdicionarAsync(
                filtro,
                _usuarioLogado.OrgId,
                codModeloContratacao,
                codDiretoria
            );

            return Ok(resultado);
        }

        [HttpGet]
        public async Task<ActionResult> ListarGrupos()
        {
            var resultado = await _service.ListarGrupoResumoAsync(
                _usuarioLogado.OrgId
            );

            return Ok(resultado);
        }

        [HttpGet("{grupoId}")]
        public async Task<ActionResult> ObterGrupo(string grupoId)
        {
            var resultado = await _service.ObterGrupoAsync(
                grupoId,
                _usuarioLogado.OrgId
            );

            return Ok(resultado);
        }

        [HttpPost]
        public async Task<ActionResult> InserirGrupo([FromBody] InserirGrupoRequestDTO request)
        {
            var resultado = await _service.InserirGrupoAsync(
                _usuarioLogado.Cpf,
                _usuarioLogado.OrgId,
                request
            );

            return Ok(resultado);
        }

        [HttpPut("{grupoId}")]
        public async Task<ActionResult> AtualizarGrupo(string grupoId, [FromBody] AtualizarGrupoRequestDTO request)
        {
            var resultado = await _service.AtualizarGrupoAsync(
                grupoId,
                _usuarioLogado.OrgId,
                request
            );

            return Ok(resultado);
        }

        [HttpDelete("{grupoId}")]
        public async Task<ActionResult> DeletarGrupo(string grupoId)
        {
            var resultado = await _service.DeletarGrupoAsync(
                grupoId,
                _usuarioLogado.OrgId
            );

            return Ok(resultado);
        }

        /// <summary>
        /// Retorna as permissões do usuário logado com base nos grupos em que está.
        /// Para cada permissão: true se estiver em algum grupo que permite; false se não tiver grupos ou nenhum grupo permitir.
        /// </summary>
        [HttpGet("PermissoesUsuarioLogado")]
        public async Task<ActionResult> ObterPermissoesGruposUsuarioLogado()
        {
            var resultado = await _service.ObterPermissoesGruposUsuarioLogadoAsync(
                _usuarioLogado.OrgId,
                _usuarioLogado.Cpf
            );

            return Ok(resultado);
        }
    }
}
