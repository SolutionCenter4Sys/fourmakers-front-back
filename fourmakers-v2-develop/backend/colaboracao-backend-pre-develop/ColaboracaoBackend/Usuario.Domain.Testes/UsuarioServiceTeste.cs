using ApiClient.Domain.Interfaces;
using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Usuario;
using Moq;
using System;
using System.Threading.Tasks;
using Usuario.Domain.Impl.Services;
using Usuario.Domain.Interfaces.Factorys;
using Usuario.Domain.Interfaces.Models;
using Xunit;

namespace Usuario.Domain.Testes
{
    public class UsuarioServiceTeste
    {
        private readonly Mock<IUsuarioDomainFactory> _usuarioFactory;
        private readonly Mock<IStatusColaboradorDomainFactory> _statusColaboradorFactory;
        private readonly Mock<IColaboradorClient> _colaboradorClient;
        private readonly Mock<IComentarioClient> _candidatoClient;
        private readonly Mock<ITokenDomainFactory> _tokenDomainFactory;
        private readonly Mock<ITokenUsuarioAcessoDomainFactory> _tokenUsuarioAcessoDomainFactory;
        private readonly Mock<ITokenSistemaDomainFactory> _tokenSistemaDomainFactory;
        private readonly Mock<IEnvioEmail> _envioEmail;
        private readonly Mock<ITokens> _tokens;
        private readonly Mock<IAspNetUser> _aspNetUser;
        private readonly Mock<ILogCore> _log;
        private readonly UsuarioService _usuarioService;

        public UsuarioServiceTeste()
        {
            _usuarioFactory = new Mock<IUsuarioDomainFactory>();
            _statusColaboradorFactory = new Mock<IStatusColaboradorDomainFactory>();
            _colaboradorClient = new Mock<IColaboradorClient>();
            _tokenDomainFactory = new Mock<ITokenDomainFactory>();
            _tokenUsuarioAcessoDomainFactory = new Mock<ITokenUsuarioAcessoDomainFactory>();
            _candidatoClient = new Mock<IComentarioClient>();
            _tokenSistemaDomainFactory = new Mock<ITokenSistemaDomainFactory>();
            _envioEmail = new Mock<IEnvioEmail>();
            _tokens = new Mock<ITokens>();
            _aspNetUser = new Mock<IAspNetUser>();
            _log = new Mock<ILogCore>();
        }

        private void MockToken(Mock<IUsuarioModel> usuarioBanco)
        {
            var tokenBanco = new Mock<ITokenModel>();
            tokenBanco.Setup(m => m.Token).Returns(usuarioBanco.Object.Token);
            tokenBanco.Setup(m => m.Ativo).Returns(1);
            tokenBanco.Setup(m => m.Validade).Returns(DateTime.Now.AddMinutes(1));

            var tokenModel = new Mock<ITokenModel>();
            tokenModel.Setup(m => m.Token).Returns(usuarioBanco.Object.Token);
            tokenModel.Setup(m => m.GetModel(usuarioBanco.Object.Token, _tokenDomainFactory.Object)).Returns(tokenBanco.Object);

            var tokenUsuarioAcessoModel = new Mock<ITokenUsuarioAcessoModel>();
            tokenUsuarioAcessoModel.Setup(m => m.GerarNovoToken(_tokenDomainFactory.Object, _tokenUsuarioAcessoDomainFactory.Object, usuarioBanco.Object)).Returns(tokenModel.Object);

            _tokenUsuarioAcessoDomainFactory.Setup(m => m.buildTokenUsuarioAcessoModel()).Returns(tokenUsuarioAcessoModel.Object);

            _tokens.Setup(m => m.Base64(usuarioBanco.Object.Token)).Returns(usuarioBanco.Object.Token);

            _tokenDomainFactory.Setup(m => m.buildTokenModel()).Returns(tokenModel.Object);

            var tokenSistemaModel = new Mock<ITokenSistemaModel>();
            _tokenSistemaDomainFactory.Setup(m => m.buildTokenSistemaModel()).Returns(tokenSistemaModel.Object);
        }

