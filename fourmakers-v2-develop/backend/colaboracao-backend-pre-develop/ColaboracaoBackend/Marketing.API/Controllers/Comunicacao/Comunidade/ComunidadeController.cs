using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Marketing.Comunicacao.Comunidade;
using DataTransferObject.Domain.Usuario;
using Marketing.Domain.Interfaces.Comunicacao.Comunidade;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Marketing.API.Controllers.Comunicacao.Comunidade
{
    [Authorize]
    [Route("api/Marketing/Comunicacao/[controller]")]
    [HandleException]
    [ApiController]
    public class ComunidadeController : ControllerBase
    {
        private readonly IComunicacaoComunidadeService _service;
        private readonly UsuarioLogadoDTO _usuarioLogado;

        public ComunidadeController(
            IComunicacaoComunidadeService service,
            IAspNetUser aspNetUser)
        {
            _service = service;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
        }

        [HttpGet]
        public async Task<ActionResult> ObterListaComunidades()
        {
            var resultado = await _service.ObterListarComunidadesResumoAsync(
                _usuarioLogado.OrgId,
                _usuarioLogado.Cpf
            );

            return Ok(resultado);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult> ObterComunidadePorId(Guid id)
        {
            var resultado = await _service.ObterComunidadePorIdAsync(id, _usuarioLogado.OrgId, _usuarioLogado.Cpf);

            if (resultado?.Retorno == null)
                return NotFound(resultado);

            return Ok(resultado);
        }

        [HttpPost]
        public async Task<ActionResult> InserirComunidade([FromForm] InserirComunidadeRequestDTO request, IFormFile? capaComunidade = null)
        {
            byte[] capaImagem = null;

            if (capaComunidade != null && capaComunidade.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    await capaComunidade.CopyToAsync(ms);
                    capaImagem = ms.ToArray();
                }
            }

            var resultado = await _service.InserirComunidadeAsync(
                _usuarioLogado.Cpf,
                _usuarioLogado.OrgId,
                request,
                capaImagem
            );

            return Ok(resultado);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult> AtualizarComunidade(Guid id, [FromForm] AtualizarComunidadeRequestDTO request, IFormFile? capaComunidade = null)
        {
            byte[] capaImagem = null;
            if (capaComunidade != null && capaComunidade.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    await capaComunidade.CopyToAsync(ms);
                    capaImagem = ms.ToArray();
                }
            }

            var resultado = await _service.AtualizarComunidadeAsync(
                id,
                _usuarioLogado.Cpf,
                _usuarioLogado.OrgId,
                request,
                capaImagem
            );

            return Ok(resultado);
        }

        /// <summary>
        /// Entrar / voltar a participar: públicas — insere em tb_mkt_comunidade_usuario_participando;
        /// privadas — exige grupo vinculado ou já estar em usuario_participando; se permite sair, remove opt-out em tb_mkt_comunidade_privada_usuario_nao_participando.
        /// </summary>
        [HttpPost("{id:guid}/participar")]
        public async Task<ActionResult> ParticiparComunidade(Guid id)
        {
            var resultado = await _service.ParticiparComunidadeAsync(id, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(resultado);
        }

        /// <summary>
        /// Sair da comunidade: públicas — remove de usuario_participando; privadas com PermiteSair — opt-out em nao_participando se estiver em grupo, e remove de usuario_participando se estiver só ou também por essa via.
        /// </summary>
        [HttpDelete("{id:guid}/sair")]
        public async Task<ActionResult> SairComunidade(Guid id)
        {
            var resultado = await _service.SairComunidadeAsync(id, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(resultado);
        }
    }
}
