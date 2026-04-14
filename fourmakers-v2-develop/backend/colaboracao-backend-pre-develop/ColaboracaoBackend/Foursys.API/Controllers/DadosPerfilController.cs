using DataTransferObject.Domain.Cep;
using DataTransferObject.Domain.DadosPerfil;
using DataTransferObject.Domain.DadosPerfil.GrauParentesco;
using DataTransferObject.Domain.DadosPerfil.TipoContratacao;
using Foursys.Domain.Interfaces.Services;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Foursys.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [LogAction]
    public class DadosPerfilController : Controller
    {
        private IDadosPerfilService _dadosPerfilService;

        public DadosPerfilController(IDadosPerfilService dadosPerfilService)
        {
            _dadosPerfilService = dadosPerfilService;
        }

        [HttpGet("ListarDisponibilidade")]
        public DisponibilidadeResult ListarDisponibilidade(string busca, int cursor, int limite)
        {
            var ret = new DisponibilidadeResult();
            try
            {
                ret.Disponibilidade = _dadosPerfilService.ListarDisponibilidade(busca, cursor, limite);

                return ret;
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;

                return ret;
            }
        }

        [HttpGet("ListarFonteOrigem")]
        public FonteOrigemResult ListarFonteOrigem(string busca, int cursor, int limite)
        {
            var ret = new FonteOrigemResult();
            try
            {
                ret.FonteOrigem = _dadosPerfilService.ListarFonteOrigem(busca, cursor, limite);

                return ret;
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;

                return ret;
            }
        }

        [HttpGet("ListarTipoDeCargo")]
        public TipoDeCargoResult ListarTipoDeCargo(string busca, int cursor, int limite)
        {
            var ret = new TipoDeCargoResult();

            try
            {
                ret.TipoDeCargo = _dadosPerfilService.ListarTipoDeCargo(busca, cursor, limite);

                return ret;
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;

                return ret;
            }
        }

        [HttpGet("ListarTipoDeCargaHoraria")]
        public TipoDeCargaHorariaResult ListarTipoCargaHoraria(string busca, int cursor, int limite)
        {
            var ret = new TipoDeCargaHorariaResult();

            try
            {
                ret.TipoDeCargaHoraria = _dadosPerfilService.ListarTipoCargaHoraria(busca, cursor, limite);

                return ret;
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;

                return ret;
            }
        }

        [HttpGet("ListarModalidadeContratacao")]
        public ModalidadeContratacaoResult ListarModalidadeContratacao(string busca, int cursor, int limite)
        {
            var ret = new ModalidadeContratacaoResult();

            try
            {
                ret.ModalidadeContratacao = _dadosPerfilService.ListarModalidadeContratacao(busca, cursor, limite);

                return ret;
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;

                return ret;
            }
        }

        [HttpGet("ListarEstadoCivil")]
        public EstadoCivilResult ListarEstadoCivil(string busca, int cursor, int limite)
        {
            var ret = new EstadoCivilResult();

            try
            {
                ret.EstadoCivil = _dadosPerfilService.ListarEstadoCivil(busca, cursor, limite);

                return ret;
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;

                return ret;
            }
        }

        [HttpGet("ListarNivelEscolaridade")]
        public EscolaridadeResult ListarEscolaridade(string busca, int cursor, int limite)
        {
            var ret = new EscolaridadeResult();

            try
            {
                ret.Escolaridade = _dadosPerfilService.ListarEscolaridade(busca, cursor, limite);

                return ret;
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;

                return ret;
            }
        }

        [HttpGet("ListarOrientacaoSexual")]
        public OrientacaoSexualResult ListarOrientacaoSexual(string busca, int cursor, int limite)
        {
            var ret = new OrientacaoSexualResult();

            try
            {
                ret.OrientacaoSexual = _dadosPerfilService.ListarOrientacaoSexual(busca, cursor, limite);

                return ret;
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;

                return ret;
            }
        }

        [HttpGet("ListarIdentidadeDeGenero")]
        public IdentidadeDeGeneroResult ListarIdentidadeDeGenero(string busca, int cursor, int limite)
        {
            var ret = new IdentidadeDeGeneroResult();
            try
            {
                ret.IdentidadeDeGenero = _dadosPerfilService.ListarIdentidadeDeGenero(busca, cursor, limite);

                return ret;
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;

                return ret;
            }
        }

        [HttpGet("ConsultarCep")]
        public async Task<ConsultaCepResult> ConsultarCep(string cep)
        {
            var ret = new ConsultaCepResult();

            try
            {
                ret.Cep = await _dadosPerfilService.ConsultarCep(cep);

                return ret;
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;

                return ret;
            }
        }

        [HttpGet("TipoContratacao")]
        public async Task<TipoContratacaoResult> TipoContratacao(string emailColaborador)
        {
            var ret = new TipoContratacaoResult();

            try
            {
                ret.TipoContratacao = await _dadosPerfilService.BuscarTipoContratacao(emailColaborador);

                return ret;
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;

                return ret;
            }
        }

        [HttpGet("ListarGrauParentesco")]
        public GrauParentescoResult ListarGrauParentesco(string busca, int cursor, int limite)
        {
            var ret = new GrauParentescoResult();
            try
            {
                ret.GrauParentesco = _dadosPerfilService.ListarGrauParentesco(busca, cursor, limite);

                return ret;
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;

                return ret;
            }
        }

        [HttpGet("ListarTipoContratacao")]
        public ListarTipoContratacaoResult ListarTipoContratacao(string busca, int cursor, int limite)
        {
            var ret = new ListarTipoContratacaoResult();
            try
            {
                ret.ListaTipoContratacao = _dadosPerfilService.ListarTipoContratacao(busca, cursor, limite);

                return ret;
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;

                return ret;
            }
        }

        [HttpGet("ListarEtnia")]
        public EtniaResult ListarEtnia(string busca, int cursor, int limite)
        {
            var ret = new EtniaResult();
            try
            {
                ret.Etnia = _dadosPerfilService.ListarEtnia(busca, cursor, limite);

                return ret;
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;

                return ret;
            }
        }
    }
}