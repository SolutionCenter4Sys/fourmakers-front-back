using ApiClient.Domain;
using ApiClient.Domain.Interfaces;
using Colaboracao.Core;
using Colaboracao.Core.Exceptions;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Core.Domain;
using Core.Domain.Colaborador;
using Core.Domain.TemplateEmail;
using Core.Domain.Usuario;
using Core.Domain.Usuario.Permissao;
using Core.DomainModel.Competencia;
using Core.DomainModel.Projeto;
using Core.DomainModel.SSO;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Log;
using DataTransferObject.Domain.Org;
using DataTransferObject.Domain.SRS.Candidate;
using DataTransferObject.Domain.SSO;
using DataTransferObject.Domain.TemplateEmail;
using DataTransferObject.Domain.Usuario;
using DataTransferObject.Domain.Usuario.Permissao;
using Logs.Infra.Attributes;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Core.Domain.Questionario;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Fourmakers;
using Foursys.Domain.Interfaces.Services;
using TemplateOrg.Constantes;
using TemplateOrg.Interfaces;
using Core.DomainModel;
using Usuario.Domain.Interfaces.Services;

namespace Usuario.Domain.Impl.Services
{
    [LogDomainClass]
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioDtoRepository _usuarioDtoRepository;
        private readonly IStatusDtoRepository _statusDtoRepository;
        private readonly IColaboradorClient _colaboradorClient;
        private readonly ISSOClient _ssoClient;
        private readonly ISRSClient _srsClient;
        private readonly IEnvioEmail _envioEmail;
        private readonly IAspNetUser _aspNetUser;
        private readonly ILogDBCore _logDB;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICCHClient _cchClient;
        private readonly IUsuarioColaboradorRepository _usuarioColaboradorRepository;
        private readonly ICompetenciaClient _competenciaClient;
        private readonly IUsuarioExternoRepository _usuarioExternoRepository;
        private readonly IFuncionalidadeSistemaRepository _funcionalidadeSistemaRepository;
        private readonly IColaboradorKeeperRepository _colaboradorKeeperRepository;
        private readonly ISSORepository _ssoRepository;
        private readonly ITokens _token;
        private readonly ITemplateOrgService _templateOrgService;

        private readonly ICompetenciaColaboradorRepository _competenciaColaboradorRepository;
        private readonly IBuscaColaboradorRepository _buscaColaboradorRepository;
        private readonly IProjetoOrgRepository _projetoOrgRepository;
        private readonly IAcessoUsuarioRepository _acessoUsuarioRepository;
        private readonly ITemplateRepository _templateRepository;
        private readonly IHistoricoCVRepository _historicoCvRepository;
        private readonly IBuscaParametroConfiguracaoService _buscaParametroConfiguracaoService;
        private readonly IQuestionarioRespostaRepository _questionarioRespostaRepository;
        private const string PEOPLE_PARTNER_CODE_CCH = "181";
        private const int ORG_ID_FOURMAKERS = 1;
        private const int VERDADEIRO = 1;
        private const int ATIVO = 1;
        private const int INATIVO = 0;
        private const string VARIAVEL_NOME_COMPLETO = "${NOME}";
        private const string VARIAVEL_URL = "${URL_SSO}";
        private const string VARIAVEL_SENHA = "${SENHA}";
        private readonly int TAMANHO_CODIGO_ACESSO = 6;
        private readonly int VALIDADE_TOKEN = 15;

        public UsuarioService(IUsuarioDtoRepository usuarioDtoRepository, IStatusDtoRepository statusDtoRepository, IColaboradorClient colaboradorClient,
            ISSOClient ssoClient, ISRSClient srsClient,
            IEnvioEmail envioEmail, IAspNetUser aspNetUser, ILogDBCore logDB, IUnitOfWork unitOfWork, ICCHClient cchClient,
            IUsuarioColaboradorRepository usuarioColaboradorRepository, IUsuarioExternoRepository usuarioExternoRepository, ICompetenciaClient competenciaClient,
            ITokens token, IColaboradorKeeperRepository colaboradorKeeperRepository, ISSORepository ssoRepository,
            ICompetenciaColaboradorRepository competenciaColaboradorRepository, IBuscaColaboradorRepository buscaColaboradorRepository,
            IProjetoOrgRepository projetoOrgRepository, IFuncionalidadeSistemaRepository funcionalidadeSistemaRepository,
            ITemplateOrgService templateOrgService, IAcessoUsuarioRepository acessoUsuarioRepository, ITemplateRepository templateRepository,
            IHistoricoCVRepository historicoCvRepository, IBuscaParametroConfiguracaoService buscaParametroConfiguracaoService, IQuestionarioRespostaRepository questionarioRespostaRepository)
        {
            _usuarioDtoRepository = usuarioDtoRepository;
            _statusDtoRepository = statusDtoRepository;
            _colaboradorClient = colaboradorClient;
            _ssoClient = ssoClient;
            _srsClient = srsClient;
            _envioEmail = envioEmail;
            _aspNetUser = aspNetUser;
            _logDB = logDB;
            _unitOfWork = unitOfWork;
            _cchClient = cchClient;
            _usuarioColaboradorRepository = usuarioColaboradorRepository;
            _competenciaClient = competenciaClient;
            _usuarioExternoRepository = usuarioExternoRepository;
            _colaboradorKeeperRepository = colaboradorKeeperRepository;
            _token = token;
            _ssoRepository = ssoRepository;
            _competenciaColaboradorRepository = competenciaColaboradorRepository;
            _buscaColaboradorRepository = buscaColaboradorRepository;
            _projetoOrgRepository = projetoOrgRepository;
            _funcionalidadeSistemaRepository = funcionalidadeSistemaRepository;
            _templateOrgService = templateOrgService;
            _acessoUsuarioRepository = acessoUsuarioRepository;
            _templateRepository = templateRepository;
            _historicoCvRepository = historicoCvRepository;
            _buscaParametroConfiguracaoService = buscaParametroConfiguracaoService;
            _questionarioRespostaRepository = questionarioRespostaRepository;
        }

