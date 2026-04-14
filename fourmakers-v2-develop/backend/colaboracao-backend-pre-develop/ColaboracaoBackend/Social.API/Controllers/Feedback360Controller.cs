using System;
using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Log;
using DataTransferObject.Domain.Social;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Social.Domain.Interfaces;

namespace Social.API.Controllers
{
    [Route("api/Social/[controller]")]
    [ApiController]
    [HandleException]
    [Authorize]
    [LogAction]
    public class Feedback360Controller : ControllerBase
    {
        private readonly IFeedback360Service _feedback360Service;
        private readonly IAspNetUser _aspNetUser;
        private readonly ILogCore _log;

        public Feedback360Controller(IFeedback360Service feedback360Service, IAspNetUser aspNetUser, ILogCore log)
        {
            _feedback360Service = feedback360Service;
            _aspNetUser = aspNetUser;
            _log = log;
        }

        /// <summary>
        /// Envia um feedback 360 para um colaborador (destinatário) usando estrutura STAR.
        /// Regras: situação, tarefa, ação e resultado obrigatórios; data de interação não pode ser futura; apenas usuários autenticados na org.
        /// </summary>
        [HttpPost("CriarFeedback")]
        [ProducesResponseType(typeof(ApiGenericResult<Feedback360DTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiGenericResult<Feedback360DTO>>> EnviarFeedback([FromBody] EnviarFeedback360DTO dto)
        {
            var result = new ApiGenericResult<Feedback360DTO>();
            try
            {
                var usuario = _aspNetUser.GetUsuarioLogado();
                result.Retorno = await _feedback360Service.EnviarFeedbackAsync(dto, usuario);
                result.Sucesso = true;
                result.Mensagem = "Feedback registrado com sucesso.";
                return StatusCode(StatusCodes.Status201Created, result);
            }
            catch (UnauthorizedAccessException e)
            {
                _log.Log($"[{GetType().Name}] {e.GetType().Name}: {e.Message}", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception e) when (e is ArgumentException || e is ApplicationException || e is InvalidOperationException)
            {
                _log.Log($"[{GetType().Name}] {e.GetType().Name}: {e.Message}", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return BadRequest(result);
            }
            catch (Exception e)
            {
                _log.Log($"[{GetType().Name}] {e.GetType().Name}: {e.Message}", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        /// <summary>
        /// Atualiza um feedback 360 (estrutura STAR). Permitido apenas na primeira 1h após a criação e somente pelo autor.
        /// </summary>
        [HttpPut("AtualizarFeedback")]
        [ProducesResponseType(typeof(ApiGenericResult<Feedback360DTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiGenericResult<Feedback360DTO>>> AtualizarFeedback([FromQuery] Guid id, [FromBody] AtualizarFeedback360DTO dto)
        {
            var result = new ApiGenericResult<Feedback360DTO>();
            try
            {
                if (id == Guid.Empty)
                {
                    result.Sucesso = false;
                    result.Mensagem = "Id do feedback é obrigatório.";
                    return BadRequest(result);
                }

                var usuario = _aspNetUser.GetUsuarioLogado();
                result.Retorno = await _feedback360Service.AtualizarFeedbackAsync(id, dto, usuario);
                result.Sucesso = true;
                result.Mensagem = "Feedback atualizado com sucesso.";
                return Ok(result);
            }
            catch (UnauthorizedAccessException e)
            {
                _log.Log($"[{GetType().Name}] {e.GetType().Name}: {e.Message}", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception e) when (e is ArgumentException || e is ApplicationException || e is InvalidOperationException)
            {
                _log.Log($"[{GetType().Name}] {e.GetType().Name}: {e.Message}", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return BadRequest(result);
            }
            catch (Exception e)
            {
                _log.Log($"[{GetType().Name}] {e.GetType().Name}: {e.Message}", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        /// <summary>
        /// Gera modelo STAR do feedback 360 com IA (API Moxe). Recebe situação (onde ocorreu), data da interação e contexto (o que ocorreu, impacto, ação). Retorna JSON com STAR (situacao, tarefa, acao, resultado) e previa (parágrafo fluido para exibição).
        /// </summary>
        [HttpPost("GerarModeloStarMoxe")]
        [ProducesResponseType(typeof(ApiGenericResult<GerarModeloStarMoxeResultadoDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiGenericResult<GerarModeloStarMoxeResultadoDTO>>> GerarModeloStarMoxe([FromBody] GerarModeloStarMoxeDTO dto)
        {
            var result = new ApiGenericResult<GerarModeloStarMoxeResultadoDTO>();
            try
            {
                var usuario = _aspNetUser.GetUsuarioLogado();
                result.Retorno = await _feedback360Service.GerarModeloStarMoxeAsync(dto, usuario);
                result.Sucesso = true;
                result.Mensagem = "Prévia gerada com sucesso.";
                return Ok(result);
            }
            catch (UnauthorizedAccessException e)
            {
                _log.Log($"[{GetType().Name}] {e.GetType().Name}: {e.Message}", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception e) when (e is ArgumentException || e is ApplicationException || e is InvalidOperationException)
            {
                _log.Log($"[{GetType().Name}] {e.GetType().Name}: {e.Message}", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return BadRequest(result);
            }
            catch (Exception e)
            {
                _log.Log($"[{GetType().Name}] {e.GetType().Name}: {e.Message}", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        /// <summary>
        /// Obtém um feedback 360 por id.
        /// </summary>
        [HttpGet("BuscarFeedbackPorId")]
        [ProducesResponseType(typeof(ApiGenericResult<Feedback360DTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiGenericResult<Feedback360DTO>>> GetById([FromQuery] Guid id)
        {
            var result = new ApiGenericResult<Feedback360DTO>();
            try
            {
                if (id == Guid.Empty)
                {
                    result.Sucesso = false;
                    result.Mensagem = "Id do feedback é obrigatório.";
                    return BadRequest(result);
                }

                result.Retorno = await _feedback360Service.GetByIdAsync(id);
                if (result.Retorno == null)
                {
                    result.Sucesso = false;
                    result.Mensagem = "Feedback não encontrado.";
                    return NotFound(result);
                }
                result.Sucesso = true;
                return Ok(result);
            }
            catch (UnauthorizedAccessException e)
            {
                _log.Log($"[{GetType().Name}] {e.GetType().Name}: {e.Message}", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception e) when (e is ArgumentException || e is ApplicationException || e is InvalidOperationException)
            {
                _log.Log($"[{GetType().Name}] {e.GetType().Name}: {e.Message}", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return BadRequest(result);
            }
            catch (Exception e)
            {
                _log.Log($"[{GetType().Name}] {e.GetType().Name}: {e.Message}", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        /// <summary>
        /// Lista os níveis de relacionamento (Gestor, Colega, etc.) para o campo de relacionamento.
        /// </summary>
        [HttpGet("ListarRelacionamentos")]
        [ProducesResponseType(typeof(ApiGenericResult<IEnumerable<Feedback360RelacionamentoDTO>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiGenericResult<IEnumerable<Feedback360RelacionamentoDTO>>>> ListarRelacionamentos()
        {
            var result = new ApiGenericResult<IEnumerable<Feedback360RelacionamentoDTO>>();
            try
            {
                result.Retorno = await _feedback360Service.ListarRelacionamentosAsync();
                result.Sucesso = true;
                return Ok(result);
            }
            catch (UnauthorizedAccessException e)
            {
                _log.Log($"[{GetType().Name}] {e.GetType().Name}: {e.Message}", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception e) when (e is ArgumentException || e is ApplicationException || e is InvalidOperationException)
            {
                _log.Log($"[{GetType().Name}] {e.GetType().Name}: {e.Message}", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return BadRequest(result);
            }
            catch (Exception e)
            {
                _log.Log($"[{GetType().Name}] {e.GetType().Name}: {e.Message}", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        /// <summary>
        /// Lista todos os feedbacks 360 enviados pelo usuário logado.
        /// Filtros opcionais via query: dataInicio, dataFim, sentimentoId (1–5), relacionamentoId.
        /// </summary>
        [HttpGet("ListarEnviados")]
        [ProducesResponseType(typeof(ApiGenericResult<IEnumerable<Feedback360DTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiGenericResult<IEnumerable<Feedback360DTO>>>> ListarEnviados(
            [FromQuery] DateTime? dataInicio,
            [FromQuery] DateTime? dataFim,
            [FromQuery] int? sentimentoId,
            [FromQuery] int? relacionamentoId,
            [FromQuery] int limit = 10,
            [FromQuery] int cursor = 0)
        {
            var result = new ApiGenericResult<IEnumerable<Feedback360DTO>>();
            try
            {
                var usuario = _aspNetUser.GetUsuarioLogado();
                var filtro = new FiltroFeedback360DTO
                {
                    DataInicio = dataInicio,
                    DataFim = dataFim,
                    SentimentoId = sentimentoId,
                    RelacionamentoId = relacionamentoId,
                    Limit = limit,
                    Cursor = cursor
                };
                result.Retorno = await _feedback360Service.ListarEnviadosAsync(usuario, filtro);
                result.Sucesso = true;
                return Ok(result);
            }
            catch (UnauthorizedAccessException e)
            {
                _log.Log($"[{GetType().Name}] {e.GetType().Name}: {e.Message}", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception e) when (e is ArgumentException || e is ApplicationException || e is InvalidOperationException)
            {
                _log.Log($"[{GetType().Name}] {e.GetType().Name}: {e.Message}", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return BadRequest(result);
            }
            catch (Exception e)
            {
                _log.Log($"[{GetType().Name}] {e.GetType().Name}: {e.Message}", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        /// <summary>
        /// Lista todos os feedbacks 360 recebidos pelo usuário logado (quantidade total e lista).
        /// Filtros opcionais via query: dataInicio, dataFim, sentimentoId (1–5), relacionamentoId.
        /// </summary>
        [HttpGet("ListarRecebidos")]
        [ProducesResponseType(typeof(ApiGenericResult<Feedback360RecebidosResultadoDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiGenericResult<Feedback360RecebidosResultadoDTO>>> ListarRecebidos(
            [FromQuery] DateTime? dataInicio,
            [FromQuery] DateTime? dataFim,
            [FromQuery] int? sentimentoId,
            [FromQuery] int? relacionamentoId,
            [FromQuery] int limit = 10,
            [FromQuery] int cursor = 0)
        {
            var result = new ApiGenericResult<Feedback360RecebidosResultadoDTO>();
            try
            {
                var usuario = _aspNetUser.GetUsuarioLogado();
                var filtro = new FiltroFeedback360DTO
                {
                    DataInicio = dataInicio,
                    DataFim = dataFim,
                    SentimentoId = sentimentoId,
                    RelacionamentoId = relacionamentoId,
                    Limit = limit,
                    Cursor = cursor
                };
                result.Retorno = await _feedback360Service.ListarRecebidosAsync(usuario, filtro);
                result.Sucesso = true;
                return Ok(result);
            }
            catch (UnauthorizedAccessException e)
            {
                _log.Log($"[{GetType().Name}] {e.GetType().Name}: {e.Message}", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception e) when (e is ArgumentException || e is ApplicationException || e is InvalidOperationException)
            {
                _log.Log($"[{GetType().Name}] {e.GetType().Name}: {e.Message}", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return BadRequest(result);
            }
            catch (Exception e)
            {
                _log.Log($"[{GetType().Name}] {e.GetType().Name}: {e.Message}", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        /// <summary>
        /// [Gestor] Lista os colaboradores subordinados ao gestor logado (código e nome) para uso como filtro.
        /// </summary>
        [HttpGet("Gestor/ListagemColaboradoresFiltroGestor")]
        [ProducesResponseType(typeof(ApiGenericResult<ListagemColaboradoresGestorFeedbackDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiGenericResult<ListagemColaboradoresGestorFeedbackDTO>>> ListagemColaboradoresFiltroGestor()
        {
            var result = new ApiGenericResult<ListagemColaboradoresGestorFeedbackDTO>();
            try
            {
                var usuario = _aspNetUser.GetUsuarioLogado();
                result.Retorno = await _feedback360Service.ListarColaboradoresGestorAsync(Guid.Parse(usuario.Cpf.Trim()), usuario.OrgId);
                result.Sucesso = true;
                return Ok(result);
            }
            catch (UnauthorizedAccessException e)
            {
                _log.Log($"[{GetType().Name}] {e.GetType().Name}: {e.Message}", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception e) when (e is ArgumentException || e is ApplicationException || e is InvalidOperationException)
            {
                _log.Log($"[{GetType().Name}] {e.GetType().Name}: {e.Message}", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return BadRequest(result);
            }
            catch (Exception e)
            {
                _log.Log($"[{GetType().Name}] {e.GetType().Name}: {e.Message}", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        /// <summary>
        /// [Gestor] Lista os feedbacks 360 recebidos por um colaborador da equipe (quantidade total e lista).
        /// Filtros opcionais: data início/fim (padrão: últimos 6 meses) e nota da avaliação (1 a 5).
        /// </summary>
        [HttpPost("Gestor/ListarRecebidos")]
        [ProducesResponseType(typeof(ApiGenericResult<FeedbackGestorResultadoDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiGenericResult<FeedbackGestorResultadoDTO>>> ListarRecebidosGestor([FromBody] FiltroFeedbackGestorDTO filtro)
        {
            var result = new ApiGenericResult<FeedbackGestorResultadoDTO>();
            try
            {
                result.Retorno = await _feedback360Service.ListarRecebidosGestorAsync(filtro);
                result.Sucesso = true;
                return Ok(result);
            }
            catch (UnauthorizedAccessException e)
            {
                _log.Log($"[{GetType().Name}] {e.GetType().Name}: {e.Message}", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception e) when (e is ArgumentException || e is ApplicationException || e is InvalidOperationException)
            {
                _log.Log($"[{GetType().Name}] {e.GetType().Name}: {e.Message}", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return BadRequest(result);
            }
            catch (Exception e)
            {
                _log.Log($"[{GetType().Name}] {e.GetType().Name}: {e.Message}", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        /// <summary>
        /// Lista as opções de avaliação (1 a 5: Precisa evoluir até Referência para os demais).
        /// </summary>
        [HttpGet("ListarAvaliacoes")]
        [ProducesResponseType(typeof(ApiGenericResult<IEnumerable<Feedback360AvaliacaoDTO>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiGenericResult<IEnumerable<Feedback360AvaliacaoDTO>>>> ListarAvaliacoes()
        {
            var result = new ApiGenericResult<IEnumerable<Feedback360AvaliacaoDTO>>();
            try
            {
                result.Retorno = await _feedback360Service.ListarAvaliacoesAsync();
                result.Sucesso = true;
                return Ok(result);
            }
            catch (UnauthorizedAccessException e)
            {
                _log.Log($"[{GetType().Name}] {e.GetType().Name}: {e.Message}", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception e) when (e is ArgumentException || e is ApplicationException || e is InvalidOperationException)
            {
                _log.Log($"[{GetType().Name}] {e.GetType().Name}: {e.Message}", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return BadRequest(result);
            }
            catch (Exception e)
            {
                _log.Log($"[{GetType().Name}] {e.GetType().Name}: {e.Message}", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        /// <summary>
        /// Busca o mural de reconhecimento: feedbacks da organização em cards com reações (emojis + contagem). Cada card inclui se o usuário logado já reagiu.
        /// Filtros opcionais: dataInicio, dataFim, sentimentoId (1–5), relacionamentoId, busca (nome remetente/destinatário, situação, ação, prévia), limit (padrão 10), cursor (padrão 0).
        /// </summary>
        [HttpGet("BuscarMuralReconhecimento")]
        [ProducesResponseType(typeof(ApiGenericResult<IEnumerable<Feedback360MuralReconhecimentoDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiGenericResult<IEnumerable<Feedback360MuralReconhecimentoDTO>>>> BuscarMuralReconhecimento(
            [FromQuery] DateTime? dataInicio,
            [FromQuery] DateTime? dataFim,
            [FromQuery] int? sentimentoId,
            [FromQuery] int? relacionamentoId,
            [FromQuery] string busca = "",
            [FromQuery] int limit = 10,
            [FromQuery] int cursor = 0)
        {
            var result = new ApiGenericResult<IEnumerable<Feedback360MuralReconhecimentoDTO>>();
            try
            {
                var usuario = _aspNetUser.GetUsuarioLogado();
                var filtro = new FiltroFeedback360DTO
                {
                    DataInicio = dataInicio,
                    DataFim = dataFim,
                    SentimentoId = sentimentoId,
                    RelacionamentoId = relacionamentoId,
                    Busca = string.IsNullOrWhiteSpace(busca) ? null : busca.Trim(),
                    Limit = limit,
                    Cursor = cursor
                };
                result.Retorno = await _feedback360Service.BuscarMuralReconhecimentoAsync(usuario, filtro);
                result.Sucesso = true;
                return Ok(result);
            }
            catch (UnauthorizedAccessException e)
            {
                _log.Log($"[{GetType().Name}] {e.GetType().Name}: {e.Message}", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception e) when (e is ArgumentException || e is ApplicationException || e is InvalidOperationException)
            {
                _log.Log($"[{GetType().Name}] {e.GetType().Name}: {e.Message}", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return BadRequest(result);
            }
            catch (Exception e)
            {
                _log.Log($"[{GetType().Name}] {e.GetType().Name}: {e.Message}", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        /// <summary>
        /// Lista as opções de reação (emojis) ativas para o mural de reconhecimento.
        /// </summary>
        [HttpGet("ListarReacoesMural")]
        [ProducesResponseType(typeof(ApiGenericResult<IEnumerable<Feedback360MuralReacaoDTO>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiGenericResult<IEnumerable<Feedback360MuralReacaoDTO>>>> ListarReacoesMural()
        {
            var result = new ApiGenericResult<IEnumerable<Feedback360MuralReacaoDTO>>();
            try
            {
                result.Retorno = await _feedback360Service.ListarReacoesMuralAsync();
                result.Sucesso = true;
                return Ok(result);
            }
            catch (Exception e)
            {
                _log.Log($"[{GetType().Name}] {e.GetType().Name}: {e.Message}", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        /// <summary>
        /// Define a reação do usuário no card do mural (estado final desejado).
        /// ReacaoId = 0 remove a reação; ReacaoId &gt; 0 define ou substitui pela reação (apenas uma por feedback por colaborador).
        /// Retorna o reacaoId atual após a operação (0 = nenhuma reação).
        /// </summary>
        [HttpPost("ToggleReacaoMural")]
        [ProducesResponseType(typeof(ApiGenericResult<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiGenericResult<int>>> ToggleReacaoMural([FromBody] ToggleReacaoMuralDTO dto)
        {
            var result = new ApiGenericResult<int>();
            try
            {
                var usuario = _aspNetUser.GetUsuarioLogado();
                result.Retorno = await _feedback360Service.DefinirReacaoMuralAsync(dto.Feedback360Id, dto.ReacaoId, usuario);
                result.Sucesso = true;
                result.Mensagem = result.Retorno > 0 ? "Reação definida." : "Reação removida.";
                return Ok(result);
            }
            catch (UnauthorizedAccessException e)
            {
                _log.Log($"[{GetType().Name}] {e.GetType().Name}: {e.Message}", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception e) when (e is ArgumentException || e is ApplicationException || e is InvalidOperationException)
            {
                _log.Log($"[{GetType().Name}] {e.GetType().Name}: {e.Message}", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return BadRequest(result);
            }
            catch (Exception e)
            {
                _log.Log($"[{GetType().Name}] {e.GetType().Name}: {e.Message}", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }
    }
}
