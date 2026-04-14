using Colaboracao.Infra.Context;
using Core.DomainModel.SSO;
using DataTransferObject.Domain.SSO;
using System.Linq;

namespace Colaboracao.Infra.Repositories.SSO
{
    public class SSORepository : ISSORepository
    {
        private readonly ColaboradorContext _colaboradorContext;
        public SSORepository(ColaboradorContext colaboradorContext)
        {
            _colaboradorContext = colaboradorContext;
        }

        SSOCredentialsDTO ISSORepository.GetCredentials(int orgId, bool isMobile)
        {
            var credentialsRow = _colaboradorContext.tb_ad_sso.Where(x => x.tb_org_id == orgId).FirstOrDefault();
            if (credentialsRow != null)
            {
                return new SSOCredentialsDTO
                {
                    BaseUrl = credentialsRow.base_url,
                    ClientId = credentialsRow.client_id,
                    ClientSecretValue = credentialsRow.client_secret_value,
                    CodeVerifierPlain = credentialsRow.code_verifier_plain,
                    GraphPath = credentialsRow.graph_path,
                    RedirectUrl = credentialsRow.redirect_url,
                    Scope = isMobile ? "openid" : credentialsRow.scope,
                    Tenant = credentialsRow.tenant,
                    TokenPath = credentialsRow.token_path,
                    IsPublic = credentialsRow.is_public == 1 ? true : false
                };
            }
            else
            {
                return null;
            }
        }
    }
}