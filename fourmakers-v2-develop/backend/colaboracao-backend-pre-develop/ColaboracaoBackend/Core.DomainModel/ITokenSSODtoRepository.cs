using DataTransferObject.Domain.SSO;

namespace Core.DomainModel
{
    public interface ITokenSSODtoRepository
    {
        TokenSSO SaveTokenSSO(TokenSSO model);
        TokenSSO GetTokenSSO(TokenSSO model);
        void DeleteTokenColaborador(TokenSSO model);
    }
}