        public async Task<UsuarioResult> Login(string login, string senha, int idOrg)
        {
            var ret = new UsuarioResult();
            var senhaHash = Criptografia.MD5Hash(senha);
            var usuarioBanco = _usuarioExternoRepository.GetUserByCPFSenha(login, senhaHash, idOrg);
            if (usuarioBanco != null)
            {
                if (!usuarioBanco.ColaboradorOrg.Ativo)
                {
                    throw new ValidationException("Usuário inativo.");
                }
                var statusBanco = _statusDtoRepository.GetStatusByCpf(usuarioBanco.Cpf);

                if (statusBanco?.Descricao == "INATIVO")
                {
                    throw new Exception("Usuário inativado.");
                }

                ret.token = GenerateJWTToken(usuarioBanco.Email, usuarioBanco.Cpf, "", idOrg, TipoLoginEnum.SISTEMA);
                ret.usuario = await ShowMe(usuarioBanco.Cpf, idOrg);
                ret.primeiroAcessoRealizado = _usuarioExternoRepository.IfPrimeiroAcessoUsuario(usuarioBanco.UsuarioId);
                ret.dataAceitePrimeiroAcesso = _usuarioExternoRepository.GetDataAceiteTermo(usuarioBanco.UsuarioId);
            }
            else
            {
                throw new Exception("CPF ou senha não conferem no sistema.");
            }

            return ret;
        }

        public async Task<ColaboradorDTO> PermitePrimeiroAcesso(string cpf)
        {
            var colaborador = await _colaboradorClient.BuscarDadosColaboradorAdmin(cpf, null);

            if (colaborador == null)
            {
                return null;
            }
            else
            {
                var usuarioBanco = _usuarioDtoRepository.GetUserByCpf(cpf);

                if (usuarioBanco == null)
                {
                    throw new Exception("Usuário não encontrado no cpf fornecido.");
                }

                if (usuarioBanco.PrimeiroAcessoRealizado == 0 && usuarioBanco.Ativo == 1)
                {
                    return colaborador;
                }
                else
                {
                    throw new Exception("Primeiro acesso não permitido.");
                }
            }
        }

