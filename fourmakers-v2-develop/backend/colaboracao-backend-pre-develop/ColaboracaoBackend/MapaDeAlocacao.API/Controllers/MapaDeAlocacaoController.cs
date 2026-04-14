using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using DataTransferObject.Domain;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.MapaDeAlocacao;
using DataTransferObject.Domain.MapaDeAlocacao.CargaMapaAlocacao;
using DataTransferObject.Domain.MapaDeAlocacao.DetalharColaborador;
using DataTransferObject.Domain.MapaDeAlocacao.EditarAlocacao;
using DataTransferObject.Domain.MapaDeAlocacao.ExcluirAlocacao;
using DataTransferObject.Domain.MapaDeAlocacao.Projetos;
using DataTransferObject.Domain.Projeto;
using DataTransferObject.Domain.Usuario;
using Logs.Infra.Attributes;
using MapaDeAlocacao.API.DTOs;
using MapaDeAlocacao.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MapaDeAlocacao.API.Controllers
{
    [Authorize]
    [HandleException]
    [Route("api/[controller]")]
    [ApiController]
    [LogAction]
    public class MapaDeAlocacaoController : Controller
    {
        private readonly IMapaDeAlocacaoService _mapaDeAlocacaoService;
        private readonly IAspNetUser aspNetUser;
        private readonly UsuarioLogadoDTO _usuarioLogado;
        private readonly IProjetoMapaDeAlocacaoService _projetoMapaDeAlocacaoService;
        private readonly IExtracaoAlocacaoService _extracaoAlocacaoService;
        public MapaDeAlocacaoController(IMapaDeAlocacaoService mapaDeAlocacaoService, IAspNetUser aspNetUser, IProjetoMapaDeAlocacaoService projetoMapaDeAlocacaoService, IExtracaoAlocacaoService extracaoAlocacaoService)
        {
            _mapaDeAlocacaoService = mapaDeAlocacaoService;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
            _projetoMapaDeAlocacaoService = projetoMapaDeAlocacaoService;
            _extracaoAlocacaoService = extracaoAlocacaoService;
        }

        [HttpGet("BuscaCargaMapaAlocacaoBI")]
        public async Task<IActionResult> BuscaCargaMapaAlocacaoBI()
        {
            var cargaMapaResponse = new List<BuscarCargaMapaAlocacaoDTO>();
            cargaMapaResponse = await _mapaDeAlocacaoService.BuscarCargaMapaLocacao();
            return Ok(cargaMapaResponse);
        }

        [HttpGet("ConsultaProjetoHoras")]
        public async Task<ActionResult<List<ConsultaProjetoHorasDTO>>> ConsultaProjetoHoras(int cursor, int limite, string cdProjeto, string nomeProjeto,
                                                                                DisponibilidadeHorarioEnum disponibilidadeHorario, int? cdStatusProjeto,
                                                                                string? codDiretoria, string cdCliente,
                                                                                string cliente, string gestorProjeto)
        {
            var ret = new ApiGenericResult<List<ConsultaProjetoHorasDTO>>();
            try
            {
                ValidacaoUtil.ObrigaCursorLimite(cursor, limite);

                if (String.IsNullOrEmpty(cdProjeto) || cdProjeto.ToIntOuZero() == 0) cdProjeto = null;

                if (cdStatusProjeto == 0) cdStatusProjeto = null;

                if (String.IsNullOrEmpty(cdCliente) || cdCliente.ToIntOuZero() == 0) cdCliente = null;

                ret.Retorno = await _projetoMapaDeAlocacaoService.ConsultaProjetoHoras(cursor, limite, cdProjeto, nomeProjeto, disponibilidadeHorario, cdStatusProjeto, String.IsNullOrEmpty(codDiretoria) || codDiretoria == "null" ? null : int.Parse(codDiretoria), cdCliente, cliente, gestorProjeto, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
                ret.Sucesso = true;

                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpGet("ConsultarColaboradoresProjeto")]
        public ActionResult<ConsultarColaboradorProjetoResult> ConsultarColaboradoresProjeto(string cdProjeto, string cpf, int? tbdBusca, DateTime? de, DateTime? ate, bool exibirColabComAlocacoesAtuaisEFuturas = true)
        {
            string cpfTratado = cpf.ToNullSeTextoNull();

            var ret = _projetoMapaDeAlocacaoService.ConsultarColaboradoresProjeto(cdProjeto, cpfTratado, tbdBusca, de, ate, _usuarioLogado.Cpf, _usuarioLogado.OrgId, exibirColabComAlocacoesAtuaisEFuturas);

            if (ret.Sucesso)
            {
                return Ok(ret);
            }
            else
            {
                return StatusCode(500, ret);
            }
        }

        [HttpGet("GetAlocacaoColaborador")]
        public async Task<IActionResult> GetAlocacaoColaborador(string cpf, int tbdId, string projetoId, string mes, string ano)
        {
            int mesAux = 0;
            int anoAux = 0;
            var ret = new DetalharColaboradorResult();
            try
            {
                if (string.IsNullOrEmpty(cpf) && tbdId == 0)
                {
                    throw new Exception("CPF ou TBD são necessários.");
                }
                else if (!string.IsNullOrEmpty(cpf) && tbdId > 0)
                {
                    throw new Exception("Apenas um dos campos entre CPF e TBD ID devem ser preenchidos.");
                }

                if ((!string.IsNullOrEmpty(mes) && !int.TryParse(mes, out mesAux))
                    || (!string.IsNullOrEmpty(ano) && !int.TryParse(ano, out anoAux)))
                {
                    var message = "Para informar uma data, mes e ano precisam ser preenchidos";
                    throw new ApplicationException(message);
                }

                if (mesAux == 0 && anoAux == 0)
                {
                    mesAux = DateTime.Now.Month;
                    anoAux = DateTime.Now.Year;
                }

                ret.DetalharColaborador = await _mapaDeAlocacaoService.GetAlocacaoColaborador(cpf, tbdId, projetoId, mesAux, anoAux, _usuarioLogado.Cpf, _usuarioLogado.OrgId);

                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpGet("GetFiltroColaboradorProjeto")]
        public ActionResult<List<ColaboradorAlocadoDTO>> GetFiltroColaboradorProjeto(string projetoId)
        {
            var colaboradoresAlocados = _mapaDeAlocacaoService.GetColaboradoresAlocadosNoProjeto(projetoId, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(colaboradoresAlocados);
        }

        [HttpGet("GetMapaAlocacaoRecurso")]
        public async Task<IActionResult> GetMapaAlocacaoRecurso([FromQuery] GetMapaAlocacaoRecursoInputParam inputParam)
        {
            var ret = new ApiGenericResult<GetMapaAlocacaoRecursoOutputDTO>();

            ret.Retorno = await _mapaDeAlocacaoService.GetMapaAlocacaoRecurso(inputParam, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(ret);
        }

        [HttpGet("GetMapaAlocacaoResumo")]
        public async Task<IActionResult> GetMapaAlocacaoResumo([FromQuery] GetMapaAlocacaoInputParam inputParam)
        {
            var ret = new ApiGenericResult<GetMapaAlocacaoResumoOutputDTO>();

            ret.Retorno = await _mapaDeAlocacaoService.GetMapaAlocacaoResumo(inputParam, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(ret);
        }

        [HttpGet("GetPeriodoAlocacaoColaboradorVisaoEdicao")]
        public async Task<IActionResult> GetPeriodoAlocacaoColaboradorVisaoEdicao(string cpf, int tbdId, string projetoId, string mes, string ano)
        {
            var ret = new DetalharColaboradorEditarResult();
            ret.DetalharColaborador = await _mapaDeAlocacaoService.GetPeriodoAlocacaoColaboradorVisaoEdicao(cpf.ToNullSeTextoNull(), tbdId, projetoId, mes, ano, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(ret);
        }

        [HttpPost("ListarAlocacoesColabETbd")]
        public async Task<ActionResult<IEnumerable<AlocacaoColabETbdDTO>>> ListarAlocacoesColabETbd([FromBody] ListarAlocacoesColabETbdInput dto)
        {
            var ret = new ApiGenericResult<IEnumerable<AlocacaoColabETbdDTO>>();

            // Passa o dto diretamente para o serviço, junto com Cpf e OrgId
            ret.Retorno = await _mapaDeAlocacaoService.ListarAlocacoesColaboradoresETbds(dto, _usuarioLogado.Cpf, _usuarioLogado.OrgId, _usuarioLogado.Token, _usuarioLogado.Cpf);

            return Ok(ret);
        }

        [HttpGet("ListarColaboradoresETbds")]
        public async Task<ActionResult> ListarColaboradoresETbds(string codigoDiretoria, string codigoGestor, string codigoDepartamento, TipoProfissionalEnum filtroTipoProfissional)
        {
            var ret = new ApiGenericResult<IEnumerable<ColaboradorETbdDTO>>();
            ret.Retorno = await _mapaDeAlocacaoService.ListarColaboradoresETbds(_usuarioLogado.OrgId, codigoDiretoria.ToNullSeTextoNullOuZero(), codigoGestor.ToNullSeTextoNullOuZero(), _usuarioLogado.Cpf, codigoDepartamento.ToNullSeTextoNullOuZero(), filtroTipoProfissional);
            return Ok(ret);
        }

        [HttpGet("ListarColaboradoresGestor")]
        public async Task<IActionResult> ListarColaboradoresGestor(string codigoGestor)
        {
            var ret = new NomeRecursoResult();
            ret.NomesRecursos = _mapaDeAlocacaoService.ListarColaboradoresGestor(codigoGestor, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(ret);
        }

        [HttpGet("ListarColaboradoresOrg")]
        public async Task<ActionResult<ColaboradoresCchResult>> ListarColaboradoresOrg(string busca, int cursor, int limite)
        {
            var ret = new ColaboradoresCchResult();
            ret.ColaboradoresCch = await _mapaDeAlocacaoService.ListarColaboradoresOrgAsync(busca, cursor, limite, _usuarioLogado.OrgId, _usuarioLogado.Cpf);
            return Ok(ret);
        }

        [HttpGet("ListarNomesGestores")]
        public async Task<IActionResult> ListarNomesGestores(string? codDiretoria, string codDepartamento)
        {
            var ret = new NomesGestoresResult();
            ret.Gestores = await _mapaDeAlocacaoService.ListarNomesGestores(_usuarioLogado.Cpf, _usuarioLogado.OrgId, codDiretoria, codDepartamento);
            return Ok(ret);
        }

        [HttpPost("ListarProjetosColaborador")]
        public async Task<IActionResult> ListarProjetosColaborador([FromBody] ListarProjetosColaboradorInput listarProjetosColaboradorInput)
        {
            var ret = new ProjetosColaboradorResult();
            ret.projetos = await _mapaDeAlocacaoService.ListarProjetosColaborador(listarProjetosColaboradorInput.CodigoProfissional, listarProjetosColaboradorInput.EhTbd ?? false, _usuarioLogado.Cpf, _usuarioLogado.OrgId, listarProjetosColaboradorInput.CodigoGerenteProjeto, listarProjetosColaboradorInput.ListaCodigoCliente, listarProjetosColaboradorInput.Status, listarProjetosColaboradorInput.PrioritarioFiltro ?? FiltroProjetosPrioritariosEnum.Todos);
            return Ok(ret);
        }

        [HttpGet("ListarProjetosOrg")]
        public ActionResult<ProjetosCchResult> ListarProjetosOrg(string busca, int cursor, int limite)
        {
            var ret = new ProjetosCchResult();
            if (!MapaUtil.StringSemNumerosOuCaracteresEspeciais(busca))
            {
                throw new ApplicationException("Busca possui caracteres inválidos!");
            }
            ValidacaoUtil.ObrigaCursorLimite(cursor, limite);
            ret.projetosCch = _mapaDeAlocacaoService.ListarProjetosOrg(busca, cursor, limite, _usuarioLogado.OrgId);
            return Ok(ret);
        }

        [HttpGet("ListaStatusProjeto")]
        public ActionResult<List<StatusProjetosDTO>> ListaStatusProjeto(string busca, int cursor, int limite)
        {
            var ret = new ApiGenericResult<List<StatusProjetosDTO>>();
            try
            {
                if (!MapaUtil.StringSemNumerosOuCaracteresEspeciais(busca))
                {
                    throw new ApplicationException("Busca possui caracteres inválidos!");
                }

                ValidacaoUtil.ObrigaCursorLimite(cursor, limite);
                ret.Retorno = _projetoMapaDeAlocacaoService.ListarStatusProjetos(busca, cursor, limite, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
                ret.Sucesso = true;
                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpPost("CadastroMapaAlocacao")]
        public async Task<IActionResult> CadastroMapaAlocacao(CadastroMapaAlocacaoDTO cadastroMapaAlocacaoDTO)
        {
            var ret = new CadastroMapaAlocacaoResult();

            ret.cadastroMapaAlocacao = await _mapaDeAlocacaoService.CadastroMapaAlocacao(cadastroMapaAlocacaoDTO, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(ret);
        }

        [HttpPost("RemoveSkillAlocacao")]
        public async Task<IActionResult> RemoveSkillAlocacao(AlteraSkillAlocacaoParam param)
        {
            var ret = new EditarAlocacaoResult();
            try
            {
                ret.AlocacaoEditada = await _mapaDeAlocacaoService.RemoveSkillAlocacao(_usuarioLogado.Cpf, param.PeriodoAlocacaoId, param.Skill, _usuarioLogado.OrgId);
                return Ok(ret);
            }
            catch (Exception e)
            {
                return StatusCode(500, new StatusResult
                {
                    Sucesso = false,
                    Mensagem = e.Message
                });
            }
        }

        [HttpPost("AdicionaSkillAlocacao")]
        public async Task<IActionResult> AdicionaSkillAlocacao(AlteraSkillAlocacaoParam param)
        {
            var ret = new EditarAlocacaoResult();
            try
            {
                ret.AlocacaoEditada = await _mapaDeAlocacaoService.AdicionaSkillAlocacao(_usuarioLogado.Cpf, param.PeriodoAlocacaoId, param.Skill, _usuarioLogado.OrgId);
                return Ok(ret);
            }
            catch (Exception e)
            {
                return StatusCode(500, new StatusResult
                {
                    Sucesso = false,
                    Mensagem = e.Message
                });
            }
        }

        [HttpPost("AlteraPerfilAlocacao")]
        public async Task<IActionResult> AlteraPerfilAlocacao(AlteraPerfilAlocacaoParam param)
        {
            var ret = new EditarAlocacaoResult();
            try
            {
                ret.AlocacaoEditada = await _mapaDeAlocacaoService.AlteraPerfilAlocacao(_usuarioLogado.Cpf, param.PeriodoAlocacaoId, param.PerfilId, _usuarioLogado.OrgId);
                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpPost("InserirCargaMapaAlocacao")]
        public async Task<IActionResult> InserirCargaMapaAlocacao()
        {
            var ret = new CargaMapaAlocacaoResult();
            try
            {
                ret = await _mapaDeAlocacaoService.InserirCargaMapaAlocacao(_usuarioLogado.Cpf, _usuarioLogado.OrgId);
                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpPost("RemoverAlocacao")]
        public async Task<IActionResult> RemoverAlocacao(RemoverPeriodoAlocado param)
        {
            var ret = new StatusResult();
            try
            {
                ret = await _mapaDeAlocacaoService.RemoverAlocacao(param.idPeriodoAlocacao, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpPost("RemoverAlocacoesEmLote")]
        public async Task<IActionResult> RemoverAlocacoesEmLote([FromBody] RemoverAlocacoesEmLoteDTO idsPeriodoAlocacao)
        {
            var ret = new ApiGenericResult<List<RemoverAlocacoesEmLoteResult>>();

            ret.Retorno = await _mapaDeAlocacaoService.RemoverAlocacoesEmLote(idsPeriodoAlocacao.Ids, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(ret);
        }

        [HttpGet("RecalcularColaboradorNoPeriodoMensal")]
        public async Task<IActionResult> RecalcularColaboradorNoPeriodoMensal(string codigoColaborador, bool ehTbd, bool forcarRecalculoHorasPrevistas)
        {
            var ret = new ApiGenericResult<bool>();

            ret.Retorno = await _mapaDeAlocacaoService.RecalcularColaboradorNoPeriodoMensal(codigoColaborador, ehTbd, _usuarioLogado.Cpf, _usuarioLogado.OrgId, forcarRecalculoHorasPrevistas);
            return Ok(ret);
        }

        [HttpPost("EditarAlocacao")]
        public async Task<IActionResult> EditarAlocacao(EditarPeriodoAlocacaoParam param)
        {
            var ret = new EditarAlocacaoResult();
            try
            {
                ret.AlocacaoEditada = await _mapaDeAlocacaoService.EditarAlocacao(param.PeriodoAlocacaoId, param.DataInicio, param.DataFim, param.IncluiFimDeSemana, param.QuantidadeDeHoras, _usuarioLogado.Cpf, _usuarioLogado.OrgId, param.Prioritario, param.Observacao.ToNullSomenteSeTextoNull(), param.Oportunidade.ToNullSomenteSeTextoNull(), param.Percentual, param.FlagRetroalimentaCV);
                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpPost("SubstituirDadosAlocacaoesPorPeriodo")]
        public async Task<ActionResult<IEnumerable<AlocacaoColabETbdDTO>>> SubstituirDadosAlocacaoesPorPeriodo([FromBody] SubstituirDadosAlocacaoesPorPeriodoParam param)
        {
            var ret = new ApiGenericResult<IEnumerable<AlocacaoColabETbdDTO>>();

            ret.Retorno = await _mapaDeAlocacaoService.SubstituirDadosAlocacaoesPorPeriodo(param, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(ret);
        }

        [HttpGet("ListarPerfilAlocacao")]
        public ActionResult<ApiGenericResult<List<PerfilAlocacaoDTO>>> ListarPerfilAlocacao(string codProjeto, bool? ocultarSkill)
        {
            var ret = new ApiGenericResult<List<PerfilAlocacaoDTO>>
            {
                Retorno = _mapaDeAlocacaoService.ListarPerfilAlocacao(codProjeto, _usuarioLogado.Cpf, _usuarioLogado.OrgId, ocultarSkill?? false)
            };
            return Ok(ret);
        }

        [HttpGet("BuscarHabilidadesNaoDefinidas")]
        public async Task<ActionResult> BuscarHabilidadesNaoDefinidas()
        {
            var ret = new ApiGenericResult<List<SkillNivelDTO>>
            {
                Retorno = await _mapaDeAlocacaoService.BuscarHabilidadesNaoDefinidasDoColaborador(_usuarioLogado.Cpf, _usuarioLogado.Token, _usuarioLogado.OrgId)
            };
            return Ok(ret);
        }
    }
}