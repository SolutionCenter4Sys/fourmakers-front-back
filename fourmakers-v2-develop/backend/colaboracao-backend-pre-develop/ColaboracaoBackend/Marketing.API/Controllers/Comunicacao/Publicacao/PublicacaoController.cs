using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Marketing.Comunicacao.Profissionais;
using DataTransferObject.Domain.Marketing.Comunicacao.Publicacao;
using DataTransferObject.Domain.Usuario;
using Marketing.Domain.Interfaces.Comunicacao.Publicacao;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Marketing.API.Controllers.Comunicacao.Publicacao
{
    [Authorize]
    [Route("api/Marketing/Comunicacao/[controller]")]
    [HandleException]
    [ApiController]
    public class PublicacaoController : ControllerBase
    {
        private readonly IComunicacaoPublicacaoService _service;
        private readonly UsuarioLogadoDTO _usuarioLogado;

        public PublicacaoController(
            IComunicacaoPublicacaoService service,
            IAspNetUser aspNetUser)
        {
            _service = service;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
        }

        [HttpPost("ObterListaPublicacaoGeral")]
        public async Task<ActionResult> ObterListaPublicacaoGeralAsync([FromBody] ObterListaPublicacaoGeralRequestDTO request)
        {
            var resultado = await _service.ObterListaPublicacaoGeralAsync(
                _usuarioLogado.Cpf,
                _usuarioLogado.OrgId,
                request?.SomenteLeituraObrigatoria ?? false,
                request?.Tipo,
                request?.Labels,
                request?.Tags,
                request?.Status,
                request?.StatusAprovacao,
                request?.ComunidadeId,
                request?.SomenteNaoOcultoNoFeed ?? false,
                request?.SomentePublicacaoOficial ?? false
            );

            return Ok(resultado);
        }

        /// <summary>Lista nomes de pasta sugeridos para a org (autocomplete), conforme tabela sincronizada com publicações ativas.</summary>
        [HttpGet("ObterSugestoesPastas")]
        public async Task<ActionResult> ObterSugestoesPastasAsync()
        {
            var resultado = await _service.ObterSugestoesPastasAsync(_usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(resultado);
        }

        [HttpPost("ConfirmarLeituraObrigatoria")]
        public async Task<ActionResult> ConfirmarLeituraObrigatoria([FromBody] ConfirmarLeituraObrigatoriaRequestDTO request)
        {
            var resultado = await _service.ConfirmarLeituraObrigatoriaAsync(
                _usuarioLogado.Cpf,
                _usuarioLogado.OrgId,
                request
            );

            return Ok(resultado);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> InserirPublicacao([FromForm] InserirPublicacaoRequestDTO request, [FromForm] List<IFormFile> anexosUpload)
        {
            var anexosUploadMapeados = await MapearAnexosUploadAsync(anexosUpload);

            var resultado = await _service.InserirPublicacaoAsync(
                _usuarioLogado.Cpf,
                _usuarioLogado.OrgId,
                request,
                anexosUploadMapeados
            );

            return Ok(resultado);
        }

        [HttpGet("{publicacaoId}")]
        public async Task<ActionResult> ObterPublicacaoPorId(string publicacaoId)
        {
            if (string.IsNullOrWhiteSpace(publicacaoId))
                return BadRequest(new ApiGenericResult<PublicacaoDetalheDTO> { Sucesso = false, Mensagem = "Parâmetro publicacaoId não informado." });
            if (string.Equals(publicacaoId, "ObterListaPublicacaoGeral", StringComparison.OrdinalIgnoreCase))
                return BadRequest(new ApiGenericResult<PublicacaoDetalheDTO> { Sucesso = false, Mensagem = "ObterListaPublicacaoGeral com GET não existe." });
            if (!Guid.TryParse(publicacaoId, out _))
                return BadRequest(new ApiGenericResult<PublicacaoDetalheDTO> { Sucesso = false, Mensagem = "Parâmetro publicacaoId não informado." });

            var resultado = await _service.ObterPublicacaoPorIdAsync(
                publicacaoId,
                _usuarioLogado.Cpf,
                _usuarioLogado.OrgId
            );

            return Ok(resultado);
        }

        /// <summary>
        /// Retorna a lista de pessoas que aceitaram/confirmaram leitura do informativo ou documento (mesma estrutura de profissionais).
        /// </summary>
        [HttpGet("{publicacaoId}/ConfirmacoesLeitura")]
        public async Task<ActionResult> ObterColaboradoresQueConfirmaramLeitura(string publicacaoId)
        {
            if (string.IsNullOrWhiteSpace(publicacaoId))
                return BadRequest(new ApiGenericResult<List<ProfissionalDTO>> { Sucesso = false, Mensagem = "Parâmetro publicacaoId não informado." });
            if (!Guid.TryParse(publicacaoId, out _))
                return BadRequest(new ApiGenericResult<List<ProfissionalDTO>> { Sucesso = false, Mensagem = "Parâmetro publicacaoId inválido." });

            var resultado = await _service.ObterColaboradoresQueConfirmaramLeituraAsync(
                publicacaoId,
                _usuarioLogado.Cpf,
                _usuarioLogado.OrgId
            );

            return Ok(resultado);
        }

        [HttpPut]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> AtualizarPublicacao([FromForm] AtualizarPublicacaoRequestDTO request)
        {
            var resultado = await _service.AtualizarPublicacaoAsync(
                _usuarioLogado.Cpf,
                _usuarioLogado.OrgId,
                request
            );

            return Ok(resultado);
        }

        [HttpDelete("{publicacaoId}")]
        public async Task<ActionResult> DeletarPublicacao(string publicacaoId)
        {
            var resultado = await _service.DeletarPublicacaoAsync(
                publicacaoId,
                _usuarioLogado.Cpf,
                _usuarioLogado.OrgId
            );

            return Ok(resultado);
        }

        [HttpPost("{publicacaoId}/Arquivar")]
        public async Task<ActionResult> ArquivarPublicacao(string publicacaoId)
        {
            var resultado = await _service.ArquivarPublicacaoAsync(
                publicacaoId,
                _usuarioLogado.Cpf,
                _usuarioLogado.OrgId
            );

            return Ok(resultado);
        }

        /// <summary>
        /// Atualiza apenas o bit ocultar_no_feed da publicação (true = ocultar no feed, false = exibir no feed).
        /// </summary>
        [HttpPatch("OcultarNoFeed")]
        public async Task<ActionResult> AtualizarOcultarNoFeed([FromBody] AtualizarOcultarNoFeedRequestDTO request)
        {
            if (request == null)
                return BadRequest(new ApiGenericResult { Sucesso = false, Mensagem = "Request obrigatório." });
            if (request.PublicacaoId == Guid.Empty)
                return BadRequest(new ApiGenericResult { Sucesso = false, Mensagem = "PublicacaoId obrigatório." });

            var resultado = await _service.AtualizarOcultarNoFeedAsync(
                _usuarioLogado.Cpf,
                _usuarioLogado.OrgId,
                request
            );

            return Ok(resultado);
        }

        /// <summary>
        /// Adiciona um ou mais anexos à publicação. Arquivos são enviados ao S3 via UploadFiles (IUploadFilesClient.UploadFile).
        /// </summary>
        [HttpPost("{publicacaoId}/Anexos")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> AdicionarAnexos(string publicacaoId, [FromForm] PublicacaoAnexosRequestDTO request, [FromForm] List<IFormFile> anexosUpload)
        {
            var anexosUploadMapeados = await MapearAnexosUploadAsync(anexosUpload);

            var resultado = await _service.AdicionarAnexosAsync(
                publicacaoId,
                _usuarioLogado.Cpf,
                _usuarioLogado.OrgId,
                request?.Anexos,
                anexosUploadMapeados
            );

            return Ok(resultado);
        }

        /// <summary>
        /// Remove um anexo da publicação. O arquivo é excluído do S3 via UploadFiles (IUploadFilesClient.DeleteFile).
        /// </summary>
        [HttpDelete("{publicacaoId}/Anexos/{anexoId}")]
        public async Task<ActionResult> RemoverAnexo(string publicacaoId, string anexoId)
        {
            var resultado = await _service.RemoverAnexoAsync(
                publicacaoId,
                anexoId,
                _usuarioLogado.Cpf,
                _usuarioLogado.OrgId
            );

            return Ok(resultado);
        }

        /// <summary>
        /// Remove múltiplos anexos da publicação. Cada arquivo é excluído do S3 via UploadFiles (IUploadFilesClient.DeleteFile).
        /// </summary>
        [HttpDelete("{publicacaoId}/Anexos")]
        public async Task<ActionResult> RemoverAnexos(string publicacaoId, [FromBody] RemoverAnexosRequestDTO request)
        {
            if (request?.AnexoIds == null || request.AnexoIds.Count == 0)
                return BadRequest(new ApiGenericResult { Sucesso = false, Mensagem = "Informe ao menos um AnexoId no corpo da requisição (AnexoIds)." });

            var resultado = await _service.RemoverAnexosAsync(
                publicacaoId,
                request.AnexoIds,
                _usuarioLogado.Cpf,
                _usuarioLogado.OrgId
            );

            return Ok(resultado);
        }

        [HttpPost("Interacao")]
        public async Task<ActionResult> AdicionarInteracaoPublicacao([FromBody] AdicionarInteracaoPublicacaoRequestDTO request)
        {
            if (request == null)
                return BadRequest(new ApiGenericResult { Sucesso = false, Mensagem = "Request obrigatório." });
            if (request.PublicacaoId == Guid.Empty)
                return BadRequest(new ApiGenericResult { Sucesso = false, Mensagem = "PublicacaoId obrigatório." });
            if (request.Emoji != null && request.Emoji.Length > 50)
                return BadRequest(new ApiGenericResult { Sucesso = false, Mensagem = "Emoji deve ter no máximo 50 caracteres." });

            var resultado = await _service.AdicionarInteracaoPublicacaoAsync(
                _usuarioLogado.Cpf,
                _usuarioLogado.OrgId,
                request
            );
            return Ok(resultado);
        }

        [HttpDelete("Interacao")]
        public async Task<ActionResult> RemoverInteracaoPublicacao([FromBody] RemoverInteracaoPublicacaoRequestDTO request)
        {
            if (request == null)
                return BadRequest(new ApiGenericResult { Sucesso = false, Mensagem = "Request obrigatório." });
            if (request.PublicacaoId == Guid.Empty)
                return BadRequest(new ApiGenericResult { Sucesso = false, Mensagem = "PublicacaoId obrigatório." });

            var resultado = await _service.RemoverInteracaoPublicacaoAsync(
                _usuarioLogado.Cpf,
                _usuarioLogado.OrgId,
                request
            );
            return Ok(resultado);
        }

        private static async Task<List<PublicacaoAnexoUploadDTO>> MapearAnexosUploadAsync(List<IFormFile> anexos)
        {
            if (anexos == null || anexos.Count == 0)
                return null;

            var resultado = new List<PublicacaoAnexoUploadDTO>(anexos.Count);
            foreach (var arquivo in anexos)
            {
                if (arquivo == null)
                    continue;

                using (var ms = new MemoryStream())
                {
                    await arquivo.CopyToAsync(ms);
                    resultado.Add(new PublicacaoAnexoUploadDTO
                    {
                        NomeArquivo = Path.GetFileName(arquivo.FileName),
                        ContentType = arquivo.ContentType,
                        TamanhoBytes = arquivo.Length,
                        Conteudo = ms.ToArray()
                    });
                }
            }

            return resultado;
        }
    }
}