        public async Task<UsuarioColaboradorDTO> ConfirmaPrimeiroAcessoCandidato(string cpf, string fcmToken)
        {
            var ret = new UsuarioColaboradorDTO();
            try
            {
                var usuarioBanco = _usuarioDtoRepository.GetUserByCpf(cpf);

                var novaSenha = geraSenhaUsuario();
                usuarioBanco.Senha = Criptografia.MD5Hash(novaSenha);
                usuarioBanco.PrimeiroAcessoRealizado = VERDADEIRO;
                if (!String.IsNullOrEmpty(fcmToken))
                    usuarioBanco.FcmToken = fcmToken;
                _usuarioDtoRepository.UpdateUser(usuarioBanco);

                ret.Cpf = usuarioBanco.CodigoInternoColaborador;
                ret.Email = usuarioBanco.Email;
                ret.UsuarioId = usuarioBanco.Id;
                var tokenSistema = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.TOKEN_SISTEMA_COLABORACAO);
                ret.Colaborador = await _colaboradorClient.GetColaboradorByCpf(usuarioBanco.CodigoInternoColaborador, tokenSistema);

                try
                {
                    _envioEmail.EnviaEmailTemplateFoursys(ret.Colaborador.NomeCompleto, "Você agora tem acesso ao aplicativo da Foursys.</br>Sua senha de acesso é: <b>" + novaSenha + "</b>", "Primeiro Acesso App Foursys", usuarioBanco.Email);
                }
                catch (Exception e)
                {
                    throw e;
                }
                return ret;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private string geraSenhaUsuario()
        {
            return new Random().Next(0, 999999).ToString().PadLeft(6, '0');
        }

        public async Task<bool> AlteraSenhaUsuario(string cpf, string senhaAntiga, string novaSenha)
        {
            var usuarioBanco = _usuarioDtoRepository.GetUserByCpf(cpf);

            if (usuarioBanco == null)
            {
                throw new Exception("Colaborador não cadastrado.");
            }

            if (usuarioBanco.PrimeiroAcessoRealizado == 0)
            {
                throw new Exception("Colaborador sem primeiro acesso. Favor realizar primeiro acesso.");
            }

            if (usuarioBanco.Ativo == 0)
            {
                throw new Exception("Usuário desativado.");
            }

            if (String.IsNullOrEmpty(senhaAntiga) || novaSenha.Length < 6)
            {
                throw new Exception("A senha deve conter pelo menos seis dígitos.");
            }

            var senhaHash = Criptografia.MD5Hash(senhaAntiga);

            if (senhaHash != usuarioBanco.Senha)
            {
                throw new Exception("A senha atual não confere.");
            }

            usuarioBanco.Senha = Criptografia.MD5Hash(novaSenha);
            _usuarioDtoRepository.UpdateUser(usuarioBanco);

            try
            {
                var colaborador = await _colaboradorClient.BuscarDadosColaboradorAdmin(usuarioBanco.CodigoInternoColaborador, null);
                _envioEmail.EnviaEmailTemplateFoursys(colaborador.NomeCompleto, "Sua nova senha de acesso é: <b>" + novaSenha + "</b>", "Senha Resetada", usuarioBanco.Email);
            }
            catch (Exception e)
            {
                throw e;
            }

            return true;
        }

        public async Task<bool> ResetaSenhaUsuario(string cpf)
        {
            var usuarioBanco = _usuarioDtoRepository.GetUserByCpf(cpf);
            if (usuarioBanco == null)
            {
                throw new Exception("Colaborador não cadastrado.");
            }

            if (usuarioBanco.PrimeiroAcessoRealizado == 0)
            {
                throw new Exception("Colaborador sem primeiro acesso. Favor realizar primeiro acesso.");
            }

            if (usuarioBanco.Ativo == 0)
            {
                throw new Exception("Usuário desativado.");
            }

            var novaSenha = geraSenhaUsuario();
            usuarioBanco.Senha = Criptografia.MD5Hash(novaSenha);
            _usuarioDtoRepository.UpdateUser(usuarioBanco);

            try
            {
                var colaborador = await _colaboradorClient.BuscarDadosColaboradorAdmin(usuarioBanco.CodigoInternoColaborador, null);
                _envioEmail.EnviaEmailTemplateFoursys(colaborador.NomeCompleto, "Sua nova senha de acesso é: <b>" + novaSenha + "</b>", "Senha Resetada", usuarioBanco.Email);
            }
            catch (Exception e)
            {
                throw e;
            }

            return true;
        }

        public bool ValidaAcessoGrupoFuncionalidade(string cpf, FuncionalidadeSistemaEnum funcionalidade)
        {
            var usuarioBanco = _usuarioDtoRepository.GetUserByCpf(cpf);

            if (usuarioBanco == null)
            {
                throw new Exception("Nenhum usuário encontrado para este CPF.");
            }

            if (usuarioBanco.Ativo == INATIVO)
            {
                throw new Exception("Usuario inativo.");
            }

            // TODO: Implementar a verificação de acesso por grupo de acesso

            //var funcionalidadeSistemaModel = _funcionalidadeSistemaDomainFactory.buildFuncionalidadeSistemaModel();
            //IFuncionalidadeSistemaModel funcionalidadeSistemaBanco = funcionalidadeSistemaModel.GetByIdFuncionalidade((int)funcionalidade);

            //if (funcionalidadeSistemaBanco == null)
            //{
            //    throw new Exception("Grupo de acesso não encontrado.");
            //}

            //var usuarioGrupoAcessoModel = _usuarioGrupoAcessoDomainFactory.buildUsuarioGrupoAcessoModel();
            //List<IUsuarioGrupoAcessoModel> usuarioGruposAcessoBanco = usuarioGrupoAcessoModel.ListByUserId(usuarioBanco.Usuario.UsuarioId, _usuarioGrupoAcessoDomainFactory);

            //if (usuarioGruposAcessoBanco == null)
            //{
            //    return false;
            //}

            //if (usuarioGruposAcessoBanco.Count == 0)
            //{
            //    return false;
            //}

            //var grupoAcessoFuncionalidadeSistemaModel = _grupoAcessoFuncionalidadeSistemaDomainFactory.buildGrupoAcessoFuncionalidadeSistemaModel();

            //foreach (var grupo in usuarioGruposAcessoBanco)
            //{
            //    List<IGrupoAcessoFuncionalidadeSistemaModel> grupoAcessoFuncionalidadeSistemaBanco = grupoAcessoFuncionalidadeSistemaModel.ListByIdFuncionalidade(funcionalidadeSistemaBanco.Id, _grupoAcessoFuncionalidadeSistemaDomainFactory);
            //    if (grupoAcessoFuncionalidadeSistemaBanco.Count > 0)
            //    {
            //        return true;
            //    }
            //}

            return false;
        }

        public void Logout(string login)
        {
            _usuarioDtoRepository.Logout(login);
        }

        public Task<bool> AlteraSenha(string token, string novaSenha, string senhaAntiga)
        {
            return AlteraSenhaUsuario(_aspNetUser.GetUsuarioLogado().Cpf, senhaAntiga, novaSenha);
        }

        public UsuarioColaboradorDTO BuscarDadosUsuario(string cpf)
        {
            var usuarioBanco = _usuarioDtoRepository.GetUserByCpf(cpf);

            if (usuarioBanco != null)
            {
                return BuildUsuarioColaborador(usuarioBanco);
            }
            else
            {
                throw new Exception("Usuário não encotrado no cpf fornecido.");
            }
        }

        private static UsuarioColaboradorDTO BuildUsuarioColaborador(UsuarioDTO dto)
        {
            return new UsuarioColaboradorDTO
            {
                UsuarioId = dto.Id,
                Cpf = dto.CodigoInternoColaborador,
                Email = dto.Email,
                OrgId = dto.OrgId
            };
        }

        public void ConfirmaPrimeiroAcessoFourmakers(string cpf, int orgId)
        {
            var userDto = _usuarioDtoRepository.GetUserByCpfAndOrg(cpf, orgId);
            if (userDto.PrimeiroAcessoRealizado == 0)
            {
                userDto.PrimeiroAcessoRealizado = 1;
                userDto.DataAceiteTermo = DateTime.Now;
                _usuarioDtoRepository.UpdateUser(userDto);
            }
        }

        public string GenerateJWTToken(string email, string cpf, string codColaborador, int orgId, TipoLoginEnum tipoLogin)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.AUTH_SECRET));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("Email", email),
                    new Claim("Cpf", cpf),
                    new Claim("CodColaborador", codColaborador),
                    new Claim("OrgId", orgId.ToString()),
                    new Claim("LoginType", ((int)tipoLogin).ToString()),
                }),
                Expires = DateTime.UtcNow.AddDays(5),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public async Task<UsuarioResult> GenerateAccessToken(string accessCode, int orgId)
        {
            var ssoCredentials = _ssoRepository.GetCredentials(orgId, isMobile: false);
            if (ssoCredentials == null)
            {
                var exp = new Exception("No credentials");
                _logDB.SaveExceptionLogInDatabaseAndSendEmail("Não foi possível obter as credenciais do AD", exp, ProcessIdentifierEnum.UsuarioServiceGenerateAccessToken);

                throw exp;
            }

            TokenResult tokenResult;

            if (ssoCredentials.IsPublic)
                tokenResult = await _ssoClient.GetTokenPublic(ssoCredentials, accessCode);
            else
                tokenResult = await _ssoClient.GetToken(ssoCredentials, accessCode);

            if (tokenResult == null)
            {
                var exp = new Exception("Access Code Inválido");
                _logDB.SaveExceptionLogInDatabaseAndSendEmail("Não foi possível obter o token do AD", exp, ProcessIdentifierEnum.UsuarioServiceGenerateAccessToken);

                throw exp;
            }

            try
            {
                var user = await ShowMeSSO(ssoCredentials, tokenResult.RefreshToken, orgId);
                if (!_usuarioColaboradorRepository.CheckColaboradorOrgAtivo(user.Cpf, orgId))
                {
                    throw new ValidationException("Usuário inativo para a Org.");
                }
                try
                {
                    var token = GenerateJWTToken(user.Email, user.Cpf, "", orgId, TipoLoginEnum.SSO);
                    var userDto = _usuarioDtoRepository.GetUserByCpf(user.Cpf);
                    var primeiroAcessoRealizado = userDto?.PrimeiroAcessoRealizado == 1;
                    return new UsuarioResult
                    {
                        token = token,
                        usuario = await ShowMe(user.Cpf, orgId),
                        primeiroAcessoRealizado = primeiroAcessoRealizado,
                        dataAceitePrimeiroAcesso = _usuarioExternoRepository.GetDataAceiteTermo(user.UsuarioId)
                    };
                }
                catch (Exception err)
                {
                    var exp = new Exception("Falha ao obter os dados em GenerateAccessToken do colaborador: " + user.Cpf, err);
                    throw exp;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<UsuarioResult> GenerateAccessTokenMobile(string accessCode, int orgId)
        {
            var ssoCredentials = _ssoRepository.GetCredentials(orgId, isMobile: true);
            if (ssoCredentials == null)
            {
                var exp = new Exception("No credentials");
                _logDB.SaveExceptionLogInDatabaseAndSendEmail("Não foi possível obter as credenciais do AD", exp, ProcessIdentifierEnum.UsuarioServiceGenerateAccessToken);

                throw exp;
            }

            try
            {
                var user = await ShowMeSSOMobile(ssoCredentials, accessCode, orgId);
                if (!_usuarioColaboradorRepository.CheckColaboradorOrgAtivo(user.Cpf, orgId))
                {
                    throw new ValidationException("Usuário inativo para a Org.");
                }
                try
                {
                    var token = GenerateJWTToken(user.Email, user.Cpf, "", orgId, TipoLoginEnum.SSO);
                    var userDto = _usuarioDtoRepository.GetUserByCpf(user.Cpf);
                    var primeiroAcessoRealizado = userDto?.PrimeiroAcessoRealizado == 1;
                    return new UsuarioResult
                    {
                        token = token,
                        usuario = await ShowMe(user.Cpf, orgId),
                        primeiroAcessoRealizado = primeiroAcessoRealizado,
                        dataAceitePrimeiroAcesso = _usuarioExternoRepository.GetDataAceiteTermo(user.UsuarioId)
                    };
                }
                catch (Exception err)
                {
                    var exp = new Exception("Falha ao obter os dados em GenerateAccessToken do colaborador: " + user.Cpf, err);
                    throw exp;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<UsuarioColaboradorDTO> ShowMeSSO(SSOCredentialsDTO credentials, string refreshToken, int orgId)
        {
            try
            {
                TokenResult tokenResult;

                if (credentials.IsPublic)
                    tokenResult = await _ssoClient.ValidateRefreshTokenPublic(credentials, refreshToken);
                else
                    tokenResult = await _ssoClient.ValidateRefreshToken(credentials, refreshToken);
                Console.WriteLine("Token parameter: " + refreshToken);
                Console.WriteLine("Token refresh: " + tokenResult.AccessToken);
                var userSSOResult = await _ssoClient.GetUserDetail(credentials, tokenResult.AccessToken);

                if (userSSOResult == null)
                {
                    var exp = new ValidationException("Access Token Inválido");
                    _logDB.SaveExceptionLogInDatabaseAndSendEmail(exp.Message, exp, ProcessIdentifierEnum.UsuarioServiceShowMeSSO);
                    throw exp;
                }

                var emailUsuario = userSSOResult.Mail;

                try
                {
                    var validaUsuario = await _usuarioColaboradorRepository.GetCodColaboradorByEmailEOrgId(emailUsuario, orgId);
                    if (validaUsuario == null)
                    {
                        throw new ValidationException("Usuário " + emailUsuario + " não encontrado nesta organização");
                    }

                    UsuarioDTO userDto;
                    if (orgId.ToIntOuZero() > 0)
                    {
                        userDto = _usuarioDtoRepository.GetUserByLoginAndOrg(emailUsuario, orgId);
                    }
                    else
                    {
                        userDto = _usuarioDtoRepository.GetUserByLogin(emailUsuario);
                    }

                    if (userDto == null)
                    {
                        throw new ValidationException("Usuário " + emailUsuario + " não encontrado");
                    }
                    if (!(userDto.Ativo == 1))
                    {
                        throw new ValidationException("Usuário " + emailUsuario + " desativado");
                    }
                    var result = BuildUsuarioColaborador(userDto);
                    result.Colaborador = null;
                    return result;
                }
                catch (ValidationException err)
                {
                    _logDB.SaveExceptionLogInDatabaseAndSendEmail($"Erro de validação para o email {emailUsuario}: \n{err.Message}", err, ProcessIdentifierEnum.UsuarioServiceShowMeSSO);
                    throw;
                }
                catch (Exception err)
                {
                    _logDB.SaveExceptionLogInDatabaseAndSendEmail($"Erro genérico para o email {emailUsuario}: \n{err.Message}", err, ProcessIdentifierEnum.UsuarioServiceShowMeSSO);
                    throw;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<UsuarioColaboradorDTO> ShowMeSSOMobile(SSOCredentialsDTO credentials, string mobileToken, int orgId)
        {
            try
            {

                var userSSOResult = await _ssoClient.GetUserDetail(credentials, mobileToken);

                if (userSSOResult == null)
                {
                    var exp = new ValidationException("Access Token Inválido");
                    _logDB.SaveExceptionLogInDatabaseAndSendEmail(exp.Message, exp, ProcessIdentifierEnum.UsuarioServiceShowMeSSO);
                    throw exp;
                }

                var emailUsuario = userSSOResult.Mail;

                try
                {
                    var validaUsuario = await _usuarioColaboradorRepository.GetCodColaboradorByEmailEOrgId(emailUsuario, orgId);
                    if (validaUsuario == null)
                    {
                        throw new ValidationException("Usuário " + emailUsuario + " não encontrado nesta organização");
                    }

                    var userDto = _usuarioDtoRepository.GetUserByLogin(emailUsuario, orgId);
                    if (userDto == null)
                    {
                        throw new ValidationException("Usuário " + emailUsuario + " não encontrado");
                    }
                    if (!(userDto.Ativo == 1))
                    {
                        throw new ValidationException("Usuário " + emailUsuario + " desativado");
                    }
                    var result = BuildUsuarioColaborador(userDto);
                    result.Colaborador = null;
                    return result;
                }
                catch (ValidationException err)
                {
                    _logDB.SaveExceptionLogInDatabaseAndSendEmail($"Erro de validação para o email {emailUsuario}: \n{err.Message}", err, ProcessIdentifierEnum.UsuarioServiceShowMeSSO);
                    throw;
                }
                catch (Exception err)
                {
                    _logDB.SaveExceptionLogInDatabaseAndSendEmail($"Erro genérico para o email {emailUsuario}: \n{err.Message}", err, ProcessIdentifierEnum.UsuarioServiceShowMeSSO);
                    throw;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<UsuarioColaboradorDTO> ShowMeSSOCandidato(string token, int? userId = null)
        {
            var usuarioLogado = _aspNetUser.GetUsuarioLogado();
            var usuarioColaborador = _usuarioColaboradorRepository.GetUserByCPFEOrgId(usuarioLogado.Cpf, usuarioLogado.OrgId, userId);

            try
            {
                var userDto = _usuarioDtoRepository.GetUserByLogin(usuarioColaborador.Email);
                var userColaborador = BuildUsuarioColaborador(userDto);
                var colabordaorInfo = _buscaColaboradorRepository.GetColaborador(usuarioLogado.Cpf, 1);
                if (userColaborador.Colaborador?.Cpf == null)
                {
                    SRSCandidateDTO usuarioSRS;
                    var tokenCCH = (await _cchClient.Autenticacao()).token;
                    var cchUser = (await _cchClient.Validacao(usuarioColaborador.Email, tokenCCH));

                    if (cchUser.Count() == 0)
                    {
                        usuarioSRS = new SRSCandidateDTO
                        {
                            first_name = usuarioColaborador.NomeColaborador,
                            phone_home = "",
                            phone_cell = usuarioColaborador.ContatoPrincipal,
                            address = "",
                            address_number = "",
                            address_complement = "",
                            district = "",
                            city = "",
                            state = "",
                            zip = "",
                            source = "",
                            key_skills = null,
                            methodologies = null,
                            email1 = usuarioColaborador.Email,
                            email2 = "",
                            emailFoursys = usuarioColaborador.Email,
                            desired_pay = 0,
                            current_pay = 0,
                            cand_rg = "",
                            cand_estCivil = "",
                            cand_Lkdin = "",
                            cand_skype = "",
                            cand_gruporisco = "",
                            instagram = "",
                            facebook = "",
                            twitter = "",
                            cand_filhos = 0,
                            dataNascimento = null,
                            disponibilidade = "",
                            zona = "",
                            pcd = 0,
                            genre = "",
                            sexual_orientation = "",
                            ethnicity = "",
                            school_level = "",
                            refugee_person = 0
                        };

                        try
                        {
                            var authSRS = await _srsClient.Autenticacao();
                            var srsResult = await _srsClient.GetCandidate(authSRS.token, colabordaorInfo.DocumentoColaborador);
                            if (srsResult.Sucess)
                            {
                                if (srsResult.Message.ToLower() == "candidate not found")
                                {
                                    await _srsClient.PostCandidate(authSRS.token, colabordaorInfo.DocumentoColaborador, usuarioSRS, null);
                                }

                                if (srsResult.Message.ToLower() == "candidate successfully listed")
                                {
                                    usuarioSRS = srsResult.SRSCandidateDTO;
                                    //Atualiza email no srs
                                    usuarioSRS.emailFoursys = usuarioColaborador.Email;
                                    usuarioSRS.first_name = usuarioColaborador.NomeColaborador;
                                    usuarioSRS.phone_cell = usuarioColaborador.ContatoPrincipal;
                                    await _srsClient.PostCandidate(authSRS.token, colabordaorInfo.DocumentoColaborador, usuarioSRS, usuarioSRS.candidate_emergency_contact);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _logDB.SaveExceptionLogInDatabaseAndSendEmail($"Erro ao tentar acessar SRS, e-mail: {usuarioColaborador.Email}, Obs: isso não está impedindo este usuário de entrar no sistema, essa mensagem é apenas um alerta para avisar que o SRS está offline: \n{ex.Message}", ex, ProcessIdentifierEnum.UsuarioServiceShowMeSSOCandidato);
                        }
                    }

                    userDto = _usuarioDtoRepository.GetUserByLogin(usuarioColaborador.Email);
                    userColaborador = BuildUsuarioColaborador(userDto);
                    userColaborador.Colaborador = colabordaorInfo;
                }
                return userColaborador;
            }
            catch (ValidationException err)
            {
                _logDB.SaveExceptionLogInDatabaseAndSendEmail($"Erro de validação para o email {usuarioColaborador.Email}: \n{err.Message}", err, ProcessIdentifierEnum.UsuarioServiceShowMeSSOCandidato);
                throw;
            }
            catch (Exception err)
            {
                _logDB.SaveExceptionLogInDatabaseAndSendEmail($"Erro genérico para o email {usuarioColaborador.Email}: \n{err.Message}", err, ProcessIdentifierEnum.UsuarioServiceShowMeSSOCandidato);
                throw;
            }
        }

        public void InserePrestadorServicoPessoaJuridica(string cnpj, PrestadorServicoDTO prestadorServico, RegimeTributarioDTO regimeTributario, string cpf)
        {
            try
            {
                if (!Colaboracao.Helper.ToolsUtil.ValidaCnpj(prestadorServico.Cnpj))
                {
                    throw new Exception("O CNPJ é inválido");
                }
                else
                {
                    using (var trans = _unitOfWork.BeginTransaction())
                    {
                        try
                        {
                            _usuarioColaboradorRepository.InserePrestadorServicoPessoaJuridica(cnpj, prestadorServico, regimeTributario, cpf);
                            trans.Commit();
                        }
                        catch (Exception e)
                        {
                            trans.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public void AlterarPrestadorServicoPessoaJuridica(string cnpj, PrestadorServicoDTO prestadorServico, RegimeTributarioDTO regimeTributario, string cpf)
        {
            try
            {
                using (var trans = _unitOfWork.BeginTransaction())
                {
                    try
                    {
                        _usuarioColaboradorRepository.AlterarPrestadorServicoPessoaJuridica(cnpj, prestadorServico, regimeTributario, cpf);
                        trans.Commit();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task<UsuarioColaboradorDTO> ShowMe(string cpf, int orgId)
        {
            UsuarioColaboradorDTO user = _usuarioColaboradorRepository.GetUserByCPFEOrgId(cpf, orgId);
            user.Colaborador = _buscaColaboradorRepository.GetColaborador(cpf, orgId);
            user.ColaboradorOrg = _buscaColaboradorRepository.GetColaboradorOrg(cpf, orgId);
            user.OrgHierarquia = _buscaColaboradorRepository.BuscaColaboradorOrgHierarquia(user.ColaboradorOrg.CodColaborador, orgId);
            user.FuncionalidadeSistema = BuscarFuncionalidadeSistemas(int.Parse(user.UsuarioId.ToString()), orgId);
            user.UltimaAlteracao = _historicoCvRepository.GetUltimaAtualizacao(cpf);
            user.SouGestorDeAprovadores = await _usuarioColaboradorRepository.VerificaSeEhGestorHierarquicoDeUmAprovador(orgId, cpf);
            user.QuestionariosPreenchidos = await _questionarioRespostaRepository.ObterCodigosQuestionarioColaboradorAsync(orgId,cpf);
            user.QuestionariosAtivos = await _questionarioRespostaRepository.ObterCodigosQuestionariosAtivosAsync(orgId);
            
            var configuracaoAcessoTimesheet = _buscaParametroConfiguracaoService.GetParametroConfiguracao<string>(ParametroOrgCodigoEnum.OCULTAR_MODULO_TIMESHEET_MODELO_CONTRATACAO, user.ColaboradorOrg.OrgId, user.Colaborador.Cpf);
            var ocultaTimesheet = false;
            if (!string.IsNullOrEmpty(configuracaoAcessoTimesheet))
            {
                if (configuracaoAcessoTimesheet == "Todos")
                {
                    ocultaTimesheet = true;
                }

                if (user.ColaboradorOrg.ModeloContratacao == configuracaoAcessoTimesheet)
                {
                    ocultaTimesheet = true;
                }
            }

            user.OcultaTimeSheet = ocultaTimesheet;
            return user;
        }

        private List<FuncionalidadeSistemaDTO> BuscarFuncionalidadeSistemas(int usuarioId, int orgId)
        {
            var funcionalidadeSistemaList = _funcionalidadeSistemaRepository.GetFuncionalidadeSistemaPorUsuario(usuarioId, orgId).Result;
            return funcionalidadeSistemaList;
        }

        public async Task<List<BuscarHierarquiaResult>> BuscarHierarquia(string cpfUsuario, int orgId)
        {
            try
            {
                var colaboradores = new List<BuscarHierarquiaResult>();
                var subordinados = _buscaColaboradorRepository.GetSubordinadosColaboradorOrg(cpfUsuario, orgId);
                foreach (var recurso in subordinados)
                {
                    var colaboradorOrg = _buscaColaboradorRepository.GetColaboradorOrg(recurso.Cpf, orgId);
                    var dadosColaborador = _buscaColaboradorRepository.GetColaborador(recurso.Cpf, orgId);
                    var novoColaborador = new BuscarHierarquiaResult()
                    {
                        Cpf = recurso.Cpf,
                        EnumeradorDeAcesso = EnumeradorDeAcesso.Usuario,
                        CodigoProfissional = colaboradorOrg.CodColaborador,
                        NomeProfissional = dadosColaborador.NomeCompleto,
                        CodigoCargo = colaboradorOrg.CodCargo,
                        NomeCargo = colaboradorOrg.Cargo,
                        NomeProjetosRecurso = _projetoOrgRepository.GetProjetosColaboradorOrg(recurso.CodColaborador, orgId),
                        Competencias = _competenciaColaboradorRepository.GetCompetenciaColaborador(recurso.Cpf),
                        Kepper = _colaboradorKeeperRepository.SeKeeper(recurso.Cpf),
                        Aniversario = dadosColaborador.DataNascimento,
                        FotoPerfil = dadosColaborador.UrlFoto
                    };
                    colaboradores.Add(novoColaborador);
                }

                colaboradores.Where(o => o.CodigoCargo == PEOPLE_PARTNER_CODE_CCH)
                             .ToList()
                             .ForEach(colaborador => colaborador.EnumeradorDeAcesso = EnumeradorDeAcesso.PeoplePartner);

                return colaboradores.GroupBy(x => x.CodigoProfissional).Select(d => d.First()).GroupBy(x => x.Cpf).Select(y => y.Last()).ToList();
            }
            catch (Exception e)
            {
                throw new Exception("Não foi possivel gerar uma lista de subordinados");
            }
        }

        public bool SeTokenSistema(string token)
        {
            return _usuarioColaboradorRepository.SeTokenSistema(token);
        }

        public async Task InsereUsuarioFourmakers(string cpf, string nomeCompleto, string email, string senha)
        {
            try
            {
                var usuariosOrg = await _usuarioColaboradorRepository.BuscarUsuariosOrgPorEmail(email);
                var usuarioOutraOrg = usuariosOrg.Where(x => x.OrgId != ORG_ID_FOURMAKERS).FirstOrDefault();
                var existeNaFourmakers = usuariosOrg.Where(x => x.OrgId == ORG_ID_FOURMAKERS).FirstOrDefault() != null;

                if (existeNaFourmakers)
                {
                    throw new ArgumentException("Não é possível cadastrar este usuário. Usuário já cadastrado");
                }

                var regexSenha = new Regex("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$");
                if (!regexSenha.Match(senha).Success)
                    throw new Exception("Senha inválida, a senha precisa conter no mínimo 8 caracteres, uma letra maiúscula, uma letra minúscula, um caracter especial e um número.");

                using (var dbTrans = _unitOfWork.BeginTransaction())
                {
                    try
                    {
                        var codColaborador = string.Empty;
                        if (usuarioOutraOrg != null) // key 0 significa valor default , dictionary nao retorna null
                        {
                            codColaborador = usuarioOutraOrg.CodigoInternoColaborador;
                            _usuarioColaboradorRepository.InserirUsuarioEColaboradorOrg(
                                new UsuarioColaboradorDTO
                                {
                                    Cpf = codColaborador,
                                    OrgId = ORG_ID_FOURMAKERS,
                                    Email = email,
                                },
                                new ColaboradorOrgDTO
                                {
                                    Cargo = "",
                                    CodCargo = "",
                                    CodColaborador = codColaborador,
                                    CodDepartamento = "",
                                    Departamento = "",
                                    CodDiretoria = "",
                                    Diretoria = "",
                                    DataAdmissao = DateTime.Now,
                                    OrgId = ORG_ID_FOURMAKERS,
                                    ModeloContratacao = null,
                                    EmpresaRelacionada = null,
                                    ModeloTrabalho = null,
                                    DiasPorSemana = null,
                                    ValorHora = null,
                                    CustoHora = null,
                                    BaseHoraMes = null,
                                    Ativo = true
                                }
                            );
                        }
                        else
                        {
                            codColaborador = Guid.NewGuid().ToString();
                            _usuarioColaboradorRepository.UpsertUsuarioColaborador(new UsuarioColaboradorDTO
                            {
                                Email = email,
                                Cpf = codColaborador,
                                OrgId = ORG_ID_FOURMAKERS
                            }, new ColaboradorDTO
                            {
                                NomeCompleto = nomeCompleto,
                                Cpf = codColaborador,
                                DocumentoColaborador = cpf
                            }, null);
                            _usuarioColaboradorRepository.UpsertColaboradorOrg(
                                new ColaboradorDTO
                                {
                                    NomeCompleto = nomeCompleto,
                                    Cpf = codColaborador,
                                    CodigoModeloContratacao = null
                                },
                                new ColaboradorOrgDTO
                                {
                                    Cargo = "",
                                    CodCargo = "",
                                    CodColaborador = codColaborador,
                                    CodDepartamento = "",
                                    Departamento = "",
                                    CodDiretoria = "",
                                    Diretoria = "",
                                    DataAdmissao = DateTime.Now,
                                    OrgId = ORG_ID_FOURMAKERS,
                                    ModeloContratacao = null,
                                    EmpresaRelacionada = null,
                                    ModeloTrabalho = null,
                                    DiasPorSemana = null,
                                    ValorHora = null,
                                    CustoHora = null,
                                    BaseHoraMes = null,
                                    Ativo = true,
                                    CodigoModeloContratacao = null
                                }
                            );
                        }
                        _usuarioColaboradorRepository.AlteraSenhaUsuario(codColaborador, senha, ORG_ID_FOURMAKERS);
                        dbTrans.Commit();
                        await EnviaEmailComCodigoAcesso(codColaborador, nomeCompleto, email, ORG_ID_FOURMAKERS);
                    }
                    catch (Exception err)
                    {
                        dbTrans.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public void AlterarSenhaPorToken(string token, string senha)
        {
            try
            {
                var userId = _usuarioExternoRepository.GetUsuarioFromTokenReseteSenha(token);
                var regexSenha = new Regex("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$");
                if (!regexSenha.Match(senha).Success)
                    throw new Exception("Senha inválida, a senha precisa conter no mínimo 8 caracteres, uma letra maiúscula, uma letra minúscula, um caracter especial e um número.");
                if (userId == 0)
                    throw new Exception("Recuperação de senha não encontrado");
                using (var dbTrans = _unitOfWork.BeginTransaction())
                {
                    try
                    {
                        _usuarioExternoRepository.AlteraSenhaUsuario(userId, Criptografia.MD5Hash(senha));
                        dbTrans.Commit();
                    }
                    catch (Exception err)
                    {
                        dbTrans.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public void EnviarEmailAlterarSenha(string email)
        {
            try
            {
                var user = _usuarioExternoRepository.GetUserByEmail(email, ORG_ID_FOURMAKERS);

                if (user == null)
                    throw new ValidationException("Usuario não encontrado");

                var userOrg = _usuarioColaboradorRepository.GetUserByCPFEOrgId(user.Cpf, ORG_ID_FOURMAKERS);

                int? orgEmail = _buscaColaboradorRepository.GetEmailOrg(email);

                if (orgEmail != null)
                {
                    var retTemplateService = _templateOrgService.BuscaTemplateEmail(TemplateOrgParametroEnum.RESET_SENHA_ORG_SSO);
                    var templateParametrizado = ParametrizarTemplateEsqueciSenha((int)orgEmail, user.NomeColaborador, retTemplateService);

                    _templateOrgService.RegistraTemplateEmail((int)orgEmail, email, null, templateParametrizado);
                }
                else
                {
                    var token = _token.Base64(Criptografia.Encrypt(user.Cpf + "|" + DateTime.Now.ToUniversalTime() + "|" + new Random().Next(0, 99999) + "|5"));
                    _usuarioExternoRepository.SavePedidoReseteSenha(token, user.UsuarioId, DateTime.Now.AddMinutes(15));
                    _envioEmail.EnviaEmailSemTemplate("",
                            Utils.TemplateEmail.MontaTemplateReseteSenha(VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.URL_RECUPERACAO_SENHA) + "?token=" + token),
                            "Recuperação de senha Fourmakers", user.Email);
                }
            }
            catch (Exception err)
            {
                throw new Exception("O método de AtualizaColaborador retornou: " + err.Message);
            }
        }

        private string ParametrizarTemplateEsqueciSenha(int orgId, string nomeColaborador, TemplateEmailDTO retTemplateService)
        {
            Dictionary<string, object> parametros = new Dictionary<string, object>();

            var templateParametrizado = retTemplateService.Template;
            var Url = _buscaColaboradorRepository.MontarUrlSSO(orgId);

            parametros.Add(VARIAVEL_NOME_COMPLETO, nomeColaborador);
            parametros.Add(VARIAVEL_URL, Url);
            parametros.Add(VARIAVEL_SENHA, StringUtil.GetStringAleatoria(12));

            foreach (var item in parametros)
            {
                if (!string.IsNullOrEmpty(item.Key))
                {
                    templateParametrizado = templateParametrizado.Replace(item.Key, (string)item.Value);
                }
            }
            return templateParametrizado;
        }

        public void SubmeterConviteEmpresa(string cpf, string cnpj, bool confirmado)
        {
            try
            {
                _usuarioExternoRepository.SubmeterConvite(cpf, cnpj, confirmado);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public void AlterarSenhaUsuarioLogado(string senha)
        {
            string cpf = _aspNetUser.GetUsuarioLogado().Cpf;

            try
            {
                long userId = _usuarioExternoRepository.AlterarSenhaUsuarioLogado(cpf);

                var regexSenha = new Regex("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$");
                if (!regexSenha.Match(senha).Success)
                    throw new Exception("Senha inválida, a senha precisa conter no mínimo 8 caracteres, uma letra maiúscula, uma letra minúscula, um caracter especial e um número.");
                if (userId == 0)
                    throw new Exception("Recuperação de senha não encontrado");
                using (var dbTrans = _unitOfWork.BeginTransaction())
                {
                    try
                    {
                        _usuarioExternoRepository.AlteraSenhaUsuario(userId, Criptografia.MD5Hash(senha));
                        dbTrans.Commit();
                    }
                    catch (Exception err)
                    {
                        dbTrans.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public void ConfirmacaoEmail(int codigo, string cpf)
        {
            ConfirmacaoEmailDTO confirmacaoEmailDTO = _usuarioExternoRepository.RetornaEmailByCpf(cpf);

            _usuarioExternoRepository.SalvarAtualizarConfirmacaoEmail(codigo, confirmacaoEmailDTO, false);
        }

        public void EnviarConfirmacaoEmail(string cpf)
        {
            int codigoGerado = Convert.ToInt32(GerarCodigo());
            ConfirmacaoEmailDTO confirmacaoEmailDTO = _usuarioExternoRepository.RetornaEmailByCpf(cpf);

            bool enviar = _usuarioExternoRepository.SalvarAtualizarConfirmacaoEmail(codigoGerado, confirmacaoEmailDTO, false);

            if (enviar)
                EnviarEmailComCodigo(codigoGerado, confirmacaoEmailDTO.Email, confirmacaoEmailDTO.NomeColaborador);
        }

        public void ReenviarCodigoConfirmacao(string cpf)
        {
            int codigoGerado = Convert.ToInt32(GerarCodigo());
            ConfirmacaoEmailDTO confirmacaoEmailDTO = _usuarioExternoRepository.RetornaEmailByCpf(cpf);

            bool enviar = _usuarioExternoRepository.SalvarAtualizarConfirmacaoEmail(codigoGerado, confirmacaoEmailDTO, true);

            if (enviar)
                EnviarEmailComCodigo(codigoGerado, confirmacaoEmailDTO.Email, confirmacaoEmailDTO.NomeColaborador);
        }

        private static string GerarCodigo()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString("D6");
        }

        public bool IsEmailValidated(string cpf, int orgId)
        {
            UsuarioColaboradorDTO user = _usuarioExternoRepository.GetUserByCpfEOrgId(cpf, orgId);
            ConfirmacaoEmailDTO confirmacaoEmailDTO = _usuarioExternoRepository.GetConfirmacaoEmailByCpf(cpf);
            if (!String.IsNullOrEmpty(user.Cpf) && String.IsNullOrEmpty(confirmacaoEmailDTO.Cpf))
            {
                // Usuarios antigos devem validar email? Se sim, return false e descomentar linha abaixo
                //EnviarConfirmacaoEmail(cpf);
                return true;
            }
            else if (!String.IsNullOrEmpty(user.Cpf) && !confirmacaoEmailDTO.CodigoConfirmado)
            {
                ReenviarCodigoConfirmacao(cpf);
                return false;
            }
            return true;
        }

        public void EnviarEmailComCodigo(int codigoGerado, string email, string nome)
        {
            _envioEmail.EnviaEmailTemplateFoursys(nome, $"Seu código de confirmação é: {codigoGerado}", "Confirmação de email Fourmakers", email);
        }

        public async Task EnviaEmailComCodigoAcesso(string codColaboradorInterno, string nomeColaborador, string enderecoEmail, int orgId)
        {
            var token = StringUtil.GetStringNumericaAleatoria(TAMANHO_CODIGO_ACESSO);
            var validade = DateTime.UtcNow.AddMinutes(VALIDADE_TOKEN);

            _acessoUsuarioRepository.FechaTokens(codColaboradorInterno);
            await _acessoUsuarioRepository.CriaTokenAcesso(token, codColaboradorInterno, validade, DataTransferObject.Domain.TipoTokenAcessoEnum.Email, orgId);

            var templateEmail = _templateRepository.BuscaTemplateEmail(TemplateOrg.Constantes.TemplateOrgParametroEnum.TOKEN_ACESSO_EMAIL).Template;
            templateEmail = templateEmail.Replace("${NOME}", nomeColaborador);
            templateEmail = templateEmail.Replace("${TOKEN}", token);

            _envioEmail.EnviaEmailSemTemplate(nomeColaborador, templateEmail, "Acesso Fourmakers", enderecoEmail);
        }

        public async Task<ApiGenericResult> EnviarEmailDenuncia(string email, string mensagem, int orgId)
        {
            var apiResult = new ApiGenericResult();
            
            var templateEmail = _templateRepository.BuscaTemplateEmail(TemplateOrg.Constantes.TemplateOrgParametroEnum.CANAL_DENUNCIA).Template;
            var codUsuario = await _usuarioColaboradorRepository.GetCodColaboradorByEmailEOrgId(email, orgId);
            var nomeDestinatario = await _usuarioColaboradorRepository.GetNomeColaboradorPorCodigoInterno(codUsuario);
            
            templateEmail = templateEmail.Replace("{MESSAGE}", mensagem);
            _envioEmail.EnviaEmailSemTemplate(nomeDestinatario, templateEmail, "Canal de Denuncias", email);
            
            return apiResult;
        }
    }
}