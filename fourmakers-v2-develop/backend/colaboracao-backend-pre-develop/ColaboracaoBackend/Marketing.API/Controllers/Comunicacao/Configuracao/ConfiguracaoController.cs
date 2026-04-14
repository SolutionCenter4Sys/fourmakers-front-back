using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Marketing.Comunicacao.Configuracao;
using DataTransferObject.Domain.Usuario;
using Marketing.Domain.Interfaces.Comunicacao.Configuracao;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Marketing.API.Controllers.Comunicacao.Configuracao
{
    [Authorize]
    [Route("api/Marketing/Comunicacao/[controller]")]
    [HandleException]
    [ApiController]
    public class ConfiguracaoController : ControllerBase
    {
        private readonly IComunicacaoConfiguracaoService _service;
        private readonly UsuarioLogadoDTO _usuarioLogado;

        public ConfiguracaoController(
            IComunicacaoConfiguracaoService service,
            IAspNetUser aspNetUser)
        {
            _service = service;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
        }

        [HttpPost]
        public async Task<ActionResult> InserirOuAtualizarConfiguracao([FromBody] ConfiguracaoNotificacaoDTO request)
        {
            var resultado = await _service.InserirOuAtualizarConfiguracaoAsync(
                _usuarioLogado.Cpf,
                _usuarioLogado.OrgId,
                request
            );

            return Ok(resultado);
        }

        [HttpGet]
        public async Task<ActionResult> ObterConfiguracao()
        {
            var resultado = await _service.ObterConfiguracaoAsync(
                _usuarioLogado.Cpf,
                _usuarioLogado.OrgId
            );

            return Ok(resultado);
        }
    }
}