        private void MockStatus(string status, string cpf)
        {
            var statusColaboradorDTO = new StatusColaboradorDTO();
            statusColaboradorDTO.Descricao = status;

            var statusBanco = new Mock<IStatusColaboradorModel>();
            statusBanco.Setup(m => m.Status).Returns(statusColaboradorDTO);

            var statusColaboradorModel = new Mock<IStatusColaboradorModel>();
            statusColaboradorModel.Setup(m => m.GetStatusByCpfColaborador(cpf)).Returns(statusBanco.Object);

            _statusColaboradorFactory.Setup(m => m.buildStatusModel()).Returns(statusColaboradorModel.Object);
        }

        private Mock<IUsuarioModel> MockUsuario(string senha, sbyte primeiroAcessoRealizado, string token, string login, sbyte ativo, string cpf, string nomeCompleto)
        {
            var usuarioBanco = new Mock<IUsuarioModel>();
            usuarioBanco.Setup(m => m.Senha).Returns(senha);
            usuarioBanco.Setup(m => m.PrimeiroAcessoRealizado).Returns(primeiroAcessoRealizado);
            usuarioBanco.Setup(m => m.Usuario).Returns(new UsuarioColaboradorDTO { Cpf = cpf, Colaborador = new ColaboradorDTO { NomeCompleto = nomeCompleto, Cpf = cpf } });
            usuarioBanco.Setup(m => m.Token).Returns(token);
            usuarioBanco.Setup(m => m.Login).Returns(login);
            usuarioBanco.Setup(m => m.Ativo).Returns(ativo);
            usuarioBanco.Setup(m => m.Sistemico).Returns(1);

            var usuarioModel = new Mock<IUsuarioModel>();
            usuarioModel.Setup(m => m.GetUserByLogin(login, _usuarioFactory.Object,2)).Returns(usuarioBanco.Object);
            usuarioModel.Setup(m => m.GetUserByLogin(cpf, _usuarioFactory.Object,2)).Returns(usuarioBanco.Object);
            usuarioModel.Setup(m => m.GetUserByCpf(cpf)).Returns(usuarioBanco.Object);
            _usuarioFactory.Setup(m => m.buildUsuarioModel()).Returns(usuarioModel.Object);
            return usuarioBanco;
        }

        private void MockColaborador(Mock<IUsuarioModel> usuarioBanco)
        {
            _colaboradorClient.Setup(m => m.BuscarDadosColaboradorAdmin(usuarioBanco.Object.Usuario.Cpf, null)).Returns(Task.FromResult(usuarioBanco.Object.Usuario.Colaborador));
            _colaboradorClient.Setup(m => m.GetColaboradorByCpf(usuarioBanco.Object.Usuario.Cpf, null, false)).Returns(Task.FromResult(usuarioBanco.Object.Usuario.Colaborador));
            _colaboradorClient.Setup(m => m.GetColaboradorByCpf(usuarioBanco.Object.Usuario.Cpf, usuarioBanco.Object.Token, false)).Returns(Task.FromResult(usuarioBanco.Object.Usuario.Colaborador));
        }

        [Fact]
        public async Task PermitePrimeiroAcessoTeste()
        {
            var senha = "e8d95a51f3af4a3b134bf6bb680a213a";
            sbyte primeiroAcessoRealizado = 0;
            var token = "token";
            var login = "login";
            sbyte ativo = 1;
            var cpf = "81031459715";
            var nomeCompleto = "NomeCompleto";

            Mock<IUsuarioModel> usuarioBanco = MockUsuario(senha, primeiroAcessoRealizado, token, login, ativo, cpf, nomeCompleto);
            MockColaborador(usuarioBanco);

            var colaboradorRetornado = await _usuarioService.PermitePrimeiroAcesso(cpf);

            var colaboradorEsperado = "81031459715";

            Assert.Equal(colaboradorEsperado, colaboradorRetornado.Cpf);
        }

