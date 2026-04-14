using Colaboracao.Core;
using Colaboracao.Core.Exceptions;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Core.Domain;
using Core.Domain.Colaborador;
using Core.Domain.TemplateEmail;
using Core.Domain.Usuario;
using Core.DomainModel.Org;
using Core.DomainModel.SSO;
using DataTransferObject.Domain;
using DataTransferObject.Domain.Usuario;
using DataTransferObject.Domain.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Usuario.Domain.Interfaces.Services;

using Logs.Infra.Attributes;

namespace Usuario.Domain.Impl.Services
{
    [LogDomainClass]
    public class AcessoUsuarioService : IAcessoUsuarioService
    {
        private readonly IEnvioEmail _envioEmail;
        private readonly IUsuarioColaboradorRepository _usuarioColaboradorRepository;
        private readonly IUsuarioService _usuarioService;
        private readonly IUsuarioExternoRepository _usuarioExternoRepository;
        private readonly IAcessoUsuarioRepository _acessoUsuarioRepository;
        private readonly ITemplateRepository _templateRepository;
        private readonly IOrgRepository _orgRepository;
        private readonly ISSORepository _ssoRepository;
        private readonly IBuscaColaboradorRepository _buscaColaboradorRepository;
        private readonly ITokenSistemaService _tokenSistemaService;

        public AcessoUsuarioService(IEnvioEmail envioEmail, IUsuarioColaboradorRepository usuarioColaboradorRepository,
            IUsuarioService usuarioService, IUsuarioExternoRepository usuarioExternoRepository, IAcessoUsuarioRepository acessoUsuarioRepository,
            ITemplateRepository templateRepository, IOrgRepository orgRepository, ISSORepository ssoRepository, IBuscaColaboradorRepository buscaColaboradorRepository,
            ITokenSistemaService tokenSistemaService
        )
        {
            _envioEmail = envioEmail;
            _usuarioColaboradorRepository = usuarioColaboradorRepository;
            _usuarioService = usuarioService;
            _usuarioExternoRepository = usuarioExternoRepository;
            _acessoUsuarioRepository = acessoUsuarioRepository;
            _templateRepository = templateRepository;
            _orgRepository = orgRepository;
            _ssoRepository = ssoRepository;
            _buscaColaboradorRepository = buscaColaboradorRepository;
            _tokenSistemaService = tokenSistemaService;
        }

        public async Task<TipoAcessoEnum> EnviaTokenAcessoEmail(string email, int orgId, bool forceCodigoEmail = false)
        {
            try
            {
                if (!ToolsUtil.ValidaEmail(email))
                    throw new ValidationException("Endereço email inválido");

                if (orgId == 0)
                {
                    throw new ValidationException("Organização não informada");
                }

                var codColabInterno = await _usuarioColaboradorRepository.GetCodColaboradorByEmailEOrgId(email, orgId);
                if (string.IsNullOrEmpty(codColabInterno))
                    throw new ValidationException("Nenhum usuário foi encontrado para este e-mail");

                if (!_usuarioColaboradorRepository.CheckColaboradorOrgAtivo(codColabInterno, orgId))
                    throw new ValidationException("Colaborador inativo na organização. Acesso não permitido.");

                var userInfo = await _usuarioService.ShowMe(codColabInterno, orgId);

                if (forceCodigoEmail || _ssoRepository.GetCredentials(orgId) == null)
                {
                    await _usuarioService.EnviaEmailComCodigoAcesso(codColabInterno, userInfo.NomeColaborador, email, orgId);
                    return TipoAcessoEnum.Email;
                }
                else
                {
                    var getSubdominioOrg = await _orgRepository.BuscaSubDominioOrg(orgId);
                    var retTemplateService = _templateRepository.BuscaTemplateEmail(TemplateOrg.Constantes.TemplateOrgParametroEnum.CADASTRO_NOVO_USUARIO_SSO);
                    var templateParametrizado = retTemplateService.Template;
                    templateParametrizado = templateParametrizado.Replace("${NOME}", userInfo.NomeColaborador);
                    templateParametrizado = templateParametrizado.Replace("${URL}", getSubdominioOrg);

                    _envioEmail.EnviaEmailSemTemplate(userInfo.NomeColaborador, templateParametrizado, "Acesso Fourmakers", email);

                    return TipoAcessoEnum.SSO;
                }
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception err)
            {
                Console.WriteLine("Falha ao enviar o token de acesso por email: " + err.Message);
                Console.WriteLine(err.StackTrace);
                throw;
            }
        }

