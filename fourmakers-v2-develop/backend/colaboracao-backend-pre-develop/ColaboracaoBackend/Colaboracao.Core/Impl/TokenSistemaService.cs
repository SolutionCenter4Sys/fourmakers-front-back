using Colaboracao.Core.Interfaces;
using Core.Domain.Usuario;
using DataTransferObject.Domain.Util.Enum;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;

namespace Colaboracao.Core.Impl
{
    public class TokenSistemaService : ITokenSistemaService
    {

        private readonly ITokenSistemaRepository _tokenRepository;
        public TokenSistemaService(ITokenSistemaRepository tokenSistemaRepository)
        {
            _tokenRepository = tokenSistemaRepository;
        }

        public int GetOrgTokenSistema(string tokenSistema)
        {
            tokenSistema = Base64UrlEncoder.Decode(tokenSistema);
            var orgId = int.Parse(tokenSistema.Split("|")[0]);
            var token = tokenSistema.Split("|")[1];
            if (!_tokenRepository.ValidaTokenSistema(token, orgId))
                throw new AccessViolationException("not authorized");
            return orgId;
        }

        public int GetOrgTokenSistemaWithValidatingOrgs(string tokenSistema, params EnumORG[] allowedOrgs)
        {
            int orgId = GetOrgTokenSistema(tokenSistema); // chama o método original para decodificar e validar o token

            if (!allowedOrgs.Any(enumOrg => orgId == (int)enumOrg))
                throw new AccessViolationException("Não autorizado para essa organização");

            return orgId;
        }

    }
}