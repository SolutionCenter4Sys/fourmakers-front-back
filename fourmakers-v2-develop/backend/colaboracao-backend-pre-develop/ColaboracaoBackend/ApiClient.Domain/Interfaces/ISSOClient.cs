using DataTransferObject.Domain.SSO;
using System.Threading.Tasks;

namespace ApiClient.Domain.Interfaces
{
    public interface ISSOClient
    {
        Task<TokenResult> GetToken(SSOCredentialsDTO credentials, string authCode);
        Task<TokenResult> GetTokenPublic(SSOCredentialsDTO credentials, string authCode);
        Task<TokenResult> ValidateRefreshToken(SSOCredentialsDTO credentials, string refreshToken);
        Task<TokenResult> ValidateRefreshTokenPublic(SSOCredentialsDTO credentials, string refreshToken);
        Task<SSOUserDetailResult> GetUserDetail(SSOCredentialsDTO credentials, string token);
    }
}