        [Fact]
        public async Task ConfirmaPrimeiroAcessoCandidatoTeste()
        {
            var senha = "e8d95a51f3af4a3b134bf6bb680a213a";
            sbyte primeiroAcessoRealizado = 0;
            var token = "s2OuzkrEerNLqDR6/+QcmHWugdxS7uppw9Lw69xv7vYTGXZjBHDUasEPW3R/HL6L";
            var login = "login";
            sbyte ativo = 1;
            var cpf = "81031459715";
            var nomeCompleto = "NomeCompleto";

            Mock<IUsuarioModel> usuarioBanco = MockUsuario(senha, primeiroAcessoRealizado, token, login, ativo, cpf, nomeCompleto);
            MockColaborador(usuarioBanco);
            MockToken(usuarioBanco);

            var usuarioRetornado = await _usuarioService.ConfirmaPrimeiroAcessoCandidato(usuarioBanco.Object.Usuario.Cpf, "");

            var usuarioEsperado = "81031459715";

            Assert.Equal(usuarioEsperado, usuarioRetornado.Cpf);
        }

        [Fact]
        public async Task AlteraSenhaUsuarioTeste()
        {
            var senha = "e8d95a51f3af4a3b134bf6bb680a213a";
            sbyte primeiroAcessoRealizado = 1;
            var token = "s2OuzkrEerNLqDR6/+QcmHWugdxS7uppw9Lw69xv7vYTGXZjBHDUasEPW3R/HL6L";
            var login = "login";
            sbyte ativo = 1;
            var cpf = "81031459715";
            var nomeCompleto = "NomeCompleto";

            Mock<IUsuarioModel> usuarioBanco = MockUsuario(senha, primeiroAcessoRealizado, token, login, ativo, cpf, nomeCompleto);
            MockColaborador(usuarioBanco);
            MockToken(usuarioBanco);

            var senhaTrocada = await _usuarioService.AlteraSenhaUsuario(usuarioBanco.Object.Usuario.Cpf, "senha", "novaSenha");

            Assert.True(senhaTrocada);
        }

        [Fact]
        public async Task ResetaSenhaUsuarioTeste()
        {
            var senha = "e8d95a51f3af4a3b134bf6bb680a213a";
            sbyte primeiroAcessoRealizado = 1;
            var token = "s2OuzkrEerNLqDR6/+QcmHWugdxS7uppw9Lw69xv7vYTGXZjBHDUasEPW3R/HL6L";
            var login = "login";
            sbyte ativo = 1;
            var cpf = "81031459715";
            var nomeCompleto = "NomeCompleto";

            Mock<IUsuarioModel> usuarioBanco = MockUsuario(senha, primeiroAcessoRealizado, token, login, ativo, cpf, nomeCompleto);
            MockColaborador(usuarioBanco);
            MockToken(usuarioBanco);

            var senharesetada = await _usuarioService.ResetaSenhaUsuario(usuarioBanco.Object.Usuario.Cpf);

            Assert.True(senharesetada);
        }

        [Fact]
        public void BuscarDadosUsuarioTeste()
        {
            var senha = "e8d95a51f3af4a3b134bf6bb680a213a";
            sbyte primeiroAcessoRealizado = 1;
            var token = "s2OuzkrEerNLqDR6/+QcmHWugdxS7uppw9Lw69xv7vYTGXZjBHDUasEPW3R/HL6L";
            var login = "login";
            sbyte ativo = 1;
            var cpf = "81031459715";
            var nomeCompleto = "NomeCompleto";

            Mock<IUsuarioModel> usuarioBanco = MockUsuario(senha, primeiroAcessoRealizado, token, login, ativo, cpf, nomeCompleto);

            var usuarioRetornado = _usuarioService.BuscarDadosUsuario(cpf);

            var usuarioEsperado = cpf;

            Assert.Equal(usuarioEsperado, usuarioRetornado.Cpf);
        }
    }
}