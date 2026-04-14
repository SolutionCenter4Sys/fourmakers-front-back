using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Usuario;
using Marketing.Domain.Interfaces.Comunicacao.Profissionais;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Marketing.API.Controllers.Comunicacao.Profissionais
{
    [Authorize]
    [Route("api/Marketing/Comunicacao/[controller]")]
    [HandleException]
    [ApiController]
    public class ProfissionaisController : ControllerBase
    {
        private readonly IComunicacaoProfissionaisService _service;
        private readonly UsuarioLogadoDTO _usuarioLogado;

        public ProfissionaisController(
            IComunicacaoProfissionaisService service,
            IAspNetUser aspNetUser)
        {
            _service = service;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
        }

        [HttpGet]
        public async Task<ActionResult> ObterListaProfissionais()
        {
            var resultado = await _service.ObterListaProfissionaisAsync(
                _usuarioLogado.OrgId,
                _usuarioLogado.Cpf
            );

            return Ok(resultado);
        }

        [HttpPost("Favoritar")]
        public async Task<ActionResult> FavoritarProfissional([FromQuery] string codigoInternoColaborador)
        {
            if (string.IsNullOrWhiteSpace(codigoInternoColaborador))
                return BadRequest("codigoInternoColaborador é obrigatório.");
            var resultado = await _service.FavoritarProfissionalAsync(
                codigoInternoColaborador,
                _usuarioLogado.Cpf,
                _usuarioLogado.OrgId
            );
            return Ok(resultado);
        }

        [HttpPost("Desfavoritar")]
        public async Task<ActionResult> DesfavoritarProfissional([FromQuery] string codigoInternoColaborador)
        {
            if (string.IsNullOrWhiteSpace(codigoInternoColaborador))
                return BadRequest("codigoInternoColaborador é obrigatório.");
            var resultado = await _service.DesfavoritarProfissionalAsync(
                codigoInternoColaborador,
                _usuarioLogado.Cpf,
                _usuarioLogado.OrgId
            );
            return Ok(resultado);
        }

        [HttpGet("Favoritados")]
        public async Task<ActionResult> ListarProfissionaisFavoritados()
        {
            var resultado = await _service.ListarProfissionaisFavoritadosAsync(
                _usuarioLogado.Cpf,
                _usuarioLogado.OrgId
            );
            return Ok(resultado);
        }
    }
}
