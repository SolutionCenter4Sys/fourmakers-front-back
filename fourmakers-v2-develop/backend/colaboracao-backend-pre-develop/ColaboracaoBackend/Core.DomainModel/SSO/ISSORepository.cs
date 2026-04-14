using DataTransferObject.Domain.SSO;

namespace Core.DomainModel.SSO
{
    public interface ISSORepository
    {
        /// <param name="orgId">Id da organização.</param>
        /// <param name="isMobile">Quando true (app mobile), Scope = "openid"; quando false (web), Scope vem do banco.</param>
        SSOCredentialsDTO GetCredentials(int orgId, bool isMobile = false);
    }
}