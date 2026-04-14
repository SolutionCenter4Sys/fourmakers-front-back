using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Usuario;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Usuario.API.DTOs;
using Usuario.Domain.Interfaces.Services;

namespace Usuario.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [LogAction]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IAspNetUser _aspNetUser;
        private readonly UsuarioLogadoDTO usuarioLogado;
        private readonly IExtracaoUsuarioService _extracaoUsuarioService;

        public UsuarioController(IUsuarioService usuarioService, IAspNetUser aspNetUser, IExtracaoUsuarioService extracaoUsuarioService)
        {
            _usuarioService = usuarioService;
            _aspNetUser = aspNetUser;
            this.usuarioLogado = _aspNetUser.GetUsuarioLogado();
            _extracaoUsuarioService = extracaoUsuarioService;
        }

        [HttpPost("LoginExterno")]
        public async Task<ActionResult<UsuarioResult>> LoginExterno(LoginParam param)
        {
            var ret = new UsuarioResult();
            try
            {
                var orgId = 1; //Fourmakers
                var usuarioInfo = await _usuarioService.Login(param.cpfemail, param.senha, orgId);
                var tokenAcesso = usuarioInfo.token;
                ret.Sucesso = true;
                if (!_usuarioService.IsEmailValidated(param.cpfemail, orgId))
                {
                    ret.Mensagem = "validar";
                    return Ok(ret);
                }
                ret.Mensagem = "login";
                ret.usuario = usuarioInfo.usuario;
                ret.primeiroAcessoRealizado = usuarioInfo.primeiroAcessoRealizado;
                ret.token = tokenAcesso;
                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return Unauthorized(ret);
            }
        }

        [HttpPost("LoginCpfOrg")]
        public async Task<ActionResult<UsuarioResult>> LoginCpfOrg(LoginParam param)
        {
            var ret = new UsuarioResult();
            try
            {
                var usuarioInfo = await _usuarioService.Login(param.cpfemail, param.senha, param.orgId);
                var tokenAcesso = usuarioInfo.token;
                ret.Sucesso = true;
                ret.Mensagem = "login";
                ret.usuario = usuarioInfo.usuario;
                ret.primeiroAcessoRealizado = usuarioInfo.primeiroAcessoRealizado;
                ret.token = tokenAcesso;
                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return Unauthorized(ret);
            }
        }

        [HttpPost("Logout")]
        public StatusResult Logout()
        {
            var ret = new StatusResult();
            try
            {
                _usuarioService.Logout(usuarioLogado.Cpf);
                ret.Sucesso = true;
                return ret;
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return ret;
            }
        }

        [HttpPost("EsqueciSenha")]
        public async System.Threading.Tasks.Task<StatusResult> EsqueciSenha(EsqueciSenhaParam param)
        {
            var ret = new StatusResult();
            try
            {
                ret.Sucesso = await _usuarioService.ResetaSenhaUsuario(param.cpf);
                return ret;
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return ret;
            }
        }

        [HttpPost("AlteraSenha")]
        [Authorize]
        public async System.Threading.Tasks.Task<StatusResult> AlteraSenha(AlteraSenhaParam param)
        {
            var ret = new StatusResult();
            try
            {
                //TODO add template troca de senha
                ret.Sucesso = await _usuarioService.AlteraSenha("", param.novaSenha, param.senhaAntiga);

                return ret;
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return ret;
            }
        }

        [HttpGet("PermitePrimeiroAcesso")]
        public async System.Threading.Tasks.Task<PrimeiroAcessoResult> PermitePrimeiroAcesso(string cpf)
        {
            var ret = new PrimeiroAcessoResult();
            try
            {
                ret.colaborador = await _usuarioService.PermitePrimeiroAcesso(cpf);
                if (ret.colaborador == null)
                {
                    ret.colaborador = new ColaboradorDTO
                    {
                        Cpf = cpf
                    };
                }
                return ret;
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return ret;
            }
        }

        [HttpPost("ConfirmaPrimeiroAcessoCandidato")]
        public async System.Threading.Tasks.Task<UsuarioResult> ConfirmaPrimeiroAcessoCandidato([FromBody] ConfirmaPrimeiroAcessoParam param)
        {
            var ret = new UsuarioResult();
            try
            {
                ret.usuario = await _usuarioService.ConfirmaPrimeiroAcessoCandidato(param.cpf, param.fcmToken);

                return ret;
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return ret;
            }
        }

        [HttpPost("InserePrestadorServico")]
        public void InserePrestadorServico([FromBody] InserePrestadorServicoDTO inputDTO, string cnpj)
        {
            try
            {
                _usuarioService.InserePrestadorServicoPessoaJuridica(cnpj, inputDTO.PrestadorServico, inputDTO.RegimeTributario, usuarioLogado.Cpf);
            }
            catch
            {
                throw;
            }
        }

        [HttpPost("AlterarPrestadorServico")]
        public void AlterarPrestadorServico([FromBody] InserePrestadorServicoDTO inputDTO, string cnpj)
        {
            try
            {
                _usuarioService.AlterarPrestadorServicoPessoaJuridica(cnpj, inputDTO.PrestadorServico, inputDTO.RegimeTributario, usuarioLogado.Cpf);
            }
            catch
            {
                throw;
            }
        }

        [Authorize]
        [HttpGet("BuscarDadosUsuario")]
        public UsuarioResult BuscarDadosUsuario(string cpf)
        {
            var ret = new UsuarioResult();
            try
            {
                ret.usuario = _usuarioService.BuscarDadosUsuario(cpf);

                return ret;
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return ret;
            }
        }

        [Authorize]
        [HttpPost("ValidaAcessoGrupoFuncionalidade")]
        public ActionResult<bool> ValidaAcessoGrupoFuncionalidade(ValidaAcessoGrupoFuncionalidadeParam param)
        {
            try
            {
                return Ok(_usuarioService.ValidaAcessoGrupoFuncionalidade(param.cpf, param.funcionalidadeSistemaEnum));
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [Authorize]
        [HttpPost("ConfirmaPrimeiroFourmakers")]
        public StatusResult ConfirmaPrimeiroFourmakers()
        {
            var ret = new StatusResult();
            try
            {
                _usuarioService.ConfirmaPrimeiroAcessoFourmakers(usuarioLogado.Cpf, usuarioLogado.OrgId);
                return ret;
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return ret;
            }
        }

        [HttpPost("GetAccessToken")]
        public async System.Threading.Tasks.Task<ActionResult<UsuarioResult>> GenerateAccessToken(GetAccessTokenParam param)
        {
            var ret = new UsuarioResult();

            try
            {
                ret = await _usuarioService.GenerateAccessToken(param.AccessCode, param.OrgId);
                ret.Sucesso = true;

                return Ok(ret);
            }
            catch (Exception e)
            {
                var mensagemErro = e.Message;
                var InnerException = e.InnerException;
                while (InnerException != null)
                {
                    mensagemErro += "\n" + InnerException.Message;
                    InnerException = InnerException.InnerException;
                }
                Console.WriteLine(mensagemErro);
                Console.WriteLine("StackTrace: \n" + e.StackTrace);
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(401, ret);
            }
        }

        [HttpPost("GetAccessTokenMobile")]
        public async System.Threading.Tasks.Task<ActionResult<UsuarioResult>> GenerateAccessTokenMobile(GetAccessTokenParam param)
        {
            var ret = new UsuarioResult();

            try
            {
                ret = await _usuarioService.GenerateAccessTokenMobile(param.AccessCode, param.OrgId);
                ret.Sucesso = true;

                return Ok(ret);
            }
            catch (Exception e)
            {
                var mensagemErro = e.Message;
                var InnerException = e.InnerException;
                while (InnerException != null)
                {
                    mensagemErro += "\n" + InnerException.Message;
                    InnerException = InnerException.InnerException;
                }
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(401, ret);
            }
        }

        [Authorize]
        [HttpGet("ShowMeSSOCandidato")]
        public async Task<ActionResult<UsuarioResult>> ShowMeSSOCandidato()
        {
            var ret = new UsuarioResult();
            try
            {
                ret.usuario = await _usuarioService.ShowMeSSOCandidato("");
                ret.Sucesso = true;
                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = "Invalid Access Token";
                return StatusCode(401, ret);
            }
        }

        [Authorize]
        [HttpGet("ShowMe")]
        public async Task<ActionResult<UsuarioResult>> ShowMe()
        {
            var ret = new UsuarioResult();
            try
            {
                ret.usuario = await _usuarioService.ShowMe(usuarioLogado.Cpf, usuarioLogado.OrgId);
                ret.Sucesso = true;
                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = "Invalid Access Token";
                return StatusCode(401, ret);
            }
        }

        [Authorize]
        [HttpGet("BuscarHierarquia")]
        public async Task<ActionResult<List<BuscarHierarquiaResult>>> BuscarHierarquia()
        {
            var ret = new List<BuscarHierarquiaResult>();
            try
            {
                var listaDeUsuarios = await _usuarioService.BuscarHierarquia(usuarioLogado.Cpf, usuarioLogado.OrgId);

                foreach (BuscarHierarquiaResult usuario in listaDeUsuarios)
                {
                    ret.Add(usuario);
                }

                return ret;
            }
            catch (Exception e)
            {
                return StatusCode(401, e.Message);
            }
        }

        [HttpPost("InsereUsuarioFourmaker")]
        public async Task<ActionResult<StatusResult>> InsereUsuarioFourmaker(InsereUsuarioFourmakerParam param)
        {
            var ret = new StatusResult();
            try
            {
                await _usuarioService.InsereUsuarioFourmakers(param.Cpf, param.NomeCompleto, param.Email, param.Senha);

                return ret;
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpPost("EnviarEmailAlterarSenha")]
        public ActionResult<StatusResult> EnviarEmailAlterarSenha(EnviarEmailParam param)
        {
            var ret = new StatusResult();
            try
            {
                _usuarioService.EnviarEmailAlterarSenha(param.Email);
                return ret;
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = "Um email foi enviado para o endereço " + param.Email + " com instruções.";
                return StatusCode(500, ret);
            }
        }

        [HttpPost("AlterarSenhaPorToken")]
        public ActionResult<StatusResult> AlterarSenhaPorToken(AlterarSenhaPorTokenParam param)
        {
            var ret = new StatusResult();
            try
            {
                _usuarioService.AlterarSenhaPorToken(param.Token, param.Senha);
                return ret;
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [Authorize]
        [HttpPost("SubmeterConviteEmpresa")]
        public ActionResult<StatusResult> SubmeterConviteEmpresa(SubmeterConviteEmpresaParam param)
        {
            var ret = new StatusResult();
            try
            {
                _usuarioService.SubmeterConviteEmpresa(usuarioLogado.Cpf, param.Cnpj, param.Confirmado);
                return ret;
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [Authorize]
        [HttpPost("AlterarSenhaUsuarioLogado")]
        public ActionResult<StatusResult> AlterarSenhaUsuarioLogado([FromBody] AlterarSenhaPorTokenParam param)
        {
            var ret = new StatusResult();
            try
            {
                _usuarioService.AlterarSenhaUsuarioLogado(param.Senha);
                return ret;
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpPost("ConfirmaEmail")]
        public ActionResult<StatusResult> ConfirmaEmail(int codigo, string cpf)
        {
            var ret = new StatusResult();
            try
            {
                _usuarioService.ConfirmacaoEmail(codigo, cpf);
                return ret;
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpPost("EnviarConfirmacaoEmail")]
        public ActionResult<StatusResult> EnviarConfirmacaoEmail(string cpf)
        {
            var ret = new StatusResult();
            try
            {
                _usuarioService.EnviarConfirmacaoEmail(cpf);
                return ret;
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpPost("ReenviarCodigoConfirmacao")]
        public ActionResult<StatusResult> ReenviarCodigoConfirmacao(string cpf)
        {
            var ret = new StatusResult();
            try
            {
                _usuarioService.ReenviarCodigoConfirmacao(cpf);
                return ret;
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [Authorize]
        [HttpGet("RelatorioColaboradores")]
        public async Task<IActionResult> RelatorioColaboradores()
        {
            var fileResult = await _extracaoUsuarioService.RelatorioColaboradores(_aspNetUser.GetUsuarioLogado().Cpf, _aspNetUser.GetUsuarioLogado().OrgId);

            if (!fileResult.Sucesso)
            {
                return StatusCode(500, fileResult.Mensagem);
            }

            return File(fileResult.Retorno.FileContents, fileResult.Retorno.ContentType, fileResult.Retorno.FileDownloadName);
        }
        
        [Authorize]
        [HttpPost("EnviarDenunciaViaCanalDenuncia")]
        public async Task<IActionResult> EnviarDenunciaViaCanalDenuncia([FromBody] CanalDenunciaParam param)
        {
            var result = await _usuarioService.EnviarEmailDenuncia(param.Email, param.Mensagem, _aspNetUser.GetUsuarioLogado().OrgId);

            if (!result.Sucesso)
            {
                return StatusCode(500, result.Mensagem);
            }
            
            return Ok(result);
        }
    }
}