        public async Task<UsuarioResult> ValidaTokenAcessoEmail(string email, string token, int orgId)
        {
            try
            {
                var codColabInterno = await _usuarioColaboradorRepository.GetCodColaboradorByEmailEOrgId(email, orgId);

                if (string.IsNullOrEmpty(codColabInterno))
                    throw new ValidationException("Nenhum usuário foi encontrado para este e-mail");

                if (!_usuarioColaboradorRepository.CheckColaboradorOrgAtivo(codColabInterno, orgId))
                    throw new ValidationException("Colaborador inativo na organização. Acesso não permitido.");

                var tokenInfo = _acessoUsuarioRepository.GetToken(codColabInterno, TipoTokenAcessoEnum.Email, orgId);
                if (tokenInfo == null || tokenInfo.Token != token || tokenInfo.Validade < DateTime.UtcNow)
                    throw new ValidationException("Código de acesso inválido");

                _acessoUsuarioRepository.FechaTokens(codColabInterno);
                var ret = new UsuarioResult();
                var userInfo = await _usuarioService.ShowMe(codColabInterno, orgId);
                ret.token = _usuarioService.GenerateJWTToken(email, codColabInterno, "", orgId, TipoLoginEnum.SISTEMA);
                ret.usuario = userInfo;
                ret.primeiroAcessoRealizado = _usuarioExternoRepository.IfPrimeiroAcessoUsuario(userInfo.UsuarioId);
                ret.dataAceitePrimeiroAcesso = _usuarioExternoRepository.GetDataAceiteTermo(userInfo.UsuarioId);
                return ret;
            }
            catch
            {
                throw;
            }
        }

        public async Task<UsuarioAcessoSemOrgResult> EnviaTokenAcessoEmailSemOrg(string email)
        {
            try
            {
                var usuarioOrgs = await _usuarioColaboradorRepository.BuscarUsuariosOrgPorEmail(email);
                if (usuarioOrgs.Count == 0)
                {
                    throw new ValidationException("Nenhum usuário foi encontrado para este e-mail");
                }
                // Só considera orgs em que o colaborador está ativo (tb_colaborador_org.ativo = 1)
                var orgsOndeColaboradorAtivo = usuarioOrgs
                    .Where(u => _usuarioColaboradorRepository.CheckColaboradorOrgAtivo(u.CodigoInternoColaborador, u.OrgId))
                    .Select(x => x.OrgId)
                    .Distinct()
                    .ToList();
                if (orgsOndeColaboradorAtivo.Count == 0)
                {
                    throw new ValidationException("Colaborador inativo na organização. Acesso não permitido.");
                }
                var orgsPorPrioridade = _orgRepository.GetAllOrgsId(false);

                var orgId = BuscarOrgPorFilaPrioridade(orgsOndeColaboradorAtivo, orgsPorPrioridade);

                var acesso = await EnviaTokenAcessoEmail(email, orgId);

                return new()
                {
                    TipoAcesso = acesso,
                    OrgId = orgId
                };
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception err)
            {
                throw;
            }
        }

        public async Task<string> ObtemCodigoAcessoEmail(string tokenSistema, string email, int orgId)
        {
            _tokenSistemaService.GetOrgTokenSistema(tokenSistema);

            if (!ToolsUtil.ValidaEmail(email))
                throw new ValidationException("Endereço email inválido");

            if (orgId == 0)
                throw new ValidationException("Organização não informada");

            var codColabInterno = await _usuarioColaboradorRepository.GetCodColaboradorByEmailEOrgId(email, orgId);
            if (string.IsNullOrEmpty(codColabInterno))
                throw new ValidationException("Nenhum usuário foi encontrado para este e-mail");

            var tokenInfo = _acessoUsuarioRepository.GetToken(codColabInterno, TipoTokenAcessoEnum.Email, orgId);
            if (tokenInfo == null || tokenInfo.Validade < DateTime.UtcNow)
                throw new ValidationException("Nenhum código de acesso ativo encontrado para este e-mail");

            return tokenInfo.Token;
        }

        private int BuscarOrgPorFilaPrioridade(List<int> usuarioOrgs, List<OrgDTO> listaOrgId)
        {
            var listasDeOrgIdFiltro = listaOrgId.Where(x => usuarioOrgs.Contains((int)x.Id)).OrderBy(x => x.Prioridade).FirstOrDefault()?.Id;

            return listasDeOrgIdFiltro.ToIntOuZero();
        }
    }
}