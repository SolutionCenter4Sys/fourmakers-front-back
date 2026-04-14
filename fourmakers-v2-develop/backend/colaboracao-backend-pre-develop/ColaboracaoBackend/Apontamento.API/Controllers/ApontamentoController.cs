using Apontamento.Domain.Interfaces.Service;
using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using DataTransferObject.Domain.Apontamento;
using DataTransferObject.Domain.Apontamento.ColaboradorEApontamento;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Usuario;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Apontamento.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [LogAction]
    public class ApontamentoController : ControllerBase
    {
        private readonly IApontamentoService _apontamentoService;
        private readonly UsuarioLogadoDTO _usuarioLogado;

        private IStringLocalizer<FourmakersCoreMessage> _stringLocalizerFourmakersCore;
        private IStringLocalizer<ApontamentoMessage> _stringLocalizerApontamento;

        public ApontamentoController(IApontamentoService apontamentoService,
                                     IAspNetUser aspNetUser,
                                     IStringLocalizer<FourmakersCoreMessage> stringLocalizerFourmakersCore,
                                     IStringLocalizer<ApontamentoMessage> stringLocalizerApontamento)
        {
            _apontamentoService = apontamentoService;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();

            _stringLocalizerFourmakersCore = stringLocalizerFourmakersCore;
            _stringLocalizerApontamento = stringLocalizerApontamento;
        }

        [HttpGet("ListarTemplateSemanaVigencia")]
        public ActionResult<TemplateSemanaVigenciaResult> ListarTemplateSemanaVigencia(int mes, int ano, int? diaQuebraSemana)
        {
            var result = new StatusResult();

            try
            {
                TemplateSemanaVigenciaResult resultTemplate = _apontamentoService.ObterTemplateSemanaVigencia(mes, ano, diaQuebraSemana ?? 0, _usuarioLogado.OrgId);
                return Ok(resultTemplate);
            }
            catch (Exception e)
            {
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        [HttpGet("ListarProjetosComAtividades")]
        public async Task<ActionResult<ListaProjetoAtividadeResult>> ListarProjetosComAtividades(string cpfColaborador)
        {
            var listaProjetoAtividadeResult = new ListaProjetoAtividadeResult();

            try
            {
                listaProjetoAtividadeResult.ProjetosAtividades = await _apontamentoService.ListarProjetosComAtividadesPorColaborador(
                    _usuarioLogado.Cpf,
                    _usuarioLogado.OrgId,
                    cpfColaborador);

                listaProjetoAtividadeResult.Sucesso = true;
                listaProjetoAtividadeResult.Mensagem = _stringLocalizerFourmakersCore.GetStringOuVazio("QUERY_COMPLETED_SUCCESSFULLY");

                return StatusCode(200, listaProjetoAtividadeResult);
            }
            catch (Exception e)
            {
                listaProjetoAtividadeResult.Sucesso = false;
                listaProjetoAtividadeResult.Mensagem = e.Message;

                return StatusCode(500, listaProjetoAtividadeResult);
            }
        }

        [HttpGet("ListarApontamentosPorVigencia/{mes}/{ano}")]
        public async Task<ActionResult<ApontamentosPorVigenciaResult>> ListarApontamentosPorVigencia([FromRoute] int mes, [FromRoute] int ano, string cpfColaborador)
        {
            var apontamentosPorVigenciaResult = new ApontamentosPorVigenciaResult();

            try
            {
                apontamentosPorVigenciaResult.ApontamentosPorVigencia = await _apontamentoService.ListarApontamentosPorVigencia(
                    mes,
                    ano,
                    _usuarioLogado.Cpf,
                    _usuarioLogado.CodColaborador,
                    _usuarioLogado.OrgId,
                    cpfColaborador);

                apontamentosPorVigenciaResult.Sucesso = true;
                apontamentosPorVigenciaResult.Mensagem = _stringLocalizerFourmakersCore.GetStringOuVazio("QUERY_COMPLETED_SUCCESSFULLY");

                return StatusCode(200, apontamentosPorVigenciaResult);
            }
            catch (UnauthorizedAccessException e)
            {
                var result = new ApiGenericResult();
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception e)
            {
                apontamentosPorVigenciaResult.Sucesso = false;
                apontamentosPorVigenciaResult.Mensagem = e.Message;

                return StatusCode(500, apontamentosPorVigenciaResult);
            }
        }

        [HttpGet("ListarApontamentosPorVigenciaRelacionadosAoGerente/{cpfColaborador}/{mes}/{ano}")]
        public async Task<ActionResult<ApontamentosPorVigenciaResult>> ListarApontamentosPorVigenciaRelacionadosAoGerente([FromRoute] string cpfColaborador, int mes, [FromRoute] int ano)
        {
            var apontamentosPorVigenciaResult = new ApontamentosPorVigenciaResult();

            try
            {
                apontamentosPorVigenciaResult.ApontamentosPorVigencia = await _apontamentoService.ListarApontamentosVigenciaPorGerente(
                    mes,
                    ano,
                    cpfColaborador,
                    cpfColaborador,
                    _usuarioLogado.OrgId,
                    _usuarioLogado.Cpf);

                apontamentosPorVigenciaResult.Sucesso = true;
                apontamentosPorVigenciaResult.Mensagem = _stringLocalizerFourmakersCore.GetStringOuVazio("QUERY_COMPLETED_SUCCESSFULLY");

                return StatusCode(200, apontamentosPorVigenciaResult);
            }
            catch (Exception e)
            {
                apontamentosPorVigenciaResult.Sucesso = false;
                apontamentosPorVigenciaResult.Mensagem = e.Message;

                return StatusCode(500, apontamentosPorVigenciaResult);
            }
        }

        [HttpGet("ListarStatus")]
        public async Task<ActionResult<List<StatusApontamentoGrupoDTO>>> ListarGrupoStatus()
        {
            var result = new StatusResult();
            try
            {
                List<StatusApontamentoGrupoDTO> listGrupos = await _apontamentoService.ObterListaStatusGrupo();
                return Ok(listGrupos);
            }
            catch (Exception e)
            {
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        [HttpGet("ListarProjetosGerenteDeProjeto")]
        public async Task<ActionResult<List<ProjetoGerenteResult>>> ListarProjetosGerenteDeProjetos()
        {
            var result = new StatusResult();

            try
            {
                var projetosGerente = await _apontamentoService.ListarProjetosGerenteDeProjetos(
                    _usuarioLogado.Cpf,
                    _usuarioLogado.OrgId);

                return StatusCode(200, projetosGerente);
            }
            catch (Exception e)
            {
                result.Sucesso = false;
                result.Mensagem = e.Message;

                return StatusCode(500, result);
            }
        }

        [HttpGet("ListarVigenciasApontamentosGerenteDeProjeto")]
        public async Task<ActionResult<ListarVigenciaResult>> ListarVigenciasApontamentosGerenteDeProjeto()
        {
            var result = new StatusResult();

            try
            {
                ListarVigenciaResult listarVigenciaResult = await _apontamentoService.ListarVigenciasProjetosGerente(
                    _usuarioLogado.Cpf,
                    _usuarioLogado.OrgId);

                return StatusCode(200, listarVigenciaResult);
            }
            catch (Exception e)
            {
                result.Sucesso = false;
                result.Mensagem = e.Message;

                return StatusCode(500, result);
            }
        }

        [HttpGet("ListarVigenciasColaborador")]
        public async Task<ActionResult<ListarVigenciaResult>> ListarVigenciasColaborador(string cpfColaborador)
        {
            var result = new StatusResult();

            try
            {
                ListarVigenciaResult listarVigenciaResult = await _apontamentoService.ListarVigenciasColaborador(
                    _usuarioLogado.Cpf,
                    _usuarioLogado.OrgId,
                    cpfColaborador);

                return StatusCode(200, listarVigenciaResult);
            }
            catch (UnauthorizedAccessException e)
            {
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception e)
            {
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        [HttpGet("ListarColaboradoresVinculadosGerenteDeProjeto")]
        public async Task<ActionResult<IEnumerable<ColaboradoresVinculadosGerenteDTO>>> ListarColaboradoresVinculadosGerenteDeProjeto(string codProjeto, int mes, int ano)
        {
            try
            {
                var projetosGerente = await _apontamentoService.ListarColaboradoresVinculadosGerenteDeProjetoComApontamentos(mes, ano, codProjeto, _usuarioLogado.Cpf, _usuarioLogado.OrgId);

                return Ok(projetosGerente);
            }
            catch (Exception e)
            {
                var result = new StatusResult();
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return BadRequest(result);
            }
        }

        [HttpGet("ListarGerenteAdmDosColabsGP")]
        public async Task<ActionResult<ApiGenericResult<IEnumerable<ColaboradoresVinculadosGerenteDTO>>>> ListarGerenteAdmDosColabsGP()
        {
            try
            {
                var ret = new ApiGenericResult<IEnumerable<ColaboradoresVinculadosGerenteDTO>>();
                ret.Retorno = await _apontamentoService.ListarGerentesAdministrativosDosColaboradoresVinculadosGerenteProjeto(_usuarioLogado.Cpf, _usuarioLogado.OrgId);
                return Ok(ret);
            }
            catch (Exception e)
            {
                var result = new StatusResult();
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return BadRequest(result);
            }
        }

        [HttpGet("ListarProjetosVisaoGerenteDeProjeto")]
        public async Task<ActionResult<ApiGenericResult<ListaComTotalizadorHorasResult<ProjetoVisaoGerenteDTO>>>> ListarProjetosVisaoGerenteProjeto(string codProjeto, int mes, int ano, string cpfColaborador, int codStatusGrupo, string cpfGerenteAdm)
        {
            try
            {
                var ret = new ApiGenericResult<ListaComTotalizadorHorasResult<ProjetoVisaoGerenteDTO>>
                {
                    Retorno = await _apontamentoService.ListarProjetosVisaoGerenteDeProjeto(codProjeto, mes, ano, cpfColaborador, codStatusGrupo, _usuarioLogado.Cpf, _usuarioLogado.OrgId, cpfGerenteAdm)
                };
                return Ok(ret);
            }
            catch (Exception e)
            {
                var result = new StatusResult();
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return BadRequest(result);
            }
        }

        [HttpPost("ApontarHorasEmLote")]
        public async Task<ActionResult<ColaboradorApontamentoDTO>> ApontarHorasEmLote([FromBody] ApontarHorasEmLoteDTO apontarHorasDTO)
        {
            var result = new StatusResult();
            try
            {
                var retorno = await _apontamentoService.ApontarHorasEmLote(_usuarioLogado.Cpf, _usuarioLogado.OrgId, apontarHorasDTO.ProjetoId, apontarHorasDTO.AtividadeId, apontarHorasDTO.Horas, apontarHorasDTO.DataInicio, apontarHorasDTO.DataFim, apontarHorasDTO.DiaQuebraSemana ?? 0, apontarHorasDTO.DeveSomarApontamentoDia ?? false, apontarHorasDTO.IncluirSabado, apontarHorasDTO.IncluirDomingo, apontarHorasDTO.IncluirFeriado, apontarHorasDTO.cpfColaborador, apontarHorasDTO.Observacao);
                result.Sucesso = retorno.Sucesso;
                return Ok(result);
            }
            catch (ArgumentException e)
            {
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return BadRequest(result);
            }
            catch (Exception e)
            {
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        [HttpPost("ApontarHoras")]
        public async Task<ActionResult<ColaboradorApontamentoDTO>> ApontarHoras([FromBody] ApontarHorasDTO apontarHorasDTO)
        {
            var result = new StatusResult();
            try
            {
                var apontamento = await _apontamentoService.ApontarHoras(_usuarioLogado.Cpf, _usuarioLogado.OrgId, apontarHorasDTO.ProjetoId, apontarHorasDTO.AtividadeId, apontarHorasDTO.Horas, apontarHorasDTO.DataRegistro, apontarHorasDTO.DiaQuebraSemana ?? 0, apontarHorasDTO.DeveSomarApontamentoDia ?? false, apontarHorasDTO.cpfColaborador, apontarHorasDTO.Observacao);
                return Ok(apontamento);
            }
            catch (ArgumentException e)
            {
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return BadRequest(result);
            }
            catch (Exception e)
            {
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        [HttpPost("AprovarApontamentoEmLoteGerenteDeProjeto")]
        public async Task<ActionResult<StatusResult>> AprovarApontamentoEmLoteGestorProjeto([FromBody] AprovarHorasGestorProjetoEmLoteDTO aprovarHorasDTO)
        {
            var result = new StatusResult();
            try
            {
                var qtdApontamentosFechados = await _apontamentoService.AprovarHorasGestorProjetoEmLote(aprovarHorasDTO, _usuarioLogado.Cpf, _usuarioLogado.OrgId, aprovarHorasDTO.DataColetaDeDados);
                result.Sucesso = true;

                var mensagem = _stringLocalizerApontamento.GetStringOuVazio("TIME_ENTRIES_APPROVED_SUCCESSFULLY");
                mensagem = mensagem.Replace("{{QT_TIME_ENTRIES}}", qtdApontamentosFechados.ToString());
                result.Mensagem = mensagem;

                return Ok(result);
            }
            catch (ArgumentException e)
            {
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return BadRequest(result);
            }
            catch (Exception e)
            {
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        [HttpPost("ReprovarApontamentosEmLoteGerenteDeProjeto")]
        public async Task<ActionResult<StatusResult>> ReprovarApontamentosEmLoteGerenteDeProjeto([FromBody] ReprovarHorasGestorProjetoEmLoteDTO reprovarHoras)
        {
            var result = new StatusResult();
            try
            {
                var qtdApontamentosFechados = await _apontamentoService.ReprovarHorasGestorProjetoEmLote(reprovarHoras, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
                result.Sucesso = true;

                var mensagem = _stringLocalizerApontamento.GetStringOuVazio("TIME_ENTRIES_DISAPPROVE_SUCCESSFULLY");
                mensagem = mensagem.Replace("{{QT_TIME_ENTRIES}}", qtdApontamentosFechados.ToString());
                result.Mensagem = mensagem;

                return Ok(result);
            }
            catch (ArgumentException e)
            {
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return BadRequest(result);
            }
            catch (Exception e)
            {
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        [HttpDelete("DeletarApontamentoColaborador")]
        public async Task<ActionResult<StatusResult>> DeletarApontamentoColaborador(string colaboradorApontamentoId, DateTime dataColetaDeDados)
        {
            var result = new StatusResult();
            try
            {
                result.Sucesso = await _apontamentoService.DeletarApontamentoColaborador(colaboradorApontamentoId, _usuarioLogado.Cpf, _usuarioLogado.OrgId, dataColetaDeDados);
                result.Mensagem = _stringLocalizerFourmakersCore.GetStringOuVazio("EXCLUSION_COMPLETED_SUCCESSFULLY");
                return Ok(result);
            }
            catch (ArgumentException e)
            {
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return BadRequest(result);
            }
            catch (UnauthorizedAccessException e)
            {
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception e)
            {
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        [HttpGet("RelatorioApontamentos/{mes}/{ano}")]
        public async Task<ActionResult<ApiGenericResult<FileContentResult>>> RelatorioApontamentos(int mes, int ano)
        {
            var result = new ApiGenericResult<FileContentResult>();

            try
            {
                var fileResult = await _apontamentoService.RelatorioApontamentos(mes, ano, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
                if (!fileResult.Sucesso)
                {
                    result.Mensagem = fileResult.Mensagem;
                    result.Sucesso = false;
                    return Ok(result);
                }

                result.Retorno = File(fileResult.Retorno.FileContents, fileResult.Retorno.ContentType, fileResult.Retorno.FileDownloadName, true);
                return Ok(result);
            }
            catch (UnauthorizedAccessException e)
            {
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return Unauthorized(result);
            }
            catch (Exception e)
            {
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        [HttpGet("RelatorioApontamentosSimplificado/{mes}/{ano}")]
        public async Task<ActionResult<ApiGenericResult<FileContentResult>>> RelatorioApontamentosSimplificado(int mes, int ano)
        {
            var result = new ApiGenericResult<FileContentResult>();

            try
            {
                var fileResult = await _apontamentoService.RelatorioApontamentosSimplificado(mes, ano, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
                if (!fileResult.Sucesso)
                {
                    result.Mensagem = fileResult.Mensagem;
                    result.Sucesso = false;
                    return Ok(result);
                }

                result.Retorno = File(fileResult.Retorno.FileContents, fileResult.Retorno.ContentType, fileResult.Retorno.FileDownloadName, true);
                return Ok(result);
            }
            catch (UnauthorizedAccessException e)
            {
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return Unauthorized(result);
            }
            catch (Exception e)
            {
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        [Authorize]
        [HttpGet("ListarColaboradoresEApontamentosPorGestor")]
        public async Task<ActionResult<ApiGenericResult<ListaComTotalizadorHorasResult<ColaboradorEApontamentoResult>>>> ListarColaboradoresEApontamentosPorGestor(string nomeColaborador, string codigoGerente, string codigoStatus, int mesVigencia, int anoVigencia, string codProjeto, string codColaboradorExternoAprovador, int cursor, int limite)
        {
            var result = new StatusResult();
            try
            {
                var ret = new ApiGenericResult<ListaComTotalizadorHorasResult<ColaboradorEApontamentoResult>>()
                {
                    Retorno = await _apontamentoService.ListarColaboradoresEApontamentosPorGestor(nomeColaborador, codigoGerente, codigoStatus, mesVigencia, anoVigencia, _usuarioLogado.OrgId, codProjeto, codColaboradorExternoAprovador, cursor, limite, _usuarioLogado.Cpf)
                };
                return Ok(ret);
            }
            catch (ArgumentException e)
            {
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return BadRequest(result);
            }
            catch (Exception e)
            {
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        [Authorize]
        [HttpGet("ListaStatusApontamentoGerenteProjeto")]
        public async Task<ActionResult> ListaStatusApontamentoGerenteProjeto()
        {
            var result = new ApiGenericResult<List<StatusApontamentoGrupoResult>>();

            try
            {
                result.Retorno = await _apontamentoService.ListaStatusApontamentoGerenteProjeto(_usuarioLogado.OrgId);
                return Ok(result);
            }
            catch (Exception e)
            {
                result.Sucesso = false;
                result.Mensagem = e.Message.ToString();
                return StatusCode(500, result);
            }
        }

        [Authorize]
        [HttpGet("ListarGestoresApontamento")]
        public async Task<ActionResult> ListarGestoresApontamento()
        {
            var result = new ApiGenericResult<List<GestoresApontamentoResult>>();
            try
            {
                result.Retorno = await _apontamentoService.ListarGestoresApontamento(_usuarioLogado.OrgId);
                return Ok(result);
            }
            catch (Exception e)
            {
                result.Sucesso = false;
                result.Mensagem = e.Message.ToString();
                return StatusCode(500, result);
            }
        }

        [Authorize]
        [HttpPost("AprovarProjetoVigenciaEmLote")]
        public async Task<ActionResult> AprovarProjetoVigenciaEmLote([FromBody] AprovarProjetoVigenciaEmLoteInput input)
        {
            try
            {
                var ret = await _apontamentoService.AprovarProjetoVigenciaEmLote(input.Projetos, input.Mes, input.Ano, input.Justificativa, _usuarioLogado.Cpf, _usuarioLogado.OrgId, input.DataColetaDeDados);
                if (ret.Sucesso)
                {
                    return Ok(ret);
                }
                else
                {
                    return BadRequest(ret);
                }
            }
            catch (Exception e)
            {
                var result = new ApiGenericResult<bool>();
                result.Sucesso = false;
                result.Mensagem = e.Message.ToString();
                return StatusCode(500, result);
            }
        }

        [HttpGet("RelatorioColaboradoresQueNaoApontaram/{mes}/{ano}")]
        public async Task<ActionResult> RelatorioColaboradoresQueNaoApontaram(int mes, int ano)
        {
            var result = new ApiGenericResult<FileContentResult>();
            var orgId = _usuarioLogado.OrgId;
            var cpf = _usuarioLogado.Cpf;

            var fileResult = await _apontamentoService.RelatorioColaboradoresQueNaoApontaram(orgId, cpf, mes, ano);

            if (!fileResult.Sucesso)
            {
                return StatusCode(500, fileResult);
            }

            result.Retorno = File(fileResult.Retorno.FileContents, fileResult.Retorno.ContentType, fileResult.Retorno.FileDownloadName);
            return Ok(result);
        }

        [HttpGet("EnviaEmailNotificacaoAprovadoresStatusPendentes")]
        public async Task<ActionResult> EnviaEmailNotificacaoAprovadoresStatusPendentes(string nomeColaborador, string codigoGerente, string codigoStatus, int mesVigencia, int anoVigencia, string codProjeto, string codColaboradorExternoAprovador, int cursor, int limite)
        {
            var result = new ApiGenericResult<string>();
            result = await _apontamentoService.EnviaEmailNotificacaoAprovadoresStatusPendentes(nomeColaborador, codigoGerente, codigoStatus, mesVigencia, anoVigencia, codProjeto, codColaboradorExternoAprovador, cursor, limite, _usuarioLogado);

            if (result.Sucesso)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }


        [HttpPost("EditarApontamento")]
        public async Task<ActionResult<ColaboradorApontamentoDTO>> EditarApontamento([FromBody] EditarApontamentoDTO parametro)
        {
            var result = new StatusResult();
            try
            {
                var apontamento = await _apontamentoService.EditarApontamento(parametro, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
                return Ok(apontamento);
            }
            catch (ArgumentException e)
            {
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return BadRequest(result);
            }
            catch (Exception e)
            {
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }
    }
}