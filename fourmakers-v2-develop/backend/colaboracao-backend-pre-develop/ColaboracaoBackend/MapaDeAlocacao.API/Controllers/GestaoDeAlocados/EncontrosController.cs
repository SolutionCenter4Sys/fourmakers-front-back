using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.SolicitacaoParticipacao;
using DataTransferObject.Domain.Usuario;
using Logs.Infra.Attributes;
using MapaDeAlocacao.Domain.Interfaces.GestaoDeAlocados;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace MapaDeAlocacao.API.Controllers.GestaoDeAlocados
{
    [Route("api/GestaoDeAlocados/[controller]")]
    [ApiController]
    [HandleException]
    [Authorize]
    [LogAction]
    public class EncontrosController : ControllerBase
    {
        private readonly UsuarioLogadoDTO _usuarioLogado;
        private readonly IEncontrosService _encontrosService;

        public EncontrosController(IAspNetUser aspNetUser, IEncontrosService encontrosService)
        {
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
            _encontrosService = encontrosService;
        }

        [HttpPost("InserirArquivo")]
        public async Task<ActionResult<ArquivoEncontroDto>> InserirArquivo()
        {
            try
            {
                StringValues arquivoParam;
                Request.Form.TryGetValue("arquivoParam", out arquivoParam);


                ArquivoEncontroParam param = JsonConvert.DeserializeObject<ArquivoEncontroParam>(arquivoParam);
                byte[] imagem = null;
                var filePath = Path.GetTempFileName();
                var fileType = ".png";

                foreach (var formFile in Request.Form.Files)
                {
                    if (formFile.Length > 0)
                    {
                        using (var inputStream = new FileStream(filePath, FileMode.Create))
                        {
                            if (!String.IsNullOrEmpty(formFile.ContentType))
                            {
                                switch (formFile.ContentType)
                                {
                                    case "image/jpg":
                                        fileType = ".jpg";
                                        break;

                                    case "image/jpeg":
                                        fileType = ".jpeg";
                                        break;

                                    case "application/pdf":
                                        fileType = ".pdf";
                                        break;
                                    case "image/png":
                                        fileType = ".png";
                                        break;
                                    case "audio/mpeg":
                                        fileType = ".mp3";
                                        break;
                                    case "audio/m4a":
                                        fileType = ".m4a";
                                        break;
                                    case "video/mp4":
                                        fileType = ".mp4";
                                        break;
                                    case "audio/ogg":
                                        fileType = ".ogg";
                                        break;
                                }
                            }
                            await formFile.CopyToAsync(inputStream);
                            imagem = new byte[inputStream.Length];
                            param.NomeArquivo = formFile.FileName;
                            param.TipoArquivo = fileType;
                            inputStream.Seek(0, SeekOrigin.Begin);
                            inputStream.Read(imagem, 0, imagem.Length);
                        }
                    }
                }
                param.bytes = imagem;
                var result = await _encontrosService.InserirArquivo(param);
                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500, new { sucesso = false, mensagem = e.Message });
            }
        }

        [HttpDelete("deletarArquivo/{id}")]
        public async Task<ActionResult<ApiGenericResult<StatusResult>>> deletarArquivo(int id) 
            => Ok(await _encontrosService.DeletarArquivo(id));

        [HttpPost("CriarEncontro")]
        public async Task<ActionResult<ApiGenericResult<InteracoesDTO>>> CriarEncontro([FromBody] EncontrosDTO param) 
            => Ok(await _encontrosService.criarEncontro(param, _usuarioLogado));
        
        [HttpDelete("deletarEncontro/{encontroId}")]
        public async Task<ActionResult<ApiGenericResult<bool>>> deletarEncontro(string encontroId) 
            => Ok(await _encontrosService.deletarEncontro(encontroId));

        [HttpPost("CriarAgenda")]
        public async Task<ActionResult<ApiGenericResult<AgendaEncontroResult>>> CriarAgenda([FromBody] AgendaEncontroParam param) 
            => Ok(await _encontrosService.criarAgenda(param, _usuarioLogado));
        
        [HttpGet("carregarAgenda/{id}")]
        public async Task<ActionResult<ApiGenericResult<AgendaEncontroResult>>> carregarAgenda(string id)
            => Ok(await _encontrosService.carregarAgenda(int.Parse(id), _usuarioLogado.Cpf));
        
        [HttpDelete("deletarAgenda/{agendaId}")]
        public async Task<ActionResult<ApiGenericResult<bool>>> deletarAgenda(string agendaId)
            => Ok(await _encontrosService.deletarAgenda(agendaId));
        
        [HttpPost("atualizarAgenda/{agendaId}")]
        public async Task<ActionResult<ApiGenericResult<AgendaEncontroResult>>> atualizarAgenda(string agendaId, [FromBody] AgendaEncontroParam param) 
            => Ok(await _encontrosService.atualizarAgenda(int.Parse(agendaId), param, _usuarioLogado));
        
        [HttpGet("buscarEncontrosPorAgendaId/{agendaId}")]
        public async Task<ActionResult<ApiGenericResult<List<EncontrosResponse>>>> buscarEncontrosPorAgendaId(string agendaId) 
            => Ok(await _encontrosService.buscarEncontrosPorAgendaId(agendaId));
            
        
        [HttpPost("atualizarEncontro/{encontroId}")]
        public async Task<ActionResult<ApiGenericResult<InteracoesDTO>>> atualizarEncontro(string encontroId,[FromBody] EncontrosParam param) 
            => Ok(await _encontrosService.atualizarEncontro(encontroId, param));
        
        [HttpGet("buscarEncontroPorId/{encontroId}")]
        public async Task<ActionResult<ApiGenericResult<EncontrosResponse>>> buscarEncontroPorId(string encontroId) 
            => Ok(await _encontrosService.buscarEncontroPorId(encontroId));

        [HttpGet("BuscarEncontroPorIdComComentarios/{encontroId}")]
        public async Task<ActionResult<ApiGenericResult<EncontrosResponse>>> BuscarEncontroPorIdComComentarios(string encontroId)
            => Ok(await _encontrosService.BuscarEncontroPorIdComComentarios(encontroId));

        [HttpGet("ListarCategoriasSubPorInteracaoId/{interacaoId}")]
        public async Task<ActionResult<ApiGenericResult<IEnumerable<InteracoesCategoriaSubResponseDTO>>>> ListarCategoriasSubPorInteracaoId(int interacaoId)
            => Ok(await _encontrosService.ListarCategoriasSubPorInteracaoId(interacaoId));

        [HttpGet("buscarTodosEncontros")]
        public async Task<ActionResult<ApiGenericResult<List<EncontrosResponse>>>> buscarTodosEncontros() 
            => Ok(await _encontrosService.buscarTodosEncontros());
        
        [HttpGet("buscarTodasAgendas")]
        public async Task<ActionResult<ApiGenericResult<List<AgendaEncontroResult>>>> buscarTodasAgendas([FromQuery] string? dataInicio, [FromQuery] string? dataFim) 
            => Ok(await _encontrosService.buscarAgendaEncontros(_usuarioLogado.Cpf, dataInicio, dataFim));
        
        [HttpGet("buscarAgendasPorColaboradorId/{colaboradorId}")]
        public async Task<ActionResult<ApiGenericResult<List<AgendaEncontroResult>>>> buscarAgendasPorColaboradorId(string colaboradorId) 
            => Ok(await _encontrosService.buscarAgendaEncontrosPorColaboradorId(colaboradorId));
        
        [HttpGet("buscarAgendaEncontrosPorData/{data}")]
        public async Task<ActionResult<ApiGenericResult<List<AgendaEncontroResult>>>> buscarAgendaEncontrosPorData(string data) 
            => Ok(await _encontrosService.buscarAgendaEncontrosPorData(data));
        
        [HttpGet("buscarEncontroPorColaboradorId/{colaboradorId}")]
        public async Task<ActionResult<ApiGenericResult<EncontrosResponse>>> buscarEncontroPorColaboradorId(string colaboradorId) 
            => Ok(await _encontrosService.buscarEncontroPorColaboradorId(colaboradorId));
        
        
        [HttpPost("CriarEncontroAiProximosPassos")]
        public async Task<ActionResult<ApiGenericResult<EncontroAi>>> CriarEncontroAiProximosPassos([FromBody] EncontroAi param) 
            => Ok(await _encontrosService.CriarEncontroAi(param));
        
        [HttpDelete("DeletarEncontroAiProximosPassos/{aiPassosId}")]
        public async Task<ActionResult<ApiGenericResult<bool>>> DeletarEncontroAiProximosPassos(string aiPassosId) 
            => Ok(await _encontrosService.DeletarEncontroAi(int.Parse(aiPassosId)));

        [HttpPost("AtualizarEncontroAiProximosPassos")]
        public async Task<ActionResult<ApiGenericResult<EncontroAi>>> AtualizarEncontroAiProximosPassos([FromBody] EncontroAi param) 
            => Ok(await _encontrosService.AtualizarEncontroAi(param));
        
        [HttpGet("BuscarEncontroAiPorEncontroId/{encontroId}")]
        public async Task<ActionResult<ApiGenericResult<EncontroAi>>> BuscarEncontroAiPorEncontroId(string encontroId)
            => Ok(await _encontrosService.BuscarEncontroAiPorEncontroId(int.Parse(encontroId)));

        [HttpGet("BuscarSolicitacoesAgendas")]
        public async Task<ActionResult<ApiGenericResult<List<SolicitacaoResponse>>>> BuscarSolicitacoesAgendas(int cursor, int limite)
            => Ok(await _encontrosService.BuscarSolicitacoesAgendas(_usuarioLogado.Cpf, cursor, limite));

        [HttpGet("BuscarSolicitacoesMinhasAgendas")]
        public async Task<ActionResult<ApiGenericResult<List<SolicitacaoResponse>>>> BuscarSolicitacoesMinhasAgendas(int cursor, int limite)
            => Ok(await _encontrosService.BuscarSolicitacoesMinhasAgendas(_usuarioLogado.Cpf, cursor, limite));

        [HttpGet("BuscarSolicitacoesPorAgenda")]
        public async Task<ActionResult<ApiGenericResult<List<SolicitacaoResponse>>>> BuscarSolicitacoesPorAgenda(int agendaId, int cursor, int limite)
            => Ok(await _encontrosService.BuscarSolicitacoesPorAgenda(agendaId, cursor, limite));

        [HttpPost("AprovarReprovarSolicitacaoAgenda")]
        public async Task<ActionResult<ApiGenericResult<bool>>> AprovarReprovarSolicitacaoAgenda(StatusSolicitacaoParticipante decisaoStatus, int agendaId, string codigoColaboradorSolicitante)
            => Ok(await _encontrosService.AprovarReprovarSolicitacaoAgenda(decisaoStatus, agendaId, codigoColaboradorSolicitante, _usuarioLogado.Cpf));

        [HttpPost("AceitarRecusarConviteAgenda")]
        public async Task<ActionResult<ApiGenericResult<bool>>> AceitarRecusarConviteAgenda(StatusSolicitacaoParticipante decisaoStatus, int agendaId, string codigoColaborador)
            => Ok(await _encontrosService.AceitarRecusarConviteAgenda(decisaoStatus, agendaId, codigoColaborador));

        [HttpPost("SolicitarParticipacaoAgenda")]
        public async Task<ActionResult<ApiGenericResult<bool>>> SolicitarParticipacaoAgenda(int agendaId, string codigoColaboradorCriador)
            => Ok(await _encontrosService.SolicitarParticipacaoAgenda(agendaId, _usuarioLogado.Cpf, codigoColaboradorCriador));

        [HttpPost("ConvidarParticipanteAgenda")]
        public async Task<ActionResult<ApiGenericResult<ConvidarParticipantesAgendaParam>>> ConvidarParticipanteAgenda(ConvidarParticipantesAgendaParam param)
            => Ok(await _encontrosService.ConvidarParticipanteAgenda(param));

        [HttpGet("BuscarAgendasFilhosCompletas")]
        public async Task<ActionResult<ApiGenericResult<List<AgendaEncontroResult>>>> BuscarAgendasFilhosCompletas(string agendaId)
            => Ok(await _encontrosService.BuscarAgendasFilhosCompletas(agendaId, _usuarioLogado.Cpf));

        [HttpGet("BuscarIdAgendasFilhos")]
        public async Task<ActionResult<ApiGenericResult<List<int>>>> BuscarIdAgendasFilhos(string agendaId)
            => Ok(await _encontrosService.BuscarIdAgendasFilhos(agendaId));

        [HttpGet("ListarTodosObjetivos")]
        public async Task<ActionResult<ApiGenericResult<List<AgendaObjetivoDTO>>>> ListarTodosObjetivos()
            => Ok(await _encontrosService.ListarTodosObjetivos());
    }
};