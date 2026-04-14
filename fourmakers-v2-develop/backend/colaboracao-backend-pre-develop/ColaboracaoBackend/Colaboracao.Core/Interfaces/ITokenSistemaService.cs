using DataTransferObject.Domain.Usuario;
using DataTransferObject.Domain.Util.Enum;
using System;

namespace Colaboracao.Core.Interfaces
{
    public interface ITokenSistemaService
    {
        int GetOrgTokenSistema(string tokenSistema);
        int GetOrgTokenSistemaWithValidatingOrgs(string tokenSistema, params EnumORG[] allowedOrgs);
    }
}