using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.SRS.AdmissaoHistoricoStatus;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Admissao.Domain.Interfaces.Service;

namespace Admissao.API.Controllers;

/// <summary>
/// Consulta da timeline de movimentações de status (<c>tb_admissao_historico_status</c>).
/// Escrita é feita exclusivamente via <c>POST /Admissao/{id}/MoverStatus</c>.
/// </summary>
[Authorize]
[Route("api/Admissao/HistoricoStatus")]
[ApiController]
[LogAction]
public class AdmissaoHistoricoStatusController : ControllerBase
{
    private readonly IAdmissaoHistoricoStatusService _service;
    private readonly IAspNetUser _aspNetUser;

    public AdmissaoHistoricoStatusController(IAdmissaoHistoricoStatusService service, IAspNetUser aspNetUser)
    {
        _service = service;
        _aspNetUser = aspNetUser;
    }

    /// <summary>
    /// Lista o histórico de movimentações de uma admissão específica.
    /// Requer ADMISSAO_LISTAR.
    /// </summary>
    [HttpGet("PorAdmissao/{admissaoId:guid}")]
    public async Task<ActionResult<ApiGenericResult<List<AdmissaoHistoricoStatusResult>>>> ListarPorAdmissao(
        Guid admissaoId,
        [FromQuery] int? cursor = null,
        [FromQuery] int? limite = null)
    {
        var usuario = _aspNetUser.GetUsuarioLogado();
        var result = await _service.ListarPorAdmissaoAsync(usuario.Cpf, usuario.OrgId, admissaoId, cursor, limite);
        return Ok(result);
    }

    /// <summary>
    /// Obtém um registro de histórico de status por ID. Requer ADMISSAO_LISTAR.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiGenericResult<AdmissaoHistoricoStatusResult>>> ObterPorId(Guid id)
    {
        var usuario = _aspNetUser.GetUsuarioLogado();
        var result = await _service.ObterPorIdAsync(usuario.Cpf, usuario.OrgId, id);
        if (!result.Sucesso && result.Retorno == null)
            return NotFound(result);
        return Ok(result);
    }
}
