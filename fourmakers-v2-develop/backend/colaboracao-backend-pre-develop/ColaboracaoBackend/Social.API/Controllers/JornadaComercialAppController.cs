using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.SolicitacaoParticipacao;
using DataTransferObject.Domain.Social;
using DataTransferObject.Domain.Usuario;
using DataTransferObject.Domain.Vaga;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Social.Domain.Impl;
using Social.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Social.API.Controllers
{
    [HandleException]
    [Route("api/Social/[controller]")]
    [ApiController]
    [Authorize]
    [LogAction]
    public class JornadaComercialAppController : ControllerBase
    {
        private readonly IJornadaComercialAppService _jornadaComercialAppService;
        UsuarioLogadoDTO _usuarioLogado;

        public JornadaComercialAppController(IJornadaComercialAppService jornadaComercialAppService, IAspNetUser aspNetUser)
        {
            _jornadaComercialAppService = jornadaComercialAppService;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
        }

        [HttpGet("BuscarKanbanEncontrosAcoesComerciais")]
        public async Task<ActionResult<ApiGenericResult<KanbanEncontrosAcoesComerciais>>> BuscarKanbanEncontrosAcoesComerciais(DateTime? dataInicio, DateTime? dataFim, string busca = null)
            => Ok(await _jornadaComercialAppService.BuscarKanbanEncontrosAcoesComerciais(busca, dataInicio, dataFim, _usuarioLogado.OrgId));

        [HttpPost("AtualizarStatusAcoes")]
        public async Task<ActionResult<ApiGenericResult<EncontroAiPassos>>> AtualizarStatusAcoes([FromBody] AtualizarIteracoesAcoesParam p)
            => Ok(await _jornadaComercialAppService.AtualizarStatusAcoes(p));

        [HttpGet("ListarInteracaoAcao")]
        public async Task<ActionResult<ApiGenericResult<EncontroAiPassos>>> ListarInteracaoAcao(int interacaoAcaoId)
            => Ok(await _jornadaComercialAppService.ListarInteracaoAcao(interacaoAcaoId));
        
        [HttpPost("InsercaoComentariosAcoes")]
        public async Task<ActionResult<ApiGenericResult<ComentariosAcoesResponseDTO>>> InsercaoComentariosAcoes([FromBody] ComentariosAcoesParamDTO p)
            => Ok(await _jornadaComercialAppService.InsercaoComentariosAcoes(p, _usuarioLogado));
        
        [HttpGet("ListarComentariosPorInteracao")]
        public async Task<ActionResult<ApiGenericResult<EncontroAiPassos>>> ListarComentariosPorInteracao(int interacaoAcaoId)
            => Ok(await _jornadaComercialAppService.ListarComentariosPorInteracao(interacaoAcaoId));
        
        [HttpPost("InserirInteracaoIA")]
        public async Task<ActionResult<ApiGenericResult<InteracaiAIResponse>>> InserirInteracaoIA([FromBody] EncontroAiParamInclusao param)
           => Ok(await _jornadaComercialAppService.InserirInteracaoIA(param));

        [HttpPost("AtualizarInteracaoIA/{interacaoId}")]
        public async Task<ActionResult<ApiGenericResult<InteracaiAIResponse>>> AtualizarInteracaoIA([FromBody] EncontroAiParamAtualizacao param)
            => Ok(await _jornadaComercialAppService.AtualizarInteracaoIA(param));

        [HttpDelete("DeletarInteracaoIA/{interacaoId}")]
        public async Task<ActionResult<ApiGenericResult<bool>>> deletarEncontro(int interacaoId)
            => Ok(await _jornadaComercialAppService.DeletarInteracaoIA(interacaoId));

        // GET: api/Assunto/ListarCategoriasAssunto
        [HttpGet("ListarCategoriasAssunto")]
        public async Task<ActionResult> ListarCategoriasAssunto()
            => Ok(await _jornadaComercialAppService.ListarCategorias());

        // GET: api/Assunto/ListarSubCategoriasAssunto/5
        [HttpGet("ListarSubCategoriasAssunto/{categoriaId:int}")]
        public async Task<ActionResult> ListarSubCategoriasAssunto(int categoriaId)
            => Ok(await _jornadaComercialAppService.ListarSubcategoriasPorCategoria(categoriaId));

        [HttpGet("ListarCategoriasAssuntoComSub")]
        public async Task<ActionResult> ListarCategoriasAssuntoComSub()
            => Ok(await _jornadaComercialAppService.ListarCategoriasComSub());

        [HttpPost("InsercaoInteracaoCategoria")]
        public async Task<IActionResult> InsercaoInteracaoCategoria([FromBody] CategoriaInsercaoParamDTO param)
            => Ok(await _jornadaComercialAppService.InsercaoInteracaoCategoria(param));

        [HttpPost("AtualizarInteracaoCategoria")]
        public async Task<IActionResult> AtualizarInteracaoCategoria([FromBody] CategoriaAtualizacaoParamDTO param)
            => Ok(await _jornadaComercialAppService.AtualizarInteracaoCategoria(param));
        
        [HttpDelete("DeletarInteracaoCategoria")]
        public async Task<ActionResult<ApiGenericResult<bool>>> DeletarInteracaoCategoria(long id)
            => Ok(await _jornadaComercialAppService.DeletarInteracaoCategoria(id));

        [HttpGet("FiltroAgendasInteracoesCategoriaSub")]
        public async Task<IActionResult> FiltroAgendasInteracoesCategoriaSub(
                                                [FromQuery] string nomeCliente = null,
                                                [FromQuery] DateTime? dataAgendadaInicio = null,
                                                [FromQuery] DateTime? dataAgendadaFim = null,
                                                [FromQuery] int? categoriaId = null,
                                                [FromQuery] int? subCategoriaId = null,
                                                [FromQuery] int pagina = 1,
                                                [FromQuery] int limite = 10)
        {
            var result = await _jornadaComercialAppService.FiltroAgendasInteracoesCategoriaSub(
                nomeCliente, dataAgendadaInicio, dataAgendadaFim, categoriaId, subCategoriaId, pagina, limite);

            return Ok(result);
        }

        [HttpPost("BuscarProximosPassosIntegracaoMoxe")]
        public async Task<ActionResult<ApiGenericResult<IntegracaoMoxeResponseDTO>>> BuscarProximosPassosIntegracaoMoxe([FromBody] IntegracaoMoxeRequestDTO request)
            => Ok(await _jornadaComercialAppService.BuscarProximosPassosIntegracaoMoxe(request));

        [HttpPost("SolicitarParticiparAgenda")]
        public async Task<ActionResult<ApiGenericResult<AgendaSolicitanteDTO>>> SolicitarParticiparAgenda(int agendaId, string codigoColaboradorCriador)
            => Ok(await _jornadaComercialAppService.SolicitarParticiparAgenda(agendaId, _usuarioLogado.Cpf, codigoColaboradorCriador));

        [HttpGet("ObterAgendaSolicitante/{id:int}")]
        public async Task<ActionResult<ApiGenericResult<AgendaSolicitanteDTO>>> ObterAgendaSolicitantePorId(int id)
            => Ok(await _jornadaComercialAppService.ObterAgendaSolicitantePorId(id));

        [HttpGet("ListarAgendaSolicitantesPorAgenda")]
        public async Task<ActionResult<ApiGenericResult<List<AgendaSolicitanteDTO>>>> ListarAgendaSolicitantesPorAgendaComercial([FromQuery] int tbAgendasComerciaisId)
            => Ok(await _jornadaComercialAppService.ListarAgendaSolicitantesPorAgendaComercial(tbAgendasComerciaisId));

        [HttpPut("AceitarRecusarSolicitanteNaAgenda")]
        public async Task<ActionResult<ApiGenericResult<AgendaSolicitanteDTO>>> AceitarRecusarSolicitanteNaAgenda([FromBody] AgendaSolicitanteAtualizacaoDTO param)
            => Ok(await _jornadaComercialAppService.AceitarRecusarSolicitanteNaAgenda(param));

        [HttpDelete("ExcluirAgendaSolicitante")]
        public async Task<ActionResult<ApiGenericResult<bool>>> ExcluirAgendaSolicitante(
            [FromQuery] int tbAgendasComerciaisId,
            [FromQuery] string codigoColaboradorExterno)
            => Ok(await _jornadaComercialAppService.ExcluirAgendaSolicitante(tbAgendasComerciaisId, codigoColaboradorExterno));

        [HttpPost("ConvidarParaAgenda")]
        public async Task<ActionResult<ApiGenericResult<List<AgendaConvidadoDTO>>>> ConvidarParaAgenda([FromBody] ConvidarParaAgendaRequestDTO param)
            => Ok(await _jornadaComercialAppService.ConvidarParaAgenda(param));

        [HttpGet("ObterAgendaConvidado/{id:int}")]
        public async Task<ActionResult<ApiGenericResult<AgendaConvidadoDTO>>> ObterAgendaConvidadoPorId(int id)
            => Ok(await _jornadaComercialAppService.ObterAgendaConvidadoPorId(id));

        [HttpGet("ListarAgendaConvidadosPorAgenda")]
        public async Task<ActionResult<ApiGenericResult<List<AgendaConvidadoDTO>>>> ListarAgendaConvidadosPorAgendaComercial([FromQuery] int tbAgendasComerciaisId)
            => Ok(await _jornadaComercialAppService.ListarAgendaConvidadosPorAgendaComercial(tbAgendasComerciaisId));

        [HttpPut("AceitarRecusarConvidadoNaAgenda")]
        public async Task<ActionResult<ApiGenericResult<AgendaConvidadoDTO>>> AceitarRecusarConvidadoNaAgenda([FromBody] AgendaConvidadoAtualizacaoDTO param)
            => Ok(await _jornadaComercialAppService.AceitarRecusarConvidadoNaAgenda(param));

        [HttpDelete("ExcluirAgendaConvidado")]
        public async Task<ActionResult<ApiGenericResult<bool>>> ExcluirAgendaConvidado(
            [FromQuery] int tbAgendasComerciaisId,
            [FromQuery] string codigoColaboradorExterno)
            => Ok(await _jornadaComercialAppService.ExcluirAgendaConvidado(tbAgendasComerciaisId, codigoColaboradorExterno));
    }
}