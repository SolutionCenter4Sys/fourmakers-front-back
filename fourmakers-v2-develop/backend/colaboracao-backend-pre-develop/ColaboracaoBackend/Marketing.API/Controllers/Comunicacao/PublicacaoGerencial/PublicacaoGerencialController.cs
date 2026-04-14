using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Marketing.Comunicacao.Publicacao;
using DataTransferObject.Domain.Usuario;
using Marketing.Domain.Interfaces.Comunicacao.PublicacaoGerencial;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Marketing.API.Controllers.Comunicacao.PublicacaoGerencial
{
    [Authorize]
    [Route("api/Marketing/Comunicacao/[controller]")]
    [HandleException]
    [ApiController]
    public class PublicacaoGerencialController : ControllerBase
    {
        private readonly IComunicacaoPublicacaoGerencialService _service;
        private readonly UsuarioLogadoDTO _usuarioLogado;

        public PublicacaoGerencialController(
            IComunicacaoPublicacaoGerencialService service,
            IAspNetUser aspNetUser)
        {
            _service = service;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
        }

        //[HttpGet("AgendadosEAprovacao")]
        //public async Task<ActionResult> ObterAgendadosEAprovacao()
        //{
        //    var resultado = await _service.ObterListaPublicacaoAgendadoEAprovacaoAsync(
        //        _usuarioLogado.OrgId,
        //        _usuarioLogado.Cpf
        //    );

        //    return Ok(resultado);
        //}

        [HttpPost("{publicacaoId}/PublicarAgora")]
        public async Task<ActionResult> PublicarAgora(string publicacaoId)
        {
            var resultado = await _service.PublicarAgoraAsync(
                publicacaoId,
                _usuarioLogado.Cpf,
                _usuarioLogado.OrgId
            );

            return Ok(resultado);
        }

        [HttpPost("{publicacaoId}/Aprovar")]
        public async Task<ActionResult> AprovarComunicacao(string publicacaoId)
        {
            var resultado = await _service.AprovarComunicacaoAsync(
                publicacaoId,
                _usuarioLogado.Cpf,
                _usuarioLogado.OrgId
            );

            return Ok(resultado);
        }

        [HttpPost("{publicacaoId}/Rejeitar")]
        public async Task<ActionResult> RejeitarComunicacao(string publicacaoId, [FromBody] RejeitarComunicacaoRequestDTO request)
        {
            var resultado = await _service.RejeitarComunicacaoAsync(
                publicacaoId,
                _usuarioLogado.Cpf,
                _usuarioLogado.OrgId,
                request
            );

            return Ok(resultado);
        }
    }
}
